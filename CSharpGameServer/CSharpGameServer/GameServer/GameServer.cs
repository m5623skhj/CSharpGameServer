using CSharpGameServer.Core;

namespace CSharpGameServer.GameServer
{
    public class GameServer : ServerBase
    {
        string serverName = "GameServer";

        public void Run()
        {
            Run(serverName);
        }
    }
}
