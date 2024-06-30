using CSharpGameServer.Core;

namespace CSharpGameServer.Protocol
{
    public abstract class PacketBase
    {
        public PacketBase() 
        {
            SetPacketType();
        }

        public PacketType type = PacketType.InvalidPacketType;
        public abstract void SetPacketType();
    }

    public abstract class RequestPacket : PacketBase
    {
        protected abstract Action<Client, RequestPacket> GetHandler();

        public bool RegisterPacket()
        {
            return PacketFactory.Instance.RegisterPacket(type, GetType()) || 
                PacketHandlerManager.Instance.RegisterPacketHandler(type, GetHandler());
        }
    }

    public abstract class ReplyPacket : PacketBase
    {
    }
}
