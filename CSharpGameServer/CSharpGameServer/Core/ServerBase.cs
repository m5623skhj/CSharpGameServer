using CSharpGameServer.Logger;

namespace CSharpGameServer.Core
{
    public abstract class ServerBase
    {
        protected void Run(string serverName)
        {
            ServerCore.Instance.Run();
            LoggerManager.Instance.WriteLogDebug("------------ " + serverName + " Server running ------------");
            ServerRunning();

            ServerCore.Instance.Stop();
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
