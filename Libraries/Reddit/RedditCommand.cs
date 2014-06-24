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
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.Security.Cryptography;
using RedditSharp;

namespace Aricie.PortalKeeper.Reddit
{
    public class RedditCommand: NamedConfig
    {

        private List<Regex> _BuiltRegexes;

        public RedditCommand()
        {
            CommandSource = RedditCommandSource.Comment;
            Regexes = new List<CData>();
            ParsingDepth = 1;
            CommandUsers = new List<SimpleOrSimpleExpression<string>>();
            RedditAnswers = new SerializableList<RedditAnswer>();
        }

        [ExtendedCategory("Parsing")]
        public List<CData> Regexes { get; set; }

        [ExtendedCategory("Parsing")]
        public int ParsingDepth { get; set; }

        [ExtendedCategory("Scope")]
        public RedditCommandSource CommandSource { get; set; }

        [ExtendedCategory("Scope")]
        public bool EnableAllUsers { get; set; }

        [ExtendedCategory("Scope")]
        public List<SimpleOrSimpleExpression<String>> CommandUsers { get; set; }

        [ExtendedCategory("Actions")]
        public SerializableList<RedditAnswer> RedditAnswers { get; set; }

       

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





        public IEnumerable<RedditAnswer> ParseThing(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext) //where TEngineEvent : IConvertible
        {
            var toReturn = new List<RedditAnswer>();


            if (EnableAllUsers || SimpleOrExpression<String>.GetValues( CommandUsers.ToArray(), actionContext, actionContext).Contains(objThing.AuthorName))
            {
                var parentsParsed = false;
                var message = ParseThingVariables(objThing, actionContext, "",0);

                foreach (var objRegex in BuiltRegexes)
                {
                    var objMatch = objRegex.Match(message);
                    if (objMatch.Success)
                    {
                        var previousAnswer = GetExistingAnswer(objThing);
                        // We only proceed with messages we have not answered yet
                        if (previousAnswer ==  null)
                        {
                            if (ParsingDepth > 0)
                            {
                                if (!parentsParsed)
                                {
                                    ParseThingVariables(objThing.ParentThing, actionContext, "Parent", ParsingDepth - 1);
                                    parentsParsed = true;
                                }
                            }
                            foreach (string groupName in objRegex.GetGroupNames())
                            {
                                actionContext.SetVar(groupName, objMatch.Groups[groupName].Value);
                            }

                            foreach (var objRedditAction in RedditAnswers)
                            {
                                if (objRedditAction.IsMatch(actionContext))
                                {
                                    toReturn.Add(objRedditAction);
                                }

                            }
                            
                        }
                        else
                        {
                            //var candidateThings = previousAnswer.Children;
                            //foreach (var candidateThing in candidateThings)
                            //{
                            //    foreach (var command in NextCommands)
                            //    {
                            //       toReturn = toReturn | command.ParseThing(candidateThing, actionContext);
                            //    }    
                            //}
                        }

                        return toReturn;
                    }

                }
            }

            return toReturn;
        }

        private string ParseThingVariables(Thing objThing, PortalKeeperContext<ScheduleEvent> actionContext, string prefix, int depth)
        {
            string toReturn = "";
            if (objThing != null && depth >= 0)
            {
                actionContext.SetVar(prefix + "Thing", objThing);
                actionContext.SetVar(prefix + "AuthorName", objThing.AuthorName);
                switch (objThing.Kind)
                {
                    case "Comment":
                        var objComment = (Comment)objThing;

                        actionContext.SetVar(prefix + "Comment", objComment);
                        toReturn = objComment.Body;
                        actionContext.SetVar(prefix + "Message", objComment.Body);

                        break;
                    case "PrivateMessage":
                        var pm = (PrivateMessage)objThing;
                        actionContext.SetVar(prefix + "PrivateMessage", pm);
                        toReturn = pm.Body;
                        actionContext.SetVar(prefix + "Message", pm.Body);
                        break;
                    case "Post":
                        var objPost = (Post)objThing;
                        actionContext.SetVar(prefix + "Post", objPost);
                        toReturn = objPost.SelfText;
                        actionContext.SetVar(prefix + "Message", objPost.SelfText);
                        break;
                }
                if (depth > 0)
                {
                    ParseThingVariables(objThing.ParentThing, actionContext, "Parent" + prefix, depth - 1);
                }
            }
            return toReturn;
        }

        private Thing GetExistingAnswer(Thing objThing)
        {
            Thing previousAnswer = null;
            switch (objThing.Kind)
            {
                case "Comment":
                    var comment = (Comment)objThing;
                    previousAnswer = comment.Comments.First((com) => com.AuthorName == objThing.Reddit.User.Name);
                    break;
                case "PrivateMessage":
                    var pm = (PrivateMessage)objThing;
                    previousAnswer = pm.Replies.First((com) => com.AuthorName == objThing.Reddit.User.Name);
                    break;
                case "Post":
                    var objPost = (Post)objThing;
                    previousAnswer = objPost.Comments.First((com) => com.AuthorName == objThing.Reddit.User.Name);
                    break;
            }
            return previousAnswer;
        }

    }

}
