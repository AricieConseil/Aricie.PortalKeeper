using System;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	public class AsksAndBids : ResponseObject
	{
		public Order[] Asks;

		public Order[] Bids;

		public AsksAndBids()
		{
		}

		public AsksAndBids(ResponseObject objresponseObject) : base(objresponseObject)
		{
		}

		public MarketDepth ToMarketDepth()
		{
			var toReturn = new MarketDepth(this);
		    toReturn.AskOrders.AddRange(this.Asks);
		    toReturn.BidOrders.AddRange(this.Bids);
			return toReturn;
		}

		public Wallet ToWallet()
		{
			var toReturn = new Wallet(this);
			toReturn.Orders.AddRange(this.Asks);
            toReturn.Orders.AddRange(this.Bids);
			return toReturn;
		}
	}
}