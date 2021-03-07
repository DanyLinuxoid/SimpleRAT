using RAT.Models;
using System;
using System.Net.Sockets;

namespace RAT.Interfaces
{
    /// <summary>
    /// Holds logic for client state management.
    /// </summary>
    public interface ISocketStateLogic
    {
        /// <summary>
        /// Client state representation and connection information holder.
        /// </summary>
        StateObject State { get; }

        /// <summary>
        /// Gets state from async result during socket connections.
        /// </summary>
        /// <param name="result">Status of async operation.</param>
        /// <returns>Updated state object.</returns>
        StateObject GetStateFromAsyncResult(IAsyncResult result);

        /// <summary>
        /// Creates new state for new connection.
        /// </summary>
        /// <param name="handler">Connection related to client/state.</param>
        StateObject CreateNewState(Socket handler);
    }
}