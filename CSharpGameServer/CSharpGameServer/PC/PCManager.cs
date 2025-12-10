using CSharpGameServer.Core;

namespace CSharpGameServer.PC
{
    public class PcManager
    {
        private ServerCore? serverCore;

        private static PcManager? _instance;
        private readonly Dictionary<ulong, Pc> pcIdToPcDict = new();

        private readonly object pcIdToPcDictLock = new();

        public static PcManager Instance => _instance ??= new PcManager();

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
                var isFind = pcIdToPcDict.TryGetValue(pcId, out findPc);
                if (isFind == false)
                {
                    return null;
                }
            }

            return findPc;
        }
    }
}
