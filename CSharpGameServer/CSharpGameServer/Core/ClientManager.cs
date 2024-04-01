using CSharpGameServer.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGameServer.Core
{
    public class ClientManager
    {
        private static ClientManager? instance = null;
        private Dictionary<ulong, Client> sessionIdToClientDict = new Dictionary<ulong, Client>();
        private Dictionary<ulong, ulong> pcIdToSessionIdDict = new Dictionary<ulong, ulong>();

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
            sessionIdToClientDict.Clear();
            pcIdToSessionIdDict.Clear();
        }

        public void InsertSessionIdToClient(ulong sessionId, Client client)
        {
            sessionIdToClientDict.Add(sessionId, client);
        }

        public void RemoveSessionidToClient(ulong sessionId)
        {
            sessionIdToClientDict.Remove(sessionId);
        }

        public void InsertPCIdToSessionId(ulong pcId, ulong sessionId) 
        {
            pcIdToSessionIdDict.Add(pcId, sessionId);
        }

        public void RemovePCIdToSessionId(ulong pcId)
        {
            pcIdToSessionIdDict.Remove(pcId);
        }

        public Client? FindBySessionId(ulong sessionId)
        {
            sessionIdToClientDict.TryGetValue(sessionId, out Client? client);
            return client;
        }

        public Client? FindByPCId(ulong pcId)
        {
            bool isFinded = pcIdToSessionIdDict.TryGetValue(pcId, out ulong sessionid);
            if (isFinded == false)
            {
                return null;
            }

            return FindBySessionId(sessionid);
        }
    }
}
