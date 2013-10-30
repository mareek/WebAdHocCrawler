using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdHocCrawler
{
    class SteamGame
    {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public decimal NormalPrice { get; private set; }
        public decimal SalePrice { get; private set; }
        public decimal Rebate { get { return (NormalPrice - SalePrice) / NormalPrice * 100m; } }

        public SteamGame(string title, string url, decimal normalPrice, decimal salePrice)
        {
            this.Title = title;
            this.Url = url;
            this.NormalPrice = normalPrice;
            this.SalePrice = salePrice;
        }
    }
}
