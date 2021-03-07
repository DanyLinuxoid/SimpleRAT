using System.Diagnostics;

namespace RAT.Interfaces
{
    /// <summary>
    /// Logic related to windows cmd.
    /// </summary>
    public interface ICmdLogic
    {
        /// <summary>
        /// Creates and configures new cmd process.
        /// </summary>
        /// <returns>Launched cmd process.</returns>
        Process CreateNewCmdProcess();
    }
}