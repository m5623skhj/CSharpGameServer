using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public partial class PacketHandlerManager
    {
        public static void HandlePing(Client client, RequestPacket packet)
        {
            if (packet is not PingPacket pingpacket)
            {
                return;
            }

            client.HandlePing(pingpacket);
        }

    }
}