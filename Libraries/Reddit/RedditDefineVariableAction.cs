using Aricie.DNN.Services.Flee;

namespace Aricie.PortalKeeper.Reddit
{
    public class RedditDefineVariableAction : RedditOutputAction
    {
        public RedditDefineVariableAction()
        {
            Expression = new SimpleExpression<object>();
        }

        public SimpleExpression<object> Expression { get; set; }


    }
}