using System.Globalization;
using System.Linq;
using Aricie.Collections;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Services.Filtering;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls.EditControls;
using DotNetNuke.UI.WebControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class ExchangeInfo : NamedConfig
	{
	    [ExtendedCategory("Dynamics")]
	    public int AmountDecil { get; set; }

	    [ExtendedCategory("Dynamics")]
	    public decimal AskCommission { get; set; }

	    [ExtendedCategory("Dynamics")]
	    public decimal BidCommission { get; set; }


	    [ExtendedCategory("Filter")]
	    public ExpressionFilterInfo Filter { get; set; }

	    [ExtendedCategory("Dynamics")]
	    public decimal MinOrderAmount { get; set; }

	    [ExtendedCategory("Dynamics")]
	    public decimal MinOrderValue { get; set; }

	    [ExtendedCategory("Dynamics")]
	    public int PriceDecil { get; set; }

	    [CollectionEditor( false, true, true, true, 10, CollectionDisplayStyle.List, true), ExtendedCategory("Urls"), ValueEditor(typeof(CustomTextEditControl), typeof(ExchangeInfo.TradingUrlsValueAttributes))]
	    public SerializableDictionary<TradingAPIUrls, string> TradingUrls { get; set; }

	    public ExchangeInfo()
		{
			this.MinOrderAmount = new decimal(1, 0, 0, false, 1);
			this.MinOrderValue = new decimal();
			this.AskCommission = new decimal(65, 0, 0, false, 2);
			this.BidCommission = new decimal(65, 0, 0, false, 2);
			this.AmountDecil = 1;
			this.PriceDecil = 5;
			this.TradingUrls = new SerializableDictionary<TradingAPIUrls, string>();
			this.Filter = new ExpressionFilterInfo();
		}

		public void ExecuteOrders(MarketInfo objMarket, ref Wallet targetWallet, ref TradingHistory history)
		{
			var trades = new List<Trade>();
			var fees = new List<Payment>();
			var matchedOrders = this.MatchOrders(ref targetWallet, objMarket.Ticker.Last);
		    foreach (var matchedOrder in matchedOrders)
		    {
                Trade trade = new Trade()
                {
                    Time = objMarket.Time,
                    Price = matchedOrder.Price,
                    Amount = matchedOrder.Amount
                };
                Payment fee = new Payment()
                {
                    Time = objMarket.Time,
                    Label = matchedOrder.FriendlyId
                };
                if (matchedOrder.OrderType == OrderType.Buy)
                {
                    trade.TradeType = TradeType.Buy;
                    fee.Currency = objMarket.PrimaryCode;
                    fee.Amount = matchedOrder.Amount * BidCommission / 100m;
                    fee.Label = string.Format("{0} - Bid Fee: {1} % = {2} {3}"
                        , fee.Label
                        , BidCommission.ToString(CultureInfo.InvariantCulture)
                        , fee.Amount
                        , fee.Currency);
                    targetWallet.SecondaryBalance = targetWallet.SecondaryBalance - matchedOrder.Value;
                    targetWallet.PrimaryBalance = targetWallet.PrimaryBalance + matchedOrder.Amount - fee.Amount;
                }
                else if (matchedOrder.OrderType == OrderType.Sell)
                {
                    trade.TradeType = TradeType.Sell;
                    fee.Currency = objMarket.SecondaryCode;
                    fee.Amount = matchedOrder.Value * AskCommission / 100m;
                    fee.Label = string.Format("{0} - Ask Fee: {1} % = {2} {3}"
                       , fee.Label
                       , AskCommission.ToString(CultureInfo.InvariantCulture)
                       , fee.Amount
                       , fee.Currency);
                    targetWallet.SecondaryBalance = targetWallet.SecondaryBalance +matchedOrder.Value- fee.Amount;
                    targetWallet.PrimaryBalance = targetWallet.PrimaryBalance- matchedOrder.Amount;
                }
                trades.Add(trade);
                fees.Add(fee);
                targetWallet.Orders.Remove(matchedOrder);
		    }
			
			targetWallet.Time = objMarket.Time;
			history.Update(targetWallet, objMarket, trades, fees);
		}

        public ExchangeInfo FromCustomFees(decimal newCommission)
        {
            return FromCustomFees(newCommission, newCommission);
        }


        public ExchangeInfo FromCustomFees(decimal askFee, decimal bidFee)
		{
			var toReturn = new ExchangeInfo()
			{
				MinOrderAmount = this.MinOrderAmount,
				AmountDecil = this.AmountDecil,
				PriceDecil = this.PriceDecil,
				TradingUrls = this.TradingUrls,
				Filter = this.Filter,
                AskCommission = askFee,
                BidCommission = bidFee
			};
			return toReturn;
		}

		public List<Order> MatchOrders(ref Wallet objWallet, decimal price)
		{
            var toReturn = objWallet.OrderedBids.Where(objOrder => objOrder.Price > price).ToList();
		    toReturn.AddRange(objWallet.OrderedAsks.Where(objOrder => objOrder.Price < price).ToList());
			return toReturn;
		}

		public class TradingUrlsValueAttributes : IAttributesProvider
		{
			[DebuggerNonUserCode]
			public TradingUrlsValueAttributes()
			{
			}

			public IEnumerable<Attribute> GetAttributes()
			{
				var toReturn = new List<Attribute>()
				{
					new LineCountAttribute(2),
					new WidthAttribute(400),
					new RequiredAttribute(true)
				};
				return toReturn;
			}
		}
	}
}