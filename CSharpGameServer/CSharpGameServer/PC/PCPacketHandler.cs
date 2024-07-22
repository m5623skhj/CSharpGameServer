﻿using CSharpGameServer.Core;
using CSharpGameServer.Packet;

namespace CSharpGameServer.PC
{
    public partial class PC : Client
    {
        public override void HandlePing(Ping ping)
        {
            Pong pong = new Pong();
            Send(pong);
        }
    }
}
