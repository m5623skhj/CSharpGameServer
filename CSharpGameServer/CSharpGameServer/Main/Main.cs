using CSharpGameServer.Core;
using CSharpGameServer.DB.Migration;
using CSharpGameServer.Logger;

class Program
{
    private static bool IsMigrationSuccess(int migrationResult)
    {
        return migrationResult == 1;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("------------ Try migration ------------");
        if (IsMigrationSuccess(MigrationRunner.Instance.RunMigration()) == false)
        {
            Console.WriteLine("------------ Migration failed ------------");
        }
        Console.WriteLine("------------ Migration succeded ------------");

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