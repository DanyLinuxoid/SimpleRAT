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
        /// <summary>
        /// Holds received data in byte representation.
        /// </summary>
        public byte[] DataArray { get; set; } = new byte[1024];

        /// <summary>
        /// Holds received data in string representation.
        /// </summary>
        public StringBuilder DataBuilder { get; set; } = new StringBuilder();

        /// <summary>
        /// Client socket on which connection currently happens.
        /// </summary>
        public Socket ClientSocket { get; set; }

        /// <summary>
        /// Client socket for file downloads.
        /// </summary>
        public Socket ClientFileDownloadSocket { get; set; }

        /// <summary>
        /// Cmd process related to client.
        /// </summary>
        public Process ClientCmdProcess { get; set; }

        /// <summary>
        /// Resets client connection state and kills cmd process.
        /// </summary>
        public void ResetState()
        {
            DataArray = new byte[1024];
            DataBuilder = new StringBuilder();
            ClientCmdProcess.Kill();
        }
    }
}