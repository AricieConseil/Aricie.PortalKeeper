using System.Linq;
using Aricie.DNN.ComponentModel;
using Aricie.Services;
using System;
using System.Collections.Generic;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class TradingStrategies 
        : ProviderHost<DotNetType<IContextualStrategy>, IContextualStrategy, IGenericizer<IContextualStrategy>>
        , IContextualStrategy
        , ITradingStrategy
	{
		private readonly static Type GenerictypeLessTypeCondStrat;

		protected List<DotNetType> ExpressionTypes;

		static TradingStrategies()
		{
			TradingStrategies.GenerictypeLessTypeCondStrat = typeof(ComplexStrategy<>);
		}

		public TradingStrategies()
		{
			this.ExpressionTypes = new List<DotNetType>();
		}


        public override IDictionary<string, DotNetType<IContextualStrategy>> GetAvailableProviders()
        {
            if (this.ExpressionTypes.Count == 0)
            {
                this.ExpressionTypes.AddRange(GetInitialTypes());
            }
            return (from simpleDotNetType 
                        in ExpressionTypes 
                        let simpleType = simpleDotNetType.GetDotNetType() 
                        where simpleType != null 
                    select new DotNetType<IContextualStrategy>(GenerictypeLessTypeCondStrat, simpleDotNetType))
                    .ToDictionary(toAdd => toAdd.Name);
        }



        protected virtual IList<DotNetType> GetInitialTypes()
        {
            var toReturn = new List<DotNetType>()
			{
				new DotNetType(typeof(TradingStrategies)),
				new DotNetType(typeof(BandTradingStrategy)),
				new DotNetType(typeof(IssueOrderStrategy))
			};
            return toReturn;
        }



		public void ComputeNewOrders(ref TradingContext tContext)
		{
            foreach (IContextualStrategy objAction in this.Instances)
            {
                objAction.ComputeNewOrders(ref tContext);
            }
		}

		public Wallet ComputeNewOrders(Wallet currentOrders, MarketInfo objMarket, ExchangeInfo objExchange, TradingHistory history)
		{

            //the newOrders Wallet variable will contain all ask/bid/cancel orders to issue

            var newOrders = new Wallet();

            //start with reserved resource
            //Dim askReserve As Decimal = Math.Max(Me._AskReserveAmount, currentOrders.btcs * (Me._AskReserveRate / 100))
            //Dim bidReserve As Decimal = Math.Max(Me._BidReserveValue, currentOrders.usds * (Me._BidReserveRate / 100))

            decimal avBtcsForTrading = currentOrders.PrimaryBalance;
            //- askReserve
            decimal avUsdsForTrading = currentOrders.SecondaryBalance;
            //- bidReserve

            //Then We simplify the current orders by merging orders of the same price, issueing corresponding cancel/new orders 
            newOrders.ConsolidateOrders(ref currentOrders, true);

            //Feed the new orders wallet with available resources, Reserve resources for current open orders
            newOrders.PrimaryBalance = Math.Max(avBtcsForTrading - currentOrders.GetTotalAsksPrimary(), 0);
            newOrders.SecondaryBalance = Math.Max(avUsdsForTrading - currentOrders.GetTotalBidsSecondary(), 0);

            var tContext = new TradingContext(currentOrders, newOrders, objMarket, objExchange, history.GetLastTrend(), this);

            this.ComputeNewOrders(ref tContext);


            tContext.NewOrders.FitOrders(objExchange);
            //we update the trading history with last data
            history.Update(currentOrders, objMarket, tContext.NewOrders);
            return tContext.NewOrders;
        }

		

	
	}
}