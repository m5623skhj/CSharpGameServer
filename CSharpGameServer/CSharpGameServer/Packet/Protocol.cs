using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public class Ping : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Ping;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandlePing;
        }
    }

    public class Pong : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Pong;
        }
    }

}