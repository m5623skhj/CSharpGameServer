using CSharpGameServer.ChattingRoom;
using CSharpGameServer.Packet;

namespace ServerUnitTest
{
    public class ChattingRoomUnitTest : IDisposable
    {
        public void Dispose()
        {
            ChattingRoomManager.Instance.ClearChattingRooms();
        }

        public static IEnumerable<object[]> GetAddChattingRoomTestData
        {
            get
            {
                yield return
                [
                    0UL,
                    "TestUser",
                    new CreateRoomPacket
                    {
                        Data = new CreateRoom
                        {
                            RoomName = "TestRoomName"
                        }
                    }
                ];
            }
        }

        public static IEnumerable<object[]> GetAddChattingRoomInvalidTestData
        {
            get
            {
                yield return
                [
                    0UL,
                    "TestUser",
                    new CreateRoomPacket
                    {
                        Data = new CreateRoom
                        {
                            RoomName = string.Empty
                        }
                    }
                ];

                yield return
                [
                    0UL,
                    "TestUser",
                    new CreateRoomPacket
                    {
                        Data = new CreateRoom
                        {
                            RoomName = "ThisRoomNameIsTooLongSoItsInvalid"
                        }
                    }
                ];

                yield return
                [
                    0UL,
                    "TestUser",
                    new CreateRoomPacket
                    {
                        Data = new CreateRoom
                        {
                            RoomName = "        "
                        }
                    }
                ];
            }
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomTestData))]
        public void AddChattingRoom_Test(ulong id, string name, CreateRoomPacket packet)
        {
            ChattingRoomManager.Instance.AddChattingRoom(id, name, packet);
            Assert.True(ChattingRoomManager.Instance.ExistsChattingRoom(packet.Data.RoomName));
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomInvalidTestData))]
        public void AddChattingRoom_Invalid_Test(ulong id, string name, CreateRoomPacket packet)
        {
            ChattingRoomManager.Instance.AddChattingRoom(id, name, packet);
            Assert.False(ChattingRoomManager.Instance.ExistsChattingRoom(packet.Data.RoomName));
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomTestData))]
        public void RemoveChattingRoom_Test(ulong id, string name, CreateRoomPacket packet)
        {
            ChattingRoomManager.Instance.AddChattingRoom(id, name, packet);
            Assert.True(ChattingRoomManager.Instance.ExistsChattingRoom(packet.Data.RoomName));
            ChattingRoomManager.Instance.RemoveChattingRoom(packet.Data.RoomName);
            Assert.False(ChattingRoomManager.Instance.ExistsChattingRoom(packet.Data.RoomName));
        }
    }
}
