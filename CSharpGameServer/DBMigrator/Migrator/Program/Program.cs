using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;

namespace Migrator.Program
{
    class Program
    {
        struct DBConnectionInfo
        {
            public string server;
            public string name;
            public string userName;
            public string password;
        }

        static int Main(string[] args)
        {
            var connectionInfo = MakeDBConnectionInfo();
            if(connectionInfo == null)
            {
                Console.WriteLine("Connection info is null");
                return 0;
            }

            var servicesProvider = CreateServices(connectionInfo.Value);

            using var scope = servicesProvider.CreateScope();
            return UpdateDatabase(scope.ServiceProvider);
        }

        private static DBConnectionInfo? MakeDBConnectionInfo()
        {
            try
            {
                string configJson = File.ReadAllText("Config/config.json");
                return JsonSerializer.Deserialize<DBConnectionInfo>(configJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Config read failed with {ex}", ex);
            }

            return null;
        }

        private static IServiceProvider CreateServices(DBConnectionInfo connectionInfo)
        {
            string connectionString = 
                "server=" + connectionInfo.server + 
                ";database=" + connectionInfo.name + 
                ";UID=" + connectionInfo.userName + 
                ";password=" + connectionInfo.password ;

            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .WithGlobalStripComments(false)
                    .ScanIn(Assembly.GetExecutingAssembly())
                        .For.Migrations()
                        .For.EmbeddedResources())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static int UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            try
            {
                runner.MigrateUp();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Migration failed : " + ex.Message);
                return 1;
            }
        }
    }
}
