﻿using System;
using System.Net.Sockets;

namespace RAT.Interfaces
{
    /// <summary>
    /// Socket connection stage handling logic for client.
    /// </summary>
    public interface ISocketConnectionSendingLogic
    {
        /// <summary>
        /// Sends specified data to client on specified socket and then executes post send method (async callback)
        /// i.e if file was sended then OnFileSend will be executed, if simple string was sended (message) then OnMessageSend, etc.
        /// </summary>
        /// <param name="socket">Socket to send data by.</param>
        /// <param name="data">Data to send in byte format.</param>
        /// <param name="OnDataSend">Async callback that will be executed after data is sended.</param>
        void SendDataToClient(Socket socket, byte[] data, AsyncCallback OnDataSend);

        /// <summary>
        /// Event that will be triggered as result of message sending.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        void OnMessageSend(IAsyncResult res);

        /// <summary>
        /// Event that will be triggered as result of file sending.
        /// </summary>
        /// <param name="asyncResult">State of async result.</param>
        void OnFileSend(IAsyncResult asyncResult);
    }
}