namespace CSharpGameServer.PC
{
    public class PCManager
    {
        private static PCManager? instance = null;
        private Dictionary<ulong, PC> pcIdToPCDict = new Dictionary<ulong, PC>();

        object pcIdToPCDictLock = new object();

        public static PCManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PCManager();
                }

                return instance;
            }
        }

        public void InsertPC(PC pc, ulong pcId)
        {
            lock (pcIdToPCDictLock)
            {
                pcIdToPCDict.Add(pcId, pc);
            }
        }

        public void RemovePC(ulong pcId)
        {
            lock (pcIdToPCDictLock)
            {
                pcIdToPCDict.Remove(pcId);
            }
        }

        public PC? FindPC(ulong pcId)
        {
            PC? findPC;
            lock (pcIdToPCDictLock)
            {
                bool isFind = pcIdToPCDict.TryGetValue(pcId, out findPC);
                if (isFind == false)
                {
                    return null;
                }
            }

            return findPC;
        }
    }
}
