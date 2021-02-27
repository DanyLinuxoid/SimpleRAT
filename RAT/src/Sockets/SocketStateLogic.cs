using RAT.src.Interfaces;
using RAT.src.Models;
using System;
using System.Net.Sockets;

namespace RAT.src.Sockets
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
        /// Trivial steps to get socket and state from async result during socket connections.
        /// </summary>
        /// <param name="result">Status of async operation.</param>
        /// <returns>Socket and update state object.</returns>
        public (Socket, StateObject) GetSocketAndStateFromAsyncResult(IAsyncResult result)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            // IAsyncResult in this case can be StateObject or Socket.
            Socket handler = null;
            StateObject state = null;
            if (result.AsyncState is StateObject)
            {
                state = (StateObject)result.AsyncState;
                handler = state.ClientSocket;
            }
            else
            {
                // Get the socket that handles the client request
                // and update state object with it's values.
                handler = (Socket)result.AsyncState;
                state = State;
            }

            return (handler, state);
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