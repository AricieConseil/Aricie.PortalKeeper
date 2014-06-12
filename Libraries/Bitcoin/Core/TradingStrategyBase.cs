using Aricie.DNN.ComponentModel;
using Aricie.DNN.UI.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public abstract class TradingStrategyBase : NamedEntity, ITradingStrategy, IContextualStrategy
	{
	    [ExtendedCategory("Reserve")]
	    public decimal AskReserveAmount { get; set; }

	    [ExtendedCategory("Reserve")]
	    public decimal AskReserveRate { get; set; }

	    [ExtendedCategory("Reserve")]
	    public decimal BidReserveRate { get; set; }

	    [ExtendedCategory("Reserve")]
	    public decimal BidReserveValue { get; set; }

	    [ConditionalVisible("NoAsks", false, true), ExtendedCategory("Reserve")]
	    public bool ClearAsks { get; set; }

	    [ConditionalVisible("NoBids", false, true), ExtendedCategory("Reserve")]
	    public bool ClearBids { get; set; }

	    [Browsable(false)]
	    public virtual decimal LimitOrderMarginRate { get; set; }

	    [ExtendedCategory("Reserve")]
	    public bool NoAsks { get; set; }

	    [ExtendedCategory("Reserve")]
	    public bool NoBids { get; set; }

	    [Browsable(false)]
	    public virtual bool TakeMarginOnOppositeOrder { get; set; }

	    protected TradingStrategyBase()
		{
			this.AskReserveRate = 30m;
			this.AskReserveAmount = 0m;
			this.BidReserveRate = 20m;
			this.BidReserveValue = 0m;
			this.LimitOrderMarginRate = 1m;
		}

		public Wallet ComputeNewOrders(Wallet currentOrders, MarketInfo objMarket, ExchangeInfo objExchange, TradingHistory history)
		{
			var newOrders = new Wallet();
            var askReserve = Math.Max(this.AskReserveAmount, currentOrders.PrimaryBalance * AskReserveRate / 100m);
            var bidReserve = Math.Max(this.BidReserveValue, currentOrders.SecondaryBalance * BidReserveRate / 100m);

			decimal avBtcsForTrading = currentOrders.PrimaryBalance- askReserve;
			decimal avUsdsForTrading = currentOrders.SecondaryBalance- bidReserve;

            //todo: should we update the original wallet?
			newOrders.ConsolidateOrders(ref currentOrders, true);

			newOrders.PrimaryBalance = Math.Max(avBtcsForTrading- currentOrders.GetTotalAsksPrimary(), decimal.Zero);
			newOrders.SecondaryBalance = Math.Max(decimal.Subtract(avUsdsForTrading, currentOrders.GetTotalBidsSecondary()), decimal.Zero);

			var tContext = new TradingContext(currentOrders, newOrders, objMarket, objExchange, history.GetLastTrend(), this);
			this.ComputeNewOrders(ref tContext);
			if (this.NoAsks)
			{
				tContext.NewOrders.ClearAsks();
				if (this.ClearAsks)
				{
					tContext.NewOrders.CancelExistingOrders(tContext.CurrentOrders.OrderedAsks.ToArray());
				}
			}
			if (this.NoBids)
			{
				tContext.NewOrders.ClearBids();
				if (this.ClearBids)
				{
					tContext.NewOrders.CancelExistingOrders(tContext.CurrentOrders.OrderedBids.ToArray());
				}
			}
			tContext.NewOrders.FitOrders(objExchange);
			history.Update(currentOrders, objMarket, tContext.NewOrders);
			return tContext.NewOrders;
		}

		public abstract void ComputeNewOrders(ref TradingContext tContext);
	}
}