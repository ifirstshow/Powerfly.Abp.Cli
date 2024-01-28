using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli.Definitions
{
    public class ApiSchemaDefinition
    {
        public string Name { get; }

        public Dictionary<string, string> Enums { get; }

        public List<ApiPropertyDefinition> Properties { get; }

        public ApiSchemaDefinition(string name, JsonSchema schema)
        {
            Name = RefectionHelper.FormatTypeName(name);

            if (schema.IsEnumeration)
            {
                Enums = schema.Enumeration.Select(Convert.ToString)
                    .Zip(schema.EnumerationNames ?? schema.Enumeration.Select(t => $"Value{Convert.ToInt32(t)}"),
                    (value, name) => new { Value = value, Name = name })
                    .ToDictionary(t => t.Name, t => t.Value);
            }
            else
            {
                Properties = new List<ApiPropertyDefinition>();
                foreach (var property in schema.ActualProperties)
                {
                    Properties.Add(new ApiPropertyDefinition(property.Key, property.Value));
                }
            }
        }
    }
}
