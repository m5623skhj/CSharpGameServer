using CSharpGameServer.GameServer;

class Program
{
    static void Main(string[] args)
    {
        GameServer gameServer = new GameServer();
        gameServer.Run();
    }
}