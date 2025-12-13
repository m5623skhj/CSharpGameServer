using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpGameServer;

namespace TestClient.Client
{
    partial class ChattingClient
    {
        private void ProcessReceivedData(short packetType, byte[] buffer)
        {
            switch ((PacketType)packetType)
            {
                case PacketType.Pong:
                {
                    OnPong(buffer);
                    break;
                }
                case PacketType.RoomCreated:
                {
                    OnRoomCreated(buffer);
                    break;
                }
                case PacketType.RoomJoined:
                {
                    OnRoomJoined(buffer);
                    break;
                }
                case PacketType.RoomLeft:
                {
                    OnRoomLeft(buffer);
                    break;
                }
                case PacketType.ChatMessage:
                {
                    OnChatMessage(buffer);
                    break;
                }
                case PacketType.RoomListUpdate:
                {
                    OnRoomListUpdate(buffer);
                    break;
                }
                case PacketType.SetMyNameResult:
                {
                    OnSetMyNameResult(buffer);
                    break;
                }
                default:
                {
                    Console.WriteLine("Invalid packet type {0}", packetType);
                    break;
                }
            }
        }

        private void OnPong(byte[] buffer)
        {
            var pong = DeserializePacket<PongPacket>(buffer);
        }

        private void OnRoomCreated(byte[] buffer)
        {
            var onRoomCreated = DeserializePacket<RoomCreatedPacket>(buffer);
        }

        private void OnRoomJoined(byte[] buffer)
        {
            var onRoomJoined = DeserializePacket<RoomJoinedPacket>(buffer);
        }

        private void OnRoomLeft(byte[] buffer)
        {
            var onRoomLeft = DeserializePacket<RoomLeftPacket>(buffer);
        }

        private void OnChatMessage(byte[] buffer)
        {
            var onChatMessage = DeserializePacket<ChatMessagePacket>(buffer);
        }

        private void OnRoomListUpdate(byte[] buffer)
        {
            var onRoomListUpdate = DeserializePacket<RoomListUpdatePacket>(buffer);
        }

        private void OnSetMyNameResult(byte[] buffer)
        {
            var onSetMyNameResult = DeserializePacket<SetMyNameResultPacket>(buffer);
        }
    }
}
