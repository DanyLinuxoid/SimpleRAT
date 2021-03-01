using RAT.src.Models.Enums;
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
        public byte[] CommandDataArray { get; set; } = new byte[1024];

        /// <summary>
        /// Holds received data in string representation.
        /// </summary>
        public StringBuilder DataBuilder { get; set; } = new StringBuilder();

        /// <summary>
        /// Client socket on which connection currently happens.
        /// </summary>
        public Socket ClientMainSocket { get; set; }

        /// <summary>
        /// Client socket for file downloads.
        /// </summary>
        public Socket ClientFileDownloadSocket { get; set; }

        /// <summary>
        /// Cmd process related to client.
        /// </summary>
        public Process ClientCmdProcess { get; set; }

        ///------------------- CAN GO TO MODEL CLASS FOR FILE DOWNLOAD
        public Socket ClientFileUploadSocket { get; set; } // ----- TEMP

        public CurrentOperation CurrentOperation { get; set; } // ----- TEMP

        public byte[] FileDataArray { get; set; }

        public StringBuilder FileDataBuilder { get; set; } = new StringBuilder(); // ----- TEMP

        public string PathForFileUpload { get; set; } // ----- TEMP
        ///------------------- CAN GO TO MODEL CLASS FOR FILE DOWNLOAD

        /// <summary>
        /// Resets client connection state and kills cmd process.
        /// </summary>
        public void ResetState()
        {
            CommandDataArray = new byte[1024];
            FileDataArray = new byte[0];
            DataBuilder = new StringBuilder();
            FileDataBuilder = new StringBuilder();
            CurrentOperation = CurrentOperation.None;
            PathForFileUpload = string.Empty;
            ClientCmdProcess.Kill();
        }
    }
}