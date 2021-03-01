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
        /// Disconnects client from file download socket but preserves socket.
        /// </summary>
        /// <param name="fileDownloadSocket">Current connection.</param>
        void DisconnectFromFileDownloadSocket(Socket fileDownloadSocket);
    }
}