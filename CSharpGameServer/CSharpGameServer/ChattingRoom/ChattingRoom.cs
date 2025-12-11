using CSharpGameServer.Packet;
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

        public void BroadcastMessage(string message, Pc? exceptPc = null)
        {
            ChatMessage packet = new()
            {
                Message = message
            };

            lock (membersLock)
            {
                foreach (var member in members.Where(member => member != exceptPc))
                {
                    member.Send(packet);
                }
            }
        }
    }
}
