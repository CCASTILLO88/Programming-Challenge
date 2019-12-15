using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finder.Helpers
{
    public class Utils
    {
        public static string RemoveChar( string OldString, char TheChar )
        {
            string NewString = "";

            foreach (char letter in OldString)
            {
                if (letter != TheChar)
                    NewString += letter;
            }

            return NewString;
        }
    }
}
