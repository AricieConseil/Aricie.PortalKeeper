using Aricie.DNN.UI.Attributes;
using DotNetNuke.UI.WebControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[DefaultProperty("FriendlyId")]
	[Serializable]
	public class TradingHistory
	{
	    private TradingSeries _lastEvents;

		private TradingSeries _fiveMinEvents;

		private TradingSeries _hourlyEvents;

		private TradingSeries _dailyEvents;

		private TradingSeries _weeklyEvents;

		private TradingSeries _monthlyEvents;

        [Browsable(false)]
        public string FriendlyId
        {
            get
            {
                return string.Format("History:  {0} {1},".PadRight(25)
                    + "{2} {3}".PadRight(18)
                    + "Hourly: {4}%".PadRight(18)
                    + "Daily: {5}%".PadRight(18)
                    + "Weekly: {6}%".PadRight(18)
                    + "Monthly: {7}%".PadRight(18),
                    LastWallet.PrimaryBalance.ToString(CultureInfo.InvariantCulture)
                    , LastWallet.PrimarySymbol
                    , LastWallet.SecondaryBalance.ToString(CultureInfo.InvariantCulture)
                    , LastWallet.SecondarySymbol
                    , _hourlyEvents.TotalEarningsRateFixedPrice.ToString(CultureInfo.InvariantCulture)
                    , _dailyEvents.TotalEarningsRateFixedPrice.ToString(CultureInfo.InvariantCulture)
                    , _weeklyEvents.TotalEarningsRateFixedPrice.ToString(CultureInfo.InvariantCulture)
                    , _monthlyEvents.TotalEarningsRateFixedPrice.ToString(CultureInfo.InvariantCulture));
            }
        }

        [ExtendedCategory("Wallet")]
        public Wallet LastWallet { get; set; }

        [ExtendedCategory("Wallet"), IsReadOnly(true)]
        public List<Wallet> LastOrdersSeries { get; set; }

        [ExtendedCategory("Transactions")]
        [IsReadOnly(true)]
        public List<Trade> Trades { get; set; }

        [ExtendedCategory("Transactions")]
        [IsReadOnly(true)]
        public List<Payment> Fees { get; set; }

        [ExtendedCategory("Last")]
        [IsReadOnly(true)]
        public TradingSeries LastEvents
        {
            get
            {
                return this._lastEvents;
            }
            set
            {
                this._lastEvents = value;
                this._lastEvents.MaxNbItems = 15;
            }
        }

        [ExtendedCategory("FiveMin")]
        [IsReadOnly(true)]
        public TradingSeries FiveMinEvents
        {
            get
            {
                return this._fiveMinEvents;
            }
            set
            {
                this._fiveMinEvents = value;
                this._fiveMinEvents.MaxNbItems = 13;
            }
        }

        [ExtendedCategory("Hourly")]
        [IsReadOnly(true)]
        public TradingSeries HourlyEvents
        {
            get
            {
                return this._hourlyEvents;
            }
            set
            {
                this._hourlyEvents = value;
                this._hourlyEvents.MaxNbItems = 25;
            }
        }

		[ExtendedCategory("Daily")]
		[IsReadOnly(true)]
		public TradingSeries DailyEvents
		{
			get
			{
				return this._dailyEvents;
			}
			set
			{
				this._dailyEvents = value;
				this._dailyEvents.MaxNbItems = 8;
			}
		}

        [ExtendedCategory("Weekly")]
        [IsReadOnly(true)]
        public TradingSeries WeeklyEvents
        {
            get
            {
                return this._weeklyEvents;
            }
            set
            {
                this._weeklyEvents = value;
                this._weeklyEvents.MaxNbItems = 5;
            }
        }

	    [ExtendedCategory("Monthly")]
		[IsReadOnly(true)]
		public TradingSeries MonthlyEvents
		{
			get
			{
				return this._monthlyEvents;
			}
			set
			{
				this._monthlyEvents = value;
				this._monthlyEvents.MaxNbItems = 13;
			}
		}

		public TradingHistory()
		{
			this.LastWallet = new Wallet();
			this.LastOrdersSeries = new List<Wallet>();
			this._lastEvents = new TradingSeries(15, TimeSpan.FromMilliseconds(100));
			this._fiveMinEvents = new TradingSeries(13, TimeSpan.FromMinutes(5));
			this._hourlyEvents = new TradingSeries(25, TimeSpan.FromHours(1));
			this._dailyEvents = new TradingSeries(8, TimeSpan.FromDays(1));
			this._weeklyEvents = new TradingSeries(5, TimeSpan.FromDays(7));
			this._monthlyEvents = new TradingSeries(13, TimeSpan.FromDays(30));
			this.Trades = new List<Trade>();
			this.Fees = new List<Payment>();
		}

		public void AddEvent(TradingEvent objEvent)
		{
			this._lastEvents.AddEvent(objEvent);
			this._fiveMinEvents.AddEvent(objEvent);
			this._hourlyEvents.AddEvent(objEvent);
			this._dailyEvents.AddEvent(objEvent);
			this._weeklyEvents.AddEvent(objEvent);
			this._monthlyEvents.AddEvent(objEvent);
		}


        public void Update(Wallet currentWallet, MarketInfo currentMarket, Wallet newOrders)
        {
            this.AddEvent(new TradingEvent(currentMarket.Time, currentWallet.GetBalance(currentMarket.Ticker)));
            this.LastWallet = currentWallet;
            if (newOrders.Orders.Count > 0)
            {
                this.LastOrdersSeries.Insert(0, newOrders);
                if (this.LastOrdersSeries.Count > 10)
                {
                    this.LastOrdersSeries.RemoveRange(10, this.LastOrdersSeries.Count - 10);
                }
            }
        }

        public void Update(Wallet currentWallet, MarketInfo currentMarket, IList<Trade> objTrades, IList<Payment> objfees)
        {
            this.AddEvent(new TradingEvent(currentMarket.Time, currentWallet.GetBalance(currentMarket.Ticker)));
            this.LastWallet = currentWallet;
            this.Trades.AddRange(objTrades);
            this.Fees.AddRange(objfees);
        }


		public TradingTrend GetLastTrend()
		{
            var series = new List<TradingSeries>
            {
                this._fiveMinEvents,
                this._hourlyEvents,
                this._dailyEvents,
                this._weeklyEvents,
                this._monthlyEvents
            };
		    foreach (TradingSeries objSeries in series)
            {

                if (objSeries.Instances.Count > 1)
                {
                    decimal lastBalance = objSeries.Instances[0].Balance.Primary;
                    int i = 1;
                    while (i < objSeries.Instances.Count - 1 && objSeries.Instances[i].Balance.Primary == lastBalance)
                    {
                        i += 1;
                    }
                    if (lastBalance < objSeries.Instances[i].Balance.Primary)
                    {
                        return TradingTrend.Ask;
                    }
                    else if (lastBalance > objSeries.Instances[i].Balance.Primary)
                    {
                        return TradingTrend.Bid;
                    }
                }
            }
            return TradingTrend.Neutral;
		}

	
	}
}