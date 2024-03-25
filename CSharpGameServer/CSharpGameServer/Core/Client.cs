using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGameServer.Core
{
    public class Client
    {
        private Socket socket;
        private ulong clientSessionId;

        public Client(Socket inSocket, ulong inClientSessionId) 
        {
            socket = inSocket;
            clientSessionId = inClientSessionId;
        }

        public void OnClosed()
        {

        }
    }
}
