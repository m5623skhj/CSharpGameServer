using CSharpGameServer;
using System.Net.Sockets;
using System.Text;

namespace TestClient.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 10001;

            TcpClient client = new TcpClient(ip, port);
            NetworkStream stream = client.GetStream();

            PacketType packetType = PacketType.Ping;
            byte[] sendData = Encoding.ASCII.GetBytes(packetType.ToString());

            stream.Write(sendData, 0, sendData.Length);

            byte[] recvData = new byte[512];
            int recvBytes = stream.Read(recvData, 0, recvData.Length);
            string packet = Encoding.ASCII.GetString(recvData, 0, recvBytes);

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