using CSharpGameServer.Protocol;
using System.Runtime.InteropServices;
using System.Text;

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
                    instance.packetTypeDict.Clear();
                }

                return instance;
            }
        }

        public bool RegisterPacket(PacketType packetType, Type packetObjectType)
        {
            if (packetType == PacketType.InvalidPacketType)
            {
                Console.WriteLine("Invalid packet type {0}", packetObjectType.GetType());
                return false;
            }

            if (packetTypeDict.ContainsKey(packetType))
            {
                Console.WriteLine("Duplicated packet type {0} / {1}", packetType, packetObjectType);
                return false;
            }

            packetTypeDict[packetType] = packetObjectType;
            return true;
        }

        public RequestPacket? CreatePacket(string receivedData)
        {
            if (receivedData.Length < 4)
            {
                return null;
            }

            int.TryParse(receivedData.Substring(0, 4), out int packetType);
            if (packetTypeDict.TryGetValue((PacketType)packetType, out Type? packetObjectType) == false)
            {
                Console.WriteLine("Invalid packet type {0}", packetType);
                return null;
            }

            if (typeof(RequestPacket).IsAssignableFrom(packetObjectType) == false)
            {
                Console.WriteLine("Packet type {0} is valid but is not assignable", packetType);
                return null;
            }

            byte[] recvStream = Encoding.UTF8.GetBytes(receivedData);
            return ToStr(recvStream, packetObjectType) as RequestPacket;
        }

        private object? ToStr(byte[] data, Type type)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            object? result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            handle.Free();
            return result;
        }
    }
}
