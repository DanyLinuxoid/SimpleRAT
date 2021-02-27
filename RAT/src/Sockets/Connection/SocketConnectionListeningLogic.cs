using RAT.src.Configurations;
using RAT.src.Interfaces;
using RAT.src.Models;
using System.Net;
using System.Net.Sockets;

namespace RAT.src.Sockets.Connection
{
    /// <summary>
    /// Class contains logic for socket initial setup before socket messaging system start, listening logic.
    /// </summary>
    public class SocketConnectionListeningLogic : ISocketConnectionListeningLogic
    {
        private ISocketConnectionAcceptLogic _socketConnectionAcceptLogic { get; }

        /// <summary>
        /// Class contains logic for socket initial setup before socket messaging system start, listening logic.
        /// </summary>
        /// <param name="acceptLogic">Logic for connection acceptance.</param>
        public SocketConnectionListeningLogic(ISocketConnectionAcceptLogic acceptLogic)
        {
            _socketConnectionAcceptLogic = acceptLogic;
        }

        /// <summary>
        /// Configures socket, starts listening, and when client connection, then delegates process to connection logic.
        /// </summary>
        public void StartListening()
        {
            // Get socket configuration with local endpoint binding.
            var socketConfigurator = new SocketConfigurator();
            Socket socket = socketConfigurator.GetConfiguredSocket();
            IPEndPoint localEndPoint = socketConfigurator.GetLocalEndpointConfigurationForSocket();

            socket.Bind(localEndPoint);

            socket.Listen(backlog: 100);

            // Waiting for someone to knock.
            while (true)
            {
                // Block thread until someone knocks.
                ManualResetEventWrapper.ResetEvent.Reset();
                socket.BeginAccept(_socketConnectionAcceptLogic.OnAccept, socket);
                ManualResetEventWrapper.ResetEvent.WaitOne();
            }
        }
    }
}