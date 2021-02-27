using RAT.src.Cmd;
using RAT.src.Interfaces;
using RAT.src.Rat;
using RAT.src.SocketLogic;
using RAT.src.Sockets;
using RAT.src.Sockets.Connection;
using SimpleInjector;

namespace RAT
{
    public class DependencyInjectionConfigurator
    {
        public Container GetConfiguredContainer()
        {
            var container = new Container();
            container.RegisterSingleton<ICmdLogic, CmdLogic>();
            container.RegisterSingleton<ISocketStateLogic, SocketStateLogic>();
            container.RegisterSingleton<IRatCommandLogic, RatCommandLogic>();

            this.RegisterSocketConnections(container);

            return container;
        }

        private void RegisterSocketConnections(Container container)
        {
            container.RegisterSingleton<ISocketConnectionListeningLogic, SocketConnectionListeningLogic>();
            container.RegisterSingleton<ISocketConnectionAcceptLogic, SocketConnectionAcceptLogic>();
            container.RegisterSingleton<ISocketConnectionDisconnectLogic, SocketConnectionDisconnectLogic>();
            container.RegisterSingleton<ISocketConnectionReceiveLogic, SocketConnectionReceiveLogic>();
            container.RegisterSingleton<ISocketConnectionSendingLogic, SocketConnectionSendingLogic>();
        }
    }
}