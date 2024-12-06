using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public static class PacketRegisterList
    {
        public static bool RegisterAllPacket()
        {
            bool result = true;

            result &= RegisterPacket(new Ping());

            return result;
        }

        private static bool RegisterPacket(RequestPacket packet)
        {
            return packet.RegisterPacket();
        }
    }
}
