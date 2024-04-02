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

    public partial class Ping : RequestPacket
    {
        public override void SetPacketType()
        {
            type = PacketType.Ping;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandlePing;
        }
    }

    public abstract class ReplyPacket : PacketBase
    {
    }

    public class Pong : ReplyPacket
    {
        public override void SetPacketType()
        {
            type = PacketType.Pong;
        }
    }
}
