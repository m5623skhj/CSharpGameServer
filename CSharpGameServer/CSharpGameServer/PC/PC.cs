using CSharpGameServer.Core;
using CSharpGameServer.PC.PCComponent;
using System.Net.Sockets;

namespace CSharpGameServer.PC
{
    public partial class Pc(ServerCore inServerCore, Socket inSocket, ulong inClientSessionId)
        : Client(inServerCore, inSocket, inClientSessionId)
    {
        private ulong sessionId;
        private ulong pcId;

        private ComponentManager componentManager = new ComponentManager();

        public delegate void CallbackForInitFromDbComplete();

        public override void OnClosed() 
        {
        
        }

        public override void OnSend()
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
            var callback = new CallbackForInitFromDbComplete(OnDBInitializeCompleted);
        }
    }
}
