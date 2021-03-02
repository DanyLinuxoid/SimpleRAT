using RAT.src.Interfaces;
using RAT.src.Models.Enums;

namespace RAT.src.Interfaces
{
    /// <summary>
    /// Factory for rat command clases.
    /// </summary>
    public interface IRatCommandFactory
    {
        /// <summary>
        /// Gets rat command by specified enum value.
        /// </summary>
        /// <param name="command">Rat enum available command.</param>
        /// <returns>Rat command class responsible for command specified in enum.</returns>
        IRatCommand GetRatCommand(RatAvailableCommands command);
    }
}