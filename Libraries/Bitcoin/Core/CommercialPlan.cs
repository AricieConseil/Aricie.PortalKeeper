using Aricie;
using Aricie.DNN.ComponentModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class CommercialPlan : NamedConfig
	{
        

        public string TargetAddress { get; set; }

		public DateTime StartDate{ get; set; }
		
		public STimeSpan TrialDuration{ get; set; }

        public SimpleList<CommercialFee> Fees { get; set; }

		public CommercialPlan()
		{
			this.TargetAddress = "";
			this.TrialDuration = new STimeSpan();
			this.Fees = new SimpleList<CommercialFee>();
		}

		public void ComputeFees(CommercialAccount objAccount, TradingHistory history)
		{

            if (DateTime.Now > this.StartDate)
            {

                if (objAccount.PastDebits.Count == 0)
                {
                    //todo: handle that case
                }
                else
                {
                    if (objAccount.LastDebit.Time.Subtract(objAccount.FirstDebit.Time) > TrialDuration.Value)
                    {
                        foreach (CommercialFee objFee in Fees.Instances)
                        {
                            List<WalletPayment> feeList;
                            if (!objAccount.PastDebits.TryGetValue(objFee.Name,out feeList))
                            {
                                feeList = new List<WalletPayment>();
                                objAccount.PastDebits[objFee.Name] = feeList;
                            }
                            if (feeList.Count == 0 || feeList[0].Time.Add(objFee.Period.Value) < DateTime.Now)
                            {
                                var newPayment = objFee.ComputePayment(history);
                                objAccount.AddDebit(objFee, newPayment);
                            }
                        }
                    }
                }
            }
		}
	}
}