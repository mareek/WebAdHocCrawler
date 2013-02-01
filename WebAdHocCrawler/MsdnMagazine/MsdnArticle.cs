using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebAdHocCrawler.MsdnMagazine
{
    class MsdnArticle
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Column { get; private set; }
        private readonly Uri _url;

        public string Label { get { return string.Format("{0} : {1} by {2}     [{3}]", Column, Title, Author, _url); ; } }

        public MsdnArticle(string title, Uri url, string author, string column)
        {
            Title = title;
            _url = url;
            Author = author;
            Column = column;
        }

        public MsdnArticle(Uri baseUri, HtmlNode articleElement)
        {
            var links = articleElement.Descendants("a").ToList();
            var authorlinks = links.Where(l => l.GetAttributeValue("href", "").StartsWith("/magazine/ee")).ToList();
            var articleLink = links.Where(l => l.GetAttributeValue("href", "").StartsWith("jj")).Last();
            var columnLink = links.Where(l => l.GetAttributeValue("href", "").StartsWith("jj") && l.Descendants().OfType<HtmlTextNode>().Any()).First();

            Title = articleLink.Descendants().OfType<HtmlTextNode>().Last().InnerText.Trim();
            _url = new Uri(baseUri, articleLink.GetAttributeValue("href", ""));
            Column = columnLink.Descendants().OfType<HtmlTextNode>().First().InnerText.Trim();
            Column = Column.EndsWith(":") ? Column.Substring(0, Column.Length - 1) : Column;
            Author = string.Join(", ", authorlinks.Select(l => l.InnerText.Trim()));
        }
    }
}
