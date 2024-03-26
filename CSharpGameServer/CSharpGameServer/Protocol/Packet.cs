using CSharpGameServer.Core;

namespace CSharpGameServer.Protocol
{
    public enum PacketType
    { 
        InvalidPacketType = 0,
    }

    public abstract class Packet
    {
        public PacketType type = PacketType.InvalidPacketType;

        public abstract void HandlePacket(Packet thisPacket, Client client);
    }
}
