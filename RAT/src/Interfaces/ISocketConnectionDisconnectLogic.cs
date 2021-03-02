using System;
using System.Net.Sockets;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Handles client disconnection.
    /// </summary>
    public interface ISocketConnectionDisconnectLogic
    {
        /// <summary>
        /// Disconnects client but preserves socket.
        /// </summary>
        /// <param name="socket">Current connection.</param>
        void DisconnectFromMainSocket(Socket socket);

        /// <summary>
        /// Disconnects client specified socket, should not be used for main socket.
        /// </summary>
        /// <param name="socket">Socket that client should be disconnected from.</param>
        void DisconnectSocket(Socket socket);
    }
}