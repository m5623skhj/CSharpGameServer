using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Packet
{
    public partial class PacketHandlerManager
    {
        public static void HandlePing(Client client, RequestPacket packet)
        {
            if (packet is not Ping ping)
            {
                return;
            }

            client.HandlePing(ping);
        }

        public static void HandleCreateRoom(Client client, RequestPacket packet)
        {
            if (packet is not CreateRoom createroom)
            {
                return;
            }

            client.HandleCreateRoom(createroom);
        }

        public static void HandleJoinRoom(Client client, RequestPacket packet)
        {
            if (packet is not JoinRoom joinroom)
            {
                return;
            }

            client.HandleJoinRoom(joinroom);
        }

        public static void HandleLeaveRoom(Client client, RequestPacket packet)
        {
            if (packet is not LeaveRoom leaveroom)
            {
                return;
            }

            client.HandleLeaveRoom(leaveroom);
        }

        public static void HandleSendChat(Client client, RequestPacket packet)
        {
            if (packet is not SendChat sendchat)
            {
                return;
            }

            client.HandleSendChat(sendchat);
        }

        public static void HandleGetRoomList(Client client, RequestPacket packet)
        {
            if (packet is not GetRoomList getroomlist)
            {
                return;
            }

            client.HandleGetRoomList(getroomlist);
        }

    }
}