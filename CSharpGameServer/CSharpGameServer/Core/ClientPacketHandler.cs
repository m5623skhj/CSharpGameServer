using CSharpGameServer.Packet;

namespace CSharpGameServer.Core
{
    public partial class Client
    {
        public virtual void HandlePing(Ping ping) { }
    }
}
