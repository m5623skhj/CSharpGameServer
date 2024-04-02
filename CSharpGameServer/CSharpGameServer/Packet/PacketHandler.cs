﻿using CSharpGameServer.Core;
using CSharpGameServer.Protocol;

namespace CSharpGameServer
{
    public partial class PacketHandlerManager
    {
        public static void HandlePing(Client client, RequestPacket packet)
        {
            Ping? ping = packet as Ping;
            if (ping == null) 
            {
                return;
            }

            Pong pong = new Pong();
        }
    }
}