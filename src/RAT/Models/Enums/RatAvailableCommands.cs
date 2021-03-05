namespace RAT.Models.Enums
{
    /// <summary>
    /// Represents all available operations for program.
    /// </summary>
    public enum RatAvailableCommands
    {
        /// <summary>
        /// File downloading option (from victim => client).
        /// </summary>
        DownloadFile = 1,

        /// <summary>
        /// File upload option (from client => victim).
        /// </summary>
        UploadFile = 2,

        /// <summary>
        /// Option to abort any current pending operation.
        /// </summary>
        Abort = 3,

        /// <summary>
        /// Option to show help to client.
        /// </summary>
        Help = 4,
    }
}