using System;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	public class TickerInfo : ResponseObject
	{
		public Ticker ticker;

		[DebuggerNonUserCode]
		public TickerInfo()
		{
		}
	}
}