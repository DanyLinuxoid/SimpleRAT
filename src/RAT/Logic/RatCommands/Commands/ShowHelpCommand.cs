using RAT.Interfaces;

namespace RAT.Logic.RatCommands.Commands
{
    /// <summary>
    /// Command to to show helping information to user.
    /// </summary>
    public class ShowHelpCommand : IShowHelpCommand
    {
        private readonly IClientNotificationLogic _notificationLogic;

        /// <summary>
        /// Command to to show helping information to user.
        /// </summary>
        /// <param name="clientNotificationLogic">Logic to send messages to client.</param>
        public ShowHelpCommand(IClientNotificationLogic clientNotificationLogic)
        {
            _notificationLogic = clientNotificationLogic;
        }

        /// <summary>
        /// Prints help/usage message to client.
        /// </summary>
        public void Execute(string command)
        {
            string message = this.GetHelpUsageMessage();
            _notificationLogic.NotifyClient(message);
        }

        /// <summary>
        /// Aborts operation.
        /// </summary>
        public void Abort() { }

        /// <summary>
        /// Gets help message for client.
        /// </summary>
        /// <returns>Help message for client.</returns>
        private string GetHelpUsageMessage()
        {
            return
                "\n---- HELP ----\n" +
                "Usage: RAT <command> <arguments>\n" +
                "Available commands: file upload, file download, abort, help\n" +
                "Examples:\n" +
                "       file uploading example: RAT file upload -p(path) path/to/exe -s(file size in bytes) 4096\n" +
                "       file downloading example: RAT file download -p(path) path/to/exe\n" +
                "       operation abort: RAT abort\n" +
                "       print help: RAT help\n";
        }
    }
}