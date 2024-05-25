using CSharpGameServer.Core;
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
                Console.WriteLine("Invalid packet type {0}", handler.Method.Name);
                return false;
            }

            if (packetHandlerDict.ContainsKey(packetType))
            {
                Console.WriteLine("Duplicated packet type {0} / {1}", packetType, handler.Method.Name);
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
                Console.WriteLine("Invalid packet type {0} / {1}", client.clientSessionId, packet.type);
                return;
            }

            action(client, packet);
        }
    }
}
