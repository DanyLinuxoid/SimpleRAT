using System;
using System.Net.Sockets;

namespace RAT.Interfaces
{
    /// <summary>
    /// Logic related to connection acceptance stage.
    /// </summary>
    public interface ISocketConnectionAcceptLogic
    {
        /// <summary>
        /// Wrapper for socket accept command.
        /// </summary>
        void BeginAccept(Socket socket);
    }
}