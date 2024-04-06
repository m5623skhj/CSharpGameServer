namespace CSharpGameServer.Core
{
    public class ClientManager
    {
        private static ClientManager? instance = null;
        private Dictionary<ulong, Client> sessionIdToClientDict = new Dictionary<ulong, Client>();
        private Dictionary<ulong, ulong> pcIdToSessionIdDict = new Dictionary<ulong, ulong>();

        object sessionIdToClientDictLock = new object();
        object pcIdToSessionIdDictLock = new object();

        public static ClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientManager();
                }

                return instance;
            }
        }

        private ClientManager()
        {
            ClearDict();
        }

        ~ClientManager()
        {
            ClearDict();
        }

        private void ClearDict()
        {
            lock(sessionIdToClientDictLock)
            {
                sessionIdToClientDict.Clear();
            }

            lock(pcIdToSessionIdDictLock)
            {
                pcIdToSessionIdDict.Clear();
            }
        }

        public void CloseAllSession()
        {
            lock(sessionIdToClientDictLock)
            {
                foreach (var client in sessionIdToClientDict.Values)
                {
                    ServerCore.Instance.CloseClient(client.clientSessionId);
                }
            }
        }

        public void InsertSessionIdToClient(ulong sessionId, Client client)
        {
            lock(sessionIdToClientDictLock)
            {
                sessionIdToClientDict.Add(sessionId, client);
            }
        }

        public void RemoveSessionidToClient(ulong sessionId)
        {
            lock (sessionIdToClientDictLock)
            {
                sessionIdToClientDict.Remove(sessionId);
            }
        }

        public void InsertPCIdToSessionId(ulong pcId, ulong sessionId) 
        {
            lock (pcIdToSessionIdDictLock)
            {
                pcIdToSessionIdDict.Add(pcId, sessionId);
            }
        }

        public void RemovePCIdToSessionId(ulong pcId)
        {
            lock (pcIdToSessionIdDictLock)
            {
                pcIdToSessionIdDict.Remove(pcId);
            }
        }

        public Client? FindBySessionId(ulong sessionId)
        {
            lock (sessionIdToClientDictLock)
            {
                sessionIdToClientDict.TryGetValue(sessionId, out Client? client);
                return client;
            }
        }

        public Client? FindByPCId(ulong pcId)
        {
            ulong sessionId = 0;
            lock (pcIdToSessionIdDictLock)
            {
                bool isFind = pcIdToSessionIdDict.TryGetValue(pcId, out sessionId);
                if (isFind == false)
                {
                    return null;
                }
            }

            return FindBySessionId(sessionId);
        }
    }
}
