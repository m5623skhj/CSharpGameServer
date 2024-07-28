using CSharpGameServer.Core;
using CSharpGameServer.DB.Migration;

namespace CSharpGameServer.GameServer
{
    public class GameServer : ServerBase
    {
        private const int migrationSuccessResult = 1;
        private string serverName = "GameServer";

        private static bool IsMigrationSuccess(int migrationResult)
        {
            return migrationResult == migrationSuccessResult;
        }

        public void Run()
        {
            Console.WriteLine("------------ Try migration ------------");
            if (IsMigrationSuccess(MigrationRunner.Instance.RunMigration()) == false)
            {
                Console.WriteLine("------------ Migration failed ------------");
            }
            Console.WriteLine("------------ Migration succeded ------------");

            Run(serverName, new GameServerCore());
        }
    }
}
