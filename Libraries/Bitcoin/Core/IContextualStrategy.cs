using System;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	public interface IContextualStrategy
	{
		void ComputeNewOrders(ref TradingContext tContext);
	}
}