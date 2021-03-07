using System.Collections.Generic;

namespace RAT.Interfaces
{
    /// <summary>
    /// Class responsible for file handling, read, write, etc.
    /// </summary>
    public interface IFileLogic
    {
        /// <summary>
        /// Checks if file on path is valid by checking if it exists and if size more than 0 bytes.
        /// </summary>
        /// <param name="path">Path to file to check.</param>
        /// <returns>True if file is valid, false otherwise.</returns>
        bool IsValidFile(string path);
    }
}