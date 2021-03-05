using System.Net;
using System.Net.Sockets;

namespace RAT.Interfaces
{
    /// <summary>
    /// Socket configuration logic.
    /// </summary>
    public interface ISocketConfigurationLogic
    {
        /// <summary>
        /// Gets fully configured and "ready-to-use" socket.
        /// </summary>
        /// <returns>Configured socket.</returns>
        Socket GetConfiguredSocket();

        /// <summary>
        /// Gets configuration for local endpoint which later can be used with socket configuration.
        /// </summary>
        /// <returns><see cref="IPEndPoint"/> class which holds all needed information about local endpoint.</returns>
        IPEndPoint GetLocalEndpointConfigurationForStandartSocket();

        /// <summary>
        /// Gets configuration for remote endpoint which later can be used with socket configuration for file download.
        /// </summary>
        /// <param name="ipAddress">Ip address of remote endpoint.</param>
        /// <returns><see cref="IPEndPoint"/> class which holds all needed information about local endpoint.</returns>
        IPEndPoint GetRemoteEndpointConfigurationForFileDownloadSocket(IPAddress ipAddress);
    }
}