using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WFDataLoader.AllItem
{
    public static class AllItemDownloader
    {
        private static List<Item> AllItems;

        public static List<Item> DownloadItems()
        {
            var url = "http://warframe.market/api/get_all_items_v2";
            string contents;
            using (var wc = new System.Net.WebClient())
                contents = wc.DownloadString(url);

            //var serializer = new JsonSerializer
            //{
            //    NullValueHandling = NullValueHandling.Ignore
            //};

            AllItems = JsonConvert.DeserializeObject<List<Item>>(contents);

            return AllItems;
        }
    }
}
