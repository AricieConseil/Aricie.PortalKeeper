using System;
using System.ComponentModel;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[DefaultProperty("Time")]
	[Serializable]
	public class TradingEvent
	{
	    public Balance Balance { get; set; }

	    public DateTime Time { get; set; }

	    public TradingEvent()
		{
			Balance = new Balance();
		}

		public TradingEvent(DateTime time, Balance objBalance)
		{
			Balance = new Balance();
			Time = time;
			Balance = objBalance;
		}
	}
}