using Aricie.DNN.Services.Flee;
using System;
using System.Collections;
using System.Diagnostics;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class ComplexStrategy<TStrategy> : IContextualStrategy
	where TStrategy : TradingStrategyBase, new()
	{

        public TStrategy Strategy { get; set; }

        public string NewStatus { get; set; }

        public bool IsConditional { get; set; }

        public SimpleExpression<bool> Condition { get; set; }
        
        public CompoundTradingStrategy Alternate{get;set;}

		public bool IsLoop{get;set;}

        public SimpleExpression<IEnumerable> LoopEnumerableExpression { get; set; }

		public string LoopCurrentItemName{get;set;}
		

		public ComplexStrategy()
		{
			this.Condition = new SimpleExpression<bool>("true");
			this.Alternate = new CompoundTradingStrategy();
			this.LoopEnumerableExpression = new SimpleExpression<IEnumerable>();
			this.LoopCurrentItemName = "CurrentItem";
		}

		public void ComputeNewOrders(ref TradingContext tContext)
		{
			if (this.Condition.Evaluate(tContext, tContext))
			{
				this.Strategy.ComputeNewOrders(ref tContext);
				if (!string.IsNullOrEmpty(this.NewStatus))
				{
					tContext.CurrentOrders.Status = this.NewStatus;
					tContext.NewOrders.Status = this.NewStatus;
				}
			}
		}
	}
}