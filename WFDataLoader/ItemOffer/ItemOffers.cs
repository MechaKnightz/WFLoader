// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using WFDataLoader;
//
//    var data = ItemOffers.FromJson(jsonString);
//

using System.Collections.Generic;
using Newtonsoft.Json;

namespace WFDataLoader.ItemOffer
{
    public partial class ItemOffers
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("render_rank")]
        public bool RenderRank { get; set; }

        [JsonProperty("buy")]
        public List<Buy> Buy { get; set; }

        [JsonProperty("sell")]
        public List<Buy> Sell { get; set; }
    }

    public partial class Buy
    {
        [JsonProperty("ingame_name")]
        public string IngameName { get; set; }

        [JsonProperty("online_status")]
        public bool OnlineStatus { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("online_ingame")]
        public bool OnlineIngame { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }
    }

    public partial class ItemOffers
    {
        public static ItemOffers FromJson(string json) => JsonConvert.DeserializeObject<ItemOffers>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ItemOffers self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
