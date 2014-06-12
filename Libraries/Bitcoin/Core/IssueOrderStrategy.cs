using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.Services;
using System;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class IssueOrderStrategy : TradingStrategyBase
	{

        public Order StaticOrder { get; set; }

	    public bool DynamicAmount { get; set; }

        [ConditionalVisible("DynamicAmount", false, true)]
	    public SimpleExpression<decimal> DynamicAmountExpression { get; set; }

        public bool DynamicPrice { get; set; }

        [ConditionalVisible("DynamicPrice", false, true)]
        public SimpleExpression<decimal> DynamicPriceExpression { get; set; }

        public bool DynamicType { get; set; }

        [ConditionalVisible("DynamicType", false, true)]
        public SimpleExpression<int> DynamicTypeExpression { get; set; }

	    public bool DynamicId { get; set; }

        [ConditionalVisible("DynamicId", false, true)]
	    public SimpleExpression<string> DynamicIdExpression { get; set; }

	    public IssueOrderStrategy()
		{
			this.StaticOrder = new Order();
			this.DynamicAmountExpression = new SimpleExpression<decimal>();
			this.DynamicPriceExpression = new SimpleExpression<decimal>();
			this.DynamicTypeExpression = new SimpleExpression<int>();
			this.DynamicIdExpression = new SimpleExpression<string>();
		}

		public override void ComputeNewOrders(ref TradingContext tContext)
		{
			var newOrder = ReflectionHelper.CloneObject<Order>(this.StaticOrder);
			if (this.DynamicAmount)
			{
				newOrder.Amount = this.DynamicAmountExpression.Evaluate(tContext, tContext);
			}
			if (this.DynamicPrice)
			{
				newOrder.Price = this.DynamicPriceExpression.Evaluate(tContext, tContext);
			}
			if (this.DynamicType)
			{
				newOrder.Type = this.DynamicTypeExpression.Evaluate(tContext, tContext);
			}
			if (this.DynamicId)
			{
				newOrder.Oid = this.DynamicIdExpression.Evaluate(tContext, tContext);
			}
			tContext.NewOrders.Orders.Add(newOrder);
		}
	}
}