using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli.Commands
{
    internal abstract class CommandBase<TOptions>
    {
        internal abstract Command GetCommand();

        protected abstract Task HandleAsync(TOptions options);
    }
}
