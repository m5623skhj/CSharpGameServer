using CSharpGameServer.Core;
using CSharpGameServer.Logger;

namespace CSharpGameServer.GameServer
{
    public class GameServer : ServerBase
    {
        private const string ServerName = "ChattingServerServer";

        public void Run()
        {
            LoggerManager.Instance.WriteLogInfo("Starting {ServerName}...", ServerName);
            Run(ServerName, new GameServerCore());
            LoggerManager.Instance.WriteLogInfo("{ServerName} has stopped.", ServerName);
        }
    }
}
