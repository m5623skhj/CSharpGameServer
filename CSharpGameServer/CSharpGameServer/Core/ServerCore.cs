using CSharpGameServer.Core.LogicWorkerThread;
using CSharpGameServer.DB;
using CSharpGameServer.Logger;
using System.Net;
using System.Net.Sockets;
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

        public virtual bool Initialize()
        {
            if (InitializeByConfig() == false)
            {
                return false;
            }
            if (PacketRegisterList.RegisterAllPacket() == false)
            {
                LoggerManager.Instance.WriteLogError("RegisterAllPacket failed");
                return false;
            }

            ClientManager.Instance.SetServerCore(this);

            logicWorkerThreadManager.MakeThreads(LogicThreadSize);
            return true;
        }

        private bool InitializeByConfig()
        {
            if (config.ReadConfig() == false)
            {
                return false;
            }
            
            LoggerManager.Instance.SetLogLevel(config.Conf.LogLevel);
            DbConnectionManager.Initialize(
                config.Conf.DbServerIp,
                config.Conf.DbSchemaName,
                config.Conf.DbUserId,
                config.Conf.DbUserPassword);

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
            if (listenSocket == null || running == false)
            {
                return;
            }

            while (running && listenSocket != null)
            {
                var acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += AcceptCompleted;

                if (listenSocket.AcceptAsync(acceptEventArgs))
                {
                    return;
                }

                CompleteAccept(acceptEventArgs);
            }
        }

        protected virtual void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            var clientSocket = acceptEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref AtomicSessionId);
            var newClient = new Client(this, clientSocket, newSessionId);

            ThreadPool.QueueUserWorkItem(StartReceive, newClient);

            ClientManager.Instance.InsertSessionIdToClient(newSessionId, newClient);
            acceptEventArgs.AcceptSocket = null;
        }

        private void AcceptCompleted(object? sender, SocketAsyncEventArgs acceptEventArgs)
        {
            CompleteAccept(acceptEventArgs);
            StartAccept();
        }

        protected void StartReceive(object? inClient)
        {
            if (inClient is not Client client)
            {
                return;
            }

            while (true)
            {
                var receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.SetBuffer(new byte[BufferSize], 0, BufferSize);
                receiveEventArgs.Completed += ReceiveCompleted;
                receiveEventArgs.UserToken = client;

                if (client.Socket.ReceiveAsync(receiveEventArgs))
                {
                    return;
                }

                var shouldContinue = ProcessReceive(receiveEventArgs);
                receiveEventArgs.Dispose();
                if (shouldContinue == false)
                {
                    return;
                }
            }
        }

        private void ReceiveCompleted(object? sender, SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.UserToken is not Client client)
            {
                receiveEventArgs.Dispose();
                return;
            }

            var shouldContinue = ProcessReceive(receiveEventArgs);
            receiveEventArgs.Dispose();
            if (shouldContinue)
            {
                StartReceive(client);
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

            while (true)
            {
                var storedStream = receivedClient.PeekAllStreamData();
                if (storedStream.Length == 0)
                {
                    return true;
                }

                var requestPacketResult = GetPacketFromReceivedData(storedStream, 0);
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
                        if (receivedClient.RemoveStreamData(requestPacketResult.PacketLength) == false)
                        {
                            return false;
                        }

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

        }

        private static RequestPacketResult GetPacketFromReceivedData(byte[] buffer, ushort bufferStartPoint)
        {
            return PacketFactory.Instance.CreatePacket(buffer, bufferStartPoint);
        }

        public void SendPacket(Client client, ReplyPacket packet)
        {
            SendStream(client, packet.ToBytes());
        }

        private static void SendCompleted(object? sender, SocketAsyncEventArgs sendEventArgs)
        {
            try
            {
                if (sendEventArgs.UserToken is not Client client)
                {
                    return;
                }

                client.OnSend();
            }
            finally
            {
                sendEventArgs.Dispose();
            }
        }

        private static void SendStream(Client client, byte[] inData)
        {
            var sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(inData, 0, inData.Length);
            sendEventArgs.Completed += SendCompleted;
            sendEventArgs.UserToken = client;

            if (client.Socket.SendAsync(sendEventArgs) == false)
            {
                SendCompleted(null, sendEventArgs);
            }
        }

        private void CompleteAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                ProcessAccept(acceptEventArgs);
            }
            finally
            {
                acceptEventArgs.Dispose();
            }
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
