namespace RAT.Interfaces
{
    /// <summary>
    /// Class contains logic for socket initial setup before socket messaging system start, listening logic.
    /// </summary>
    public interface ISocketConnectionListeningLogic
    {
        /// <summary>
        /// Configures socket, starts listening, and when client connection, then delegates process to connection logic.
        /// </summary>
        void StartListening();
    }
}