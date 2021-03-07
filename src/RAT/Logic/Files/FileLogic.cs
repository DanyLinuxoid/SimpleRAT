using RAT.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace RAT.Logic.Files
{
    /// <summary>
    /// Class responsible for file handling, read, write, etc.
    /// </summary>
    public class FileLogic : IFileLogic
    {
        private readonly IClientNotificationLogic _notificationLogic;

        /// <summary>
        /// Class responsible for file handling, read, write, etc.
        /// </summary>
        /// <param name="notificationLogic">Logic for sending notifications to client.</param>
        public FileLogic(IClientNotificationLogic notificationLogic)
        {
            _notificationLogic = notificationLogic;
        }

        /// <summary>
        /// Checks if file on path is valid by checking if it exists and if size more than 0 bytes.
        /// </summary>
        /// <param name="path">Path to file to check.</param>
        /// <returns>True if file is valid, false otherwise.</returns>
        public bool IsValidFile(string path)
        {
            if (!File.Exists(path))
            {
                _notificationLogic.NotifyClient($"\nFile on path \"{path}\" does not exist or is directory\n");
                return false;
            }

            long fileSize = 0;
            try
            {
                fileSize = new FileInfo(path).Length;
            }
            catch (Exception exception)
            {
                _notificationLogic.NotifyClient($"Error during file check: {exception.Message}");
            }

            return fileSize <= 0;
        }
    }
}