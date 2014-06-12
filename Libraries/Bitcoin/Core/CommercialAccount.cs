using Aricie.Collections;
using Aricie.DNN.ComponentModel;
using DotNetNuke.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class CommercialAccount
	{
		private WalletPayment _firstDebit;

		private WalletPayment _lastDebit;

		[IsReadOnly(true)]
		public decimal Balance
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		[XmlIgnore]
		public WalletPayment FirstDebit
		{
			get
			{
				Dictionary<string, List<WalletPayment>>.Enumerator enumerator = new Dictionary<string, List<WalletPayment>>.Enumerator();
				List<WalletPayment>.Enumerator enumerator1 = new List<WalletPayment>.Enumerator();
				if (this._firstDebit == null)
				{
					try
					{
						enumerator = this.PastDebits.GetEnumerator();
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, List<WalletPayment>> feeList = enumerator.Current;
							try
							{
								enumerator1 = feeList.Value.GetEnumerator();
								while (enumerator1.MoveNext())
								{
									WalletPayment objPayment = enumerator1.Current;
									if ((this._firstDebit == null || DateTime.Compare(objPayment.Time, this._firstDebit.Time) < 0 ? true : false))
									{
										this._firstDebit = objPayment;
									}
								}
							}
							finally
							{
								((IDisposable)enumerator1).Dispose();
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				return this._firstDebit;
			}
		}

		[XmlIgnore]
		public WalletPayment LastDebit
		{
			get
			{
				Dictionary<string, List<WalletPayment>>.Enumerator enumerator = new Dictionary<string, List<WalletPayment>>.Enumerator();
				List<WalletPayment>.Enumerator enumerator1 = new List<WalletPayment>.Enumerator();
				if (this._lastDebit == null)
				{
					try
					{
						enumerator = this.PastDebits.GetEnumerator();
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, List<WalletPayment>> feeList = enumerator.Current;
							try
							{
								enumerator1 = feeList.Value.GetEnumerator();
								while (enumerator1.MoveNext())
								{
									WalletPayment objPayment = enumerator1.Current;
									if ((this._lastDebit == null || DateTime.Compare(objPayment.Time, this._lastDebit.Time) > 0 ? true : false))
									{
										this._lastDebit = objPayment;
									}
								}
							}
							finally
							{
								((IDisposable)enumerator1).Dispose();
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				return this._lastDebit;
			}
		}

		[IsReadOnly(true)]
		public List<OnlinePayment> PastCredits
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		[IsReadOnly(true)]
		public SerializableDictionary<string, List<WalletPayment>> PastDebits
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		public CommercialAccount()
		{
			this.PastCredits = new List<OnlinePayment>();
			this.PastDebits = new SerializableDictionary<string, List<WalletPayment>>();
		}

		public void AddCredit(OnlinePayment objPayment)
		{
			this.Balance = decimal.Add(this.Balance, objPayment.Amount);
			this.PastCredits.Insert(0, objPayment);
		}

		public void AddDebit(CommercialFee objFee, WalletPayment objPayment)
		{
			List<WalletPayment> feeList;
			this.Balance = decimal.Subtract(this.Balance, objPayment.Amount);
			if (!this.PastDebits.TryGetValue(objFee.Name, out feeList))
			{
				feeList = new List<WalletPayment>();
				this.PastDebits[objFee.Name] = feeList;
			}
			feeList.Insert(0, objPayment);
		}
	}
}