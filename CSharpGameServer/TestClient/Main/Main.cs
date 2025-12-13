using CSharpGameServer;
using System.Net.Sockets;
using System.Text;
using TestClient.Client;

namespace TestClient.Main
{
    internal class Program
    {
        private static ChattingClient? client;

        private static string ip = "127.0.0.1";
        private static int port = 10001;

        private static void Main(string[] args)
        {
            client = new ChattingClient(ip, port);


        }
    }
}