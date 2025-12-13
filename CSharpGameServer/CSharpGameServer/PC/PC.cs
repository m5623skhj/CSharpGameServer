using CSharpGameServer.Core;
using System.Net.Sockets;
using CSharpGameServer.ChattingRoom;
using CSharpGameServer.Packet;

namespace CSharpGameServer.PC
{
    public partial class Pc(ServerCore inServerCore, Socket inSocket, ulong inClientSessionId)
        : Client(inServerCore, inSocket, inClientSessionId)
    {
        public string Name { get; private set; } = "";

        public override void OnConnected()
        {
            ChattingRoomManager.Instance.OnEnterUser(ClientSessionId);
        }

        public override void OnClosed() 
        {
            ChattingRoomManager.Instance.OnLeaveUser(ClientSessionId);
        }

        public override void OnSend()
        {
        }

        public ErrorCode SetMyName(string inName)
        {
            const int nameMax = 10;
            if (inName.Length > nameMax || string.IsNullOrWhiteSpace(inName))
            {
                return ErrorCode.InvalidName;
            }

            Name = inName;
            return ErrorCode.Success;
        }
    }
}
