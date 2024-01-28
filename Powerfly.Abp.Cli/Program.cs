using Powerfly.Abp.Cli.Commands;
using System.CommandLine;

namespace Powerfly.Abp.Cli
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            args = ["generate", "api", "-url", @"https://localhost:44330/swagger/v1/swagger.json", "-url", @"https://localhost:44353/swagger/v1/swagger.json"];

            var rootCommand = new RootCommand()
            {
                Description = "A tool to make development with abp vnext easier.",
                Name = "abpcli"
            };
            rootCommand.Add(new GenerateCommand().GetCommand());

            return await rootCommand.InvokeAsync(args);
        }
    }
}
