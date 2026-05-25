using CSharpGameServer;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TestClient.Main
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 10001;

            using var client = new TcpClient(ip, port);
            using var stream = client.GetStream();

            var pingPacket = new PingPacket
            {
                Header = new PacketHeader
                {
                    PacketType = (int)PacketType.Ping,
                    PacketSize = (ushort)Marshal.SizeOf<PingPacket>()
                }
            };

            var sendData = StructToBytes(pingPacket);
            stream.Write(sendData, 0, sendData.Length);

            var recvData = new byte[Marshal.SizeOf<PongPacket>()];
            var recvBytes = stream.Read(recvData, 0, recvData.Length);
            if (recvBytes < Marshal.SizeOf<PongPacket>())
            {
                Console.WriteLine("Failed to receive a full pong packet");
                return;
            }

            var pongPacket = BytesToStruct<PongPacket>(recvData);
            var receivedPacketType = (PacketType)pongPacket.Header.PacketType;

            Console.WriteLine("Received Packet Type : " + receivedPacketType);
        }

        private static byte[] StructToBytes<T>(T packet) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var buffer = new byte[size];
            var pointer = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(packet, pointer, false);
                Marshal.Copy(pointer, buffer, 0, size);
                return buffer;
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
        }

        private static T BytesToStruct<T>(byte[] buffer) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var pointer = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.Copy(buffer, 0, pointer, size);
                return Marshal.PtrToStructure<T>(pointer);
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
        }
    }
}
