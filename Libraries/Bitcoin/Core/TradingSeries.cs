using Aricie;
using Aricie.DNN.ComponentModel;
using DotNetNuke.UI.WebControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class TradingSeries : SimpleList<TradingEvent>
	{

        public TradingSeries()
        {
            this.Period = new STimeSpan();
        }

        public TradingSeries(int maxNbItems, TimeSpan period)
        {
            this.Period = new STimeSpan();
            this.MaxNbItems = maxNbItems;
            this.Period.Value = period;
        }


        [Browsable(false)]
        public int MaxNbItems { get; set; }

        [Browsable(false)]
        public STimeSpan Period { get; set; }

	    [SortOrder(0)]
		public decimal EarningsPerPeriodRate
		{
			get
			{
				return decimal.Round(TotalEarningsRate * GetSpanRatio(), 3);
			}
		}

	  

	    [SortOrder(1)]
		public decimal TotalEarnings
		{
			get
			{
                if (this.Instances.Count > 1)
                {
                    return decimal.Round(this.Instances[0].Balance.Total 
                        - this.Instances[this.Instances.Count - 1].Balance.Total, 5);
                }
                return 0m;
			}
		}

        [SortOrder(2)]
        public decimal TotalEarningsRate
        {
            get
            {

                if (this.Instances.Count > 1 && this.Instances[this.Instances.Count - 1].Balance.Total > 0m)
                {
                    return decimal.Round((TotalEarnings 
                        / this.Instances[this.Instances.Count - 1].Balance.Total) * 100m, 3);
                }
                return 0m;
            }
        }


		[SortOrder(3)]
		public decimal TotalEarningsFixedPrice
		{
			get
			{

                if (this.Instances.Count > 1)
                {
                    return decimal.Round(this.Instances[0].Balance.Total 
                        - (this.Instances[this.Instances.Count - 1].Balance.Secondary 
                        + this.Instances[this.Instances.Count - 1].Balance.Primary * this.Instances[0].Balance.TickerLast), 5);
                }
                return 0m;
			}
		}

	

		[SortOrder(4)]
		public decimal TotalEarningsRateFixedPrice
		{
			get
			{

                if (this.Instances.Count > 1 && this.Instances[this.Instances.Count - 1].Balance.Total > 0)
                {
                    return decimal.Round((TotalEarningsFixedPrice 
                        / (this.Instances[this.Instances.Count - 1].Balance.Secondary 
                            + this.Instances[this.Instances.Count - 1].Balance.Primary * this.Instances[0].Balance.TickerLast)) * 100m, 3);
                }
                return 0m;
			}
		}

		

		public void AddEvent(TradingEvent objEvent)
		{
            //Doing some clean up to remove bad entries

            {
                if (this.Instances.Count > 0)
                {
                    if (this.Instances[0].Balance.Total == 0m)
                    {
                        //we don't want to keep logs with 0 balance, which prevent the comptutation
                        this.Instances.RemoveAt(0);
                    }
                    else if ((this.Period.Value.TotalDays < 10 && this.TotalEarningsRateFixedPrice > 30m) || this.TotalEarningsRateFixedPrice > 100m)
                    {
                        //we don't want to keep logs with too much earnings, which indicate that funds were added manually
                        this.Instances.RemoveAt(this.Instances.Count - 1);
                    }
                }
                if (this.Instances.Count == 0 || objEvent.Time.Subtract(this.Instances[0].Time) > this.Period.Value)
                {
                    this.Instances.Insert(0, objEvent);
                    if (this.Instances.Count > MaxNbItems)
                    {
                        this.Instances.RemoveRange(MaxNbItems, this.Instances.Count - MaxNbItems);
                    }
                }
            }
		}

		private decimal GetSpanRatio()
		{

            decimal toreturn = 1m;
            if (this.Period.Value.Ticks > 0 && this.Instances.Count > 0)
            {
                decimal span = this.Instances[0].Time.Subtract(this.Instances[this.Instances.Count - 1].Time).Ticks;
                if (span > 0)
                {
                    toreturn = this.Period.Value.Ticks / span;
                }
            }
            return toreturn;
		}
	}
}