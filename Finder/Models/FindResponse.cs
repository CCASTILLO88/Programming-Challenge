using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finder.Models
{
    public class FindResponse
    {
        public string Keyword { get; set; }
        public long GoogleResults { get; set; }
        public long YahooResults { get; set; }
    }
}
