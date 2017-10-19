using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WFDataLoader.ItemOffer
{
    public static class SaveModel
    {
        public static void UploadSnapshot(Dictionary<string, Tuple<DateTime, ItemOffers>> offers, string mongoUsername, string mongoPass)
        {
            restart:
            //try
            {
                var newDict = new Dictionary<string, ItemOffersModel>();
                foreach (var offer in offers)
                {
                    newDict.Add(offer.Key, new ItemOffersModel(offer.Value, offer.Key));
                }

                var mongoClient = new MongoClient($"mongodb://{mongoUsername}:{mongoPass}@cluster0-shard-00-00-kp9r9.mongodb.net:27017,cluster0-shard-00-01-kp9r9.mongodb.net:27017,cluster0-shard-00-02-kp9r9.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin");

                var database = mongoClient.GetDatabase("WFMarket");

                var collection = database.GetCollection<BsonDocument>("SaveModel");

                foreach (var dictItem in newDict)
                {
                    var item = dictItem.Value;

                    var filter = Builders<BsonDocument>.Filter.Eq("Name", item.Name);

                    var userDocument = collection.Find(filter).FirstOrDefault();

                    if (userDocument == null)
                    {
                        var document = new BsonDocument
                    {
                        { "Name", item.Name},
                        { "AveragePrice", new BsonArray() }
                    };
                        collection.InsertOne(document);
                    }

                    var value = new BsonDocument
                {
                    {"AverageSellPrice", item.LatestAveragePrice.AverageSellPrice},
                    {"AverageBuyPrice", item.LatestAveragePrice.AverageBuyPrice},
                    {"Date", item.LatestAveragePrice.Timestamp}
                };

                    var update = Builders<BsonDocument>.Update.Push("AveragePrice", value);

                    collection.FindOneAndUpdateAsync(filter, update);
                }
                Console.WriteLine("Updated database");
            }
            //catch (Exception e)
            //{
            //    throw new Exception();
            //    Console.WriteLine($"Error: {e}");
            //    Thread.Sleep(10000);
            //    goto restart;
            //}
        }
    }
    internal class ItemOffersModel
    {
        public string Name;
        public PriceDate LatestAveragePrice;

        internal ItemOffersModel(Tuple<DateTime, ItemOffers> offers, string name)
        {
            Name = name;

            var onlineSellOrders = offers.Item2.Response.Sell.Where(x => x.OnlineIngame).ToList();
            var onlineBuyOrders = offers.Item2.Response.Buy.Where(x => x.OnlineIngame).ToList();

            if (onlineSellOrders.Count > 5) onlineSellOrders.RemoveRange(5, onlineSellOrders.Count - 5);
            if (onlineBuyOrders.Count > 5) onlineBuyOrders.RemoveRange(5, onlineBuyOrders.Count - 5);

            var sellPrice = 0.0;
            var buyPrice = 0.0;
            if (onlineSellOrders.Count != 0)
                sellPrice = onlineSellOrders.Average(x => x.Price);
            if (onlineBuyOrders.Count != 0)
                buyPrice = onlineBuyOrders.Average(x => x.Price);

            LatestAveragePrice = new PriceDate(sellPrice, buyPrice, offers.Item1);
        }
    }

    internal class PriceDate
    {
        public double AverageSellPrice;
        public double AverageBuyPrice;
        public DateTime Timestamp;

        internal PriceDate(double sellPrice, double buyPrice, DateTime time)
        {
            AverageSellPrice = sellPrice;
            AverageBuyPrice = buyPrice;
            Timestamp = time;
        }
    }
}
