using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WFDataLoader.ItemOffer
{
    public static class SaveModel
    {
        public static void UploadSnapshot(Dictionary<string, ItemOffers> offers, string mongoUsername, string mongoPass)
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
                        { "AverageSellPrice", new BsonArray() },
                        { "AverageBuyPrice", new BsonArray() },
                    };
                    collection.InsertOne(document);
                }

                var sellValue = new BsonDocument
                {
                    {"Value", item.AverageSellPrice.AveragePrice},
                    {"Date", item.AverageSellPrice.Timestamp}
                };

                var buyValue = new BsonDocument
                {
                    {"Value", item.AverageBuyPrice.AveragePrice},
                    {"Date", item.AverageBuyPrice.Timestamp}
                };

                var update = Builders<BsonDocument>.Update.Push("AverageSellPrice", sellValue).Push("AverageBuyPrice", buyValue);

                collection.FindOneAndUpdateAsync(filter, update);
            }
            Console.WriteLine("Updated database");
        }
    }
    internal class ItemOffersModel
    {
        public string Name;
        public PriceDate AverageSellPrice;
        public PriceDate AverageBuyPrice;

        internal ItemOffersModel(ItemOffers offers, string name)
        {
            Name = name;

            var onlineSellOrders = offers.Response.Sell.Where(x => x.OnlineIngame).ToList();
            var onlineBuyOrders = offers.Response.Buy.Where(x => x.OnlineIngame).ToList();

            if (onlineSellOrders.Count > 5) onlineSellOrders.RemoveRange(5, onlineSellOrders.Count - 5);
            if (onlineBuyOrders.Count > 5) onlineBuyOrders.RemoveRange(5, onlineBuyOrders.Count - 5);

            if (onlineSellOrders.Count != 0)
                AverageSellPrice = new PriceDate(onlineSellOrders.Average(x => x.Price));
            else
                AverageSellPrice = new PriceDate(0);

            if (onlineBuyOrders.Count != 0)
                AverageBuyPrice = new PriceDate(onlineBuyOrders.Average(x => x.Price));
            else
                AverageBuyPrice = new PriceDate(0);
        }
    }

    internal class PriceDate
    {
        public double AveragePrice;
        public DateTime Timestamp;

        internal PriceDate(double value)
        {
            AveragePrice = value;
            Timestamp = DateTime.UtcNow;
        }
    }
}
