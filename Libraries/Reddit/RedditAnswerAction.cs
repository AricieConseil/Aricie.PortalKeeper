using System;
using System.Collections.Generic;
using Aricie.ComponentModel;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.Services;
using DotNetNuke.UI.WebControls;
using RedditSharp;

namespace Aricie.PortalKeeper.Reddit
{
    [Flags]
    public enum ThingKind
    {
        Comment = 1,
        PrivateMessage = 2,
        Post = 4
    }

    public abstract class RedditMessageAction : RedditActionBase
    {

        private Random _Random = new Random();

        [Required(true)]
        public CData MainAnswer { get; set; }

        public List<CData> AlternateAnswers { get; set; }


        public RedditMessageAction()
        {
            MainAnswer = "";
            AlternateAnswers = new List<CData>();
        }

        protected string GetMessage(PortalKeeperContext<ScheduleEvent> actionContext)
        {
            string message = this.MainAnswer;
            if (this.AlternateAnswers.Count > 0)
            {
                int nextIdx = _Random.Next(this.AlternateAnswers.Count);
                if (nextIdx > 0)
                {
                    message = this.AlternateAnswers[nextIdx];
                }
            }
            var atr = actionContext.GetAdvancedTokenReplace();
            message = atr.ReplaceAllTokens(message);
            return message;
        }

        protected void  Reply(Thing objThing, string message)
        {
            switch (objThing.Kind)
            {
                case "Comment":
                    var comment = (Comment)objThing;
                    comment.Reply(message);
                    break;
                case "PrivateMessage":
                    var pm = (PrivateMessage)objThing;
                    pm.Reply(message);
                    break;
                case "Post":
                    var objPost = (Post)objThing;
                    objPost.Comment(message);
                    break;
            }
        }

        public abstract void PostMessage(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext);

    }

    public class RedditPostAction : RedditMessageAction
    {
        public ThingKind AnswerTypes { get; set; }

        [ConditionalVisible("AnswerTypes", false, true, ThingKind.PrivateMessage, ThingKind.Post)]
        public SimpleOrSimpleExpression<string> MessageTitle { get; set; }

        [ConditionalVisible("AnswerTypes", false, true, ThingKind.PrivateMessage)]
        public SimpleOrSimpleExpression<string> TargetUser { get; set; }

        [ConditionalVisible("AnswerTypes", false, true, ThingKind.Post)]
        public SimpleOrSimpleExpression<string> TargetSubReddit { get; set; }

        [ConditionalVisible("AnswerTypes", false, true, ThingKind.Comment)]
        public SimpleOrSimpleExpression<string> TargetParentFullName { get; set; }


        public override void PostMessage(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext)
        {
            var message = GetMessage(actionContext);
            var title = MessageTitle.GetValue(actionContext, actionContext);
            if ((AnswerTypes & ThingKind.Post) == ThingKind.Post)
            {
                string strTargetSubReddit = TargetSubReddit.GetValue(actionContext, actionContext);
                Subreddit objSubReddit = objThing.Reddit.GetSubreddit(strTargetSubReddit);
                objSubReddit.SubmitTextPost(title, message);
            }
            if ((AnswerTypes & ThingKind.Comment) == ThingKind.Comment)
            {
                string strParentFullName = TargetParentFullName.GetValue(actionContext, actionContext);
                Thing targetParent = objThing.Reddit.GetThingByFullname(strParentFullName);
                 this.Reply(targetParent, message);
            }
            if ((AnswerTypes & ThingKind.PrivateMessage) == ThingKind.PrivateMessage)
            {
                string targetUserName = TargetUser.GetValue(actionContext, actionContext);
                objThing.Reddit.ComposePrivateMessage(title, message, targetUserName);
            }
        }
    }


    public class RedditAnswerAction : RedditMessageAction
    {
       

        public ThingKind AnswerTypes { get; set; }

        [ConditionalVisible("AnswerTypes", false, true, ThingKind.PrivateMessage)]
        public bool SpecificUser { get; set; }



        public override void PostMessage(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext)
        {
            var message = GetMessage(actionContext);
            this.Reply(objThing, message);
        }

       

    }
}