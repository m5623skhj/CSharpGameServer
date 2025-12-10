namespace CSharpGameServer.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameServer = new GameServer.GameServer();
            gameServer.Run();
        }
    }
}