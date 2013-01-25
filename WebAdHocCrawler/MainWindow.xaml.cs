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
            GetMsdnIssues();
        }

        private async void GetMsdnIssues()
        {
            EnterWaitMode();

            var issues = from issue in await MsdnMagazine.MsdnIssue.GetAllIssues()
                         select issue.Title + " " + issue.Year.ToString();

            this.ResultTextBox.Text = string.Join("\n", issues);

            ExitWaitMode();
        }

        private async void GetAuthorsFromFeedBooks()
        {
            EnterWaitMode();

            var urls = new[] { "http://fr.feedbooks.com/list/8301/", "http://fr.feedbooks.com/list/8302/", "http://fr.feedbooks.com/list/8303/", };

            this.ResultTextBox.Text = "";
            foreach (var url in urls)
            {
                this.ResultTextBox.Text += await FeedBookHelper.GetAuthorsWithBooks(url) + "\r\n\r\n";
            }


            ExitWaitMode();
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
