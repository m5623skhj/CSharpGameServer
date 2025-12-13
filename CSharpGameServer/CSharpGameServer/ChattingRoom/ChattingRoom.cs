using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;
using CSharpGameServer.PC;

namespace CSharpGameServer.ChattingRoom
{
    public class ChattingRoom
    {
        private readonly HashSet<Pc> members = [];
        private readonly Lock membersLock = new();

        public void AddMember(Pc pc)
        {
            lock (membersLock)
            {
                members.Add(pc);
            }
        }

        public void RemoveMember(Pc pc)
        {
            lock (membersLock)
            {
                members.Remove(pc);
            }
        }

        public void RoomBroadcastMessage(string message)
        {
            ChatMessagePacket packet = new()
            {
                Data = new ChatMessage()
                {
                    Message = message
                }
            };

            RoomBroadcast(packet);
        }

        private void RoomBroadcast(ReplyPacket packet)
        {
            lock (membersLock)
            {
                foreach (var member in members)
                {
                    member.Send(packet);
                }
            }
        }
    }
}
