using RAT.Cmd;
using RAT.Interfaces;
using RAT.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;

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
        /// Creates new state for new connection.
        /// </summary>
        /// <param name="handler">Connection related to client/state.</param>
        public StateObject CreateNewState(Socket handler)
        {
            // Create the state object with cmd process attached to client.
            // This represents client with all needed information.
            var clientState = new StateObject()
            {
                ClientMainSocket = handler,
                FileUploadInformation = new FileUploadInformation(),
            };

            this.State = clientState;
            return clientState;
        }
    }
}