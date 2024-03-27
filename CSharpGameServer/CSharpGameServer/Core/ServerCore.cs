using CSharpGameServer.Protocol;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSharpGameServer.Core
{
    public class ServerCore
    {
        private static ServerCore? instance = null;

        private Socket? listenSocket = null;
        private int port = 0;
        private int backlogSize = 0;
        private ulong atomicSessionId = 1;

        private const int bufferSize = 2048;

        private Dictionary<ulong, Client> clientDict = new Dictionary<ulong, Client>();

        public static ServerCore Instance
        {
            get
            {
                if (instance == null)
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
            SocketAsyncEventArgs acceptEventArgs = new SocketAsyncEventArgs();
            acceptEventArgs.Completed += AcceptCompleted;

            if (listenSocket == null)
            {
                return;
            }

            if (listenSocket.AcceptAsync(acceptEventArgs) == false)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            Socket? clientSocket = acceptEventArgs.AcceptSocket;
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

            StartReceive(newClient);

            clientDict.Add(newSessionId, newClient);
            acceptEventArgs.Completed += (sender, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    CloseClient(newSessionId);
                }
            };

            acceptEventArgs.AcceptSocket = null;
        }

        private void AcceptCompleted(object? sender, SocketAsyncEventArgs acceptEventArgs)
        {
            ProcessAccept(acceptEventArgs);
            StartAccept();
        }

        private void StartReceive(Client client)
        {
            var receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.SetBuffer(new byte[bufferSize], 0, bufferSize);
            receiveEventArgs.Completed += ReceiveCompleted;
            receiveEventArgs.UserToken = client;

            if (client.socket.ReceiveAsync(receiveEventArgs) == false)
            {
                ProcessReceive(receiveEventArgs);
            }
        }

        private void ReceiveCompleted(object? sender, SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.UserToken == null)
            {
                return;
            }

            ProcessReceive(receiveEventArgs);
            StartReceive((Client)receiveEventArgs.UserToken);
        }

        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.UserToken == null)
            {
                return;
            }

            var receivedClient = (Client)receiveEventArgs.UserToken;
            if (receiveEventArgs.SocketError != SocketError.Success 
                || receiveEventArgs.BytesTransferred <= 0
                || receiveEventArgs.Buffer == null)
            {
                CloseClient(receivedClient.clientSessionId);
                return;
            }

            string receivedData = Encoding.ASCII.GetString(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred);
            Packet? createdPacket = GetPacketFromReceivedData(receivedData);
            if (createdPacket == null)
            {
                CloseClient(receivedClient.clientSessionId);
                return;
            }

            createdPacket.HandlePacket(receivedClient);
        }

        private Packet? GetPacketFromReceivedData(string receivedData)
        {
            if (receivedData.Length < 4)
            {
                return null;
            }

            int.TryParse(receivedData.Substring(0, 4), out int packetType);
            return PacketFactory.Instance.CreatePacket((PacketType)packetType);
        }

        public void Send(Client client, string inData)
        {
            byte[] data = Encoding.ASCII.GetBytes(inData);
            var sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(data, 0, data.Length);
            sendEventArgs.UserToken = client;

            client.socket.SendAsync(sendEventArgs);
        }

        private void CloseClient(ulong closedClientSessionid)
        {
            var closeClient = clientDict[closedClientSessionid];
            if (closeClient != null)
            {
                closeClient.OnClosed();
                clientDict.Remove(closedClientSessionid);
            }
        }
    }
}
