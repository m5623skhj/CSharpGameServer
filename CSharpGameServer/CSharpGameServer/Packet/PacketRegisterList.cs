using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public static class PacketRegisterList
    {
        public static bool RegisterAllPacket()
        {
            var result = true;

            result &= RegisterPacket(new PingPacket());
            result &= RegisterPacket(new CreateRoomPacket());
            result &= RegisterPacket(new JoinRoomPacket());
            result &= RegisterPacket(new LeaveRoomPacket());
            result &= RegisterPacket(new SendChatPacket());
            result &= RegisterPacket(new SetMyNamePacket());

            return result;
        }

        private static bool RegisterPacket(RequestPacket packet)
        {
            return packet.RegisterPacket();
        }
    }
}
