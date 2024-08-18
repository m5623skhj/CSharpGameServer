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

        public delegate void CallbackForInitFromDBComplete();

        public override void OnClosed() 
        {
        
        }

        public override void OnSend()
        {

        }

        public PC(ServerCore inServerCore, Socket inSocket, ulong inClientSessionId) 
            : base(inServerCore, inSocket, inClientSessionId)
        {
        }

        public void OnPCIdLoadCompleted(ulong inPCId)
        {
            pcId = inPCId;
            PCInitializeFromDB();
        }

        public void OnDBInitalizeCompleted()
        {
            Logger.LoggerManager.Instance.WriteLogInfo("Loading completed pcId : {0}", pcId);

        }

        private void PCInitializeFromDB()
        {
            CallbackForInitFromDBComplete callback = new CallbackForInitFromDBComplete(OnDBInitalizeCompleted);
        }
    }
}
