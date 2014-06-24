using Aricie.DNN.Services.Filtering;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;

namespace Aricie.PortalKeeper.Reddit
{
    public class RedditWebAction: RedditOutputAction
    {
        public RedditWebAction()
        {
            WebRequestUrl = new SimpleOrSimpleExpression<string>("http://www.reddit.com");
            XPath = new XPathInfo("", true, true);
        }

        public SimpleOrSimpleExpression<string> WebRequestUrl { get; set; }

        public bool UseXpathXtract {get; set;}

        [ConditionalVisible("UseXpathXtract", false, true)]
        public XPathInfo XPath {get; set;}


    }
}