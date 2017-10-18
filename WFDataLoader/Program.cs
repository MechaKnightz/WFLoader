﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using WFDataLoader.AllItem;
using WFDataLoader.ItemOffer;

namespace WFDataLoader
{
    internal static class Program
    {
        private static string _mongoUsername = "";
        private static string _mongoPass = "";

        private static void Main(string[] args)
        {
            Console.Write("Enter MongoDB username: ");
            _mongoUsername = Console.ReadLine();
            Console.Write("Enter MongoDB password: ");
            _mongoPass = Console.ReadLine();

            var unused = new Timer(GetData, new object(), 0, new TimeSpan(0, 1, 0, 0).Milliseconds);
            while (true)
            {
                var keyEvent = Console.ReadKey();
                if (keyEvent.Key == ConsoleKey.Escape) break;
            }
        }

        static void GetData(object obj)
        {
            var allItems = AllItemDownloader.DownloadItems();
            var allItemOffers = new Dictionary<string, Tuple<DateTime, ItemOffers>>();

            int i = 0;
            var time = DateTime.Now;
            foreach (var item in allItems)
            {
                allItemOffers.Add(item.item_name, DownloadOffers(item));
                if (i > 10) break;
                i++;
            }

            var latestTime = DateTime.Now - time;
            Console.WriteLine($"Download took {latestTime.Minutes}:{latestTime.Seconds}");

            SaveModel.UploadSnapshot(allItemOffers, _mongoUsername, _mongoPass);
        }

        private static Tuple<DateTime, ItemOffers> DownloadOffers(Item item)
        {
            var url = "http://warframe.market/api/get_orders/firstPart/secondPart";

            var itemType = item.item_type;
            var itemName = item.item_name;

            if (itemName.Contains(" ")) itemName = itemName.Replace(" ", "%20");

            url = url.Replace("firstPart", itemType);
            url = url.Replace("secondPart", itemName);

            string contents;
            restart:
            try
            {
                using (var wc = new WebClient())
                    contents = wc.DownloadString(url);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
                goto restart;
            }

            return new Tuple<DateTime, ItemOffers>(DateTime.UtcNow, ItemOffers.FromJson(contents));
        }
    }
}
