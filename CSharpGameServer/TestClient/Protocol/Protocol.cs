using System.Runtime.InteropServices;

namespace CSharpGameServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public int PacketType;
        public ushort PacketSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ping
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Pong
    {
        public PacketHeader Header;
    }

}