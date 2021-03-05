using RAT.Models;
using System;

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
        /// Updates state to provided one.
        /// </summary>
        /// <param name="state">State to update/set with.</param>
        void SetState(StateObject state);
    }
}