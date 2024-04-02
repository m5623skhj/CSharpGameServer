using CSharpGameServer.Protocol;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
            Initialize();
        }

        private void Initialize()
        {
            if (PacketRegisterList.RegisterAllPacket() == false)
            {
                Console.WriteLine("RegisterAllPacket failed");
                return;
            }
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

        public void Stop()
        {

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

            ClientManager.Instance.InsertSessionIdToClient(newSessionId, newClient);
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
            RequestPacket? createdPacket = GetPacketFromReceivedData(receivedData);
            if (createdPacket == null)
            {
                CloseClient(receivedClient.clientSessionId);
                return;
            }

            PacketHandlerManager.Instance.CallHandler(receivedClient, createdPacket);
        }

        private RequestPacket? GetPacketFromReceivedData(string receivedData)
        {
            return PacketFactory.Instance.CreatePacket(receivedData);
        }

        public void SendPacket(Client client, ReplyPacket packet)
        {
            SendStream(client, ReplyPacketToStream(packet));
        }

        private byte[] ReplyPacketToStream(ReplyPacket packet)
        {
            var packetSize = Marshal.SizeOf(packet);
            var stream = new byte[packetSize];
            var pointer = Marshal.AllocHGlobal(packetSize);
            Marshal.StructureToPtr(packet, pointer, false);
            Marshal.Copy(pointer, stream, 0, packetSize);
            Marshal.FreeHGlobal(pointer);

            return stream;
        }

        private void SendStream(Client client, byte[] inData)
        {
            var sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(inData, 0, inData.Length);
            sendEventArgs.UserToken = client;

            client.socket.SendAsync(sendEventArgs);
        }

        private void CloseClient(ulong closedClientSessionid)
        {
            var closeClient = ClientManager.Instance.FindBySessionId(closedClientSessionid);
            if (closeClient != null)
            {
                closeClient.OnClosed();
                ClientManager.Instance.RemoveSessionidToClient(closedClientSessionid);
            }
        }
    }
}
