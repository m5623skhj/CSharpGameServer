using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public static class PacketRegisterList
    {
        public static bool RegisterAllPacket()
        {
            var result = true;

            result &= RegisterPacket(new PingPacket());

            return result;
        }

        private static bool RegisterPacket(RequestPacket packet)
        {
            return packet.RegisterPacket();
        }
    }
}
