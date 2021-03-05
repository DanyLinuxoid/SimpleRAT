using RAT.Interfaces;
using RAT.Models;
using System;

namespace RAT.Logic.Sockets
{
    /// <summary>
    /// Holds logic for client state management.
    /// </summary>
    public class SocketStateLogic : ISocketStateLogic
    {
        /// <summary>
        /// Client state representation and connection information holder.
        /// </summary>
        public StateObject State { get; private set; } = new StateObject();

        /// <summary>
        /// Gets state from async result during socket connections.
        /// </summary>
        /// <param name="result">Status of async operation.</param>
        /// <returns>Updated state object.</returns>
        public StateObject GetStateFromAsyncResult(IAsyncResult result)
        {
            // Retrieve the state object from the asynchronous result.
            return result.AsyncState is StateObject
                ? (StateObject)result.AsyncState
                : State;
        }

        /// <summary>
        /// Updates state to provided one.
        /// </summary>
        /// <param name="state">State to update/set with.</param>
        public void SetState(StateObject state)
        {
            this.State = state;
        }
    }
}