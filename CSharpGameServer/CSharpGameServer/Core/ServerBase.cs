using CSharpGameServer.Logger;

namespace CSharpGameServer.Core
{
    public abstract class ServerBase
    {
        public void Run(string serverName, ServerCore? targetServerCore = null)
        {
            ServerCore? serverCore = null;
            if (targetServerCore == null)
            {
                serverCore = new ServerCore();
            }
            else
            {
                serverCore = targetServerCore;
            }

            serverCore.Run();
            LoggerManager.Instance.WriteLogDebug("------------ " + serverName + " Server running ------------");
            ServerRunning();

            serverCore.Stop();
            LoggerManager.Instance.WriteLogDebug("------------ " + serverName + " Server stopped ------------");
        }

        protected void ServerRunning()
        {
            bool running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            {
                                running = false;
                            }
                            break;
                        default:
                            break;
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
