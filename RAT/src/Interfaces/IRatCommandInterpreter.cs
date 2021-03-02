namespace RAT.src.Interfaces
{
    /// <summary>
    /// Interpreter for rat commands.
    /// </summary>
    public interface IRatCommandInterpreter
    {
        /// <summary>
        /// Checks if command can be used by interpreter further, if it is defined in list of commands.
        /// </summary>
        /// <param name="command">Rat command.</param>
        /// <returns>Combination of check result and formatted command.</returns>
        (bool, string) IsKnownRatCommand(string command);

        /// <summary>
        /// Determines if first word of string is command for rat.
        /// </summary>
        /// <param name="command">Command from internet i.e "cd .." for shell, or "dir", whatever.</param>
        /// <returns>True if command is rat command, false is something else.</returns>
        bool IsRatCommand(string command);

        /// <summary>
        /// Gets arguments that were provided with command.
        /// </summary>
        /// <param name="command">Command in format "RAT some command -a arg1 -b arg2".</param>
        /// <returns>String converted to array of arguments with options.</returns>
        string[] GetArgumentsForCommand(string command);
    }
}