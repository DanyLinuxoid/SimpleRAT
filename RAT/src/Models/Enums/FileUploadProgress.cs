namespace RAT.src.Models.Enums
{
    /// <summary>
    /// Progress of file upload.
    /// </summary>
    public enum FileUploadProgress
    {
        /// <summary>
        /// No file is being uploaded.
        /// </summary>
        None,

        /// <summary>
        /// File upload begins.
        /// </summary>
        Begins,

        /// <summary>
        /// File upload is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// File upload is finished.
        /// </summary>
        Finished,
    }
}