using CSharpGameServer.Core;
using CSharpGameServer.Logger;
using CSharpGameServer.Protocol;

namespace CSharpGameServer
{
    public partial class PacketHandlerManager
    {
        private static PacketHandlerManager? instance = null;
        private Dictionary<PacketType, Action<Client, RequestPacket>> packetHandlerDict = new Dictionary<PacketType, Action<Client, RequestPacket>>();

        public static PacketHandlerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PacketHandlerManager();
                    instance.packetHandlerDict.Clear();
                }

                return instance;
            }
        }

        public bool RegisterPacketHandler(PacketType packetType, Action<Client, RequestPacket> handler)
        {
            if (packetType == PacketType.InvalidPacketType)
            {
                LoggerManager.Instance.WriteLogError("Invalid packet type {handler.Method.Name}", handler.Method.Name);
                return false;
            }

            if (packetHandlerDict.ContainsKey(packetType))
            {
                LoggerManager.Instance.WriteLogError("Duplicated packet type {pakcetType} / {handler.Method.Name}", packetType, handler.Method.Name);
                return false;
            }

            packetHandlerDict[packetType] = handler;
            return true;
        }

        public void CallHandler(Client client, RequestPacket packet)
        {
            if (packetHandlerDict.TryGetValue(packet.type, out Action<Client, RequestPacket>? action) == false)
            {
                // add client info
                LoggerManager.Instance.WriteLogError("Invalid packet type {client.clientSessionId} / {packet.type}", client.clientSessionId, packet.type);
                return;
            }

            client.RefreshRecvTime();
            action(client, packet);
        }
    }
}
