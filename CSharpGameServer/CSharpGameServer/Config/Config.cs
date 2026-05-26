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
        public string MigratorFilePath = "";
    }

    public class Config
    {
        private const string DefaultConfigPath = "Config/config.json";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public ConfigItem Conf { get; private set; }

        public bool ReadConfig(string configPath = DefaultConfigPath)
        {
            try
            {
                var configJson = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<ConfigItem>(configJson, JsonOptions);
                if (HasRequiredValues(config) == false)
                {
                    Logger.LoggerManager.WriteLogFatal("Config read failed: required config value is empty");
                    return false;
                }

                Conf = config;

                return true;
            }
            catch (Exception ex)
            {
                Logger.LoggerManager.WriteLogFatal("Config read failed with {ex}", ex);
            }

            return false;
        }

        private static bool HasRequiredValues(ConfigItem config)
        {
            return string.IsNullOrWhiteSpace(config.DbServerIp) == false &&
                   string.IsNullOrWhiteSpace(config.DbSchemaName) == false &&
                   string.IsNullOrWhiteSpace(config.DbUserId) == false &&
                   string.IsNullOrWhiteSpace(config.DbUserPassword) == false &&
                   string.IsNullOrWhiteSpace(config.MigratorFilePath) == false;
        }
    }
}
