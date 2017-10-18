// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using WFLoader;
//
//    var data = GettingStarted.FromJson(jsonString);
//

namespace WFDataLoader.AllItem
{
    public class Item
    {
        public string item_name { get; set; }
        public string item_type { get; set; }
        public string item_wiki { get; set; }
        public int? mod_max_rank { get; set; }
        public string rarity { get; set; }
        public string category { get; set; }
        public string wiki_link { get; set; }
    }
}
