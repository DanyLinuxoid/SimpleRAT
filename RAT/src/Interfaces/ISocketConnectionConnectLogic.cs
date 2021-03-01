using System.Net;
using System.Net.Sockets;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Logic for making connections to sockets.
    /// </summary>
    public interface ISocketConnectionConnectLogic
    {
        /// <summary>
        /// Wrapper for BeginConnect method.
        /// </summary>
        /// <param name="socket">Socket for file downloads.</param>
        /// <param name="endPoint">Endpoint with which socket should be connected.</param>
        void BeginFileSocketConnect(Socket socket, IPEndPoint endPoint);

        /// <summary>
        /// Gets configured and connected socket for file downloads.
        /// </summary>
        /// <returns>Configured socket for file downloadds.</returns>
        Socket GetConnectedSocketForFileDownload();
    }
}