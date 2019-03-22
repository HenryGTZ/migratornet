using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace migratornet.runner
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                CommandLineArgs parameter = new CommandLineArgs();
                if (!parameter.ContainsKey("cnn"))
                    throw new ArgumentException("No [cnn] parameter received. You need pass the connection string in order to execute the migrations");

                bool isRollBack = parameter.ContainsKey("rollback");

                string connectionString = parameter["cnn"];
                var serviceProvider = CreateServices(connectionString);
                using (var scope = serviceProvider.CreateScope())
                {
                    if (isRollBack)
                    {
                        long rollBackToVersion = 0;
                        if (parameter["rollback"].ToLower().Trim() == "one")
                        {
                            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                            var lastMigration = runner.MigrationLoader.LoadMigrations().LastOrDefault();
                            rollBackToVersion = lastMigration.Value.Version - 1;
                        }
                        else if (!long.TryParse(parameter["rollback"], out rollBackToVersion))
                            throw new ArgumentException($"Invalid rollback version value: [{parameter["rollback"]}]");

                        // Execute rollback
                        RollbackDatabase(scope.ServiceProvider, rollBackToVersion);
                    }
                    else
                        UpdateDatabase(scope.ServiceProvider);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error updating the database schema: {ex.Message}");
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </sumamry>
        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2014()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(M001_CreateInvoiceTable).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        private static void RollbackDatabase(IServiceProvider serviceProvider, long rollbackVersion)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(rollbackVersion);
        }
    }
}
