using System.Linq;
using Aricie;
using Aricie.DNN.UI.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{

    public class ExchangeSimulator
    {

        public List<Trade> Trades { get; set; }

        private MarketInfo _currentMarket =  new MarketInfo();
        private int _currentTradeIdx = 0;
        private int _currentLast24HTradeIdx = 0;

        /// <summary>
        /// Emulates the Market data at a given time, given the historical trades
        /// In order to optimise performances, it keeps previous emulated market and only performs updates
        /// It implies that the method is called sequentially at increasing timings.
        /// Note that the marketDepth is only broadly approximated from future trades found in history
        /// </summary>
        /// <param name="time">the time at which the market should be emulated</param>
        /// <returns>a MarketInfo object with emulated ticker, RecentTrades, and Marketdepth</returns>
        public MarketInfo GetMarket(DateTime time)
        {
            _currentMarket.Time = time;
            
            // First we move the current trade cursor according to the new time and update the ticker and recent trades accordingly

            Trade currentTrade;
            do
            {
                currentTrade = Trades[_currentTradeIdx];
                _currentMarket.RecentTrades.Add(currentTrade);
                _currentMarket.Ticker.Volume += currentTrade.Amount;
                if (currentTrade.Price > _currentMarket.Ticker.High)
                    _currentMarket.Ticker.High = currentTrade.Price;
                if (_currentMarket.Ticker.Low == 0m || currentTrade.Price < _currentMarket.Ticker.Low)
                    _currentMarket.Ticker.Low = currentTrade.Price;
                _currentTradeIdx += 1;
            } while (currentTrade.Time <= time);
            _currentMarket.Ticker.Last = currentTrade.Price;

            // Then we move the previous 24h trades cursor and update the Ticker accordingly
            var last24hTime = time.AddHours(-24d);
            var last24HTrade=Trades[_currentLast24HTradeIdx];
            while (last24HTrade.Time < last24hTime) 
            {
                _currentMarket.Ticker.Volume -= last24HTrade.Amount;
                if (last24HTrade.Price == _currentMarket.Ticker.High)
                {
                    _currentMarket.Ticker.High = 0m;
                }
                if (last24HTrade.Price == _currentMarket.Ticker.Low)
                {
                    _currentMarket.Ticker.Low = 0m;
                }
               _currentLast24HTradeIdx += 1;
               last24HTrade = Trades[_currentLast24HTradeIdx];
            }
            // if the ticker low or high were reset, recompute them

            if (_currentMarket.Ticker.Low == 0m || _currentMarket.Ticker.High == 0m)
            {
                for (int i = _currentLast24HTradeIdx; i < _currentTradeIdx; i++)
                {
                    var tempTrade = Trades[i];
                    if (_currentMarket.Ticker.Low == 0m || tempTrade.Price < _currentMarket.Ticker.Low)
                        _currentMarket.Ticker.Low = tempTrade.Price;
                    if (tempTrade.Price > _currentMarket.Ticker.High)
                        _currentMarket.Ticker.High = tempTrade.Price;

                }
            }
            
            // Finally, compute MarketDepth

            //Identifying executed orders segments to be removed from existing MarketDepth
            var newMinAskIdx = 0;
            var existingMinAsk = decimal.MaxValue;
            var newMaxBidIdx = 0;
            var existingMaxBid = 0m;
            if (_currentMarket.MarketDepth.AskOrders.Count > 0)
            {
                while (newMinAskIdx < _currentMarket.MarketDepth.AskOrders.Count && _currentMarket.MarketDepth.AskOrders[newMinAskIdx].Time < time)
                {
                    newMinAskIdx++;
                }
                existingMinAsk = _currentMarket.MarketDepth.AskOrders[newMinAskIdx].Price;
            }

            if (_currentMarket.MarketDepth.BidOrders.Count > 0)
            {
                
                while (newMaxBidIdx < _currentMarket.MarketDepth.BidOrders.Count && _currentMarket.MarketDepth.BidOrders[newMaxBidIdx].Time < time)
                {
                    newMaxBidIdx++;
                }
                existingMaxBid = _currentMarket.MarketDepth.BidOrders[newMaxBidIdx].Price;
            }


            //Computing new depth segments
            var exitAskCondition = false;
            var exitBidCondition = false;
            var newDepth = new MarketDepth();
            for (int depthIdx = _currentTradeIdx + 1; depthIdx < Trades.Count && !(exitAskCondition && exitBidCondition); depthIdx++)
            {
                var depthTrade = Trades[depthIdx];
                if (!exitAskCondition && (newDepth.AskOrders.Count > 0 &&
                     depthTrade.Price > newDepth.AskOrders.Last().Price)
                    || (newDepth.AskOrders.Count == 0 && depthTrade.Price > currentTrade.Price))
                {
                    if (depthTrade.Price < existingMinAsk)
                    {
                        newDepth.AskOrders.Add(depthTrade.ToOrder(OrderType.Sell));
                    }
                    else
                    {
                        exitAskCondition = true;
                    }
                    
                }
                else if (!exitBidCondition && (newDepth.BidOrders.Count > 0 &&
                          depthTrade.Price < newDepth.BidOrders.Last().Price)
                         ||
                         (newDepth.BidOrders.Count == 0 && depthTrade.Price < currentTrade.Price))
                {
                    if (depthTrade.Price > existingMaxBid)
                    {
                        newDepth.BidOrders.Add(depthTrade.ToOrder(OrderType.Buy));
                    }
                    else
                    {
                        exitBidCondition = true;
                    }
                }
            }
            //joining former and new marketdepth segments
            newDepth.AskOrders.AddRange(_currentMarket.MarketDepth.AskOrders.Skip(newMinAskIdx));
            newDepth.BidOrders.AddRange(_currentMarket.MarketDepth.BidOrders.Skip(newMaxBidIdx));
            _currentMarket.MarketDepth = newDepth;


            return _currentMarket;
        }




    }
    
    /// <summary>
    /// That class serves defining simulation parameter for tradegies back testing. 
    /// A couple parameters are available to the user and a method runs the simulation given historical data
    /// </summary>
    [Serializable]
    public class SimulationInfo
    {

        /// <summary>
        /// Duration between each bot run. It should match the platform configuration for accurate results
        /// </summary>
        public STimeSpan BotPeriod {get;set;}

        /// <summary>
        /// Defines if a new custom strategy is to be used or the existing strategy instead
        /// </summary>
        public bool UseCustomStrategy { get; set; }

        /// <summary>
        /// The custom strategy to use if activated
        /// </summary>
        [ConditionalVisible("UseCustomStrategy", false, true)]
        public TradingStrategy CustomStrategy { get; set; }

        /// <summary>
        /// The date to start the simulation or the closest historical data if unavailable
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date to end the simulation or the closest historical data if unavailable
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Allows for much faster simulations by skipping suposedly useless bot runs 
        /// when the market does not move. Other parameters define the corresponding behavior.
        /// </summary>
        public bool FastSimulation { get; set; }
        
        /// <summary>
        /// The minimum number of bot runs without any new order getting issued before skipping starts 
        /// </summary>
        [ConditionalVisible("FastSimulation", false, true)]
        public decimal SkippedMinVoidRuns { get; set; }

        /// <summary>
        /// The maximum number of successive bot runs skipped 
        /// </summary>
        [ConditionalVisible("FastSimulation", false, true)]
        public decimal SkippedMaxRuns { get; set; }
       
        /// <summary>
        /// The maximum variation of the market since the last unskipped bot run to keep skipping bot runs
        /// </summary>
        [ConditionalVisible("FastSimulation", false, true)]
        public decimal SkippedVariationRate { get; set; }


        
        

        public SimulationInfo()
        {
            this.StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(60)); ;
            this.EndDate = DateTime.Now;
            this.BotPeriod = new STimeSpan(TimeSpan.FromMinutes(5));
            this.CustomStrategy = new TradingStrategy();
            this.FastSimulation = true;
            this.SkippedVariationRate = 1m;
            this.SkippedMinVoidRuns = 2m;
            this.SkippedMaxRuns = 20m;
        }

        public TradingHistory RunSimulation(Wallet initialWallet, ITradingStrategy obStrategy, ExchangeInfo objExchange, IEnumerable<Trade> exchangeHistory)
        {
            return SimulationInfo.RunSimulation(this, initialWallet, obStrategy, objExchange, exchangeHistory);
        }

        /// <summary>
        /// Static method that runs a simulation of a Market and bot runs for a given period, 
        /// with the parameters and historical data supplied as parameters
        /// </summary>
        /// <param name="objSimulation">An instance of the simulation info class with properties defining how to perform the simulation</param>
        /// <param name="initialWallet">The Wallet to use at the start of the simulation</param>
        /// <param name="obStrategy">The existing strategy to use if the simulation object does not specify a custom strategy</param>
        /// <param name="objExchange">An instance of the Exchange parameters</param>
        /// <param name="exchangeHistory">Historical data for the Exchange object</param>
        /// <returns>the trading history computed from the simulation with resulting balance, issued and executed orders</returns>
        public static TradingHistory RunSimulation(SimulationInfo objSimulation, Wallet initialWallet
            , ITradingStrategy obStrategy, ExchangeInfo objExchange, IEnumerable<Trade> exchangeHistory)
        {
            var toReturn = new TradingHistory();
            if (objSimulation.UseCustomStrategy)
            {
                obStrategy = objSimulation.CustomStrategy;
            }
            var objExchangeSimulator = new ExchangeSimulator()
            {
                Trades = exchangeHistory.ToList()
            };
            var currentWallet = (Wallet)initialWallet.Clone();
            //var lastBotMarket = new MarketInfo(DateTime.MinValue);
            var lastBotTicker = 0m;
            var lastBotTime = DateTime.MinValue;
            MarketInfo objMarket = null;
            int nbEmptyRuns = 0;
            foreach (var historicTrade in objExchangeSimulator.Trades
                    .Where(objTrade => objTrade.Time > objSimulation.StartDate
                    && objTrade.Time < objSimulation.EndDate))
            {
                if ((currentWallet.OrderedAsks.Count > 0 && historicTrade.Price > currentWallet.LowestAsk.Price)
                    || (currentWallet.OrderedBids.Count > 0 && historicTrade.Price < currentWallet.HighestBid.Price))
                {
                    nbEmptyRuns = 0;
                    objMarket = objExchangeSimulator.GetMarket(historicTrade.Time);
                    objExchange.ExecuteOrders(objMarket, ref currentWallet, ref toReturn);
                }
                if (historicTrade.Time.Subtract(lastBotTime) > objSimulation.BotPeriod.Value)
                {

                    currentWallet.Time = historicTrade.Time;
                    bool isBigVariation =
                        Math.Abs((historicTrade.Price - lastBotTicker) / historicTrade.Price)
                        > objSimulation.SkippedVariationRate/100;

                    if (!objSimulation.FastSimulation
                        || nbEmptyRuns < objSimulation.SkippedMinVoidRuns
                        || nbEmptyRuns >= objSimulation.SkippedMaxRuns
                        || isBigVariation)
                    {
                        if (objMarket == null)
                        {
                            objMarket = objExchangeSimulator.GetMarket(historicTrade.Time);
                        }
                        var newOrders = obStrategy.ComputeNewOrders(currentWallet, objMarket, objExchange, toReturn);
                        currentWallet.IntegrateOrders(newOrders.Orders.ToArray());
                        lastBotTicker = objMarket.Ticker.Last;
                        if (newOrders.Orders.Count == 0 && !isBigVariation && nbEmptyRuns < objSimulation.SkippedMaxRuns)
                        {
                            nbEmptyRuns++;
                        }
                        else
                        {
                            nbEmptyRuns = 0;
                        }
                    }
                    else
                    {
                        nbEmptyRuns++;
                    }
                    lastBotTime = historicTrade.Time;
                }
                objMarket = null;
            }
            return toReturn;
        }
    }
}