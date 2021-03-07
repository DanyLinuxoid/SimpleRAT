using System.Net.Sockets;

namespace RAT.Interfaces
{
    /// <summary>
    /// Contains logic to process commands(cmd, rat) received by client.
    /// </summary>
    public interface ISocketConnectionCommandReceiveLogic
    {
        /// <summary>
        /// Wrapper for command receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        void BeginCommandReceive(Socket socket);
    }
}