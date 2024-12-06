namespace CSharpGameServer.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer.GameServer gameServer = new GameServer.GameServer();
            gameServer.Run();
        }
    }
}