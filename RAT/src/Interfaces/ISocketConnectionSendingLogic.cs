using System;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Socket connection stage handling logic for client.
    /// </summary>
    public interface ISocketConnectionSendingLogic
    {
        /// <summary>
        /// Socket sending logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        void OnSend(IAsyncResult res);
    }
}