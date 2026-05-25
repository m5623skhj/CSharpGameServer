using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CSharpGameServer.DB.Migration
{
    public class MigrationRunner
    {
        [field: AllowNull, MaybeNull]
        public static MigrationRunner Instance => field ??= new MigrationRunner();

        public bool RunMigration()
        {
            var config = new Config.Config();
            if (config.ReadConfig() == false)
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed: config read failed");
                return false;
            }

            var migratorFilePath = config.Conf.MigratorFilePath;
            if (string.IsNullOrWhiteSpace(migratorFilePath))
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed: migrator file path is empty");
                return false;
            }

            if (File.Exists(migratorFilePath) == false)
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed: migrator file does not exist {migratorFilePath}", migratorFilePath);
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = migratorFilePath
            };

            try
            {
                var process = Process.Start(startInfo);
                if (process == null)
                {
                    Logger.LoggerManager.WriteLogFatal("Migration failed: process start returned null");
                    return false;
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    Logger.LoggerManager.WriteLogFatal("Migration failed with exit code {exitCode}", process.ExitCode);
                    return false;
                }

                Logger.LoggerManager.Instance.WriteLogInfo("Migration succeeded");
                return true;
            }
            catch (Exception e)
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed with {exception}", e.Message);
                return false;
            }
        }
    }
}
