using Serilog.Events;
using System.Text.Json;

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
        public ConfigItem Conf { get; private set; }

        public bool ReadConfig()
        {
            try
            {
                var configJson = File.ReadAllText("Config/config.json");
                Conf = JsonSerializer.Deserialize<ConfigItem>(configJson);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LoggerManager.Instance.WriteLogFatal("Config read failed with {ex}", ex);
            }

            return false;
        }
    }
}
