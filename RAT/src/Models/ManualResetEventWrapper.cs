using System.Threading;

namespace RAT.src.Models
{
    /// <summary>
    /// Wrapper for <see cref="ManualResetEvent"/>.
    /// </summary>
    public static class ManualResetEventWrapper
    {
        /// <summary>
        /// Reset event, stops memory grows and cpu eating while listening in loop, this is instead of using ugly thread blocking calls.
        /// </summary>
        public static ManualResetEvent ResetEvent { get; } = new ManualResetEvent(initialState: false);
    }
}