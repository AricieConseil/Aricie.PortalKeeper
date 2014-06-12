using System;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
    [Serializable]
	public class Ticker : ResponseObject, ICloneable
	{
	    public decimal Buy { get; set; }

	    public decimal High { get; set; }

	    public decimal Last { get; set; }

	    public decimal Low { get; set; }

	    public decimal Sell { get; set; }

	    public decimal Volume { get; set; }

	    public Ticker()
		{
		}

        public Ticker(decimal price): this(price, 0m)
		{
		}

        public Ticker(decimal price, decimal volume)
        {
            this.Last = price;
            this.Sell = price;
            this.Buy = price;
            this.Low = price;
            this.High = price;
            this.Volume = volume;
        }

	    public Ticker(Ticker cloneTicker)
	    {
	        if (cloneTicker != null)
	        {
                this.Last = cloneTicker.Last;
                this.Sell = cloneTicker.Sell;
                this.Buy = cloneTicker.Buy;
                this.Low = cloneTicker.Low;
                this.High = cloneTicker.High;
                this.Volume = cloneTicker.Volume;
	        }
	    }


	    public object Clone()
	    {
	        return new Ticker(this);
	    }
	}
}