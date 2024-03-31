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

        public void RegisterPacket(PacketType packetType, Type packetObjectType)
        {
            packetTypeDict[packetType] = packetObjectType;
        }

        public Packet? CreatePacket(string receivedData)
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

            if (typeof(Packet).IsAssignableFrom(packetObjectType) == false)
            {
                Console.WriteLine("Packet type {0} is valid but is not assignable", packetType);
                return null;
            }

            byte[] recvStream = Encoding.UTF8.GetBytes(receivedData);
            return ToStr(recvStream, packetObjectType) as Packet;
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
