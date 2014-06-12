using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class Balance : ResponseObject
	{
	    public decimal Primary { get; set; }

        public decimal Secondary { get; set; }

        [Browsable(false)]
        public Ticker Ticker { get; set; }

	    [XmlIgnore]
		public decimal Value
		{
			get
			{
			    if (Ticker == null)
			    {
                    throw new InvalidOperationException("Balance Value cannot be computed without Ticker");
			    }
				return this.Primary * this.Ticker.Last;
			}
		}

	   

	    [XmlIgnore]
		public decimal TickerLast
		{
			get
			{
				return this.Ticker.Last;
			}
		}

		[XmlIgnore]
		public decimal Total
		{
			get
			{
				return this.GetTotal();
			}
		}

		

		public Balance()
		{
		}

		public Balance(ResponseObject objresponseObject) : base(objresponseObject)
		{
		}

        public Balance(decimal objPrimary,  decimal objSecondary)
        {

            this.Primary = objPrimary;
            this.Secondary = objSecondary;
        }

		public Balance(decimal objPrimary, Ticker objTicker, decimal objSecondary): this(objPrimary, objSecondary)
		{
			this.Ticker = objTicker;
		}

		public decimal GetTotal()
		{
			return  Secondary + Value;
		}
	}
}




//[Serializable]
//public class Balance : ResponseObject
//{
//    private Ticker _Ticker;

//    public decimal usds;

//    public decimal btcs;

//    public decimal BTC
//    {
//        get
//        {
//            return this.btcs;
//        }
//        set
//        {
//            this.btcs = value;
//        }
//    }

//    [XmlIgnore]
//    public decimal Value
//    {
//        get
//        {
//            decimal Value = decimal.Round(decimal.Multiply(this.btcs, this.Ticker.Last), 5);
//            return Value;
//        }
//    }

//    [Browsable(false)]
//    public Aricie.DNN.Modules.PortalKeeper.BitCoin.Ticker Ticker
//    {
//        get
//        {
//            return this._Ticker;
//        }
//        set
//        {
//            this._Ticker = value;
//        }
//    }

//    [XmlIgnore]
//    public decimal TickerLast
//    {
//        get
//        {
//            return this._Ticker.Last;
//        }
//    }

//    [XmlIgnore]
//    public decimal Total
//    {
//        get
//        {
//            return this.GetTotal();
//        }
//    }

//    public decimal USD
//    {
//        get
//        {
//            return this.usds;
//        }
//        set
//        {
//            this.usds = value;
//        }
//    }

//    public Balance()
//    {
//    }

//    public Balance(ResponseObject objresponseObject)
//        : base(objresponseObject)
//    {
//    }

//    public Balance(decimal objUsds, decimal objBtcs, Aricie.DNN.Modules.PortalKeeper.BitCoin.Ticker objTicker)
//    {
//        this.usds = objUsds;
//        this.btcs = objBtcs;
//        this._Ticker = objTicker;
//    }

//    public decimal GetTotal()
//    {
//        if (this.Ticker == null)
//        {
//            throw new InvalidOperationException("Ticker less balance can't compute total");
//        }
//        decimal GetTotal = decimal.Round(decimal.Add(this.usds, decimal.Multiply(this.btcs, this.Ticker.Last)), 5);
//        return GetTotal;
//    }
//}