using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGameServer.Core
{
    public class ServerCore
    {
        private static ServerCore? instance = null;

        private Socket? listenSocket = null;
        private int port = 0;
        private int backlogSize = 0;
        private ulong atomicSessionId = 1;

        private Dictionary<ulong, Client> clientDict = new Dictionary<ulong, Client>();

        public static ServerCore Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ServerCore();
                }

                return instance;
            }
        }

        private ServerCore()
        {
        }

        public void Initialize()
        {

        }

        public void Run()
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (listenSocket == null) 
            {
                throw new Exception("listen socket is null");
            }
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            if (backlogSize > 0)
            {
                listenSocket.Listen(backlogSize);
            }
            else
            {
                listenSocket.Listen();
            }

            StartAccept();
        }

        private void StartAccept()
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.Completed += AcceptCompleted;

            if (listenSocket == null)
            {
                return;
            }

            if (listenSocket.AcceptAsync(socketAsyncEventArgs) == false)
            {
                ProcessAccept(socketAsyncEventArgs);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            Socket? clientSocket = socketAsyncEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref atomicSessionId);
            var newClient = new Client(clientSocket, newSessionId);
            if (newClient == null)
            {
                return;
            }

            // StartReceive()?

            clientDict.Add(newSessionId, newClient);
            socketAsyncEventArgs.Completed += (sender, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    CloseClient(newSessionId);
                }
            };

            socketAsyncEventArgs.AcceptSocket = null;
        }

        private void AcceptCompleted(object? sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            ProcessAccept(socketAsyncEventArgs);
            StartAccept();
        }

        private void CloseClient(ulong closeClientSessionid)
        {
            var closeClient = clientDict[closeClientSessionid];
            if (closeClient != null)
            {
                closeClient.OnClosed();
                clientDict.Remove(closeClientSessionid);
            }
        }
    }
}
