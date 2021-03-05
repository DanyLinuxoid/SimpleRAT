using RAT.Interfaces;
using RAT.Models.Enums;
using System;
using System.Collections.Generic;

namespace RAT.Logic.Factories
{
    /// <summary>
    /// Factory for rat command clases.
    /// </summary>
    public class RatCommandFactory : IRatCommandFactory
    {
        /// <summary>
        /// Collection of all available rat commands.
        /// </summary>
        private Dictionary<RatAvailableCommands, Type> _ratCommandCollection { get; } = new Dictionary<RatAvailableCommands, Type>();

        /// <summary>
        /// Factory for rat command clases.
        /// </summary>
        public RatCommandFactory()
        {
            _ratCommandCollection.Add(RatAvailableCommands.DownloadFile, typeof(IDownloadFileCommand));
            _ratCommandCollection.Add(RatAvailableCommands.UploadFile, typeof(IUploadFileCommand));
            _ratCommandCollection.Add(RatAvailableCommands.Help, typeof(IShowHelpCommand));
        }

        /// <summary>
        /// Gets rat command by specified enum value.
        /// </summary>
        /// <param name="command">Rat enum available command.</param>
        /// <returns>Rat command class responsible for command specified in enum.</returns>
        public IRatCommand GetRatCommand(RatAvailableCommands command)
        {
            _ratCommandCollection.TryGetValue(command, out Type commandObject);
            return (IRatCommand)DependencyInjectionConfigurator.DependencyContainer.GetInstance(commandObject);
        }
    }
}