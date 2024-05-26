namespace CSharpGameServer.Core
{
    public class ClientManager
    {
        private static ClientManager? instance = null;
        private Dictionary<ulong, Client> sessionIdToClientDict = new Dictionary<ulong, Client>();

        object sessionIdToClientDictLock = new object();

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
            lock (sessionIdToClientDictLock)
            {
                sessionIdToClientDict.Clear();
            }
        }

        public void CloseAllSession()
        {
            lock (sessionIdToClientDictLock)
            {
                foreach (var client in sessionIdToClientDict.Values)
                {
                    ServerCore.Instance.CloseClient(client.clientSessionId);
                }
            }
        }

        public void InsertSessionIdToClient(ulong sessionId, Client client)
        {
            lock (sessionIdToClientDictLock)
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

        public Client? FindBySessionId(ulong sessionId)
        {
            lock (sessionIdToClientDictLock)
            {
                sessionIdToClientDict.TryGetValue(sessionId, out Client? client);
                return client;
            }
        }
    }
}
