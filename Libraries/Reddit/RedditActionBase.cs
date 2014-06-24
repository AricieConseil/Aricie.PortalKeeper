using Aricie.DNN.ComponentModel;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;

namespace Aricie.PortalKeeper.Reddit
{
    public abstract class RedditActionBase: NamedConfig
    {

        public bool Conditional { get; set; }

        [ConditionalVisible("Conditional", false, true)]
        public SimpleExpression<bool> Condition { get; set; }


    }
}