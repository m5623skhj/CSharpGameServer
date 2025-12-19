using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CSharpGameServer.DB.Migration
{
    public class MigrationRunner
    {
        private const string MigratorFilePath = "";

        [field: AllowNull, MaybeNull]
        public static MigrationRunner Instance => field ??= new MigrationRunner();

        public int RunMigration()
        {
            var migrationResult = 0;
            var startInfo = new ProcessStartInfo
            {
                FileName = MigratorFilePath
            };

            try
            {
                var process = Process.Start(startInfo);
                if(process == null)
                {
                    return migrationResult;
                }

                process.WaitForExit();
                migrationResult = process.ExitCode;

                Logger.LoggerManager.Instance.WriteLogInfo("Migration succeeded");
            }
            catch (Exception e)
            {
                Logger.LoggerManager.WriteLogFatal("Migration failed with {exception}", e.Message);
                return migrationResult;
            }

            return migrationResult;
        }
    }
}
