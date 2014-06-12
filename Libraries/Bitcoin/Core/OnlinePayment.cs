using System;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class OnlinePayment : Payment
	{
		public string Reference
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		public OnlinePayment()
		{
			this.Reference = "";
		}
	}
}