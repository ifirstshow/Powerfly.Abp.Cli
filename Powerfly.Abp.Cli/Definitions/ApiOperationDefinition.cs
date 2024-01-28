using Humanizer;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli.Definitions
{
    public class ApiOperationDefinition
    {
        public string Path { get; }

        public string Method { get; }

        public string Name { get; }

        public List<string> Tags { get; }

        public List<ApiPropertyDefinition> PathParameters { get; }

        public List<ApiPropertyDefinition> QueryParameters { get; }

        public string? RequestBody { get; }

        public string? Response { get; }

        public ApiOperationDefinition(string path, string method, OpenApiOperation schema)
        {
            Path = path;
            Method = method.ToUpper();
            Tags = schema.Tags;

            var tag = Tags.FirstOrDefault();
            if (tag == "Login")
            {
                Tags.Remove(tag);
                Tags.Insert(0, "Account");
            }
            else if (tag == "Profile")
            {
                Tags.Remove(tag);
                Tags.Insert(0, "MyProfile");
            }

            PathParameters = schema.ActualParameters.Where(t => t.Kind == OpenApiParameterKind.Path)
                .Select(t => new ApiPropertyDefinition(t))
                .ToList();

            QueryParameters = schema.ActualParameters.Where(t => t.Kind == OpenApiParameterKind.Query)
                .Select(t => new ApiPropertyDefinition(t))
                .ToList();

            if (schema.RequestBody != null)
            {
                OpenApiMediaType body;
                if (schema.RequestBody.Content.TryGetValue("application/json", out body)
                    || schema.RequestBody.Content.TryGetValue("text/json", out body))
                {
                    if (body.Schema.HasReference)
                    {
                        RequestBody = RefectionHelper.FormatTypeName(body.Schema.Reference.Title);
                    }
                }
            }

            if (schema.ActualResponses.TryGetValue("200", out var response))
            {
                if (response.Schema?.HasReference == true)
                {
                    Response = RefectionHelper.FormatTypeName(response.Schema.Reference.Title);
                }
            }

            Name = FormatApiName();
        }

        private string FormatApiName()
        {
            var findMatch = Regex.Match(Path, @"(by-\w+)/{\w+}$");
            if (findMatch.Success && Method == "GET")
            {
                return $"Find{findMatch.Groups[1].Value.Titleize().Pascalize()}";
            }

            var builder = new StringBuilder();

            if (Tags.Any())
            {
                var tag = Tags.First();
                var reg = string.Join("[/-]", tag.Kebaberize().Split('-').Select(t => $"(?:(?:{t})|(?:{t.Pluralize()}))"));

                if (Regex.IsMatch(Path, $"{reg}(?:/{{id}})?$"))
                {
                    switch (Method)
                    {
                        case "GET":
                            builder.Append("Get");
                            if (Response?.Contains("ListResult") == true || Response?.Contains("PagedResult") == true)
                            {
                                builder.Append("List");
                            }
                            break;
                        case "POST":
                            builder.Append("Create");
                            break;
                        case "PUT":
                            builder.Append("Update");
                            break;
                        case "DELETE":
                            builder.Append("Remove");
                            break;
                        default:
                            builder.Append(Method.ToLower().Pascalize());
                            break;
                    }

                    return builder.ToString();
                }
            }

            if (Method == "GET" && Response != null && !new string[] {"search", "find"}.Contains(Path.Split('/').Last()))
            {
                builder.Append("Get");
                builder.Append(Path.Split('/').Last().Titleize().Pascalize());

                if (Response?.Contains("ListResult") == true || Response?.Contains("PagedResult") == true)
                {
                    builder.Append("List");
                }
            }
            else if (Method == "PUT")
            {
                builder.Append("Update");
                builder.Append(Path.Split('/').Last().Titleize().Pascalize());
            }
            else
            {
                builder.Append(Path.Split('/').Last().Titleize().Pascalize());
            }

            return builder.ToString();
        }
    }
}
