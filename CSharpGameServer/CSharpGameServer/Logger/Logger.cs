using System.Diagnostics.CodeAnalysis;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CSharpGameServer.Logger
{
    public class LoggerManager
    {
        private readonly LoggingLevelSwitch loggerLevel = new(LogEventLevel.Debug);

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

        [field: AllowNull, MaybeNull]
        public static LoggerManager Instance => field ??= new LoggerManager();

        public void WriteLogVerb(string log, params object[] objects)
        {
            if (loggerLevel.MinimumLevel < LogEventLevel.Verbose)
            {
                return;
            }

            Log.Verbose(log, objects);
        }

        public void WriteLogDebug(string log, params object[] objects)
        {
            if (loggerLevel.MinimumLevel < LogEventLevel.Debug)
            {
                return;
            }

            Log.Debug(log, objects);
        }

        public void WriteLogInfo(string log, params object[] objects)
        {
            if (loggerLevel.MinimumLevel < LogEventLevel.Information)
            {
                return;
            }

            Log.Information(log, objects);
        }

        public void WriteLogWarn(string log, params object[] objects)
        {
            if (loggerLevel.MinimumLevel < LogEventLevel.Warning)
            {
                return;
            }

            Log.Warning(log, objects);
        }

        public void WriteLogError(string log, params object[] objects)
        {
            if (loggerLevel.MinimumLevel < LogEventLevel.Error)
            {
                return;
            }

            Log.Error(log, objects);
        }

        public static void WriteLogFatal(string log, params object[] objects)
        {
            Log.Fatal(log, objects);
        }
    }
}
