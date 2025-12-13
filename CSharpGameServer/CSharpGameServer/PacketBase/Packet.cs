using CSharpGameServer.Core;
using CSharpGameServer.Packet;

namespace CSharpGameServer.PacketBase
{
    public abstract class PacketBase
    {
        protected PacketBase() 
        {
            SetPacketType();
        }

        public PacketType Type = PacketType.InvalidPacketType;
        public abstract void SetPacketType();
    }

    public abstract class RequestPacket : PacketBase
    {
        protected abstract Action<Client, RequestPacket> GetHandler();

        public bool RegisterPacket()
        {
            return (PacketFactory.Instance.RegisterPacket(Type, GetType()) || 
                    PacketHandlerManager.Instance.RegisterPacketHandler(Type, GetHandler()));
        }
    }

    public abstract class ReplyPacket : PacketBase
    {
    }
}
