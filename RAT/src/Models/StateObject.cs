using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RAT.src.Models
{
    public class StateObject
    {
        // Receive buffer.
        public byte[] DataArray { get; set; } = new byte[1024];

        // Received data string.
        public StringBuilder DataBuilder { get; set; } = new StringBuilder();

        // Client socket.
        public Socket ClientSocket { get; set; }

        // Async state.
        public IAsyncResult AsyncState { get; set; }

        public Process ClientCmdProcess { get; set; }
    }
}