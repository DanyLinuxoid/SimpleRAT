using RAT.src.Interfaces;
using RAT.src.Models;
using System;
using System.Net.Sockets;

namespace RAT.src.Logic.Sockets.Connection
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
        /// Disconnects client from file download socket.
        /// </summary>
        /// <param name="fileDownloadSocket">Current connection.</param>
        public void DisconnectFromFileDownloadSocket(Socket fileDownloadSocket)
        {
            if (fileDownloadSocket == null)
            {
                return;
            }

            if (fileDownloadSocket.Connected)
            {
                fileDownloadSocket.Disconnect(reuseSocket: false);
            }

            fileDownloadSocket.Close();
            _socketStateLogic.State.ClientFileDownloadSocket = null;
        }

        /// <summary>
        /// Disconnects client from socket without closing connection itself and kills process associated with client.
        /// </summary>
        /// <param name="res">State of async socket operation.</param>
        private void OnMainSocketDisconnect(IAsyncResult res)
        {
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(res);
            state.ClientSocket.EndDisconnect(res);

            // Now disconnect from file download socket.
            this.DisconnectFromFileDownloadSocket(state.ClientFileDownloadSocket);

            state.ResetState();
        }
    }
}