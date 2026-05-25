using CSharpGameServer.Logger;

namespace CSharpGameServer.Core
{
    public abstract class ServerBase
    {
        public bool Run(string serverName, ServerCore? targetServerCore = null)
        {
            var serverCore = targetServerCore ?? new ServerCore();
            if (serverCore.Initialize() == false)
            {
                LoggerManager.Instance.WriteLogError("------------ {serverName} Server initialize failed ------------", serverName);
                return false;
            }

            try
            {
                serverCore.Run();
                LoggerManager.Instance.WriteLogDebug("------------ {serverName} Server running ------------", serverName);
                ServerRunning();
                return true;
            }
            finally
            {
                serverCore.Stop();
                LoggerManager.Instance.WriteLogDebug("------------ {serverName} Server stopped ------------", serverName);
            }
        }

        private static void ServerRunning()
        {
            var running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            {
                                running = false;
                            }
                            break;
                        default:
                        {
                        }
                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
