using RAT.src.Models;
using System;
using System.Net.Sockets;

namespace RAT.src.Interfaces
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
        /// Trivial steps to get socket and state from async result during socket connections.
        /// </summary>
        /// <param name="result">Status of async operation.</param>
        /// <returns>Socket and update state object.</returns>
        (Socket, StateObject) GetSocketAndStateFromAsyncResult(IAsyncResult result);

        /// <summary>
        /// Updates state to provided one.
        /// </summary>
        /// <param name="state">State to update/set with.</param>
        void SetState(StateObject state);
    }
}