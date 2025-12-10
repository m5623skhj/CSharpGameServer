using CSharpGameServer.Core;
using CSharpGameServer.PC;
using System.Net.Sockets;

namespace CSharpGameServer.GameServer
{
    public class GameServerCore : ServerCore
    {
        public override void Initialize()
        {
            base.Initialize();
            PcManager.Instance.SetServerCore(this);
        }

        protected override void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            Socket? clientSocket = acceptEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref AtomicSessionId);
            var newPc = new Pc(this, clientSocket, newSessionId);

            ThreadPool.QueueUserWorkItem(StartReceive, newPc);

            ClientManager.Instance.InsertSessionIdToClient(newSessionId, newPc);
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