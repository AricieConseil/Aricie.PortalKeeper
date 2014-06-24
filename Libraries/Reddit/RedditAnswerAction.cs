using System;
using System.Collections.Generic;
using Aricie.ComponentModel;
using Aricie.DNN.Modules.PortalKeeper;
using DotNetNuke.UI.WebControls;
using RedditSharp;

namespace Aricie.PortalKeeper.Reddit
{
    public class RedditAnswerAction: RedditActionBase
    {
        public RedditAnswerAction()
        {
            MainAnswer = "";
            AlternateAnswers = new List<CData>();
        }

        
        [Required(true)]
        public CData MainAnswer { get; set; }

        public List<CData> AlternateAnswers { get; set; }


        public void Reply(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext)
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

        private Random _Random = new Random();

    }
}