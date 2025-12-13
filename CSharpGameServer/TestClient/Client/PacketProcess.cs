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
        private void ProcessReceivedData(short packetType, byte[] buffer, int length)
        {
            switch ((PacketType)packetType)
            {
                case PacketType.Pong:
                {
                    OnPong();
                    break;
                }
                case PacketType.RoomCreated:
                {
                    OnRoomCreated();
                    break;
                }
                case PacketType.RoomJoined:
                {
                    OnRoomJoined();
                    break;
                }
                case PacketType.RoomLeft:
                {
                    OnRoomLeft();
                    break;
                }
                case PacketType.ChatMessage:
                {
                    OnChatMessage();
                    break;
                }
                case PacketType.RoomListUpdate:
                {
                    OnRoomListUpdate();
                    break;
                }
                case PacketType.SetMyNameResult:
                {
                    OnSetMyNameResult();
                    break;
                }
                default:
                {
                    Console.WriteLine("Invalid packet type {0}", packetType);
                    break;
                }
            }
        }

        private void OnPong()
        {
        }

        private void OnRoomCreated()
        {
        }

        private void OnRoomJoined()
        {
        }

        private void OnRoomLeft()
        {
        }

        private void OnChatMessage()
        {
        }

        private void OnRoomListUpdate()
        {
        }

        private void OnSetMyNameResult()
        {
        }
    }
}
