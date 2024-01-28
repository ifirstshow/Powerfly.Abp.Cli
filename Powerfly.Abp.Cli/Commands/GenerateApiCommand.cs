using NSwag;
using Powerfly.Abp.Cli.Definitions;
using Scriban;
using Scriban.Runtime;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;

namespace Powerfly.Abp.Cli.Commands
{
    internal class GenerateApiCommand : CommandBase<GenerateApiCommandOptions>
    {
        internal override Command GetCommand()
        {
            var command = new Command("api", "Generate api code base on swagger json document");
            command.AddOption(new Option<string[]>(new string[] { "--swagger-urls", "-url" }, "The swagger json document url.") { IsRequired = true });
            command.AddOption(new Option<string>(new string[] { "--output-folder", "-o" }, () => "./", "Specifies the output folder. Default value is the ./ directory."));
            command.AddOption(new Option<string>(new string[] { "--template-source", "-ts" }, ()=> "./Templates", "Specifies a custom template source to use to build the project. Default value is the ./Templates directory."));
            command.AddOption(new Option<string>(new string[] { "--project-name", "-n" }, () => "AbpApi", "Specifies the project's name. Default value is AbpApi."));

            command.SetHandler(HandleAsync, new CommandBinder<GenerateApiCommandOptions>(command.Options));

            return command;
        }

        protected override async Task HandleAsync(GenerateApiCommandOptions options)
        {
            var schemasList = new List<ApiSchemaDefinition>();
            var operationsList = new List<ApiOperationDefinition>();

            foreach (var swaggerUrl in options.SwaggerUrls)
            {
                var document = await OpenApiDocument.FromUrlAsync(swaggerUrl);

                foreach (var item in document.Components.Schemas)
                {
                    item.Value.Title = item.Key;
                }

                var schemas = document.Components.Schemas.Select(t => new ApiSchemaDefinition(t.Key, t.Value));
                schemasList.AddRange(schemas);

                var operations = document.Paths.SelectMany(p => p.Value.Select(t => new ApiOperationDefinition(p.Key, t.Key, t.Value)));
                operationsList.AddRange(operations);
            }

            foreach (var schema in schemasList.Where(t => t.Name.Contains("<") && t.Name.EndsWith(">")))
            {
            }

            schemasList = schemasList.DistinctBy(t => t.Name)
                .OrderBy(t => t.Name)
                .ToList();

            operationsList = operationsList.DistinctBy(t => $"{t.Method} {t.Path}")
                .OrderBy(t => t.Path)
                .ThenBy(t => t.Method)
                .ToList();



            var filePath = Path.Combine(AppContext.BaseDirectory, $"{options.TemplateSource}/TypescriptApi.tmpl");
            if (File.Exists(filePath))
            {
                var tmpl = File.ReadAllText(filePath);
                var template = Template.Parse(tmpl);

                var script = new ScriptObject();
                script.Import(new
                {
                    project = options.ProjectName,
                    schemas = schemasList,
                    operations = operationsList.GroupBy(t => t.Tags?.FirstOrDefault())
                        .OrderBy(t => t.Key)
                        .ToList(),
                });
                script.Import(typeof(StringHelper));

                var content = template.Render(new TemplateContext(script));

                File.WriteAllText(Path.Combine(options.OutputFolder, options.ProjectName + ".ts"), content);
            }
        }
    }
}