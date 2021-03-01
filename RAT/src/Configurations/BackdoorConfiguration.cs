namespace RAT.src.Configuration
{
    /// <summary>
    /// Class is full representation of JSON configuration for BackdoorConfig.json file.
    /// </summary>
    public class BackdoorConfiguration
    {
        /// <summary>
        /// Socket connection port.
        /// </summary>
        public int ConnectionPort { get; set; }

        /// <summary>
        /// Port over which files can be downloaded from our rat/backdoor.
        /// </summary>
        public int FileDownloadPort { get; set; }
    }
}