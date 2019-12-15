using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Finder
{
    public class HttpQuery
    {
        public static string Get(string FindUrl)
        {

            using (WebClient ClienteHttp = new WebClient())
            {
                ClienteHttp.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                return ClienteHttp.DownloadString(FindUrl);
            }

        }
    }
}
