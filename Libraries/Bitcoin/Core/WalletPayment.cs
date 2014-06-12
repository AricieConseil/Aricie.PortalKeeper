using System;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class WalletPayment : Payment
	{
		public Aricie.DNN.Modules.PortalKeeper.BitCoin.Balance Balance
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		[DebuggerNonUserCode]
		public WalletPayment()
		{
		}
	}
}