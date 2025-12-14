using CSharpGameServer.etc;
using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;
using CSharpGameServer.PC;

namespace CSharpGameServer.ChattingRoom
{
    public class ChattingRoom
    {
        private readonly Dictionary<ulong, string> memberNames = [];
        private readonly Lock membersLock = new();

        public bool AddMember(ulong id, string name)
        {
            lock (membersLock)
            {
                return memberNames.TryAdd(id, name);
            }
        }

        public void RemoveMember(ulong id)
        {
            lock (membersLock)
            {
                memberNames.Remove(id);
            }
        }

        public void SendMessage(string message)
        {
            ChatMessagePacket packet = new();

            unsafe
            {
                fixed (byte* pMessage = packet.Data.Message)
                {
                    FixedStringUtil.Write(message, pMessage, 30);
                }
            }

            RoomBroadcast(packet);
        }

        public bool IsEmptyRoom()
        {
            lock (membersLock)
            {
                return memberNames.Count == 0;
            }
        }

        private void RoomBroadcast(ReplyPacket packet)
        {
            lock (membersLock)
            {
                foreach (var pc in memberNames.Select(member => PcManager.Instance.FindPc(member.Key)).OfType<Pc>())
                {
                    pc.Send(packet);
                }
            }
        }
    }
}
