using RAT.Interfaces;
using System.Net.Sockets;

namespace RAT.Logic.Sockets.Connection
{
    /// <summary>
    /// Handles client data receival stage over socket and delegates work to logic.
    /// </summary>
    public class SocketConnectionReceiveLogic : ISocketConnectionReceiveLogic
    {
        private readonly ISocketConnectionCommandReceiveLogic _commandReceiveLogic;
        private readonly ISocketConnectionFileReceiveLogic _fileReceiveLogic;

        /// <summary>
        /// Handles client data receival stage over socket and delegates work to logic.
        /// </summary>
        /// <param name="commandReceiveLogic">Logic for command data receive.</param>
        /// <param name="fileReceiveLogic">Logic for file data receive.</param>
        public SocketConnectionReceiveLogic(
            ISocketConnectionCommandReceiveLogic commandReceiveLogic,
            ISocketConnectionFileReceiveLogic fileReceiveLogic)
        {
            _commandReceiveLogic = commandReceiveLogic;
            _fileReceiveLogic = fileReceiveLogic;
        }

        /// <summary>
        /// Wrapper for logic in command handler class.
        /// Delegates work to that.
        /// </summary>
        /// <param name="socket">Socket that holds client connection.</param>
        public void BeginCommandReceive(Socket socket)
        {
            _commandReceiveLogic.BeginCommandReceive(socket);
        }

        /// <summary>
        /// Wrapper for logic in file handler class.
        /// Delegates work to that.
        /// </summary>
        /// <param name="socket">Socket that holds client connection.</param>
        public void BeginFileReceive(Socket socket)
        {
            _fileReceiveLogic.BeginFileDataReceive(socket);
        }
    }
}