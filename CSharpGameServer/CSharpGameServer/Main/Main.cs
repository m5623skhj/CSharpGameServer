using CSharpGameServer.Core;

class Program
{
    static void Main(string[] args)
    {
        ServerCore serverCore = ServerCore.Instance;
        serverCore.Initialize();

        serverCore.Run();
    }
}