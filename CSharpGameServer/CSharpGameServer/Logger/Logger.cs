using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CSharpGameServer.Logger
{
    public class LoggerManager
    {
        public static LoggerManager? instance = null;
        private LoggingLevelSwitch loggerLevel = new LoggingLevelSwitch(LogEventLevel.Debug);

        private LoggerManager()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggerLevel)
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File("log.txt", rollingInterval: RollingInterval.Hour))
                .CreateLogger();
        }

        ~LoggerManager()
        {
            Log.CloseAndFlush();
        }

        public void SetLogLevel(LogEventLevel inLevel)
        {
            loggerLevel.MinimumLevel = inLevel;
        }

        public static LoggerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoggerManager();
                }

                return instance;
            }
        }

        public void WriteLogVerb(string log, params object[] objects)
        {
            Log.Verbose(log, objects);
        }

        public void WriteLogDebug(string log, params object[] objects)
        {
            Log.Debug(log, objects);
        }

        public void WriteLogInfo(string log, params object[] objects)
        {
            Log.Information(log, objects);
        }

        public void WriteLogWarn(string log, params object[] objects)
        {
            Log.Warning(log, objects);
        }

        public void WriteLogError(string log, params object[] objects)
        {
            Log.Error(log, objects);
        }

        public void WriteLogFatal(string log, params object[] objects)
        {
            Log.Fatal(log, objects);
        }
    }
}
