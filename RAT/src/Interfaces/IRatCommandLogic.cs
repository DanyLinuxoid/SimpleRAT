namespace RAT.src.Interfaces
{
    /// <summary>
    /// Our rat/backdoor commands handling logic.
    /// </summary>
    public interface IRatCommandLogic
    {
        /// <summary>
        /// Determines if string contains command for rat.
        /// </summary>
        /// <param name="command">Command from internet i.e "cd .." for shell, or "dir", whatever.</param>
        /// <returns>True if command is rat command, false is something else.</returns>
        bool IsRatCommand(string command);
    }
}