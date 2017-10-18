using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WFDataLoader.AllItem;
using WFDataLoader.ItemOffer;

namespace WFDataLoader
{
    internal static class Program
    {
        private static Timer _infoTimer;

        static void Main(string[] args)
        {
            _infoTimer = new Timer(GetData, new object(), 0, new TimeSpan(0, 1, 0, 0).Milliseconds);
            while (true)
            {
                var keyEvent = Console.ReadKey();
                if (keyEvent.Key == ConsoleKey.Escape) break;
            }
        }

        static void GetData(object obj)
        {
            var allItems = AllItemDownloader.DownloadItems();

            int record = 0;

            int i = 0;

            var time = DateTime.Now;

            List<ItemOffers> allOffers = new List<ItemOffers>();

            foreach (var item in allItems)
            {
                var url = "http://warframe.market/api/get_orders/firstPart/secondPart";

                var itemType = item.item_type;
                var itemName = item.item_name;

                if (itemName.Contains(" ")) itemName = itemName.Replace(" ", "%20");

                url = url.Replace("firstPart", itemType);
                url = url.Replace("secondPart", itemName);

                string contents;
                using (var wc = new System.Net.WebClient())
                    contents = wc.DownloadString(url);

                var itemOffers = ItemOffers.FromJson(contents);
                allOffers.Add(itemOffers);
                if (itemOffers.Response.Sell.Count > record)
                {
                    record = itemOffers.Response.Sell.Count;
                    Console.WriteLine($"Record is now: {record} with item {item.item_name}");
                }
                i++;
            }

            var latestTime = DateTime.Now - time;

            Console.WriteLine($"Update took {latestTime.Minutes}:{latestTime.Seconds}");
        }
    }
}
