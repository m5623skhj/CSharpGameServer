using CSharpGameServer.DB.Migration;
using CSharpGameServer.GameServer;

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

        GameServer gameServer = new GameServer();
        gameServer.Run();
   }
}