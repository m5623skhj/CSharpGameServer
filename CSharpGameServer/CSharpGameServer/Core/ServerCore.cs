using CSharpGameServer.Core.LogicWorkerThread;
using CSharpGameServer.DB;
using CSharpGameServer.Logger;
using CSharpGameServer.Protocol;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;


namespace CSharpGameServer.Core
{
    public class ServerCore
    {
        //private static ServerCore? instance = null;

        private Socket? listenSocket = null;
        private int port = 10001;
        private int backlogSize = 0;
        protected ulong atomicSessionId = 1;

        private const int bufferSize = 2048;
        private bool running = false;

        private LogicWorkerThreadManager logicWorkerThreadManager = new LogicWorkerThreadManager();
        private readonly int logicThreadSize = 16;
        private Config.Config config = new Config.Config();

        //public static ServerCore Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new ServerCore();
        //        }

        //        return instance;
        //    }
        //}

        public ServerCore()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            if (InitializeByConfig() == false)
            {
                return;
            }
            if (PacketRegisterList.RegisterAllPacket() == false)
            {
                LoggerManager.Instance.WriteLogError("RegisterAllPacket falied");
                return;
            }

            ClientManager.Instance.SetServerCore(this);

            logicWorkerThreadManager.MakeThreads(logicThreadSize);
        }

        private bool InitializeByConfig()
        {
            if (config.ReadConfig() == false)
            {
                return false;
            }
            
            LoggerManager.Instance.SetLogLevel(config.conf.LogLevel);
            DBConnectionManager.Initialize(
                config.conf.dbServerIP,
                config.conf.dbSchemaName,
                config.conf.dbUserId,
                config.conf.dbUserPassword);

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
            running = false;
            if (listenSocket != null)
            {
                listenSocket.Close();
            }

            ClientManager.Instance.CloseAllSession();
            logicWorkerThreadManager.StopAllLogicThreads();
        }

        private void StartAccept()
        {
            SocketAsyncEventArgs acceptEventArgs = new SocketAsyncEventArgs();
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
            Socket? clientSocket = acceptEventArgs.AcceptSocket;
            if (clientSocket == null)
            {
                return;
            }

            var newSessionId = Interlocked.Increment(ref atomicSessionId);
            var newClient = new Client(this, clientSocket, newSessionId);
            if (newClient == null)
            {
                return;
            }

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
            if (inClient is null)
            {
                return;
            }

            Client? client = inClient as Client;
            if (client == null)
            {
                return;
            }

            var receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.SetBuffer(new byte[bufferSize], 0, bufferSize);
            receiveEventArgs.Completed += ReceiveCompleted;
            receiveEventArgs.UserToken = client;

            client.socket.ReceiveAsync(receiveEventArgs);
        }

        private void ReceiveCompleted(object? sender, SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.UserToken == null)
            {
                return;
            }

            if (ProcessReceive(receiveEventArgs) == true)
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
                CloseClient(receivedClient.clientSessionId);
                return false;
            }

            string receivedData = Encoding.ASCII.GetString(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred);
            if (ProcessPacket(receivedClient, receivedData) == false)
            {
                CloseClient(receivedClient.clientSessionId);
                return false;
            }

            return true;
        }

        private bool ProcessPacket(Client receivedClient, string receivedData)
        {
            if (receivedClient.PushStreamData(Encoding.UTF8.GetBytes(receivedData)) == false)
            {
                return false;
            }

            byte[] storedStream = receivedClient.PeekAllStreamData();
            int storedSize = storedStream.Length;
            ushort bufferStartPoint = 0;

            while (storedSize > 0)
            {
                var requestPacketResult = GetPacketFromReceivedData(bufferStartPoint, receivedData);
                switch (requestPacketResult.resultType) 
                {
                    case PacketResultType.InvalidReceivedData:
                        return false;
                    case PacketResultType.IncompletedReceived:
                        return true;
                    case PacketResultType.Success:
                        storedSize -= requestPacketResult.packetLength;
                        bufferStartPoint += requestPacketResult.packetLength;
                        receivedClient.RemoveStreamData(requestPacketResult.packetLength);

                        // Since the null check is already performed in GetPacketFromReceivedData(),
                        // it is not rechecked here.
                        logicWorkerThreadManager.PushPacket(receivedClient, requestPacketResult.packet);
                        logicWorkerThreadManager.DoWork(receivedClient.clientSessionId);
                        break;
                }
            }

            return true;
        }

        private RequestPacketResult GetPacketFromReceivedData(ushort bufferStartPoint, string receivedData)
        {
            return PacketFactory.Instance.CreatePacket(receivedData.Substring(bufferStartPoint));
        }

        public void SendPacket(Client client, ReplyPacket packet)
        {
            SendStream(client, ReplyPacketToStream(packet));
        }

        private void SendCompleted(object? sender, SocketAsyncEventArgs sendEventArgs)
        {
            if (sendEventArgs.UserToken == null)
            {
                return;
            }

            Client? client = sendEventArgs.UserToken as Client;
            if (client == null)
            {
                return;
            }

            client.OnSend();
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
            sendEventArgs.Completed += SendCompleted;
            sendEventArgs.UserToken = client;

            client.socket.SendAsync(sendEventArgs);
        }

        public void CloseClient(ulong closedClientSessionid)
        {
            var closeClient = ClientManager.Instance.FindBySessionId(closedClientSessionid);
            if (closeClient == null)
            {
                return;
            }

            closeClient.socket.BeginDisconnect(false, asyncResult =>
            {
                closeClient.socket.EndDisconnect(asyncResult);
                closeClient.OnClosed();
                ClientManager.Instance.RemoveSessionidToClient(closedClientSessionid);
            }, null);
        }
    }
}
