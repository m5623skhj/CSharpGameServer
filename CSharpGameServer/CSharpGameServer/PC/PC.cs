using CSharpGameServer.PC.PCComponent;

namespace TestGameServer.PC
{
    public class PC
    {
        private ulong sessionId = 0;
        private ulong pcId = 0;

        private ComponentManager componentManager = new ComponentManager();

        PC(ulong inSessionId)
        {
            sessionId = inSessionId;
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
