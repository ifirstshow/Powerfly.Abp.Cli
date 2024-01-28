using NJsonSchema;
using System.Text.RegularExpressions;

namespace Powerfly.Abp.Cli.Definitions
{
    public class ApiDefinitionBase
    {
        protected string FormatTypeName(JsonSchema schema)
        {
            switch (schema.Type)
            {
                case JsonObjectType.None:
                    if (schema.HasReference)
                    {
                        return FormatTypeName(schema.Reference.Title);
                    }
                    return "any";
                case JsonObjectType.Array:
                    return (FormatTypeName(schema.Item) ?? "any") + "[]";
                case JsonObjectType.Boolean:
                    return "boolean";
                case JsonObjectType.Integer:
                case JsonObjectType.Number:
                    return "number";
                case JsonObjectType.Null:
                    return "null";
                case JsonObjectType.Object:
                    return $"Record<string, {FormatTypeName(schema.AdditionalPropertiesSchema)}>";
                case JsonObjectType.String:
                    return "string";
                case JsonObjectType.File:
                default:
                    return "any";
            }
        }

        protected string FormatTypeName(string fullTypeName)
        {
            var m = Regex.Match(fullTypeName, @"`\d\[");
            if (m.Success)
            {
                var mainType = fullTypeName.Substring(0, fullTypeName.IndexOf(m.Value));
                var subTypes = fullTypeName.Substring(fullTypeName.IndexOf(m.Value))
                    .Substring(m.Value.Length)
                    .Trim('[', ']')
                    .Split("],[");
                return $"{FormatTypeName(mainType)}<{string.Join(", ", subTypes.Select(t => FormatTypeName(t)))}>";
            }

            return fullTypeName.Split(',')[0].Split('.').Last();
        }
    }
}
