using CSharpGameServer.Protocol;

namespace CSharpGameServer
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
