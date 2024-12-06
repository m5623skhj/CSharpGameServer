using CSharpGameServer.Core;

namespace CSharpGameServer.PC
{
    public class PcManager
    {
        private ServerCore? serverCore = null;

        private static PcManager? instance = null;
        private Dictionary<ulong, Pc> pcIdToPcDict = new Dictionary<ulong, Pc>();

        object pcIdToPcDictLock = new object();

        public static PcManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PcManager();
                }

                return instance;
            }
        }

        public void SetServerCore(ServerCore? inServerCore)
        {
            serverCore = inServerCore;
        }

        public void InsertPc(Pc pc, ulong pcId)
        {
            lock (pcIdToPcDictLock)
            {
                pcIdToPcDict.Add(pcId, pc);
            }
        }

        public void RemovePc(ulong pcId)
        {
            lock (pcIdToPcDictLock)
            {
                pcIdToPcDict.Remove(pcId);
            }
        }

        public Pc? FindPc(ulong pcId)
        {
            Pc? findPc;
            lock (pcIdToPcDictLock)
            {
                bool isFind = pcIdToPcDict.TryGetValue(pcId, out findPc);
                if (isFind == false)
                {
                    return null;
                }
            }

            return findPc;
        }
    }
}
