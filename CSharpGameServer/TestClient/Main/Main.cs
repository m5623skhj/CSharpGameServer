using CSharpGameServer;
using System.Net.Sockets;
using System.Text;

namespace TestClient.Main
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 10001;

            var client = new TcpClient(ip, port);
            using var stream = client.GetStream();

            const PacketType packetType = PacketType.Ping;
            var sendData = Encoding.ASCII.GetBytes(packetType.ToString());

            stream.Write(sendData, 0, sendData.Length);

            var recvData = new byte[512];
            var recvBytes = stream.Read(recvData, 0, recvData.Length);
            var packet = Encoding.ASCII.GetString(recvData, 0, recvBytes);

            if (Enum.TryParse(packet, out PacketType receivedPacketType))
            {
                Console.WriteLine("Received Packet Type : " + receivedPacketType);
            }
            else
            {
                Console.WriteLine("Failed to parse received packet type");
            }

            stream.Close();
            client.Close();
        }
    }
}