using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli
{
    internal static class RefectionHelper
    {
        public static string FormatTypeName(string fullTypeName)
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
