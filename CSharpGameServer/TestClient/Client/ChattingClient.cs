using System.Net.Sockets;
using System.Runtime.InteropServices;
using CSharpGameServer;
using CSharpGameServer.Core;

namespace TestClient.Client
{
    partial class ChattingClient
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private readonly string serverIp;
        private readonly int serverPort;
        private bool isConnected;
        private Thread? recvThread;
        private bool isRunning;

        private const int HeaderSize = 6;
        private readonly StreamRingBuffer ringBuffer = new();

        public ChattingClient(string ip, int port)
        {
            serverIp = ip;
            serverPort = port;
        }

        public bool Connect()
        {
            try
            {
                client = new TcpClient(serverIp, serverPort);
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
                    if (!stream.DataAvailable)
                    {
                        continue;
                    }

                    var bytesRecv = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRecv <= 0)
                    {
                        Console.WriteLine("Connection closed by server");
                        break;
                    }

                    if (ringBuffer.PushData(buffer))
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

                var packetType = BitConverter.ToInt16(availableData, 0);
                var packetLength = BitConverter.ToInt32(availableData, 2);

                if (packetLength > ringBuffer.GetUseSize())
                {
                    return;
                }

                var data = ringBuffer.PopData((uint)packetLength + HeaderSize);
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

            Send(CreatePacket(PacketType.Ping, SerializePacket(new PingPacket())));
        }

        private static byte[] SerializePacket<T>(T packet) where T : struct
        {
            var size = System.Runtime.InteropServices.Marshal.SizeOf(packet);
            var buffer = new byte[size];
            var ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            System.Runtime.InteropServices.Marshal.StructureToPtr(packet, ptr, true);
            System.Runtime.InteropServices.Marshal.Copy(ptr, buffer, 0, size);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
            return buffer;
        }

        private static byte[] CreatePacket(PacketType packetType, byte[] data)
        {
            var packetTypeBytes = BitConverter.GetBytes((short)packetType);
            var packetLengthBytes = BitConverter.GetBytes(data.Length + HeaderSize);
            var packet = new byte[HeaderSize + data.Length];
            Buffer.BlockCopy(packetTypeBytes, 0, packet, 0, packetTypeBytes.Length);
            Buffer.BlockCopy(packetLengthBytes, 0, packet, packetTypeBytes.Length, packetLengthBytes.Length);
            Buffer.BlockCopy(data, 0, packet, HeaderSize, data.Length);

            return packet;
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
                return Marshal.PtrToStructure<T>(ptr)!;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
