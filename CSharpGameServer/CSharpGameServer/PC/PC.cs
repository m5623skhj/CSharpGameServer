using CSharpGameServer.Core;
using CSharpGameServer.PC.PCComponent;
using System.Net.Sockets;

namespace CSharpGameServer.PC
{
    public partial class Pc : Client
    {
        private ulong sessionId = 0;
        private ulong pcId = 0;

        private ComponentManager componentManager = new ComponentManager();

        public delegate void CallbackForInitFromDbComplete();

        public override void OnClosed() 
        {
        
        }

        public override void OnSend()
        {

        }

        public Pc(ServerCore inServerCore, Socket inSocket, ulong inClientSessionId) 
            : base(inServerCore, inSocket, inClientSessionId)
        {
        }

        public void OnPCIdLoadCompleted(ulong inPcId)
        {
            pcId = inPcId;
            PcInitializeFromDb();
        }

        public void OnDBInitializeCompleted()
        {
            Logger.LoggerManager.Instance.WriteLogInfo("Loading completed pcId : {0}", pcId);

        }

        private void PcInitializeFromDb()
        {
            CallbackForInitFromDbComplete callback = new CallbackForInitFromDbComplete(OnDBInitializeCompleted);
        }
    }
}
