using CSharpGameServer.Core;
using CSharpGameServer.DB.Migration;

namespace CSharpGameServer.GameServer
{
    public class GameServer : ServerBase
    {
        private const string ServerName = "GameServer";

        public bool Run()
        {
            Console.WriteLine("------------ Try migration ------------");
            if (MigrationRunner.Instance.RunMigration() == false)
            {
                Console.WriteLine("------------ Migration failed ------------");
                return false;
            }

            Console.WriteLine("------------ Migration succeeded ------------");

            return Run(ServerName, new GameServerCore());
        }
    }
}
