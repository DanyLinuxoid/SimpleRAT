namespace RAT.Interfaces
{
    /// <summary>
    /// File downloading feature for rat/backdoor.
    /// </summary>
    public interface IFileDownloader
    {
        /// <summary>
        /// Downloads file by provided windows path for client over socket connection.
        /// </summary>
        /// <param name="path">Windows path of the file that should be downloaded.</param>
        void DownloadFile(string path);
    }
}