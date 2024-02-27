using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powerfly.Abp.Cli
{
    public static class StringHelper
    {
        public static string Replace(string text, string oldValue, string newValue)
        {
            return text.Replace(oldValue, newValue);
        }

        public static string Pascalize(string text)
        {
            return text.Pascalize();
        }

        public static string Camelize(string text)
        {
            return text.Camelize();
        }

        public static string Underscore(string text)
        {
            return text.Underscore();
        }

        public static string Kebaberize(string text)
        {
            return text.Kebaberize();
        }

        public static string UpperCase(string text)
        {
            return text.ToUpper();
        }

        public static string LowerCase(string text)
        {
            return text.ToLower();
        }
    }
}
