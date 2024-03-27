using CSharpGameServer.Protocol;

namespace CSharpGameServer
{
    public class PacketFactory
    {
        private static PacketFactory? instance = null;
        private Dictionary<PacketType, Type> packetTypeDict = new Dictionary<PacketType, Type>();

        public static PacketFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PacketFactory();
                }

                return instance;
            }
        }

        public void RegisterPacket(PacketType packetType, Type packetObjectType)
        {
            packetTypeDict[packetType] = packetObjectType;
        }

        public Packet? CreatePacket(PacketType packetType)
        {
            if (packetTypeDict.TryGetValue(packetType, out Type? packetObjectType) == false)
            {
                Console.WriteLine("Invalid packet type {0}", packetType);
                return null;
            }

            if (typeof(Packet).IsAssignableFrom(packetObjectType) == false)
            {
                Console.WriteLine("Packet type {0} is valid but is not assignable", packetType);
                return null;
            }

            return Activator.CreateInstance(packetObjectType) as Packet;
        }
    }
}
