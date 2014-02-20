Namespace UI.WebControls
    Public Class IconActionInfo

        Public Sub New()

        End Sub

        Public Sub New(objIconName As IconName)
            Me.IconName = objIconName
        End Sub


        Public Property IconName() As IconName = IconName.None

        Public Property IconOptions As IconOptions = IconOptions.Normal

        Public Property StackedIconName() As IconName = IconName.None

        Public Property StackedIconOptions As IconOptions = IconOptions.Normal

        Public Property StackContainerOptions As IconOptions = IconOptions.Normal




        Public Shared Icons As New Dictionary(Of IconName, String) From { _
            {IconName.None, ""}, _
            {IconName.Adjust, "fa-adjust"}, _
            {IconName.Adn, "fa-adn"}, _
            {IconName.AlignCenter, "fa-align-center"}, _
            {IconName.AlignJustify, "fa-align-justify"}, _
            {IconName.AlignLeft, "fa-align-left"}, _
            {IconName.AlignRight, "fa-align-right"}, _
            {IconName.Ambulance, "fa-ambulance"}, _
            {IconName.Anchor, "fa-anchor"}, _
            {IconName.Android, "fa-android"}, _
            {IconName.AngleDoubleDown, "fa-angle-double-down"}, _
            {IconName.AngleDoubleLeft, "fa-angle-double-left"}, _
            {IconName.AngleDoubleRight, "fa-angle-double-right"}, _
            {IconName.AngleDoubleUp, "fa-angle-double-up"}, _
            {IconName.AngleDown, "fa-angle-down"}, _
            {IconName.AngleLeft, "fa-angle-left"}, _
            {IconName.AngleRight, "fa-angle-right"}, _
            {IconName.AngleUp, "fa-angle-up"}, _
            {IconName.Apple, "fa-apple"}, _
            {IconName.Archive, "fa-archive"}, _
            {IconName.ArrowCircleDown, "fa-arrow-circle-down"}, _
            {IconName.ArrowCircleLeft, "fa-arrow-circle-left"}, _
            {IconName.ArrowCircleODown, "fa-arrow-circle-o-down"}, _
            {IconName.ArrowCircleOLeft, "fa-arrow-circle-o-left"}, _
            {IconName.ArrowCircleORight, "fa-arrow-circle-o-right"}, _
            {IconName.ArrowCircleOUp, "fa-arrow-circle-o-up"}, _
            {IconName.ArrowCircleRight, "fa-arrow-circle-right"}, _
            {IconName.ArrowCircleUp, "fa-arrow-circle-up"}, _
            {IconName.ArrowDown, "fa-arrow-down"}, _
            {IconName.ArrowLeft, "fa-arrow-left"}, _
            {IconName.ArrowRight, "fa-arrow-right"}, _
            {IconName.ArrowUp, "fa-arrow-up"}, _
            {IconName.Arrows, "fa-arrows"}, _
            {IconName.ArrowsAlt, "fa-arrows-alt"}, _
            {IconName.ArrowsH, "fa-arrows-h"}, _
            {IconName.ArrowsV, "fa-arrows-v"}, _
            {IconName.Asterisk, "fa-asterisk"}, _
            {IconName.Backward, "fa-backward"}, _
            {IconName.Ban, "fa-ban"}, _
            {IconName.BarChartO, "fa-bar-chart-o"}, _
            {IconName.Barcode, "fa-barcode"}, _
            {IconName.Bars, "fa-bars"}, _
            {IconName.Beer, "fa-beer"}, _
            {IconName.Bell, "fa-bell"}, _
            {IconName.BellO, "fa-bell-o"}, _
            {IconName.Bitbucket, "fa-bitbucket"}, _
            {IconName.BitbucketSquare, "fa-bitbucket-square"}, _
            {IconName.Bold, "fa-bold"}, _
            {IconName.Bolt, "fa-bolt"}, _
            {IconName.Book, "fa-book"}, _
            {IconName.Bookmark, "fa-bookmark"}, _
            {IconName.BookmarkO, "fa-bookmark-o"}, _
            {IconName.Briefcase, "fa-briefcase"}, _
            {IconName.Btc, "fa-btc"}, _
            {IconName.Bug, "fa-bug"}, _
            {IconName.BuildingO, "fa-building-o"}, _
            {IconName.Bullhorn, "fa-bullhorn"}, _
            {IconName.Bullseye, "fa-bullseye"}, _
            {IconName.Calendar, "fa-calendar"}, _
            {IconName.CalendarO, "fa-calendar-o"}, _
            {IconName.Camera, "fa-camera"}, _
            {IconName.CameraRetro, "fa-camera-retro"}, _
            {IconName.CaretDown, "fa-caret-down"}, _
            {IconName.CaretLeft, "fa-caret-left"}, _
            {IconName.CaretRight, "fa-caret-right"}, _
            {IconName.CaretSquareODown, "fa-caret-square-o-down"}, _
            {IconName.CaretSquareOLeft, "fa-caret-square-o-left"}, _
            {IconName.CaretSquareORight, "fa-caret-square-o-right"}, _
            {IconName.CaretSquareOUp, "fa-caret-square-o-up"}, _
            {IconName.CaretUp, "fa-caret-up"}, _
            {IconName.Certificate, "fa-certificate"}, _
            {IconName.ChainBroken, "fa-chain-broken"}, _
            {IconName.Check, "fa-check"}, _
            {IconName.CheckCircle, "fa-check-circle"}, _
            {IconName.CheckCircleO, "fa-check-circle-o"}, _
            {IconName.CheckSquare, "fa-check-square"}, _
            {IconName.CheckSquareO, "fa-check-square-o"}, _
            {IconName.ChevronCircleDown, "fa-chevron-circle-down"}, _
            {IconName.ChevronCircleLeft, "fa-chevron-circle-left"}, _
            {IconName.ChevronCircleRight, "fa-chevron-circle-right"}, _
            {IconName.ChevronCircleUp, "fa-chevron-circle-up"}, _
            {IconName.ChevronDown, "fa-chevron-down"}, _
            {IconName.ChevronLeft, "fa-chevron-left"}, _
            {IconName.ChevronRight, "fa-chevron-right"}, _
            {IconName.ChevronUp, "fa-chevron-up"}, _
            {IconName.Circle, "fa-circle"}, _
            {IconName.CircleO, "fa-circle-o"}, _
            {IconName.Clipboard, "fa-clipboard"}, _
            {IconName.ClockO, "fa-clock-o"}, _
            {IconName.Cloud, "fa-cloud"}, _
            {IconName.CloudDownload, "fa-cloud-download"}, _
            {IconName.CloudUpload, "fa-cloud-upload"}, _
            {IconName.Code, "fa-code"}, _
            {IconName.CodeFork, "fa-code-fork"}, _
            {IconName.Coffee, "fa-coffee"}, _
            {IconName.Cog, "fa-cog"}, _
            {IconName.Cogs, "fa-cogs"}, _
            {IconName.Columns, "fa-columns"}, _
            {IconName.Comment, "fa-comment"}, _
            {IconName.CommentO, "fa-comment-o"}, _
            {IconName.Comments, "fa-comments"}, _
            {IconName.CommentsO, "fa-comments-o"}, _
            {IconName.Compass, "fa-compass"}, _
            {IconName.Compress, "fa-compress"}, _
            {IconName.CreditCard, "fa-credit-card"}, _
            {IconName.Crop, "fa-crop"}, _
            {IconName.Crosshairs, "fa-crosshairs"}, _
            {IconName.Css3, "fa-css3"}, _
            {IconName.Cutlery, "fa-cutlery"}, _
            {IconName.Desktop, "fa-desktop"}, _
            {IconName.DotCircleO, "fa-dot-circle-o"}, _
            {IconName.Download, "fa-download"}, _
            {IconName.Dribbble, "fa-dribbble"}, _
            {IconName.Dropbox, "fa-dropbox"}, _
            {IconName.Eject, "fa-eject"}, _
            {IconName.EllipsisH, "fa-ellipsis-h"}, _
            {IconName.EllipsisV, "fa-ellipsis-v"}, _
            {IconName.Envelope, "fa-envelope"}, _
            {IconName.EnvelopeO, "fa-envelope-o"}, _
            {IconName.Eraser, "fa-eraser"}, _
            {IconName.Eur, "fa-eur"}, _
            {IconName.Exchange, "fa-exchange"}, _
            {IconName.Exclamation, "fa-exclamation"}, _
            {IconName.ExclamationCircle, "fa-exclamation-circle"}, _
            {IconName.ExclamationTriangle, "fa-exclamation-triangle"}, _
            {IconName.Expand, "fa-expand"}, _
            {IconName.ExternalLink, "fa-external-link"}, _
            {IconName.ExternalLinkSquare, "fa-external-link-square"}, _
            {IconName.Eye, "fa-eye"}, _
            {IconName.EyeSlash, "fa-eye-slash"}, _
            {IconName.Facebook, "fa-facebook"}, _
            {IconName.FacebookSquare, "fa-facebook-square"}, _
            {IconName.FastBackward, "fa-fast-backward"}, _
            {IconName.FastForward, "fa-fast-forward"}, _
            {IconName.Female, "fa-female"}, _
            {IconName.FighterJet, "fa-fighter-jet"}, _
            {IconName.File, "fa-file"}, _
            {IconName.FileO, "fa-file-o"}, _
            {IconName.FileText, "fa-file-text"}, _
            {IconName.FileTextO, "fa-file-text-o"}, _
            {IconName.FilesO, "fa-files-o"}, _
            {IconName.Film, "fa-film"}, _
            {IconName.Filter, "fa-filter"}, _
            {IconName.Fire, "fa-fire"}, _
            {IconName.FireExtinguisher, "fa-fire-extinguisher"}, _
            {IconName.Flag, "fa-flag"}, _
            {IconName.FlagCheckered, "fa-flag-checkered"}, _
            {IconName.FlagO, "fa-flag-o"}, _
            {IconName.Flask, "fa-flask"}, _
            {IconName.Flickr, "fa-flickr"}, _
            {IconName.FloppyO, "fa-floppy-o"}, _
            {IconName.Folder, "fa-folder"}, _
            {IconName.FolderO, "fa-folder-o"}, _
            {IconName.FolderOpen, "fa-folder-open"}, _
            {IconName.FolderOpenO, "fa-folder-open-o"}, _
            {IconName.Font, "fa-font"}, _
            {IconName.Forward, "fa-forward"}, _
            {IconName.Foursquare, "fa-foursquare"}, _
            {IconName.FrownO, "fa-frown-o"}, _
            {IconName.Gamepad, "fa-gamepad"}, _
            {IconName.Gavel, "fa-gavel"}, _
            {IconName.Gbp, "fa-gbp"}, _
            {IconName.Gift, "fa-gift"}, _
            {IconName.Github, "fa-github"}, _
            {IconName.GithubAlt, "fa-github-alt"}, _
            {IconName.GithubSquare, "fa-github-square"}, _
            {IconName.Gittip, "fa-gittip"}, _
            {IconName.Glass, "fa-glass"}, _
            {IconName.Globe, "fa-globe"}, _
            {IconName.GooglePlus, "fa-google-plus"}, _
            {IconName.GooglePlusSquare, "fa-google-plus-square"}, _
            {IconName.HSquare, "fa-h-square"}, _
            {IconName.HandODown, "fa-hand-o-down"}, _
            {IconName.HandOLeft, "fa-hand-o-left"}, _
            {IconName.HandORight, "fa-hand-o-right"}, _
            {IconName.HandOUp, "fa-hand-o-up"}, _
            {IconName.HddO, "fa-hdd-o"}, _
            {IconName.Headphones, "fa-headphones"}, _
            {IconName.Heart, "fa-heart"}, _
            {IconName.HeartO, "fa-heart-o"}, _
            {IconName.Home, "fa-home"}, _
            {IconName.HospitalO, "fa-hospital-o"}, _
            {IconName.Html5, "fa-html5"}, _
            {IconName.Inbox, "fa-inbox"}, _
            {IconName.Indent, "fa-indent"}, _
            {IconName.Info, "fa-info"}, _
            {IconName.InfoCircle, "fa-info-circle"}, _
            {IconName.Inr, "fa-inr"}, _
            {IconName.Instagram, "fa-instagram"}, _
            {IconName.Italic, "fa-italic"}, _
            {IconName.Jpy, "fa-jpy"}, _
            {IconName.Key, "fa-key"}, _
            {IconName.KeyboardO, "fa-keyboard-o"}, _
            {IconName.Krw, "fa-krw"}, _
            {IconName.Laptop, "fa-laptop"}, _
            {IconName.Leaf, "fa-leaf"}, _
            {IconName.LemonO, "fa-lemon-o"}, _
            {IconName.LevelDown, "fa-level-down"}, _
            {IconName.LevelUp, "fa-level-up"}, _
            {IconName.LightbulbO, "fa-lightbulb-o"}, _
            {IconName.Link, "fa-link"}, _
            {IconName.Linkedin, "fa-linkedin"}, _
            {IconName.LinkedinSquare, "fa-linkedin-square"}, _
            {IconName.Linux, "fa-linux"}, _
            {IconName.List, "fa-list"}, _
            {IconName.ListAlt, "fa-list-alt"}, _
            {IconName.ListOl, "fa-list-ol"}, _
            {IconName.ListUl, "fa-list-ul"}, _
            {IconName.LocationArrow, "fa-location-arrow"}, _
            {IconName.Lock, "fa-lock"}, _
            {IconName.LongArrowDown, "fa-long-arrow-down"}, _
            {IconName.LongArrowLeft, "fa-long-arrow-left"}, _
            {IconName.LongArrowRight, "fa-long-arrow-right"}, _
            {IconName.LongArrowUp, "fa-long-arrow-up"}, _
            {IconName.Magic, "fa-magic"}, _
            {IconName.Magnet, "fa-magnet"}, _
            {IconName.MailReplyAll, "fa-mail-reply-all"}, _
            {IconName.Male, "fa-male"}, _
            {IconName.MapMarker, "fa-map-marker"}, _
            {IconName.Maxcdn, "fa-maxcdn"}, _
            {IconName.Medkit, "fa-medkit"}, _
            {IconName.MehO, "fa-meh-o"}, _
            {IconName.Microphone, "fa-microphone"}, _
            {IconName.MicrophoneSlash, "fa-microphone-slash"}, _
            {IconName.Minus, "fa-minus"}, _
            {IconName.MinusCircle, "fa-minus-circle"}, _
            {IconName.MinusSquare, "fa-minus-square"}, _
            {IconName.MinusSquareO, "fa-minus-square-o"}, _
            {IconName.Mobile, "fa-mobile"}, _
            {IconName.Money, "fa-money"}, _
            {IconName.MoonO, "fa-moon-o"}, _
            {IconName.Music, "fa-music"}, _
            {IconName.Outdent, "fa-outdent"}, _
            {IconName.Pagelines, "fa-pagelines"}, _
            {IconName.Paperclip, "fa-paperclip"}, _
            {IconName.Pause, "fa-pause"}, _
            {IconName.Pencil, "fa-pencil"}, _
            {IconName.PencilSquare, "fa-pencil-square"}, _
            {IconName.PencilSquareO, "fa-pencil-square-o"}, _
            {IconName.Phone, "fa-phone"}, _
            {IconName.PhoneSquare, "fa-phone-square"}, _
            {IconName.PictureO, "fa-picture-o"}, _
            {IconName.Pinterest, "fa-pinterest"}, _
            {IconName.PinterestSquare, "fa-pinterest-square"}, _
            {IconName.Plane, "fa-plane"}, _
            {IconName.Play, "fa-play"}, _
            {IconName.PlayCircle, "fa-play-circle"}, _
            {IconName.PlayCircleO, "fa-play-circle-o"}, _
            {IconName.Plus, "fa-plus"}, _
            {IconName.PlusCircle, "fa-plus-circle"}, _
            {IconName.PlusSquare, "fa-plus-square"}, _
            {IconName.PowerOff, "fa-power-off"}, _
            {IconName.Print, "fa-print"}, _
            {IconName.PuzzlePiece, "fa-puzzle-piece"}, _
            {IconName.Qrcode, "fa-qrcode"}, _
            {IconName.Question, "fa-question"}, _
            {IconName.QuestionCircle, "fa-question-circle"}, _
            {IconName.QuoteLeft, "fa-quote-left"}, _
            {IconName.QuoteRight, "fa-quote-right"}, _
            {IconName.Random, "fa-random"}, _
            {IconName.Refresh, "fa-refresh"}, _
            {IconName.Renren, "fa-renren"}, _
            {IconName.Repeat, "fa-repeat"}, _
            {IconName.Reply, "fa-reply"}, _
            {IconName.ReplyAll, "fa-reply-all"}, _
            {IconName.Retweet, "fa-retweet"}, _
            {IconName.Road, "fa-road"}, _
            {IconName.Rocket, "fa-rocket"}, _
            {IconName.Rss, "fa-rss"}, _
            {IconName.RssSquare, "fa-rss-square"}, _
            {IconName.Rub, "fa-rub"}, _
            {IconName.Scissors, "fa-scissors"}, _
            {IconName.Search, "fa-search"}, _
            {IconName.SearchMinus, "fa-search-minus"}, _
            {IconName.SearchPlus, "fa-search-plus"}, _
            {IconName.Share, "fa-share"}, _
            {IconName.ShareSquare, "fa-share-square"}, _
            {IconName.ShareSquareO, "fa-share-square-o"}, _
            {IconName.Shield, "fa-shield"}, _
            {IconName.ShoppingCart, "fa-shopping-cart"}, _
            {IconName.SignIn, "fa-sign-in"}, _
            {IconName.SignOut, "fa-sign-out"}, _
            {IconName.Signal, "fa-signal"}, _
            {IconName.Sitemap, "fa-sitemap"}, _
            {IconName.Skype, "fa-skype"}, _
            {IconName.SmileO, "fa-smile-o"}, _
            {IconName.Sort, "fa-sort"}, _
            {IconName.SortAlphaAsc, "fa-sort-alpha-asc"}, _
            {IconName.SortAlphaDesc, "fa-sort-alpha-desc"}, _
            {IconName.SortAmountAsc, "fa-sort-amount-asc"}, _
            {IconName.SortAmountDesc, "fa-sort-amount-desc"}, _
            {IconName.SortAsc, "fa-sort-asc"}, _
            {IconName.SortDesc, "fa-sort-desc"}, _
            {IconName.SortNumericAsc, "fa-sort-numeric-asc"}, _
            {IconName.SortNumericDesc, "fa-sort-numeric-desc"}, _
            {IconName.Spinner, "fa-spinner"}, _
            {IconName.Square, "fa-square"}, _
            {IconName.SquareO, "fa-square-o"}, _
            {IconName.StackExchange, "fa-stack-exchange"}, _
            {IconName.StackOverflow, "fa-stack-overflow"}, _
            {IconName.Star, "fa-star"}, _
            {IconName.StarHalf, "fa-star-half"}, _
            {IconName.StarHalfO, "fa-star-half-o"}, _
            {IconName.StarO, "fa-star-o"}, _
            {IconName.StepBackward, "fa-step-backward"}, _
            {IconName.StepForward, "fa-step-forward"}, _
            {IconName.Stethoscope, "fa-stethoscope"}, _
            {IconName.[Stop], "fa-stop"}, _
            {IconName.Strikethrough, "fa-strikethrough"}, _
            {IconName.Subscript, "fa-subscript"}, _
            {IconName.Suitcase, "fa-suitcase"}, _
            {IconName.SunO, "fa-sun-o"}, _
            {IconName.Superscript, "fa-superscript"}, _
            {IconName.Table, "fa-table"}, _
            {IconName.Tablet, "fa-tablet"}, _
            {IconName.Tachometer, "fa-tachometer"}, _
            {IconName.Tag, "fa-tag"}, _
            {IconName.Tags, "fa-tags"}, _
            {IconName.Tasks, "fa-tasks"}, _
            {IconName.Terminal, "fa-terminal"}, _
            {IconName.TextHeight, "fa-text-height"}, _
            {IconName.TextWidth, "fa-text-width"}, _
            {IconName.Th, "fa-th"}, _
            {IconName.ThLarge, "fa-th-large"}, _
            {IconName.ThList, "fa-th-list"}, _
            {IconName.ThumbTack, "fa-thumb-tack"}, _
            {IconName.ThumbsDown, "fa-thumbs-down"}, _
            {IconName.ThumbsODown, "fa-thumbs-o-down"}, _
            {IconName.ThumbsOUp, "fa-thumbs-o-up"}, _
            {IconName.ThumbsUp, "fa-thumbs-up"}, _
            {IconName.Ticket, "fa-ticket"}, _
            {IconName.Times, "fa-times"}, _
            {IconName.TimesCircle, "fa-times-circle"}, _
            {IconName.TimesCircleO, "fa-times-circle-o"}, _
            {IconName.Tint, "fa-tint"}, _
            {IconName.TrashO, "fa-trash-o"}, _
            {IconName.Trello, "fa-trello"}, _
            {IconName.Trophy, "fa-trophy"}, _
            {IconName.Truck, "fa-truck"}, _
            {IconName.[Try], "fa-try "}, _
            {IconName.Tumblr, "fa-tumblr"}, _
            {IconName.TumblrSquare, "fa-tumblr-square"}, _
            {IconName.Twitter, "fa-twitter"}, _
            {IconName.TwitterSquare, "fa-twitter-square"}, _
            {IconName.Umbrella, "fa-umbrella"}, _
            {IconName.Underline, "fa-underline"}, _
            {IconName.Undo, "fa-undo"}, _
            {IconName.Unlock, "fa-unlock"}, _
            {IconName.UnlockAlt, "fa-unlock-alt"}, _
            {IconName.Upload, "fa-upload"}, _
            {IconName.Usd, "fa-usd"}, _
            {IconName.User, "fa-user"}, _
            {IconName.UserMd, "fa-user-md"}, _
            {IconName.Users, "fa-users"}, _
            {IconName.VideoCamera, "fa-video-camera"}, _
            {IconName.VimeoSquare, "fa-vimeo-square"}, _
            {IconName.Vk, "fa-vk"}, _
            {IconName.VolumeDown, "fa-volume-down"}, _
            {IconName.VolumeOff, "fa-volume-off"}, _
            {IconName.VolumeUp, "fa-volume-up"}, _
            {IconName.Weibo, "fa-weibo"}, _
            {IconName.Wheelchair, "fa-wheelchair"}, _
            {IconName.Windows, "fa-windows"}, _
            {IconName.Wrench, "fa-wrench"}, _
            {IconName.Xing, "fa-xing"}, _
            {IconName.XingSquare, "fa-xing-square"}, _
            {IconName.Youtube, "fa-youtube"}, _
            {IconName.YoutubePlay, "fa-youtube-play"}, _
            {IconName.YoutubeSquare, "fa-youtube-square"}}

    End Class
End Namespace
























































































































































































































































































































































































