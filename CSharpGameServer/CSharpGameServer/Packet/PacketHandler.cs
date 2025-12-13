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

        public static void HandleCreateRoom(Client client, RequestPacket packet)
        {
            if (packet is not CreateRoomPacket createroompacket)
            {
                return;
            }

            client.HandleCreateRoom(createroompacket);
        }

        public static void HandleJoinRoom(Client client, RequestPacket packet)
        {
            if (packet is not JoinRoomPacket joinroompacket)
            {
                return;
            }

            client.HandleJoinRoom(joinroompacket);
        }

        public static void HandleLeaveRoom(Client client, RequestPacket packet)
        {
            if (packet is not LeaveRoomPacket leaveroompacket)
            {
                return;
            }

            client.HandleLeaveRoom(leaveroompacket);
        }

        public static void HandleSendChat(Client client, RequestPacket packet)
        {
            if (packet is not SendChatPacket sendchatpacket)
            {
                return;
            }

            client.HandleSendChat(sendchatpacket);
        }

        public static void HandleSetMyName(Client client, RequestPacket packet)
        {
            if (packet is not SetMyNamePacket setmynamepacket)
            {
                return;
            }

            client.HandleSetMyName(setmynamepacket);
        }

    }
}