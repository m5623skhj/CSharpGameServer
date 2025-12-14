using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using CSharpGameServer;
using CSharpGameServer.etc;

namespace TestClient.Client
{
    partial class ChattingClient
    {
        private void ProcessReceivedData(int packetType, byte[] buffer)
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
            if ((ErrorCode)onRoomCreated.ErrorCode != ErrorCode.Success)
            {
                Console.WriteLine("Room join failed with {0}", onRoomCreated.ErrorCode);
            }
            else
            {
                isRoomJoined = true;
            }
        }

        private void OnRoomJoined(byte[] buffer)
        {
            var onRoomJoined = DeserializePacket<RoomJoinedPacket>(buffer);
            if ((ErrorCode)onRoomJoined.ErrorCode != ErrorCode.Success)
            {
                Console.WriteLine("Room join failed with {0}", onRoomJoined.ErrorCode);
            }
            else
            {
                isRoomJoined = true;
            }
        }

        private void OnRoomLeft(byte[] buffer)
        {
            var onRoomLeft = DeserializePacket<RoomLeftPacket>(buffer);
            if ((ErrorCode)onRoomLeft.ErrorCode != ErrorCode.Success)
            {
                Console.WriteLine("Room join failed with {0}", onRoomLeft.ErrorCode);
            }
            else
            {
                isRoomJoined = false;
            }
        }

        private void OnChatMessage(byte[] buffer)
        {
            var onChatMessage = DeserializePacket<ChatMessagePacket>(buffer);
            string message;
            unsafe
            {
                message = FixedStringUtil.Read(onChatMessage.Message, 30);
            }
            Console.WriteLine(message);
        }

        private void OnRoomListUpdate(byte[] buffer)
        {
            var onRoomListUpdate = DeserializePacket<RoomListUpdatePacket>(buffer);

            unsafe
            {
                var roomsPointer = onRoomListUpdate.Rooms;
                {
                    var offset = 0;
                    var roomCount = 0;

                    while (offset < 200)
                    {
                        var roomName = FixedStringUtil.Read(roomsPointer + offset, 20);
                        if (string.IsNullOrEmpty(roomName))
                        {
                            break;
                        }
                        Console.WriteLine("Room {0}: {1}", roomCount + 1, roomName);
                        roomCount++;
                        offset += 20;
                    }
                }
            }
        }

        private void OnSetMyNameResult(byte[] buffer)
        {
            var onSetMyNameResult = DeserializePacket<SetMyNameResultPacket>(buffer);
        }
    }
}
