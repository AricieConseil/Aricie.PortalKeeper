using Aricie;
using Aricie.DNN.ComponentModel;
using System;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class CommercialFee : NamedEntity
	{
		public FeeBaseType Base
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		public STimeSpan Period
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		public decimal Rate
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		public CommercialFee()
		{
			this.Period = new STimeSpan();
		}

		public WalletPayment ComputePayment(TradingHistory history)
		{
			WalletPayment ComputePayment = null;
			return ComputePayment;
		}
	}
}