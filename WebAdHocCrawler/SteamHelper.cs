using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdHocCrawler
{
    static class SteamHelper
    {
        public static async Task<List<SteamGame>> GetHaloweenGames()
        {
            var page = await WebHelper.DownloadPageAsync(@"http://store.steampowered.com/sale/halloween2013");

            return ExtractGamesFromPage(page);
        }

        private static List<SteamGame> ExtractGamesFromPage(HtmlDocument page)
        {
            var result = new List<SteamGame>();
            
            foreach (var divElement in page.DocumentNode.Descendants("div").Where(d => d.GetAttributeValue("class", "") == "item"))
            {
                try
                {
                    var game = GameFromDiv(divElement);
                    result.Add(game);
                }
                catch { }
            }

            return result;
        }

        public static SteamGame GameFromDiv(HtmlNode divElement)
        {
            var gamePageLink = divElement.Element("a");
            var title = gamePageLink.GetAttributeValue("title", "Error");
            var url = gamePageLink.GetAttributeValue("href", "Error");

            var priceDiv = divElement.Descendants("div").Where(d => d.GetAttributeValue("class", "") == "price").First();
            var normalPriceSpan = priceDiv.Descendants("span").Where(s => s.GetAttributeValue("class", "") == "was").First();
            var salePriceSpan = priceDiv.Descendants("span").Where(s => s.GetAttributeValue("class", "") == "").First();

            var normalPrice = ParsePrice(normalPriceSpan);
            var salePrice = ParsePrice(salePriceSpan);

            return new SteamGame(title, url, normalPrice, salePrice);
        }

        private static decimal ParsePrice(HtmlNode spanElement)
        {
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var cleanString = spanElement.InnerText.Trim().Replace("&#8364;", "").Replace(".", decimalSeparator).Replace(",", decimalSeparator);
            return Decimal.Parse(cleanString);
        }


        /*
         <div class="item">
            <a href="http://store.steampowered.com/app/242530/?snr=1_614_615_halloween2013_gridsale" title="The Chaos Engine">
            </a>
            <div class="info">
                <div class="OS">
                    <span class="platform_img win">
                    </span>
                    <span class="platform_img mac">
                    </span>
                    <span class="platform_img linux">
                    </span>
                </div>
                <div class="price">
                        <span class="was">
                            7,99&#8364;
                        </span>
                    <br/>
                    <span>
                        3,99&#8364;
                    </span>
                </div>
                <div class="percent">
                    -50%
                </div>
            </div>
        </div>

         */
    }
}
