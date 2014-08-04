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
using WebAdHocCrawler.MsdnMagazine;
using System.Web;
using System.Text.RegularExpressions;

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
            if (this.UrlTextBox.Text.Any())
            {
                LaunchLongRunningOperation(DownloadPage);
            }
            else
            {
                LaunchLongRunningOperation(DownloadMeteoRadarImages);
            }
        }

        private async Task GetMeteoRadarImages()
        {
            var radarImages = (await MeteoFranceHelper.GetLasRadarImagesInfos()).ToList();
            ResultDataGrid.ItemsSource = radarImages;
            ResultTextBox.Visibility = Visibility.Collapsed;
            ResultDataGrid.Visibility = Visibility.Visible;
        }

        private async Task DownloadMeteoRadarImages()
        {
            await MeteoFranceHelper.DownloadLastRadarImages();
        }

        private async Task GetSteamHalowwenSales()
        {
            var steamGames = (await SteamHelper.GetHaloweenGames()).ToList();
            ResultDataGrid.ItemsSource = steamGames;
            ResultTextBox.Visibility = Visibility.Collapsed;
            ResultDataGrid.Visibility = Visibility.Visible;
        }

        private async Task ScannGoogleSearch()
        {
            await SearchRange("asus S200E CT{0}H", 100, 300);
        }

        private async Task SearchRange(string queryToFormat, int rangeStart, int rangeStop)
        {
            //https://www.google.com/search?btnG=1&pws=0&q=asus+S200E+CT242H
            for (var i = rangeStart; i <= rangeStop; i++)
            {
                var searchTerm = string.Format(queryToFormat, i);
                var nbresults = await GoogleResultCount(searchTerm);
                if (nbresults > 0)
                    Log(searchTerm + " : " + nbresults.ToString());
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
        }


        private static readonly Regex resultStatsRegexp = new Regex(@"^About\s*(?<resultats>\d+.){1,5}\s*results*");
        private static async Task<long> GoogleResultCount(string searchTerm)
        {
            var url = "https://www.google.com/search?btnG=1&pws=0&q=" + HttpUtility.UrlEncode(searchTerm);

            var page = await WebHelper.DownloadPageAsync(url);

            //<div id="resultStats">About 385,000 results</div>
            var resultStatsText = (from div in page.DocumentNode.Descendants("div")
                                   where div.GetAttributeValue("id", "") == "resultStats"
                                   select HttpUtility.HtmlDecode(div.InnerText)).First();

            //About 385,000 results
            var regexpResult = resultStatsRegexp.Match(resultStatsText);

            if (!regexpResult.Success)
                return 0;
            else
            {
                var numberParts = regexpResult.Groups["resultats"].Captures.OfType<Capture>().Select(c => c.Value);
                var strNumber = string.Concat(numberParts.Select(p => p.Substring(0, p.Length - 1)));
                long result;
                return long.TryParse(strNumber, out result) ? result : 0;
            }
        }

        private async Task TinkerWithMsdn()
        {
            var issue = (await MsdnIssue.GetAllIssues()).First();
            var articles = await issue.Tinker();
            this.ResultTextBox.Text = string.Join("\n", articles);
        }

        private async Task DownloadPage()
        {
            var url = this.UrlTextBox.Text;
            var page = await WebHelper.DownloadPageAsync(url);
            this.ResultTextBox.Text = page.DocumentNode.OuterHtml;
            ResultTextBox.Visibility = Visibility.Visible;
            ResultDataGrid.Visibility = Visibility.Collapsed;
        }

        private async Task GetMsdnIssues()
        {
            var issues = from issue in await MsdnMagazine.MsdnIssue.GetAllIssues()
                         select issue.Title + " " + issue.Year.ToString();

            this.ResultTextBox.Text = string.Join("\n", issues);
        }

        private async Task GetAuthorsFromFeedBooks()
        {
            var urls = new[] { "http://fr.feedbooks.com/list/8301/", "http://fr.feedbooks.com/list/8302/", "http://fr.feedbooks.com/list/8303/", };

            this.ResultTextBox.Text = "";
            foreach (var url in urls)
            {
                this.ResultTextBox.Text += await FeedBookHelper.GetAuthorsWithBooks(url) + "\r\n\r\n";
            }
        }

        private async void LaunchLongRunningOperation(Func<Task> longRunningOperation)
        {
            SetIsEnabled(false);

            await longRunningOperation();

            SetIsEnabled(true);
        }

        private void SetIsEnabled(bool enabled)
        {
            this.LaunchButton.IsEnabled = enabled;
            this.UrlTextBox.IsEnabled = enabled;
            this.ResultTextBox.IsEnabled = enabled;
        }

        private void Log(string text)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.ResultTextBox.AppendText(text + "\r\n");
                    this.ResultTextBox.ScrollToEnd();
                }));
        }
    }
}
