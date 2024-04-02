using CSharpGameServer.Core;

class Program
{
    static void Main(string[] args)
    {
        ServerCore serverCore = ServerCore.Instance;
        serverCore.Run();
        ServerRunning();

        serverCore.Stop();
        Console.WriteLine("------------  Server stopped ------------");
    }

    private static void ServerRunning()
    {
        Console.WriteLine("------------  Server running  ------------");

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