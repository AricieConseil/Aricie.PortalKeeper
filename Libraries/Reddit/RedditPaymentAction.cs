using Aricie.DNN.Services.Flee;

namespace Aricie.PortalKeeper.Reddit
{
    public class RedditPaymentAction: RedditActionBase
    {
        public RedditPaymentAction()
        {
            PayeeExpression = new SimpleOrSimpleExpression<string>("Payee", true);
            AmountExpression = new SimpleOrSimpleExpression<decimal>("Amount", true);
            UnitExpression = new SimpleOrSimpleExpression<string>("Unit", true);
        }

        
        public SimpleOrSimpleExpression<string> PayeeExpression { get; set; }

        public SimpleOrSimpleExpression<decimal> AmountExpression { get; set; }

        public SimpleOrSimpleExpression<string> UnitExpression { get; set; }

    }
}