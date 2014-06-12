using Aricie.DNN.Entities;
using System;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{

    [Serializable]
	public class ExchangeCredentials
	{
	    public APICredentials APIKey { get; set; }


	    public decimal AskTradingFee { get; set; }

        public decimal BidTradingFee { get; set; }


	    public ExchangeCredentials()
		{
			this.APIKey = new APICredentials();
	        this.AskTradingFee = 0.3m;
            this.BidTradingFee = 0.2m;
		}
	}
}