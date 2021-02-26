using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RAT.src.Models
{
    /// <summary>
    /// Represents our client with his current state.
    /// </summary>
    public class StateObject
    {
        // Receive buffer.
        public byte[] DataArray { get; set; } = new byte[1024];

        // Received data string.
        public StringBuilder DataBuilder { get; set; } = new StringBuilder();

        // Client socket on which connection currently happens.
        public Socket ClientSocket { get; set; }

        // Async socket state.
        public IAsyncResult AsyncState { get; set; }

        /// <summary>
        /// Cmd process related to client.
        /// </summary>
        public Process ClientCmdProcess { get; set; }
    }
}