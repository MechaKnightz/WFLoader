using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using WFDataLoader.AllItem;
using WFDataLoader.ItemOffer;

namespace WFDataLoader
{
    internal static class Program
    {
        private static Timer _infoTimer;
        private static string mongoUsername = "";
        private static string mongoPass = "";

        static void Main(string[] args)
        {
            Console.Write("Enter MongoDB username: ");
            mongoUsername = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Enter MongoDB password: ");
            mongoPass = Console.ReadLine();
            Console.WriteLine();

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
            var allItemOffers = new Dictionary<string, ItemOffers>();

            var time = DateTime.Now;
            int i = 0;
            foreach (var item in allItems)
            {
                allItemOffers.Add(item.item_name, DownloadOffers(item));
                i++;
                if (i > 15) break;
            }

            var latestTime = DateTime.Now - time;
            Console.WriteLine($"Download took {latestTime.Minutes}:{latestTime.Seconds}");

            SaveModel.UploadSnapshot(allItemOffers, mongoUsername, mongoPass);
        }

        private static ItemOffers DownloadOffers(Item item)
        {
            var url = "http://warframe.market/api/get_orders/firstPart/secondPart";

            var itemType = item.item_type;
            var itemName = item.item_name;

            if (itemName.Contains(" ")) itemName = itemName.Replace(" ", "%20");

            url = url.Replace("firstPart", itemType);
            url = url.Replace("secondPart", itemName);

            string contents;
            using (var wc = new WebClient())
                contents = wc.DownloadString(url);

            return ItemOffers.FromJson(contents);
        }
    }
}
