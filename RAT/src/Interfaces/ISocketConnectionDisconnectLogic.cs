using System;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Handles client disconnection.
    /// </summary>
    public interface ISocketConnectionDisconnectLogic
    {
        /// <summary>
        /// Disconnects client from socket without closing connection itself and kills process associated with client.
        /// </summary>
        /// <param name="res">State of async socket operation.</param>
        void OnDisconnect(IAsyncResult res);
    }
}