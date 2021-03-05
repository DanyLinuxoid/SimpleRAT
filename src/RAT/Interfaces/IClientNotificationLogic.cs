namespace RAT.Interfaces
{
    /// <summary>
    /// Logic for sending notification messages to client.
    /// </summary>
    public interface IClientNotificationLogic
    {
        /// <summary>
        /// Notifies client with specified message, uses main client socket for notifications.
        /// </summary>
        /// <param name="message">Message to send to client.</param>
        void NotifyClient(string message);
    }
}