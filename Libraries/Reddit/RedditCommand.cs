using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Aricie.Collections;
using Aricie.ComponentModel;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Entities;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls.EditControls;
using Aricie.Security.Cryptography;
using DotNetNuke.UI.WebControls;
using RedditSharp;

namespace Aricie.PortalKeeper.Reddit
{

    [Flags()]
    public enum RedditCommandSource
    {
        None = 0,
        PrivateMessage = 1,
        Comment = 2,        
        SubRedditPost = 4
    }




    public class RedditCommand: NamedConfig
    {

        public RedditCommand()
        {
            CommandSource = RedditCommandSource.Comment;
            Regexes = new List<CData>();
            CommandActions = new KeeperAction<ScheduleEvent>();
            MainAnswer = "";
            AlternateAnwsers = new List<CData>();
            NextCommands = new List<RedditCommand>();
        }

        [ExtendedCategory("Parsing")]
        public List<CData> Regexes { get; set; }

        [ExtendedCategory("Answers")]
        [Required(true)]
        public CData MainAnswer { get; set; }

        [ExtendedCategory("Answers")]
        public List<CData> AlternateAnwsers { get; set; }


        [ExtendedCategory("Scope")]
        public bool EnableAllUsers { get; set; }

        [ExtendedCategory("Scope")]
        public List<String> CommandUsers { get; set; }

        [ExtendedCategory("Scope")]
        public RedditCommandSource CommandSource { get; set; }

      


        private List<Regex> _BuiltRegexes;
        private List<Regex> BuiltRegexes
        {
            get
            {
                if (_BuiltRegexes == null)
                {
                    var regexList = new List<Regex>(Regexes.Count);
                    foreach (var cDataRegex in Regexes)
                    {
                        regexList.Add(new Regex(cDataRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    }
                    _BuiltRegexes = regexList;
                }
                return _BuiltRegexes;
            }
        }

        [ExtendedCategory("Actions")]
        public KeeperAction<ScheduleEvent> CommandActions { get; set; }

        

        [ExtendedCategory("Next")]
        public List<RedditCommand> NextCommands { get; set; }

        private Random _Random = new Random();

        public bool ParseThing(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext) //where TEngineEvent : IConvertible
        {
            var toReturn = false;
            string message = "";
            string author= "";

            switch (objThing.Kind)
            {
                case "Comment":
                    var comment = (Comment) objThing;
                    message = comment.Body;
                    author = comment.Author;
                    break;
                case "PrivateMessage":
                    var pm = (PrivateMessage) objThing;
                    message = pm.Body;
                    author = pm.Author;
                    break;
                case "Post":
                    var objPost = (Post) objThing;
                    message = objPost.SelfText;
                    author = objPost.Author.Name;
                    break;
            }
            if (EnableAllUsers || CommandUsers.Contains(author))
            {
                actionContext.SetVar("Thing", objThing);
                actionContext.SetVar("Message", message);
                actionContext.SetVar("Author", message);
                foreach (var objRegex in BuiltRegexes)
                {
                    var objMatch = objRegex.Match(message);
                    if (objMatch.Success)
                    {
                        Thing previousAnswer = null;
                        switch (objThing.Kind)
                        {
                            case "Comment":
                                var comment = (Comment)objThing;
                                previousAnswer = comment.Comments.First((com) => com.Author == objThing.Reddit.User.Name);
                                break;
                            case "PrivateMessage":
                                var pm = (PrivateMessage)objThing;
                                previousAnswer = pm.Replies.First((com) => com.Author == objThing.Reddit.User.Name);
                                break;
                            case "Post":
                                var objPost = (Post)objThing;
                                previousAnswer = objPost.Comments.First((com) => com.Author == objThing.Reddit.User.Name);
                                break;
                        }
                        if (previousAnswer ==  null)
                        {
                            foreach (string groupName in objRegex.GetGroupNames())
                            {
                                actionContext.SetVar(groupName, objMatch.Groups[groupName].Value);
                            }


                            toReturn = CommandActions.Run(actionContext);

                            string reply = this.MainAnswer;
                            if (this.AlternateAnwsers.Count > 0)
                            {
                                int nextIdx = _Random.Next(this.AlternateAnwsers.Count);
                                if (nextIdx > 0)
                                {
                                    reply = this.AlternateAnwsers[nextIdx];
                                }
                            }
                            var atr = actionContext.GetAdvancedTokenReplace();
                            reply = atr.ReplaceAllTokens(reply);

                            switch (objThing.Kind)
                            {
                                case "Comment":
                                    var comment = (Comment) objThing;
                                    comment.Reply(reply);
                                    break;
                                case "PrivateMessage":
                                    var pm = (PrivateMessage) objThing;
                                    pm.Reply(reply);
                                    break;
                                case "Post":
                                    var objPost = (Post) objThing;
                                    objPost.Comment(reply);
                                    break;
                            }
                        }
                        else
                        {
                            var candidateThings = new List<Thing>();
                            switch (previousAnswer.Kind)
                            {
                                case "Comment":
                                    var comment = (Comment)previousAnswer;
                                    candidateThings.AddRange(comment.Comments.FindAll((com) => com.Author == author).ToArray());
                                    break;
                                case "PrivateMessage":
                                    var pm = (PrivateMessage) previousAnswer;
                                    candidateThings.AddRange(new List<PrivateMessage>( pm.Replies).FindAll((com) => com.Author == author).ToArray());
                                    break;
                                case "Post":
                                    var objPost = (Comment)previousAnswer;
                                    candidateThings.AddRange(new List<Comment>(objPost.Comments).FindAll((com) => com.Author == author).ToArray());
                                    break;
                            }
                            foreach (var candidateThing in candidateThings)
                            {
                                foreach (var command in NextCommands)
                                {
                                   toReturn = toReturn | command.ParseThing(candidateThing, actionContext);
                                }    
                            }
                        }

                        return toReturn;
                    }

                }
            }
          
            return false;
        }



    }



    //public class RedditBot
    //{

    //    public RedditBot()
    //    {
    //        BotUser = new LoginInfo();
    //        WalletUser = new LoginInfo();
    //        Commands = new List<RedditCommand>();
    //    }

    //    public LoginInfo BotUser { get; set; }

        

    //    public LoginInfo WalletUser { get; set; }

    //    public List<RedditCommand> Commands { get; set; }



    //}
}
