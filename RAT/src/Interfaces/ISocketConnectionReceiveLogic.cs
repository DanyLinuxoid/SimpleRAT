using System;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Handles client connection stage over socket.
    /// </summary>
    public interface ISocketConnectionReceiveLogic
    {
        /// <summary>
        /// Socket receival logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        void OnReceive(IAsyncResult res);
    }
}