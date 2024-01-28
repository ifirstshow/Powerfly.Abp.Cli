using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli.Definitions
{
    public class ApiSchemaDefinition : ApiDefinitionBase
    {
        public string Name { get; }

        public Dictionary<string, string> Enums { get; }

        public List<ApiPropertyDefinition> Properties { get; }

        public ApiSchemaDefinition(string name, JsonSchema schema)
        {
            Name = FormatTypeName(name);

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

            if (Name.Contains("<") && Name.EndsWith(">"))
            {
                var genericTypes = Name.Substring(Name.IndexOf("<")).Trim('<', '>').Split(',')
                    .Select(t => t.Trim())
                    .ToArray();
                Name = Name.Substring(0, Name.IndexOf("<"));

                if (genericTypes.Length == 1)
                {
                    Name = Name + "<T>";
                    foreach (var property in Properties)
                    {
                        property.Type = property.Type.Replace(genericTypes[0], "T");
                    }
                }
                else
                {
                    Name = Name + string.Join(", ", genericTypes.Select((t, i) => "T" + (i + 1)));
                    for (var i = 0; i < genericTypes.Length; i++)
                    {
                        foreach (var property in Properties)
                        {
                            property.Type = property.Type.Replace(genericTypes[i], "T" + (i + 1));
                        }
                    }
                }
            }
        }
    }
}
