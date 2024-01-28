using NJsonSchema;
using NSwag;

namespace Powerfly.Abp.Cli.Definitions
{
    public class ApiPropertyDefinition : ApiDefinitionBase
    {
        public string Name { get; }

        public string Type { get; internal set; }

        public bool IsRequired { get; }

        public bool IsNullable { get; }

        public bool IsReadonly { get; }

        public ApiPropertyDefinition(string name, JsonSchema schema)
        {
            Name = name;
            IsNullable = schema.IsNullableRaw ?? false;

            if (schema is JsonSchemaProperty schemaProperty)
            {
                IsRequired = schemaProperty.IsRequired;
                IsReadonly = schemaProperty.IsReadOnly;
            }
            if (schema.HasReference)
            {
                switch (schema.Reference.Type)
                {
                    case JsonObjectType.Boolean:
                    case JsonObjectType.Integer:
                    case JsonObjectType.Number:
                        IsRequired = true;
                        break;
                    default:
                        break;
                }
            }

            Type = FormatTypeName(schema);
        }

        public ApiPropertyDefinition(OpenApiParameter parameter)
            : this(parameter.Name, parameter.Schema)
        {
            IsRequired = parameter.IsRequired;
        }
    }
}