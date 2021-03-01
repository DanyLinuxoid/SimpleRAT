using RAT.src.Interfaces;
using RAT.src.Models;
using System;
using System.Net.Sockets;

namespace RAT.src.Logic.Sockets.Connection
{
    /// <summary>
    /// Socket connection stage handling logic for client.
    /// </summary>
    public class SocketConnectionSendingLogic : ISocketConnectionSendingLogic
    {
        private readonly ISocketStateLogic _socketStateLogic;
        private readonly IClientNotificationLogic _notificationLogic;

        /// <summary>
        /// Socket connection stage handling logic for client.
        /// </summary>
        /// <param name="stateLogic">Our client state logic.</param>
        public SocketConnectionSendingLogic(
            ISocketStateLogic stateLogic,
            IClientNotificationLogic notificationLogic)
        {
            _socketStateLogic = stateLogic;
            _notificationLogic = notificationLogic;
        }

        /// <summary>
        /// Sends specified data to client on specified socket and then executes post send method (async callback)
        /// i.e if file was sended then OnFileSend will be executed, if simple string was sended (message) then OnMessageSend, etc.
        /// </summary>
        /// <param name="socket">Socket to send data by.</param>
        /// <param name="data">Data to send in byte format.</param>
        /// <param name="OnDataSend">Async callback that will be executed after data is sended.</param>
        public void SendDataToClient(Socket socket, byte[] data, AsyncCallback OnDataSend)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, 0, OnDataSend, socket);
            }
            catch (Exception exception)
            {
                _notificationLogic.NotifyClient($"Error during send: {exception.Message}");
            }
        }

        /// <summary>
        /// Event that will be triggered as result of message sending.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        public void OnMessageSend(IAsyncResult res)
        {
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(res);
            state.ClientSocket.EndSend(res);
        }

        /// <summary>
        /// Event that will be triggered as result of file sending.
        /// </summary>
        /// <param name="asyncResult">State of async result.</param>
        public void OnFileSend(IAsyncResult asyncResult)
        {
            // Signal to remote that file download finished.
            StateObject state = _socketStateLogic.GetStateFromAsyncResult(asyncResult);
            Socket fileDownloadSocket = state.ClientFileDownloadSocket;
            fileDownloadSocket.EndSend(asyncResult);

            // Signal to client that download finished.
            _notificationLogic.NotifyClient($"\nFile download finished\n");
        }
    }
}