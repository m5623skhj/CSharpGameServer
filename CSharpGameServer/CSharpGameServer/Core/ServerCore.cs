using CSharpGameServer.Core.LogicWorkerThread;
using CSharpGameServer.Logger;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;


namespace CSharpGameServer.Core
{
    public class ServerCore
    {
        private Socket? listenSocket;
        private const int Port = 10001;
        private const int BacklogSize = 200;
        protected ulong AtomicSessionId = 1;

        private const int BufferSize = 2048;
        private bool running;

        private readonly LogicWorkerThreadManager logicWorkerThreadManager = new();
        private const int LogicThreadSize = 16;
        private readonly Config.Config config = new();

        public virtual void Initialize()
        {
            if (InitializeByConfig() == false)
            {
                return;
            }
            if (PacketRegisterList.RegisterAllPacket() == false)
            {
                LoggerManager.Instance.WriteLogError("RegisterAllPacket failed");
                return;
            }

            ClientManager.Instance.SetServerCore(this);

            logicWorkerThreadManager.MakeThreads(LogicThreadSize);
        }

        private bool InitializeByConfig()
        {
            if (config.ReadConfig() == false)
            {
                return false;
            }
            
            LoggerManager.Instance.SetLogLevel(config.Conf.LogLevel);
            return true;
        }

        public void Run()
        {
            running = true;

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (listenSocket == null) 
            {
                throw new Exception("listen socket is null");
            }
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, Port));

            if (BacklogSize > 0)
            {
                listenSocket.Listen(BacklogSize);
            }

            StartAccept();
        }

        public void Stop()
        {
            running = false;
            listenSocket?.Close();

            ClientManager.Instance.CloseAllSession();
            logicWorkerThreadManager.StopAllLogicThreads();
        }

        private void StartAccept()
        {
            var acceptEventArgs = new SocketAsyncEventArgs();
            acceptEventArgs.Completed += AcceptCompleted;

            if (listenSocket == null || running == false)
            {
                return;
            }

            if (listenSocket.AcceptAsync(acceptEventArgs) == false)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        protected virtual void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            var clientSocket = acceptEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref AtomicSessionId);
            var newClient = new Client(this, clientSocket, newSessionId);

            ThreadPool.QueueUserWorkItem(StartReceive, newClient);

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

        protected void StartReceive(object? inClient)
        {
            if (inClient is not Client client)
            {
                return;
            }

            var receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.SetBuffer(new byte[BufferSize], 0, BufferSize);
            receiveEventArgs.Completed += ReceiveCompleted;
            receiveEventArgs.UserToken = client;

            client.Socket.ReceiveAsync(receiveEventArgs);
        }

        private void ReceiveCompleted(object? sender, SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.UserToken == null)
            {
                return;
            }

            if (ProcessReceive(receiveEventArgs))
            {
                StartReceive((Client)receiveEventArgs.UserToken);
            }
        }

        private bool ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.UserToken == null)
            {
                return false;
            }

            var receivedClient = (Client)receiveEventArgs.UserToken;
            if (receiveEventArgs.SocketError != SocketError.Success 
                || receiveEventArgs.BytesTransferred <= 0
                || receiveEventArgs.Buffer == null)
            {
                CloseClient(receivedClient.ClientSessionId);
                return false;
            }

            var receivedData = new byte[receiveEventArgs.BytesTransferred];
            Array.Copy(receiveEventArgs.Buffer, receivedData, receiveEventArgs.BytesTransferred);
            if (ProcessPacket(receivedClient, receivedData))
            {
                return true;
            }

            CloseClient(receivedClient.ClientSessionId);
            return false;

        }

        private bool ProcessPacket(Client receivedClient, byte[] receivedData)
        {
            if (receivedClient.PushStreamData(receivedData) == false)
            {
                return false;
            }

            var storedStream = receivedClient.PeekAllStreamData();
            var storedSize = storedStream.Length;
            ushort bufferStartPoint = 0;

            while (storedSize > 0)
            {
                var requestPacketResult = GetPacketFromReceivedData(receivedData, bufferStartPoint);
                switch (requestPacketResult.ResultType) 
                {
                    case PacketResultType.InvalidReceivedData:
                    {
                        return false;
                    }
                    case PacketResultType.IncompleteReceived:
                    {
                        return true;
                    }
                    case PacketResultType.Success:
                    {
                        storedSize -= requestPacketResult.PacketLength;
                        bufferStartPoint += requestPacketResult.PacketLength;
                        receivedClient.RemoveStreamData(requestPacketResult.PacketLength);

                        // Since the null check is already performed in GetPacketFromReceivedData(),
                        // it is not rechecked here.
                        if (requestPacketResult.Packet != null)
                        {
                            logicWorkerThreadManager.PushPacket(receivedClient, requestPacketResult.Packet);
                        }

                        logicWorkerThreadManager.DoWork(receivedClient.ClientSessionId);
                        break;
                    }
                    default:
                    {
                        LoggerManager.Instance.WriteLogError("Unknown PacketResultType {resultType}", requestPacketResult.ResultType);
                        break;
                    }
                }
            }

            return true;
        }

        private static RequestPacketResult GetPacketFromReceivedData(byte[] buffer, ushort bufferStartPoint)
        {
            return PacketFactory.Instance.CreatePacket(buffer, bufferStartPoint);
        }

        public void SendPacket(Client client, ReplyPacket packet)
        {
            SendStream(client, ReplyPacketToStream(packet));
        }

        private static void SendCompleted(object? sender, SocketAsyncEventArgs sendEventArgs)
        {
            if (sendEventArgs.UserToken is not Client client)
            {
                return;
            }

            client.OnSend();
        }

        private static byte[] ReplyPacketToStream(ReplyPacket packet)
        {
            var packetSize = Marshal.SizeOf(packet);
            var stream = new byte[packetSize];
            var pointer = Marshal.AllocHGlobal(packetSize);
            Marshal.StructureToPtr(packet, pointer, false);
            Marshal.Copy(pointer, stream, 0, packetSize);
            Marshal.FreeHGlobal(pointer);

            return stream;
        }

        private static void SendStream(Client client, byte[] inData)
        {
            var sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(inData, 0, inData.Length);
            sendEventArgs.Completed += SendCompleted;
            sendEventArgs.UserToken = client;

            client.Socket.SendAsync(sendEventArgs);
        }

        public void CloseClient(ulong closedClientSessionId)
        {
            var closeClient = ClientManager.Instance.FindBySessionId(closedClientSessionId);

            closeClient?.Socket.BeginDisconnect(false, asyncResult =>
            {
                closeClient.Socket.EndDisconnect(asyncResult);
                closeClient.OnClosed();
                ClientManager.Instance.RemoveSessionIdToClient(closedClientSessionId);
            }, null);
        }
    }
}
