using CSharpGameServer.Core;
using CSharpGameServer.PC;
using System.Net.Sockets;

namespace CSharpGameServer.GameServer
{
    public class GameServerCore : ServerCore
    {
        public GameServerCore()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            PCManager.Instance.SetServerCore(this);
        }

        protected override void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            Socket? clientSocket = acceptEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref atomicSessionId);
            var newPC = new PC.PC(this, clientSocket, newSessionId);
            if (newPC == null)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(StartReceive, newPC);

            ClientManager.Instance.InsertSessionIdToClient(newSessionId, newPC);
            acceptEventArgs.Completed += (sender, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    CloseClient(newSessionId);
                }
            };

            acceptEventArgs.AcceptSocket = null;
        }
    }
}
