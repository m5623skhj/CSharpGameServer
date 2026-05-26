using CSharpGameServer.Core;
using CSharpGameServer.Config;
using CSharpGameServer.DB.SPObjects;
using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;
using MySql.Data.MySqlClient;
using System.Text.Json.Serialization;

namespace ServerUnitTest
{
    internal class FailingRequestPacket : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Ping;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return (_, _) => { };
        }

        public override bool LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            return false;
        }

        public override byte[] ToBytes()
        {
            return [];
        }
    }

    internal class ParameterizedSpObject : SpBase
    {
        public ParameterizedSpObject()
        {
            Query = "SELECT * FROM tbl WHERE id = @Id AND name = @Name";
        }

        public void SetValues(int inId, string inName)
        {
            ClearParameters();
            AddParameter("@Id", inId);
            AddParameter("@Name", inName);
        }

        public override void OnCommit() { }
        public override void OnRollback() { }
    }

    public class UnitTest
    {
        private static string CreateTempConfigFile(string configJson)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            var configPath = Path.Combine(tempDirectory, "config.json");
            File.WriteAllText(configPath, configJson);

            return configPath;
        }

        [Fact]
        public void StreamRingBuffer_PeekAllData_ReturnsWrappedDataInOrder()
        {
            var ringBuffer = new StreamRingBuffer(8);

            Assert.True(ringBuffer.PushData([1, 2, 3, 4, 5]));
            Assert.True(ringBuffer.EraseData(3));
            Assert.True(ringBuffer.PushData([6, 7, 8]));

            Assert.Equal([4, 5, 6, 7, 8], ringBuffer.PeekAllData());
        }

        [Fact]
        public void PacketFactory_CreatePacket_LoadsPacketData()
        {
            var packetFactory = new PacketFactory();
            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(PingPacket)));

            var packetBytes = new byte[]
            {
                (byte)PacketType.Ping, 0, 0, 0,
                6, 0
            };

            var result = packetFactory.CreatePacket(packetBytes, 0);

            Assert.Equal(PacketResultType.Success, result.ResultType);
            Assert.Equal((ushort)6, result.PacketLength);

            var pingPacket = Assert.IsType<PingPacket>(result.Packet);
            Assert.Equal(PacketType.Ping, pingPacket.Type);
            Assert.Equal((int)PacketType.Ping, pingPacket.Data.PacketType);
            Assert.Equal((ushort)6, pingPacket.Data.PacketSize);
        }

        [Fact]
        public void PacketFactory_CreatePacket_ReturnsIncompleteWhenHeaderIsPartial()
        {
            var packetFactory = new PacketFactory();

            var result = packetFactory.CreatePacket([1, 0, 0, 0, 6], 0);

            Assert.Equal(PacketResultType.IncompleteReceived, result.ResultType);
        }

        [Fact]
        public void PacketFactory_CreatePacket_ReturnsInvalidWhenPacketLengthIsSmallerThanHeader()
        {
            var packetFactory = new PacketFactory();
            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(PingPacket)));

            var invalidBytes = new byte[]
            {
                (byte)PacketType.Ping, 0, 0, 0,
                5, 0
            };

            var result = packetFactory.CreatePacket(invalidBytes, 0);

            Assert.Equal(PacketResultType.InvalidReceivedData, result.ResultType);
        }

        [Fact]
        public void PacketFactory_CreatePacket_ReturnsIncompleteWhenPayloadIsFragmented()
        {
            var packetFactory = new PacketFactory();
            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(PingPacket)));

            var fragmentedBytes = new byte[]
            {
                (byte)PacketType.Ping, 0, 0, 0,
                8, 0
            };

            var result = packetFactory.CreatePacket(fragmentedBytes, 0);

            Assert.Equal(PacketResultType.IncompleteReceived, result.ResultType);
        }

        [Fact]
        public void PacketFactory_CreatePacket_ReturnsInvalidWhenDeserializeFails()
        {
            var packetFactory = new PacketFactory();
            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(FailingRequestPacket)));

            var invalidBytes = new byte[]
            {
                (byte)PacketType.Ping, 0, 0, 0,
                6, 0
            };

            var result = packetFactory.CreatePacket(invalidBytes, 0);

            Assert.Equal(PacketResultType.InvalidReceivedData, result.ResultType);
            Assert.Null(result.Packet);
        }

        [Fact]
        public void AccumulatedStream_CanDrainMultiplePackets()
        {
            var packetFactory = new PacketFactory();
            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(PingPacket)));

            var ringBuffer = new StreamRingBuffer();
            var pingPacketBytes = new byte[]
            {
                (byte)PacketType.Ping, 0, 0, 0,
                6, 0
            };

            Assert.True(ringBuffer.PushData([.. pingPacketBytes, .. pingPacketBytes]));

            var parsedPackets = 0;
            while (true)
            {
                var storedStream = ringBuffer.PeekAllData();
                if (storedStream.Length == 0)
                {
                    break;
                }

                var result = packetFactory.CreatePacket(storedStream, 0);
                Assert.Equal(PacketResultType.Success, result.ResultType);
                Assert.True(ringBuffer.EraseData(result.PacketLength));
                parsedPackets++;
            }

            Assert.Equal(2, parsedPackets);
        }

        [Fact]
        public void PongPacket_ToBytes_SetsHeaderFields()
        {
            var packet = new PongPacket();

            var bytes = packet.ToBytes();

            Assert.Equal((int)PacketType.Pong, BitConverter.ToInt32(bytes, 0));
            Assert.Equal((ushort)bytes.Length, BitConverter.ToUInt16(bytes, 4));
        }

        [Fact]
        public void Config_JsonOptions_DeserializeFieldBasedJson()
        {
            var configJson = """
            {
              "LogLevel": "Debug",
              "DBServerIP": "127.0.0.1",
              "DBSchemaName": "test_schema",
              "DBUserId": "test",
              "DBUserPassword": "dbPassword",
              "MigratorFilePath": "migrator.exe"
            }
            """;

            var options = new System.Text.Json.JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var config = System.Text.Json.JsonSerializer.Deserialize<ConfigItem>(configJson, options);

            Assert.Equal("127.0.0.1", config.DbServerIp);
            Assert.Equal("test_schema", config.DbSchemaName);
            Assert.Equal("test", config.DbUserId);
            Assert.Equal("dbPassword", config.DbUserPassword);
            Assert.Equal("migrator.exe", config.MigratorFilePath);
        }

        [Fact]
        public void Config_ReadConfig_ReturnsFalseWhenRequiredValueIsEmpty()
        {
            var configJson = """
            {
              "LogLevel": "Debug",
              "DBServerIP": "127.0.0.1",
              "DBSchemaName": "test_schema",
              "DBUserId": "test",
              "DBUserPassword": "dbPassword",
              "MigratorFilePath": ""
            }
            """;

            var configPath = CreateTempConfigFile(configJson);
            try
            {
                var config = new Config();

                Assert.False(config.ReadConfig(configPath));
            }
            finally
            {
                Directory.Delete(Path.GetDirectoryName(configPath)!, true);
            }
        }

        [Fact]
        public void Config_ReadConfig_ReturnsTrueWhenRequiredValuesExist()
        {
            var configJson = """
            {
              "LogLevel": "Debug",
              "DBServerIP": "127.0.0.1",
              "DBSchemaName": "test_schema",
              "DBUserId": "test",
              "DBUserPassword": "dbPassword",
              "MigratorFilePath": "migrator.exe"
            }
            """;

            var configPath = CreateTempConfigFile(configJson);
            try
            {
                var config = new Config();

                Assert.True(config.ReadConfig(configPath));
            }
            finally
            {
                Directory.Delete(Path.GetDirectoryName(configPath)!, true);
            }
        }

        [Fact]
        public void SpBase_CreateCommand_BindsNamedParameters()
        {
            var spObject = new ParameterizedSpObject();
            spObject.SetValues(10, "tester");

            using var connection = new MySqlConnection();
            using var command = spObject.CreateCommand(connection);

            Assert.Equal("SELECT * FROM tbl WHERE id = @Id AND name = @Name", command.CommandText);
            Assert.Equal(2, command.Parameters.Count);
            Assert.Equal(10, command.Parameters["@Id"].Value);
            Assert.Equal("tester", command.Parameters["@Name"].Value);
        }

        [Fact]
        public void PacketFactory_RegisterPacket_ReturnsTrueWhenSameTypeIsRegisteredAgain()
        {
            var packetFactory = new PacketFactory();

            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(PingPacket)));
            Assert.True(packetFactory.RegisterPacket(PacketType.Ping, typeof(PingPacket)));
        }

        [Fact]
        public void PacketHandlerManager_RegisterPacketHandler_ReturnsTrueWhenSameHandlerIsRegisteredAgain()
        {
            var packetHandlerManager = new PacketHandlerManager();
            Action<Client, RequestPacket> handler = (_, _) => { };

            Assert.True(packetHandlerManager.RegisterPacketHandler(PacketType.Ping, handler));
            Assert.True(packetHandlerManager.RegisterPacketHandler(PacketType.Ping, handler));
        }
    }
}
