using RAT.src.Cmd;
using RAT.src.Configurations;
using RAT.src.Interfaces;
using RAT.src.Logic.Factories;
using RAT.src.Logic.Files;
using RAT.src.Logic.Notifications;
using RAT.src.Logic.RatCommands;
using RAT.src.Logic.RatCommands.Commands;
using RAT.src.Logic.RatCommands.Features;
using RAT.src.Logic.Sockets;
using RAT.src.Logic.Sockets.Connection;
using SimpleInjector;

namespace RAT
{
    public static class DependencyInjectionConfigurator
    {
        /// <summary>
        /// Our dependency container.
        /// </summary>
        public static Container DependencyContainer { get; private set; }

        public static Container GetConfiguredContainer()
        {
            var container = new Container();

            // Currently all instances are registered as singletons because RAT is supposed to work only
            // with one client at the time, so there is no need for multiple instances.
            RegisterCommonLogic(container);
            RegisterConfigurations(container);
            RegisterSocketConnections(container);
            RegisterRatFeatures(container);
            RegisterFactories(container);
            return container;
        }

        public static void SetContainer(Container container)
        {
            DependencyContainer = container;
        }

        private static void RegisterSocketConnections(Container container)
        {
            container.RegisterSingleton<ISocketConnectionConnectLogic, SocketConnectionConnectLogic>();
            container.RegisterSingleton<ISocketConnectionListeningLogic, SocketConnectionListeningLogic>();
            container.RegisterSingleton<ISocketConnectionAcceptLogic, SocketConnectionAcceptLogic>();
            container.RegisterSingleton<ISocketConnectionDisconnectLogic, SocketConnectionDisconnectLogic>();
            container.RegisterSingleton<ISocketConnectionReceiveLogic, SocketConnectionReceiveLogic>();
            container.RegisterSingleton<ISocketConnectionSendingLogic, SocketConnectionSendingLogic>();
        }

        private static void RegisterRatFeatures(Container container)
        {
            container.RegisterSingleton<IRatCommandLogic, RatCommandLogic>();
            container.RegisterSingleton<IFileDownloader, FileDownloader>();
            container.RegisterSingleton<IFileUploader, FileUploader>();
            RegisterCommands(container);
        }

        private static void RegisterCommonLogic(Container container)
        {
            container.RegisterSingleton<ISocketStateLogic, SocketStateLogic>(); // Logic for socket/client state.
            container.RegisterSingleton<IFileLogic, FileLogic>();
            container.RegisterSingleton<ICmdLogic, CmdOutputHandler>();
            container.RegisterSingleton<IClientNotificationLogic, ClientNotificationLogic>();
            container.RegisterSingleton<IRatCommandInterpreter, RatCommandInterpreter>();
        }

        private static void RegisterConfigurations(Container container)
        {
            container.RegisterSingleton<ISocketConfigurator, SocketConfigurator>();
        }

        private static void RegisterFactories(Container container)
        {
            container.RegisterSingleton<IRatCommandFactory, RatCommandFactory>();
        }

        private static void RegisterCommands(Container container)
        {
            container.Register<IDownloadFileCommand, DownloadFileCommand>();
            container.Register<IUploadFileCommand, UploadFileCommand>();
        }
    }
}