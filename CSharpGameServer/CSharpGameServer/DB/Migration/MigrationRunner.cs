using System.Diagnostics;

namespace CSharpGameServer.DB.Migration
{
    public class MigrationRunner
    {
        private static MigrationRunner? instance = null;
        private readonly string migratorFilePath = "";

        public static MigrationRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MigrationRunner();
                }

                return instance;
            }
        }

        public int RunMigration()
        {
            int migrationResult = 0;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = migratorFilePath;
            try
            {
                Process? process = Process.Start(startInfo);
                if(process == null)
                {
                    return migrationResult;
                }

                process.WaitForExit();
                migrationResult = process.ExitCode;

                Logger.LoggerManager.instance.WriteLogInfo("Migration succeeded");
            }
            catch (Exception e)
            {
                Logger.LoggerManager.instance.WriteLogFatal("Migration failed with {exception}", e.Message);
                return migrationResult;
            }

            return migrationResult;
        }
    }
}
