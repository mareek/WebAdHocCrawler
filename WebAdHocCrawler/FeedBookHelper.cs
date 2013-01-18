using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdHocCrawler
{
    static class FeedBookHelper
    {
        public static async Task<string> GetPageAuthorsReport(string url)
        {
            var page = await WebHelper.DownloadPageAsync(url);

            var pageTitle = GetPageTitle(page);
            var authors = ExtractAuthors(page.DocumentNode).OrderBy(a => a).Distinct();
            return pageTitle + "\r\n" + new string('-', pageTitle.Length) + "\r\n\r\n" + string.Join("\r\n", authors);
        }

        public static async Task<string> GetAuthorsWithBooks(string url)
        {
            var page = await WebHelper.DownloadPageAsync(url);

            var pageTitle = GetPageTitle(page);

            var booksByAuthor = from bookBlock in ExtractBookBlocks(page)
                                from title in ExtractTitles(bookBlock)
                                from author in ExtractAuthors(bookBlock)
                                select new { Title = title, Author = author } into singleAuthorBook
                                group singleAuthorBook.Title by singleAuthorBook.Author into bibliography
                                orderby bibliography.Key
                                select bibliography.Key + " - " + string.Join(", ", bibliography);

            return pageTitle + "\r\n" + new string('-', pageTitle.Length) + "\r\n\r\n" + string.Join("\r\n", booksByAuthor);
        }

        private static string GetPageTitle(HtmlDocument page)
        {
            return (from headNode in page.DocumentNode.Descendants("head")
                    from titleNode in headNode.Elements("title")
                    select titleNode.InnerText).FirstOrDefault();
        }

        private static IEnumerable<string> ExtractAuthors(HtmlNode container)
        {
            // <a href="/store/top?contributor=Isaac+Marion&amp;lang=fr" class="gray" title="Isaac Marion">Isaac Marion</a>
            return from element in container.Descendants("a")
                   where element.GetAttributeValue("class", "") == "gray"
                         && element.GetAttributeValue("href", "").StartsWith("/store/top?contributor=")
                   select element.InnerText;
        }

        private static IEnumerable<string> ExtractTitles(HtmlNode container)
        {
            // <a href="http://fr.feedbooks.com/item/316137/les-les-de-l-espace" itemprop="url">Les Îles de l'espace</a>
            return from element in container.Descendants("a")
                   where element.GetAttributeValue("href", "").StartsWith("http://fr.feedbooks.com/item/")
                   select element.InnerText;
        }

        private static IEnumerable<HtmlNode> ExtractBookBlocks(HtmlDocument page)
        {
            /* <div class="span-11 prepend-1 append-bottom">
                 <h3 class="fb-blue small-bottom-margin"><a href="http://fr.feedbooks.com/item/316137/les-les-de-l-espace" itemprop="url">Les Îles de l'espace</a></h3>
      
                   <h4 class="series small-bottom-margin">
                     <a href="http://fr.feedbooks.com/list/6293/la-trilogie-de-l-espace" class="charcoal">La Trilogie de l’espace #2</a>
                   </h4>
       
       
                 <h4 class="charcoal">de 
                   <em>
                     <a href="/store/top?contributor=Arthur+C.+Clarke&amp;lang=fr" class="gray" title="Arthur C. Clarke">Arthur C. Clarke</a>
                   </em>    
                 </h4> 
                 <p>Le jeune Roy Malcolm gagne un concours télévisé qui lui permet de faire un stage sur une station orbitale. Mais son séjour là-haut se révèle encore plus aventureuse que même lui aurait osé rêver !</p>
               </div> */
            return from element in page.DocumentNode.Descendants("div")
                   where element.GetAttributeValue("class", "") == "span-11 prepend-1 append-bottom"
                         && element.Descendants("h3").Any()
                         && element.Descendants("h4").Any()
                   select element;
        }
    }
}
