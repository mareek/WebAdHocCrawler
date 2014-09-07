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

/* actual list of image names:
turtles
launch-planet
stars-1
stars-2
book-launch
whatif-trade
assembly-1
atom
moon
saturn
clouds-1
clouds-2
clouds-3
clouds-4
clouds-5
quiet-turtle
sun
particles
turtles
chess-b
whatif-king
assembly-planet-3
assembly-2
assembly-3
string
mu-b
holism
reductionism
sky
sky-4
du
evolution
mario-entry
mario-sitting-1
mario-sitting-2
mario-sitting-3
server-1
time-turner
sky-3
fire-hydrant
i-am-a-turtle
mu
chess-w
assembly-4
assembly-planet-1
assembly-5
rope
reductionism-b
holism-b
stockholm
sky-2
blank-figure
e1
mario-n1
mario-sitting-b
assembly-planet
assembly-6
assembly-7
cantor
eb
e2
e3
e4
mario-n2
upgoer
e5
mario-n3
upgoer-planet
upgoer-2
upgoer-3
e6
upgoer-planet-2
upgoer-4
upgoer-5
upgoer-planet-3
upgoer-planet-4
upgoer-6
upgoer-space
walking
walking-b         
*/
    }
}
