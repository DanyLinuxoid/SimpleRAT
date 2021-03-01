using RAT.src.Cmd;
using RAT.src.Configurations;
using RAT.src.Interfaces;
using RAT.src.Logic.Files;
using RAT.src.Logic.Notifications;
using RAT.src.Logic.RatCommands;
using RAT.src.Logic.RatCommands.Features;
using RAT.src.Logic.Sockets;
using RAT.src.Logic.Sockets.Connection;
using SimpleInjector;

namespace RAT
{
    public class DependencyInjectionConfigurator
    {
        public Container GetConfiguredContainer()
        {
            var container = new Container();

            // Currently all instances are registered as singletons because RAT is supposed to work only
            // with one client at the time, so there is no need for multiple instances.
            this.RegisterLogic(container);
            this.RegisterConfigurations(container);
            this.RegisterSocketConnections(container);
            this.RegisterRatFeatures(container);
            return container;
        }

        private void RegisterSocketConnections(Container container)
        {
            container.RegisterSingleton<ISocketConnectionConnectLogic, SocketConnectionConnectLogic>();
            container.RegisterSingleton<ISocketConnectionListeningLogic, SocketConnectionListeningLogic>();
            container.RegisterSingleton<ISocketConnectionAcceptLogic, SocketConnectionAcceptLogic>();
            container.RegisterSingleton<ISocketConnectionDisconnectLogic, SocketConnectionDisconnectLogic>();
            container.RegisterSingleton<ISocketConnectionReceiveLogic, SocketConnectionReceiveLogic>();
            container.RegisterSingleton<ISocketConnectionSendingLogic, SocketConnectionSendingLogic>();
        }

        private void RegisterRatFeatures(Container container)
        {
            container.RegisterSingleton<IRatCommandLogic, RatCommandLogic>();
            container.RegisterSingleton<IFileDownloader, FileDownloader>();
        }

        private void RegisterLogic(Container container)
        {
            container.RegisterSingleton<ISocketStateLogic, SocketStateLogic>(); // Logic for socket/client state.
            container.RegisterSingleton<IFileLogic, FileLogic>();
            container.RegisterSingleton<ICmdLogic, CmdOutputHandler>();
            container.RegisterSingleton<IClientNotificationLogic, ClientNotificationLogic>();
        }

        private void RegisterConfigurations(Container container)
        {
            container.RegisterSingleton<ISocketConfigurator, SocketConfigurator>();
        }
    }
}