using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdHocCrawler.MsdnMagazine
{
    class MsdnIssue
    {
        public int Year { get; private set; }
        public string Title { get; private set; }
        public string CoverImageUrl { get; private set; }

        private readonly string _url;
        private HtmlDocument htmlDoc;

        public MsdnIssue(int year, string title, string imageUrl, string url)
        {
            Year = year;
            Title = title;
            CoverImageUrl = imageUrl;
            _url = url;
        }

        public MsdnIssue(int year, HtmlNode tableCellCover)
        {
            var cellInfos = tableCellCover.NextSibling;

            Year = year;
            Title = cellInfos.Element("strong").InnerText;
            CoverImageUrl = tableCellCover.Descendants("img").First().GetAttributeValue("src", "");
            _url = tableCellCover.Element("a").GetAttributeValue("href", "");
        }

        public static async Task<IEnumerable<MsdnIssue>> GetAllIssues()
        {
            const string allissuesPage = "http://msdn.microsoft.com/en-us/magazine/ee310108.aspx";

            var doc = await WebHelper.DownloadPageAsync(allissuesPage);

            foreach (var yearElement in doc.DocumentNode.Descendants("h1"))
            {
                foreach (var tr in yearElement.NextSibling.Descendants("tr"))
                {
                    foreach (var cell in tr.Elements("td"))
                    {
                        if (cell.Descendants("img").Any())
                        {
                            var year = int.Parse(yearElement.InnerText);
                            //var issue = new MsdnIssue(year, cell);
                        }
                    }
                }
            }

            return from yearElement in doc.DocumentNode.Descendants("h1")
                   from tr in yearElement.NextSibling.Descendants("tr")
                   from cell in tr.Elements("td")
                   where cell.Descendants("img").Any()
                   select new MsdnIssue(int.Parse(yearElement.InnerText), cell);

        }
    }
}
