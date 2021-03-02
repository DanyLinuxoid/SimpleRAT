﻿using System;
using System.Net.Sockets;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Handles client connection stage over socket.
    /// </summary>
    public interface ISocketConnectionReceiveLogic
    {
        /// <summary>
        /// Wrapper for command receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        void BeginCommandReceive(Socket socket);

        /// <summary>
        /// Wrapper for file data receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        void BeginFileDataReceive(Socket socket);
    }
}