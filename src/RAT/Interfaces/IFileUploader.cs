using System.IO;

namespace RAT.Interfaces
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
        void PrepareForFileUpload(string path);
    }
}