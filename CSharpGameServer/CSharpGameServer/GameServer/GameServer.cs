using CSharpGameServer.Core;
using CSharpGameServer.DB.Migration;

namespace CSharpGameServer.GameServer
{
    public class GameServer : ServerBase
    {
        private const int MigrationSuccessResult = 1;
        private const string ServerName = "GameServer";

        private static bool IsMigrationSuccess(int migrationResult)
        {
            return migrationResult == MigrationSuccessResult;
        }

        public void Run()
        {
            Console.WriteLine("------------ Try migration ------------");
            if (IsMigrationSuccess(MigrationRunner.Instance.RunMigration()) == false)
            {
                Console.WriteLine("------------ Migration failed ------------");
            }
            Console.WriteLine("------------ Migration succeeded ------------");

            Run(ServerName, new GameServerCore());
        }
    }
}
