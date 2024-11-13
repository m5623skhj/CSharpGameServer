using CSharpGameServer.GameServer;

namespace TestServer.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer gameServer = new GameServer();
            gameServer.Run();
        }
    }
}