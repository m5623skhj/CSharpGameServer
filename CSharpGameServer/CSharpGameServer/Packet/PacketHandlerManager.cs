using System.Diagnostics.CodeAnalysis;
using CSharpGameServer.Core;
using CSharpGameServer.Logger;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public partial class PacketHandlerManager
    {
        private readonly Dictionary<PacketType, Action<Client, RequestPacket>> packetHandlerDict = new();

        [field: AllowNull, MaybeNull]
        public static PacketHandlerManager Instance
        {
            get
            {
                if (field != null)
                {
                    return field;
                }

                field = new PacketHandlerManager();
                field.packetHandlerDict.Clear();

                return field;
            }
        }

        public bool RegisterPacketHandler(PacketType packetType, Action<Client, RequestPacket> handler)
        {
            if (packetType == PacketType.InvalidPacketType)
            {
                LoggerManager.Instance.WriteLogError("Invalid packet type {handler.Method.Name}", handler.Method.Name);
                return false;
            }

            if (packetHandlerDict.TryAdd(packetType, handler))
            {
                return true;
            }

            LoggerManager.Instance.WriteLogError("Duplicated packet type {packetType} / {handler.Method.Name}", packetType, handler.Method.Name);
            return false;
        }

        public void CallHandler(Client client, RequestPacket packet)
        {
            if (packetHandlerDict.TryGetValue(packet.Type, out var action) == false)
            {
                LoggerManager.Instance.WriteLogError("Invalid packet type {client.clientSessionId} / {packet.type}", client.ClientSessionId, packet.Type);
                return;
            }

            client.RefreshRecvTime();
            action(client, packet);
        }
    }
}
