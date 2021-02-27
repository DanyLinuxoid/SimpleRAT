using RAT.src.Interfaces;
using RAT.src.Models;
using System;
using System.Net.Sockets;

namespace RAT.src.Sockets.Connection
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
        /// <param name="stateLogic">Our client state logic.</param>
        public SocketConnectionDisconnectLogic(ISocketStateLogic stateLogic)
        {
            _socketStateLogic = stateLogic;
        }

        /// <summary>
        /// Disconnects client from socket without closing connection itself and kills process associated with client.
        /// </summary>
        /// <param name="res">State of async socket operation.</param>
        public void OnDisconnect(IAsyncResult res)
        {
            (Socket socket, StateObject state) = _socketStateLogic.GetSocketAndStateFromAsyncResult(res);
            socket.EndDisconnect(res);
            state.ResetState();
        }
    }
}