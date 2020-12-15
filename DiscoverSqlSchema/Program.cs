using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using SqlCore;

namespace DiscoverSqlSchema
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    new[] { "--server", "--s" },
                    getDefaultValue: () => @".\DevSql",
                    description: "The SQL Server to connect to"
                ),
                new Option<string>(
                    new[] { "--database", "--db" },
                    getDefaultValue: () => "AdventureWorksLT2019",
                    description: "The SQL database instance"
                )
            };

            rootCommand.Description = "Enumerate the schema for the provided database";

            rootCommand.Handler = CommandHandler.Create<string, string>(async (server, database) =>
            {
                try
                {
                    using var connection = await server
                        .BuildConnectionString(database)
                        .InitalizeConnection();

                    var schema = await connection.GetSchemaAsync("Tables");

                    // TODO: Adjust our EF OnModelCreating with: modelBuilder.HasDefaultSchema("api")
                    // This will enable the Where clause below to become:
                    // .Where(x => x[1].ToString() == "api")
                    var tables = schema
                        .AsEnumerable()
                        .Where(x =>
                            !(x[2].ToString() == "sysdiagrams") &&
                            !(x[2].ToString().StartsWith('_'))
                        )
                        .Select(x => x[2].ToString())
                        .ToList();

                    foreach (var table in tables)
                    {
                        Console.WriteLine(table);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            return await rootCommand.InvokeAsync(args);
        }
    }
}
