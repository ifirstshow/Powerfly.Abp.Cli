using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli.Commands
{
    internal record GenerateApiCommandOptions
    {
        public string[] SwaggerUrls { get; set; } = new string[0];

        public string? OutputFolder { get; set; }

        public string? TemplateSource { get; set; }

        public string? ProjectName { get; set; }
    }
}
