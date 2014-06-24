using System;

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
}