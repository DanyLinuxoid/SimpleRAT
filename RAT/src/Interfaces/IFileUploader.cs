namespace RAT.src.Interfaces
{
    /// <summary>
    /// File upload feature for rat/backdoor (to victim PC).
    /// </summary>
    public interface IFileUploader
    {
        /// <summary>
        /// Does preparations for file upload.
        /// </summary>
        /// <param name="path">Path on which file will be uploaded.</param>
        /// <param name="fileSize">Size of file that will be uploaded.</param>
        void PrepareForFileUpload(string path, int fileSize);

        /// <summary>
        /// Uploads file data to specified path.
        /// </summary>
        /// <param name="path">Path to file that should be created with data.</param>
        /// <param name="data">Data that should be written to created file.</param>
        void UploadFile(string path, byte[] data);
    }
}