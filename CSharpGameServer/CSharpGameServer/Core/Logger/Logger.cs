using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CSharpGameServer.Core.Logger
{
    public class Logger
    {
        private static Logger? instance = null;
        private LoggingLevelSwitch loggerLevel = new LoggingLevelSwitch(LogEventLevel.Debug);

        private Logger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggerLevel)
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File("log.txt", rollingInterval: RollingInterval.Hour))
                .CreateLogger();
        }

        ~Logger()
        {
            Log.CloseAndFlush();
        }

        public void SetLogLevel(LogEventLevel inLevel)
        {
            loggerLevel.MinimumLevel = inLevel;
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }

                return instance;
            }
        }

        public void WriteLogVerb(string log)
        {
            Log.Verbose(log);
        }

        public void WriteLogDebug(string log)
        {
            Log.Debug(log);
        }

        public void WriteLogInfo(string log)
        {
            Log.Information(log);
        }

        public void WriteLogWarn(string log)
        {
            Log.Warning(log);
        }

        public void WriteLogError(string log)
        {
            Log.Error(log);
        }

        public void WriteLogFatal(string log)
        {
            Log.Fatal(log);
        }
    }
}
