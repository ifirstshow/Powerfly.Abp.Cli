using NJsonSchema;
using NSwag;

namespace Powerfly.Abp.Cli.Definitions
{
    public class ApiPropertyDefinition
    {
        public string Name { get; }

        public string Type { get; }

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

            Type = FormatType(schema);
        }

        public ApiPropertyDefinition(OpenApiParameter parameter)
            : this(parameter.Name, parameter.Schema)
        {
            IsRequired = parameter.IsRequired;
        }

        private string FormatType(JsonSchema schema)
        {
            switch (schema.Type)
            {
                case JsonObjectType.None:
                    if (schema.HasReference)
                    {
                        return RefectionHelper.FormatTypeName(schema.Reference.Title);
                    }
                    return "any";
                case JsonObjectType.Array:
                    return (FormatType(schema.Item) ?? "any") + "[]";
                case JsonObjectType.Boolean:
                    return "boolean";
                case JsonObjectType.Integer:
                case JsonObjectType.Number:
                    return "number";
                case JsonObjectType.Null:
                    return "null";
                case JsonObjectType.Object:
                    return $"Record<string, {FormatType(schema.AdditionalPropertiesSchema)}>";
                case JsonObjectType.String:
                    return "string";
                case JsonObjectType.File:
                default:
                    return "any";
            }
        }
    }
}