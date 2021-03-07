using RAT.Models.Enums;
using System.IO;
using System.Net.Sockets;

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
        /// Buffer to store file data during upload.
        /// NOTE: Currently 1MB is set for receival buffer, there is no need to place more unless
        /// https://github.com/nmap/nmap/issues/1026 will be resolved.
        /// </summary>
        public int FileDataArraySizeInBytes = 1 * 1024 * 1024; // 1MB

        /// <summary>
        /// Contains file data in byte format.
        /// </summary>
        public byte[] FileDataArray { get; set; }

        /// <summary>
        /// Our current file handle that is in progress of uploading.
        /// </summary>
        public FileStream FileHandle { get; set; }

        /// <summary>
        /// Path to which file is uploaded.
        /// </summary>
        public string PathForFileUpload { get; set; }
    }
}