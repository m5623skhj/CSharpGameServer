using CSharpGameServer.Core;
using System;

class Program
{
    static void Main(string[] args)
    {
        ServerCore serverCore = ServerCore.Instance;
        serverCore.Initialize();

        serverCore.Run();
    }
}