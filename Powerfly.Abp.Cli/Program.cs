using Powerfly.Abp.Cli.Commands;
using System.CommandLine;

namespace Powerfly.Abp.Cli
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
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
