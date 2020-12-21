using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

using SqlCore;

namespace SqlCli
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
                ),
                new Option<string>(
                    new[] { "--query", "--q" },
                    getDefaultValue: () => "PersonByPhone.sql",
                    description: "The SQL query to execute"
                ),
                new Option<string>(
                    new[] { "--props", "--p" },
                    description: "Arguments to provide the SQL query"
                )
            };

            rootCommand.Description = "Dynamic SQL Query executor";

            rootCommand.Handler = CommandHandler.Create<string, string, string, string>(async (server, database, query, props) =>
            {
                try
                {
                    var script = await query.GetSqlScript();

                    if (!string.IsNullOrEmpty(props)) script = script.InterpolateScriptProps(props);

                    using var connection = await server.BuildConnectionString(database).InitalizeConnection();
                    using var command = connection.InitializeCommand(script);
                    var reader = await command.ResilientQuery();

                    Console.WriteLine(await reader.ReadResults());
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
