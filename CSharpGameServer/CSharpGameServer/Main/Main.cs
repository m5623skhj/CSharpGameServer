using CSharpGameServer.Core;
using CSharpGameServer.Logger;

class Program
{
    static void Main(string[] args)
    {
        ServerCore serverCore = ServerCore.Instance;
        serverCore.Run();
        LoggerManager.Instance.WriteLogDebug("------------  Server running  ------------");
        ServerRunning();

        serverCore.Stop();
        LoggerManager.Instance.WriteLogDebug("------------  Server stopped ------------");
    }

    private static void ServerRunning()
    {
        bool running = true;
        while (running)
        {
            if(Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        {
                            running = false;
                        }break;
                    default:
                        break;
                }
            }

            Thread.Sleep(1000);
        }
    }
}