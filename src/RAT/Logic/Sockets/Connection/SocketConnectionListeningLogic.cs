using RAT.Configurations;
using RAT.Interfaces;
using RAT.Models;
using System.Net;
using System.Net.Sockets;

namespace RAT.Logic.Sockets.Connection
{
    /// <summary>
    /// Class contains logic for socket initial setup before socket messaging system start, listening logic.
    /// </summary>
    public class SocketConnectionListeningLogic : ISocketConnectionListeningLogic
    {
        private readonly ISocketConnectionAcceptLogic _socketConnectionAcceptLogic;
        private readonly ISocketConfigurationLogic _socketConfiguration;

        /// <summary>
        /// Class contains logic for socket initial setup before socket messaging system start, listening logic.
        /// </summary>
        /// <param name="acceptLogic">Logic for connection acceptance.</param>
        /// <param name="socketConfigurator">Socket and endpoint configurations.</param>
        public SocketConnectionListeningLogic(
            ISocketConnectionAcceptLogic acceptLogic,
            ISocketConfigurationLogic socketConfigurator)
        {
            _socketConnectionAcceptLogic = acceptLogic;
            _socketConfiguration = socketConfigurator;
        }

        /// <summary>
        /// Configures socket, starts listening, and when client connection, then delegates process to connection logic.
        /// </summary>
        public void StartListening()
        {
            // Get socket configuration with local endpoint binding.
            Socket socket = _socketConfiguration.GetConfiguredSocket();
            IPEndPoint localEndPoint = _socketConfiguration.GetLocalEndpointConfigurationForStandartSocket();

            socket.Bind(localEndPoint);

            socket.Listen(backlog: 100);

            // Waiting for someone to knock.
            while (true)
            {
                // Block thread until someone knocks.
                ManualResetEventWrapper.ResetEvent.Reset();
                _socketConnectionAcceptLogic.BeginAccept(socket);
                ManualResetEventWrapper.ResetEvent.WaitOne();
            }
        }
    }
}