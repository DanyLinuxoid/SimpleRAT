using RAT.src.Interfaces;
using System;
using System.Net.Sockets;

namespace RAT.src.Sockets.Connection
{
    /// <summary>
    /// Socket connection stage handling logic for client.
    /// </summary>
    public class SocketConnectionSendingLogic : ISocketConnectionSendingLogic
    {
        private ISocketStateLogic _socketStateLogic { get; }

        /// <summary>
        /// Socket connection stage handling logic for client.
        /// </summary>
        /// <param name="stateLogic">Our client state logic.</param>
        public SocketConnectionSendingLogic(ISocketStateLogic stateLogic)
        {
            _socketStateLogic = stateLogic;
        }

        /// <summary>
        /// Socket sending logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        public void OnSend(IAsyncResult res)
        {
            (Socket socket, _) = _socketStateLogic.GetSocketAndStateFromAsyncResult(res);
            socket.EndSend(res);
        }
    }
}