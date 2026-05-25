using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpGameServer.Config
{
    public struct ConfigItem
    {
        public ConfigItem()
        {
        }

        public LogEventLevel LogLevel = 0;

        public string DbServerIp = "";
        public string DbSchemaName = "";
        public string DbUserId = "";
        public string DbUserPassword = "";
    }

    public class Config
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public ConfigItem Conf { get; private set; }

        public bool ReadConfig()
        {
            try
            {
                var configJson = File.ReadAllText("Config/config.json");
                var config = JsonSerializer.Deserialize<ConfigItem>(configJson, JsonOptions);
                Conf = config;

                return true;
            }
            catch (Exception ex)
            {
                Logger.LoggerManager.WriteLogFatal("Config read failed with {ex}", ex);
            }

            return false;
        }
    }
}
