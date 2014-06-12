
using System;
using Aricie.DNN.Modules.PortalKeeper.BitCoin;
using Aricie.DNN.UI.Attributes;

namespace Aricie.PortalKeeper.Bitcoin.Strategies
{
    
    
    /// <summary>
    /// This simple strategy aims at balancing the value of btcs and $ contained in the wallet.
    /// It will issue 1 ask order and 1 bid order on each side of the current market price, such that when those orders get executed, the wallet keeps balanced
    /// </summary>
    public class ResourceBalancingStrategy : TradingStrategyBase
    {

        /// <summary>
        /// Sets the margin percentage from the current price for the orders to issue 
        /// </summary>
        [ExtendedCategory("Parameters")]
        public Decimal PriceMargin { get; set; }

        /// <summary>
        /// Multiplicative coefficient applied to the unbalance value to define the lower order (0.5 means that the order levels the value of $ and btcs in wallet)
        /// </summary>
        [ExtendedCategory("Parameters")]
        public Decimal LowOrderBalanceCoef { get; set; }

        /// <summary>
        /// Multiplicative coefficient applied to the unbalance value to define the higher order (0.5 means that the order levels the value of $ and btcs in wallet)
        /// </summary>
        [ExtendedCategory("Parameters")]
        public Decimal HighOrderBalanceCoef { get; set; }

        public ResourceBalancingStrategy()
        {
            this.PriceMargin = 5;
            this.LowOrderBalanceCoef = 0.5m;
            this.HighOrderBalanceCoef = 0.5m;
        }

        /// <summary>
        /// This method is responsible for computing the new orders according to the strategy parameters and all the data previously retrieved online
        /// </summary>
        /// <param name="tContext">The TradingContext contains the existing wallet, the new orders wallet to be filled up, and the current market data</param>
        public override void ComputeNewOrders(ref TradingContext tContext)
        {

            // Note that tContext.NewOrders already accounts for reserve parameters and existing orders
            // See TradingStrategyBase for more details on how the TradingContext is defined

            if (tContext.CurrentOrders.Orders.Count == 0 )
            {
                // Define top order
                var targetPrice = tContext.Market.Ticker.Last * (100 + this.PriceMargin) / 100;
                var targetBalance = tContext.NewOrders.GetBalance(targetPrice);
                Order highOrder = GetBalancingOrder(targetBalance, this.HighOrderBalanceCoef);
                
                // Define bottom order
                targetPrice = tContext.Market.Ticker.Last * 100 / (100 + this.PriceMargin);
                targetBalance = tContext.NewOrders.GetBalance(targetPrice);
                Order lowOrder = GetBalancingOrder(targetBalance, this.LowOrderBalanceCoef);
                //We only issue the orders if low is a bid and high is a ask
                if (lowOrder.OrderType == OrderType.Buy && highOrder.OrderType == OrderType.Sell)
                {
                    tContext.NewOrders.Orders.Add(highOrder);
                    tContext.NewOrders.Orders.Add(lowOrder);
                }
                else
                {
                    //fix the current unbalance
                    targetBalance = tContext.NewOrders.GetBalance(tContext.Market.Ticker.Last);
                    Order balancingOrder = GetBalancingOrder(targetBalance, 0.5m);
                    tContext.NewOrders.Orders.Add(balancingOrder);
                }
            }
            else
            {
                //we clear any order outside of the defined boundaries (= 2 * defined price margin)
                foreach (Order objOrder in tContext.CurrentOrders.Orders)
                {
                    if (Math.Max(objOrder.Price, tContext.Market.Ticker.Last) > (1 + this.PriceMargin / 100) * (1 + this.PriceMargin / 100) * Math.Min( tContext.Market.Ticker.Last, objOrder.Price))
                    {
                        tContext.NewOrders.CancelExistingOrders(objOrder);            
                    }
                }
            }
        }


        /// <summary>
        /// Computes an order to level $ and btcs value in a wallet given a target price
        /// </summary>
        /// <param name="targetBalance">Contains $, btcs and target price</param>
        /// <param name="balancingCoef">defines the multiplicative coefficient applied to the unbalance value (0.5 is the leveling value)</param>
        /// <returns>an order, which when executed will level the resulting $ and btcs available </returns>
        private static Order GetBalancingOrder(Balance targetBalance, decimal balancingCoef)
        {
            var unBalanceValue = targetBalance.Secondary - targetBalance.Value;
            var orderType = OrderType.Sell;
            if (unBalanceValue > 0)
            {
                orderType = OrderType.Buy;
            }
            var newOrder = new Order()
            {
                OrderType = orderType,
                Price = targetBalance.TickerLast,
                Amount = (Math.Abs(unBalanceValue) * balancingCoef) / targetBalance.TickerLast
            };
            return newOrder;
        }


    }

}
