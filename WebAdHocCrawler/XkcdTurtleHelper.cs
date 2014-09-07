using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdHocCrawler
{
    public static class XkcdTurtleHelper
    {
        public static async Task LoadImageNames(ObservableCollection<string> resultCollection)
        {
            var buffer = new BlockingCollection<string>();
            buffer.Add("turtles");
            foreach (var image in buffer.GetConsumingEnumerable())
            {
                var nextBatch = await LoadNextImages(resultCollection, image);
                foreach (var nextImage in nextBatch)
                {
                    buffer.Add(nextImage);
                }
                if (!buffer.Any())
                {
                    buffer.CompleteAdding();
                }
            }
            //await LoadNextImages(resultCollection, new[] { "turtles" });
        }

        private static async Task<IEnumerable<string>> LoadNextImages(ObservableCollection<string> resultCollection, params string[] images)
        {
            var nextImages = new List<string>();
            foreach (var image in images)
            {
                nextImages.AddRange(await GetNextImages(image));
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            var nextBatch = nextImages.Except(resultCollection).ToList();

            foreach (var nextImage in nextBatch)
            {
                resultCollection.Add(nextImage);
            }

            return nextBatch;
        }

        private static async Task<IEnumerable<string>> GetNextImages(string imageName)
        {
            var response = await WebHelper.DownloadJsonObjectAsync("http://c.xkcd.com/turtle/" + imageName);
            // Response : {"white":["book-launch","whatif-trade"],"black":["launch-planet","stars-1","stars-2"]}
            var blackImages = response["black"].Select(e => e.ToString());
            var whiteImages = response["white"].Select(e => e.ToString());
            return blackImages.Concat(whiteImages);
        }
    }
}
