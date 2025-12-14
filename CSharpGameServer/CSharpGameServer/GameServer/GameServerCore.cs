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
            base.ProcessAccept(acceptEventArgs);
        }

        protected override Client MakeClient(Socket clientSocket, ulong sessionId)
        {
            return new Pc(this, clientSocket, sessionId);
        }
    }
}