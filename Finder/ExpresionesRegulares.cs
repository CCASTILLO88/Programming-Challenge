using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finder
{
    public class ExpresionesRegulares
    {
        public static string ExtractValue ( string value, string regex_code)
        {
            Regex regex = new Regex(regex_code, RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(value);
            if (matches.Count > 0)
                return matches[0].Groups[1].Value;
            else
                return "NULL";
        }
        public static long ExtractNumbers(string Value)
        {
            try
            {
                return Convert.ToInt64(Regex.Replace(Value, @"[^\d]", ""));
            }
            catch
            {
                return 0;
            }
            
        }
    }
}
