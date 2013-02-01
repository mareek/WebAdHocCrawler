using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdHocCrawler.MsdnMagazine
{
    class MsdnIssue
    {
        public int Year { get; private set; }
        public string Title { get; private set; }
        public Uri CoverImageUrl { get; private set; }

        private readonly Uri _url;
        private HtmlDocument htmlDoc;

        public MsdnIssue(int year, string title, Uri imageUrl, Uri url)
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
            CoverImageUrl = new Uri(tableCellCover.Descendants("img").First().GetAttributeValue("src", ""));
            _url = new Uri(tableCellCover.Element("a").GetAttributeValue("href", ""));
        }

        public MsdnIssue(int year, string title, FileInfo htmlFile)
        {
            Year = year;
            Title = title;

            this.htmlDoc = new HtmlDocument();
            using (var fileStream = htmlFile.OpenText())
                htmlDoc.Load(fileStream);
        }

        public static async Task<IEnumerable<MsdnIssue>> GetAllIssues()
        {
            const string allissuesPage = "http://msdn.microsoft.com/en-us/magazine/ee310108.aspx";

            var doc = await WebHelper.DownloadPageAsync(allissuesPage);

            return from yearElement in doc.DocumentNode.Descendants("h1")
                   from tr in yearElement.NextSibling.Descendants("tr")
                   from cell in tr.Elements("td")
                   where cell.Descendants("img").Any()
                   select new MsdnIssue(int.Parse(yearElement.InnerText), cell);
        }

        public async Task<IEnumerable<string>> Tinker()
        {
            return (await GetArticles()).Select(article => article.Label);
        }

        public async Task<IEnumerable<MsdnArticle>> GetArticles()
        {
            await LoadDocument();

            Func<HtmlNode, Uri> generateLink = link => new Uri(_url, link.GetAttributeValue("href", ""));

            var articleNodes = from authorlink in htmlDoc.DocumentNode.Descendants("a")
                               where authorlink.GetAttributeValue("href", "").StartsWith("/magazine/ee")
                               select authorlink.ParentNode;

            return from articleNode in articleNodes.Distinct()
                   select new MsdnArticle(_url, articleNode);
        }

        private async Task LoadDocument()
        {
            if (this.htmlDoc == null)
            {
                this.htmlDoc = await WebHelper.DownloadPageAsync(_url);
            }
        }
    }
}
