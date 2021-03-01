namespace RAT.src.Interfaces
{
    /// <summary>
    /// Class responsible for file handling, read, write, etc.
    /// </summary>
    public interface IFileLogic
    {
        /// <summary>
        /// Tries to read bytes of file by provided path.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>Array filled with file bytes, if error occured then returns empty array.</returns>
        byte[] GetFileBytesByPath(string path);
    }
}