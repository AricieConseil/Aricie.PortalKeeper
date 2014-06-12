using System.Collections.Generic;
using Aricie.Services;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class MarketInfo:ICloneable
	{

	    public string Id { get; set; }

        public string Label { get; set; }

        public string PrimaryName { get; set; }
        public string PrimaryCode { get; set; }
        public string SecondaryName { get; set; }
        public string SecondaryCode { get; set; }

        public DateTime Time { get; set; }

        public Ticker Ticker { get; set; }

	    public List<Trade> RecentTrades { get; set; }

	    public MarketDepth MarketDepth { get; set; }



	    public MarketInfo() : this(DateTime.Now, null, null, null, false)
	    {
	    }


        public MarketInfo(DateTime objTime)
            : this(objTime, null, null, null, false)
	    {
	    }


        public MarketInfo(Ticker ticker, MarketDepth depth)
            : this(DateAndTime.Now, ticker, depth, null, false)
		{
		}


	    public MarketInfo(DateTime objTime, Ticker ticker, MarketDepth depth, List<Trade> trades)
	        : this(objTime, ticker, depth, trades, false)
	    {
	        
	    }

        public MarketInfo(DateTime objTime, Ticker ticker, MarketDepth depth, List<Trade> trades, bool deepCopy)
		{
            Id = "";
            Label = "";
            PrimaryName = "";
            PrimaryCode = "";
            SecondaryCode = "";
            SecondaryName = "";
		    
		    Time = objTime;

            if (ticker != null)
            {
                if (deepCopy)
                {
                    Ticker = new Ticker(ticker);
                }
                else
                {
                    Ticker = ticker;
                }
            }
            else
            {
                Ticker = new Ticker();
            }


            MarketDepth = new MarketDepth(depth, deepCopy);

            if (trades != null)
            {
                if (deepCopy)
                {
                    RecentTrades = new List<Trade>(trades);
                }
                else
                {
                    RecentTrades = trades;
                }
            }
            else
            {
                RecentTrades = new List<Trade>();
            }    
		}

        public MarketInfo(MarketInfo sourceMarket): this(sourceMarket, false)
        {

        }

        public MarketInfo(MarketInfo sourceMarket, bool deepCopy)
            : this(sourceMarket, null, deepCopy)
	    {
	        
	    }

        public MarketInfo(MarketInfo sourceMarket, MarketInfo additionalData)
            : this(sourceMarket, additionalData, false)
        {
           
        }


        public MarketInfo(MarketInfo sourceMarket, MarketInfo additionalData, bool deepCopy)
            : this(sourceMarket.Time, sourceMarket.Ticker, sourceMarket.MarketDepth, sourceMarket.RecentTrades, deepCopy)
        {
            Id = sourceMarket.Id;
            Label = sourceMarket.Label;
            PrimaryName = sourceMarket.PrimaryName;
            PrimaryCode = sourceMarket.PrimaryCode;
            SecondaryCode = sourceMarket.SecondaryCode;
            SecondaryName = sourceMarket.SecondaryName;

            if (additionalData!= null)
            {
                RecentTrades.AddRange(additionalData.RecentTrades);
                MarketDepth.AskOrders.AddRange(additionalData.MarketDepth.AskOrders);
                MarketDepth.BidOrders.AddRange(additionalData.MarketDepth.BidOrders);    
            }
        }


	    public object Clone()
	    {
	        return new MarketInfo(this, true);
	    }
	}
}