namespace CSharpGameServer.Core
{
    public class ClientManager
    {
        private ServerCore serverCore = null!;
        private static ClientManager? _instance;
        private readonly Dictionary<ulong, Client> sessionIdToClientDict = new();

        private readonly Lock sessionIdToClientDictLock = new();

        public static ClientManager Instance => _instance ??= new ClientManager();

        public void SetServerCore(ServerCore inServerCore)
        {
            serverCore = inServerCore;
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
                    serverCore.CloseClient(client.ClientSessionId);
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

        public void RemoveSessionIdToClient(ulong sessionId)
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
                sessionIdToClientDict.TryGetValue(sessionId, out var client);
                return client;
            }
        }
    }
}
