using RAT.Models.Enums;
using System.Net.Sockets;
using System.Text;

namespace RAT.Models
{
    /// <summary>
    /// Model that holds information about file, that is inprocess of uploading or will be uploaded.
    /// </summary>
    public class FileUploadInformation
    {
        /// <summary>
        /// Socket over which file uploading happens.
        /// </summary>
        public Socket ClientFileUploadSocket { get; set; }

        /// <summary>
        /// Progress of file upload.
        /// </summary>
        public FileUploadProgress FileUploadProgress { get; set; }

        /// <summary>
        /// Contains file data in byte format.
        /// </summary>
        public byte[] FileDataArray { get; set; }

        /// <summary>
        /// Contains uploaded data in Base64 string format.
        /// </summary>
        public StringBuilder FileDataBuilder { get; set; } = new StringBuilder();

        /// <summary>
        /// Path to which file is uploaded.
        /// </summary>
        public string PathForFileUpload { get; set; }
    }
}