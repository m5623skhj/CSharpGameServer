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
    }

    public class Config
    {
        private ConfigItem configItem;
        public ConfigItem conf
        {
            get { return configItem; }
        }

        public bool ReadConfig()
        {
            try
            {
                string configJson = File.ReadAllText("Config/config.json");
                configItem = JsonSerializer.Deserialize<ConfigItem>(configJson);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LoggerManager.instance.WriteLogFatal("Config read failed with {ex}", ex);
            }

            return false;
        }
    }
}
