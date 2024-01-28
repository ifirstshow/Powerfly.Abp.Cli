using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli.Commands
{
    internal class GenerateCommand
    {
        internal Command GetCommand()
        {
            var command = new Command("generate", "Code generate");

            command.AddCommand(new GenerateApiCommand().GetCommand());

            return command;
        }
    }
}
