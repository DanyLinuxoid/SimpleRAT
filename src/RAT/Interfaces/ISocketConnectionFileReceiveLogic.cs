using System.Net.Sockets;

namespace RAT.Interfaces
{
    /// <summary>
    /// Contains logic to process file data received by client.
    /// </summary>
    public interface ISocketConnectionFileReceiveLogic
    {
        /// <summary>
        /// Wrapper for file data receive.
        /// </summary>
        /// <param name="socket">Our connection.</param>
        void BeginFileDataReceive(Socket socket);
    }
}