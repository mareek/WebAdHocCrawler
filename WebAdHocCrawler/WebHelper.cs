using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

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
            var result = new HtmlDocument();

            result.LoadHtml(await DownloadStringAsync(url));

            return result;
        }

        public static Task<JArray> DownloadJsonArrayAsync(string url)
        {
            return DownloadJsonArrayAsync(new Uri(url));
        }

        public static async Task<JArray> DownloadJsonArrayAsync(Uri url)
        {
            return JArray.Parse(await DownloadStringAsync(url));
        }

        public static Task<JObject> DownloadJsonObjectAsync(string url)
        {
            return DownloadJsonObjectAsync(new Uri(url));
        }

        public static async Task<JObject> DownloadJsonObjectAsync(Uri url)
        {
            return JObject.Parse(await DownloadStringAsync(url));
        }

        public static Task DownloadFileAsync(string url, FileInfo destination)
        {
            return DownloadFileAsync(new Uri(url), destination);
        }

        public static Task DownloadFileAsync(Uri url, FileInfo destination)
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };
            return webClient.DownloadFileTaskAsync(url, destination.FullName);
        }

        private static Task<string> DownloadStringAsync(Uri url)
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };
            return webClient.DownloadStringTaskAsync(url);
        }
    }
}
