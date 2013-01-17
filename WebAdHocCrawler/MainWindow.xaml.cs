using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;

namespace WebAdHocCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            GetAuthorsFromFeedBooks();
        }

        private async void GetAuthorsFromFeedBooks()
        {
            EnterWaitMode();

            var urls = new[] { "http://fr.feedbooks.com/list/8301/", "http://fr.feedbooks.com/list/8302/", "http://fr.feedbooks.com/list/8303/", };

            var reports = new List<string>();
            foreach (var url in urls)
            {
                reports.Add(await GetFeedbookPageAuthorsReport(url));
            }

            this.ResultTextBox.Text = string.Join("\r\n\r\n", reports);

            ExitWaitMode();
        }

        private async Task<string> GetFeedbookPageAuthorsReport(string url)
        {
            var page = await WebHelper.DownloadPageAsync("http://fr.feedbooks.com/list/8303/");

            var title = (from headNode in page.DocumentNode.Descendants("head")
                         from titleNode in headNode.Elements("title")
                         select titleNode.InnerText).FirstOrDefault();
            var authors = ExtractAuthors(page.DocumentNode).Distinct();
            return title + "\r\n" + new string('-', title.Length) + "\r\n\r\n" + string.Join("\r\n", authors);
        }

        private IEnumerable<string> ExtractAuthors(HtmlNode container)
        {
            // <a href="/store/top?contributor=Isaac+Marion&amp;lang=fr" class="gray" title="Isaac Marion">Isaac Marion</a>
            return from element in container.Descendants("a")
                   where element.GetAttributeValue("class", "") == "gray"
                         && element.GetAttributeValue("href", "").StartsWith("/store/top?contributor=")
                   select element.InnerText;
        }

        private IEnumerable<string> ExtractTitles(HtmlNode container)
        {
            // <a href="http://fr.feedbooks.com/item/316137/les-les-de-l-espace" itemprop="url">Les Îles de l'espace</a>
            return from element in container.Descendants("a")
                   where element.GetAttributeValue("href", "").StartsWith("http://fr.feedbooks.com/item/")
                   select element.InnerText;
        }

        private IEnumerable<HtmlNode> GetBookBlocks(HtmlDocument page)
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

        private void EnterWaitMode()
        {
            SetIsEnabled(false);
        }

        private void ExitWaitMode()
        {
            SetIsEnabled(true);
        }

        private void SetIsEnabled(bool enabled)
        {
            this.LaunchButton.IsEnabled = enabled;
            this.ResultTextBox.IsEnabled = enabled;
        }
    }
}
