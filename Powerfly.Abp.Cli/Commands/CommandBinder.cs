using System;
using System.Collections.Generic;
using System.CommandLine.Binding;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Powerfly.Abp.Cli.Commands
{
    internal class CommandBinder<TOptions> : BinderBase<TOptions> where TOptions : class
    {
        private readonly IReadOnlyList<Option> options;

        public CommandBinder(IReadOnlyList<Option> options)
        {
            this.options = options;
        }

        protected override TOptions GetBoundValue(BindingContext bindingContext)
        {
            var result = Activator.CreateInstance<TOptions>();
            var type = typeof(TOptions);
            foreach (var option in options)
            {
                var name = option.Name;
                name = name[0].ToString().ToUpper() + Regex.Replace(name.Substring(1), "-\\w", m => m.Value.TrimStart('-').ToUpper());

                var property = type.GetProperty(name);
                if (property != null)
                    property.SetValue(result, bindingContext.ParseResult.GetValueForOption(option));
            }
            return result;
        }
    }
}
