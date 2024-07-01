using CSharpGameServer.Core;
using CSharpGameServer.PC.PCComponent;
using System.Net.Sockets;

namespace CSharpGameServer.PC
{
    public partial class PC : Client
    {
        private ulong sessionId = 0;
        private ulong pcId = 0;

        private ComponentManager componentManager = new ComponentManager();

        public override void OnClosed() 
        {
        
        }

        public override void OnSend()
        {

        }

        public PC(Socket inSocket, ulong inClientSessionId) : base(inSocket, inClientSessionId)
        {
        }

        public void OnPCIdLoadCompleted(ulong inPCId)
        {
            pcId = inPCId;
            PCInitializeFromDB();
        }

        private void PCInitializeFromDB()
        {

        }
    }
}
