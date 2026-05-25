using CSharpGameServer.Core;
using CSharpGameServer.PC;
using System.Net.Sockets;

namespace CSharpGameServer.GameServer
{
    public class GameServerCore : ServerCore
    {
        public override bool Initialize()
        {
            if (base.Initialize() == false)
            {
                return false;
            }

            PcManager.Instance.SetServerCore(this);
            return true;
        }

        protected override void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            var clientSocket = acceptEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref AtomicSessionId);
            var newPc = new Pc(this, clientSocket, newSessionId);

            ThreadPool.QueueUserWorkItem(StartReceive, newPc);

            ClientManager.Instance.InsertSessionIdToClient(newSessionId, newPc);
            acceptEventArgs.AcceptSocket = null;
        }
    }
}
