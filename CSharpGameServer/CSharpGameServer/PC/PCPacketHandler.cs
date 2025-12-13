using CSharpGameServer.Packet;

namespace CSharpGameServer.PC
{
    public partial class Pc
    {
        public override void HandlePing(PingPacket ping)
        {
            var pong = new PongPacket();
            Send(pong);
        }
    }
}
