using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebAdHocCrawler
{
    public class RadarImageInfo
    {
        public Uri Url { get; private set; }
        public DateTime Date { get; private set; }
        public string FileName { get; private set; }

        public RadarImageInfo(string cheminFichier, string dateValidite)
        {
            const string ImgUrlPrefix = "http://www.meteofrance.com/integration/sim-portail/";

            Url = new Uri(ImgUrlPrefix + cheminFichier);
            Date = DateTime.ParseExact(dateValidite, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            FileName = dateValidite + ".gif";
        }
    }

    static class MeteoFranceHelper
    {
        private const string RadarWebServiceUrl = "http://www.meteofrance.com/mf3-rpc-portlet/rest/fichier/imgradarfrgf/ZONE_france?maxResults=13";

        public static async Task<IEnumerable<RadarImageInfo>> GetLasRadarImagesInfos()
        {
            var jsonResult = await WebHelper.DownloadJsonArrayAsync(RadarWebServiceUrl);

            return jsonResult.Select(j => new RadarImageInfo(j["cheminFichier"].ToString(), j["dateValidite"].ToString()));
        }

        public static async Task DownloadLastRadarImages()
        {
            var destDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Radar Meteo"));
            destDir.Create();

            var imageInfos = await GetLasRadarImagesInfos();

            foreach (var imageInfo in imageInfos)
            {
                var radarImageFile = new FileInfo(Path.Combine(destDir.FullName, imageInfo.FileName));

                if (!radarImageFile.Exists)
                {
                    await WebHelper.DownloadFileAsync(imageInfo.Url, radarImageFile);
                }
            }
        }
    }
}
