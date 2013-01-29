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
        public static Task<HtmlDocument> DownloadPageAsync(string url)
        {
            return DownloadPageAsync(new Uri(url));
        }

        public static async Task<HtmlDocument> DownloadPageAsync(Uri url)
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };
            var result = new HtmlDocument();
            
            result.LoadHtml(await webClient.DownloadStringTaskAsync(url));

            return result;
        }
    }
}
