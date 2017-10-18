using System;
using System.Collections.Generic;
using System.Linq;

namespace WFDataLoader.ItemOffer
{
    class ItemOffersSaveModel
    {
        public int AverageSellPrice;
        public int AverageBuyPrice;
        public DateTime Date;

        public void CalculateAvgPrice(ItemOffers offers)
        {
            var onlineSellers = offers.Response.Sell.Where(x => x.OnlineIngame);
            var onlineBuyers = offers.Response.Buy.Where(x => x.OnlineIngame);

            
        }
    }
}
