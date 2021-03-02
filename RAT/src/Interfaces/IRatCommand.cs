namespace RAT.src.Interfaces
{
    /// <summary>
    /// Represents command for rat.
    /// </summary>
    public interface IRatCommand
    {
        /// <summary>
        /// Gives ability to execute command.
        /// </summary>
        /// <param name="command">Command with arguments which will be processed.</param>
        void Execute(string command);

        /// <summary>
        /// Gives ability to abort command.
        /// </summary>
        void Abort();
    }
}