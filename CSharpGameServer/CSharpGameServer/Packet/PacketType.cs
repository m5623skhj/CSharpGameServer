using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGameServer
{
    public enum PacketType : int
    {
        InvalidPacketType = 0,
        Ping,
        Pong,
    }
}
