using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebAdHocCrawler
{
    static class WebHelper
    {
        public static async Task<HtmlDocument> DownloadPageAsync(string url)
        {
            var webClient = new WebClient();
            var result = new HtmlDocument();
            
            result.LoadHtml(await webClient.DownloadStringTaskAsync(url));

            return result;
        }
    }
}
