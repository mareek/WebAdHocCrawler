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
                LaunchLongRunningOperation(TinkerWithMsdn);
            }
        }

        private async Task TinkerWithMsdn()
        {
            var issue = new MsdnIssue(2013, "Janvier", new System.IO.FileInfo(@"C:\Users\Matthieu\Desktop\Raw msdn magasine.html"));
            var articles = await issue.Tinker();
            this.ResultTextBox.Text = string.Join("\n", articles);
        }

        private async Task DownloadPage()
        {
            var url = this.UrlTextBox.Text;
            var page = await WebHelper.DownloadPageAsync(url);
            this.ResultTextBox.Text = page.DocumentNode.OuterHtml;
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
    }
}
