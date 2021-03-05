using RAT.Interfaces;
using RAT.Models;
using System;
using System.Net.Sockets;

namespace RAT.Logic.Sockets.Connection
{
    /// <summary>
    /// Handles client disconnection.
    /// </summary>
    public class SocketConnectionDisconnectLogic : ISocketConnectionDisconnectLogic
    {
        private ISocketStateLogic _socketStateLogic { get; }

        /// <summary>
        /// Handles client disconnection.
        /// </summary>
        /// <param name="stateLogic">Our client current state logic.</param>
        public SocketConnectionDisconnectLogic(ISocketStateLogic stateLogic)
        {
            _socketStateLogic = stateLogic;
        }

        /// <summary>
        /// Disconnects client from main socket but preserves socket.
        /// </summary>
        /// <param name="mainSocket">Current connection.</param>
        public void DisconnectFromMainSocket(Socket mainSocket)
        {
            mainSocket.BeginDisconnect(reuseSocket: true, OnMainSocketDisconnect, _socketStateLogic.State);
        }

        /// <summary>
        /// Disconnects client specified socket, should not be used for main socket.
        /// </summary>
        /// <param name="socket">Socket that client should be disconnected from.</param>
        public void DisconnectSocket(Socket socket)
        {
            if (socket == null)
            {
                return;
            }

            try
            {
                socket.Disconnect(reuseSocket: false);
                socket.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Disconnects client from main socket without closing connection itself and kills process associated with client. Closes all socket except main.
        /// </summary>
        /// <param name="res">State of async socket operation.</param>
        private void OnMainSocketDisconnect(IAsyncResult res)
        {
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(res);
            state.ClientMainSocket.EndDisconnect(res);

            // Now disconnect from file download socket.
            this.DisconnectSocket(state.ClientFileDownloadSocket);
            state.ClientFileDownloadSocket = null;
            this.DisconnectSocket(state.FileUploadInformation.ClientFileUploadSocket);

            state.ResetState();
        }
    }
}