using CSharpGameServer.Packet;

namespace CSharpGameServer.PC
{
    public partial class Pc
    {
        public override void HandlePing(Ping ping)
        {
            var pong = new Pong();
            Send(pong);
        }
    }
}
