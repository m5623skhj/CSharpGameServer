using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CSharpGameServer.DB.Migration
{
    public class MigrationRunner
    {
        private const string MigratorFilePath = "";

        [field: AllowNull, MaybeNull]
        public static MigrationRunner Instance => field ??= new MigrationRunner();

        public bool RunMigration()
        {
            if (string.IsNullOrWhiteSpace(MigratorFilePath))
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed: migrator file path is empty");
                return false;
            }

            if (File.Exists(MigratorFilePath) == false)
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed: migrator file does not exist {migratorFilePath}", MigratorFilePath);
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = MigratorFilePath
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
