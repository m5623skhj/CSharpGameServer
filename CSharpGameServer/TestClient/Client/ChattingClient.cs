using System.Net.Sockets;
using System.Runtime.InteropServices;
using CSharpGameServer;
using CSharpGameServer.Core;
using CSharpGameServer.etc;

namespace TestClient.Client
{
    internal partial class ChattingClient(string ip, int port)
    {
        private TcpClient? client;
        private NetworkStream? stream;
        public bool isConnected { get; private set; }
        private Thread? recvThread;
        private bool isRunning;
        public bool isRoomJoined { get; private set; }

        private const int HeaderSize = 6;
        private readonly StreamRingBuffer ringBuffer = new();

        public bool Connect()
        {
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                isConnected = true;
                isRunning = true;

                recvThread = new Thread(Receive);
                recvThread.Start();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
                return false;
            }
        }

        public void Disconnect()
        {
            isRunning = false;
            isConnected = false;

            stream?.Close();
            client?.Close();

            recvThread?.Join();
        }

        public void Receive()
        {
            if (stream == null)
            {
                return;
            }

            var buffer = new byte[1024];
            while (isRunning)
            {
                try
                {
                    var bytesRecv = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRecv <= 0)
                    {
                        Console.WriteLine("Connection closed by server");
                        break;
                    }

                    if (ringBuffer.PushData(buffer, (uint)bytesRecv))
                    {
                        ProcessPackets();
                        continue;
                    }

                    Console.WriteLine("Ringbuffer push failed");
                    break;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Receive error: " + ex.Message);
                    break;
                }
            }
        }

        private void ProcessPackets()
        {
            while (true)
            {
                var availableData = ringBuffer.PeekAllData();
                if (availableData.Length < HeaderSize)
                {
                    return;
                }

                var packetType = BitConverter.ToInt32(availableData, 0);
                var packetLength = BitConverter.ToInt16(availableData, 4);

                if (packetLength > ringBuffer.GetUseSize())
                {
                    return;
                }

                var data = ringBuffer.PopData((uint)packetLength);
                if (data != null)
                {
                    ProcessReceivedData(packetType, data);
                }
            }
        }

        public void Send(byte[] dataStream)
        {
            stream?.Write(dataStream, 0, dataStream.Length);
        }

        public void SendPing()
        {
            if (!isConnected || stream == null)
            {
                Console.WriteLine("Not connected to server.");
                return;
            }

            Send(CreatePacket(PacketType.Ping, new PingPacket()));
        }

        public void SetMyName(string name)
        {
            SetMyNamePacket packet = new();
            unsafe
            {
                var pName = packet.Name;
                FixedStringUtil.Write(name, pName, 12);
            }

            Send(CreatePacket(PacketType.SetMyName, packet));
        }

        public void CreateRoom(string roomName)
        {
            var packet = new CreateRoomPacket();
            unsafe
            {
                var roomNamePointer = packet.RoomName;
                {
                    FixedStringUtil.Write(roomName, roomNamePointer, 20);
                }
            }

            Send(CreatePacket(PacketType.CreateRoom, packet));
        }

        public void JoinRoom(string roomName)
        {
            var packet = new JoinRoomPacket();
            unsafe
            {
                var roomNamePointer = packet.RoomName;
                {
                    FixedStringUtil.Write(roomName, roomNamePointer, 20);
                }
            }

            Send(CreatePacket(PacketType.JoinRoom, packet));
        }

        public void LeaveRoom()
        {
            Send(CreatePacket(PacketType.LeaveRoom, new LeaveRoomPacket()));
        }

        public void SendChatMessage(string message)
        {
            var packet = new SendChatPacket();
            unsafe
            {
                var messagePointer = packet.Message;
                {
                    FixedStringUtil.Write(message, messagePointer, 20);
                }
            }

            Send(CreatePacket(PacketType.SendChat, packet));
        }

        public void SendRequestRoomList()
        {
            var packet = new RoomListRequestPacket();
            Send(CreatePacket(PacketType.RoomListRequest, packet));
        }

        private static byte[] CreatePacket<T>(PacketType packetType, T packet) where T : struct
        {
            var size = Marshal.SizeOf(packet);
            var buffer = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, false);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            var packetTypeBytes = BitConverter.GetBytes((int)packetType);
            var packetLengthBytes = BitConverter.GetBytes((short)(buffer.Length));
            Buffer.BlockCopy(packetTypeBytes, 0, buffer, 0, packetTypeBytes.Length);
            Buffer.BlockCopy(packetLengthBytes, 0, buffer, packetTypeBytes.Length, packetLengthBytes.Length);

            return buffer;
        }

        private static T DeserializePacket<T>(byte[] data) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            if (data.Length < size)
            {
                throw new ArgumentException("Data length is less than the size of the structure.");
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(data, 0, ptr, size);
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
