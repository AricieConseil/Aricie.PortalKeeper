using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Aricie.Collections;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls.EditControls;

namespace Aricie.PortalKeeper.Reddit
{
    public class RedditAnswer: NamedConfig, IProviderContainer
    {

        public RedditAnswer()
        {

            Actions = new SerializableList<RedditActionBase>();
            NextCommands = new List<RedditCommand>();
        }

        [ExtendedCategory("Conditions")]
        public bool Conditional { get; set; }

        [ConditionalVisible("Conditional", false, true)]
        public SimpleExpression<bool> Condition { get; set; }

        [ProvidersSelector("Key", "Value")]
        [ExtendedCategory("Actions")]
        public SerializableList<RedditActionBase> Actions { get; set; }


        [ExtendedCategory("Next")]
        public List<RedditCommand> NextCommands { get; set; }

        public IList GetSelector(string propertyName)
        {
            switch (propertyName)
            {
                case "Actions":
                    var toReturn = new ListItemCollection();
                    toReturn.Add(typeof (RedditAnswerAction).Name);
                    toReturn.Add(typeof(RedditDefineVariableAction).Name);
                    toReturn.Add(typeof(RedditWebAction).Name);
                    toReturn.Add(typeof(RedditPaymentAction).Name);
                    return toReturn;
            }
            return null;
        }

        public object GetNewItem(string collectionPropertyName, string providerName)
        {
            switch (collectionPropertyName)
            {
                case "Actions":
                    switch (providerName)
                    {
                        case "RedditAnswerAction":
                            return new RedditAnswerAction();
                        case "RedditDefineVariableAction":
                            return new RedditDefineVariableAction();
                        case "RedditWebAction":
                            return new RedditWebAction();
                        case "RedditPaymentAction":
                            return new RedditPaymentAction();
                    }
                    break;
            }
            return null;
        }

        public bool IsMatch(PortalKeeperContext<ScheduleEvent> actionContext)
        {
            return Enabled && (!Conditional || Condition.Evaluate(actionContext, actionContext));
        }
    }
}