using System;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Logic related to connection acceptance stage.
    /// </summary>
    public interface ISocketConnectionAcceptLogic
    {
        /// <summary>
        /// Socket receival logic handling. Is called as first one.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        void OnAccept(IAsyncResult res);
    }
}