namespace CSharpGameServer.Main
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var gameServer = new GameServer.GameServer();
            if (gameServer.Run() == false)
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
