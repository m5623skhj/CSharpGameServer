using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;
using System.Runtime.InteropServices;

namespace CSharpGameServer.Packet
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Pong : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Pong;
        }
    }

}