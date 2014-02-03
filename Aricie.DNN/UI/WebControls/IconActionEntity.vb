Namespace UI.WebControls
    Public Class IconActionEntity


        Public Property Url() As String = String.Empty

        Public Property Text() As String = String.Empty

        Public Property IconName() As IconsName = IconsName.None
        Public Property StackedIconName() As IconsName = IconsName.None

        Public Property ZoomLevel() As IconActionEntity.Zoom = IconActionEntity.Zoom.Normal

        Public Property FixWidth As Boolean = False

        Public Property Border As Boolean = False

        Public Property Spinning As Boolean = False

        Public Property FlipAndRotate As Rotate = Rotate.Normal

        '  Public Property StackStatus As Stack = Stack.Normal

        'fa-pencil-square-o

        Enum Zoom
            Normal
            Large
            x2
            x3
            x4
            x5
        End Enum

        Enum Rotate
            Normal = 1
            Rotate90 = 2
            Rotate180 = 4
            Rotate270 = 8
            FlipHorizontal = 16
            FlipVertical = 32
        End Enum

        'Enum Stack
        '    Normal = 1
        '    Stack1x = 2
        '    Stack2x = 4
        '    InverseColor = 8

        'End Enum

        Enum IconsName
            None
            Adjust
            Adn
            AlignCenter
            AlignJustify
            AlignLeft
            AlignRight
            Ambulance
            Anchor
            Android
            AngleDoubleDown
            AngleDoubleLeft
            AngleDoubleRight
            AngleDoubleUp
            AngleDown
            AngleLeft
            AngleRight
            AngleUp
            Apple
            Archive
            ArrowCircleDown
            ArrowCircleLeft
            ArrowCircleODown
            ArrowCircleOLeft
            ArrowCircleORight
            ArrowCircleOUp
            ArrowCircleRight
            ArrowCircleUp
            ArrowDown
            ArrowLeft
            ArrowRight
            ArrowUp
            Arrows
            ArrowsAlt
            ArrowsH
            ArrowsV
            Asterisk
            Backward
            Ban
            BarChartO
            Barcode
            Bars
            Beer
            Bell
            BellO
            Bitbucket
            BitbucketSquare
            Bold
            Bolt
            Book
            Bookmark
            BookmarkO
            Briefcase
            Btc
            Bug
            BuildingO
            Bullhorn
            Bullseye
            Calendar
            CalendarO
            Camera
            CameraRetro
            CaretDown
            CaretLeft
            CaretRight
            CaretSquareODown
            CaretSquareOLeft
            CaretSquareORight
            CaretSquareOUp
            CaretUp
            Certificate
            ChainBroken
            Check
            CheckCircle
            CheckCircleO
            CheckSquare
            CheckSquareO
            ChevronCircleDown
            ChevronCircleLeft
            ChevronCircleRight
            ChevronCircleUp
            ChevronDown
            ChevronLeft
            ChevronRight
            ChevronUp
            Circle
            CircleO
            Clipboard
            ClockO
            Cloud
            CloudDownload
            CloudUpload
            Code
            CodeFork
            Coffee
            Cog
            Cogs
            Columns
            Comment
            CommentO
            Comments
            CommentsO
            Compass
            Compress
            CreditCard
            Crop
            Crosshairs
            Css3
            Cutlery
            Desktop
            DotCircleO
            Download
            Dribbble
            Dropbox
            Eject
            EllipsisH
            EllipsisV
            Envelope
            EnvelopeO
            Eraser
            Eur
            Exchange
            Exclamation
            ExclamationCircle
            ExclamationTriangle
            Expand
            ExternalLink
            ExternalLinkSquare
            Eye
            EyeSlash
            Facebook
            FacebookSquare
            FastBackward
            FastForward
            Female
            FighterJet
            File
            FileO
            FileText
            FileTextO
            FilesO
            Film
            Filter
            Fire
            FireExtinguisher
            Flag
            FlagCheckered
            FlagO
            Flask
            Flickr
            FloppyO
            Folder
            FolderO
            FolderOpen
            FolderOpenO
            Font
            Forward
            Foursquare
            FrownO
            Gamepad
            Gavel
            Gbp
            Gift
            Github
            GithubAlt
            GithubSquare
            Gittip
            Glass
            Globe
            GooglePlus
            GooglePlusSquare
            HSquare
            HandODown
            HandOLeft
            HandORight
            HandOUp
            HddO
            Headphones
            Heart
            HeartO
            Home
            HospitalO
            Html5
            Inbox
            Indent
            Info
            InfoCircle
            Inr
            Instagram
            Italic
            Jpy
            Key
            KeyboardO
            Krw
            Laptop
            Leaf
            LemonO
            LevelDown
            LevelUp
            LightbulbO
            Link
            Linkedin
            LinkedinSquare
            Linux
            List
            ListAlt
            ListOl
            ListUl
            LocationArrow
            Lock
            LongArrowDown
            LongArrowLeft
            LongArrowRight
            LongArrowUp
            Magic
            Magnet
            MailReplyAll
            Male
            MapMarker
            Maxcdn
            Medkit
            MehO
            Microphone
            MicrophoneSlash
            Minus
            MinusCircle
            MinusSquare
            MinusSquareO
            Mobile
            Money
            MoonO
            Music
            Outdent
            Pagelines
            Paperclip
            Pause
            Pencil
            PencilSquare
            PencilSquareO
            Phone
            PhoneSquare
            PictureO
            Pinterest
            PinterestSquare
            Plane
            Play
            PlayCircle
            PlayCircleO
            Plus
            PlusCircle
            PlusSquare
            PowerOff
            Print
            PuzzlePiece
            Qrcode
            Question
            QuestionCircle
            QuoteLeft
            QuoteRight
            Random
            Refresh
            Renren
            Repeat
            Reply
            ReplyAll
            Retweet
            Road
            Rocket
            Rss
            RssSquare
            Rub
            Scissors
            Search
            SearchMinus
            SearchPlus
            Share
            ShareSquare
            ShareSquareO
            Shield
            ShoppingCart
            SignIn
            SignOut
            Signal
            Sitemap
            Skype
            SmileO
            Sort
            SortAlphaAsc
            SortAlphaDesc
            SortAmountAsc
            SortAmountDesc
            SortAsc
            SortDesc
            SortNumericAsc
            SortNumericDesc
            Spinner
            Square
            SquareO
            StackExchange
            StackOverflow
            Star
            StarHalf
            StarHalfO
            StarO
            StepBackward
            StepForward
            Stethoscope
            [Stop]
            Strikethrough
            Subscript
            Suitcase
            SunO
            Superscript
            Table
            Tablet
            Tachometer
            Tag
            Tags
            Tasks
            Terminal
            TextHeight
            TextWidth
            Th
            ThLarge
            ThList
            ThumbTack
            ThumbsDown
            ThumbsODown
            ThumbsOUp
            ThumbsUp
            Ticket
            Times
            TimesCircle
            TimesCircleO
            Tint
            TrashO
            Trello
            Trophy
            Truck
            [Try]
            Tumblr
            TumblrSquare
            Twitter
            TwitterSquare
            Umbrella
            Underline
            Undo
            Unlock
            UnlockAlt
            Upload
            Usd
            User
            UserMd
            Users
            VideoCamera
            VimeoSquare
            Vk
            VolumeDown
            VolumeOff
            VolumeUp
            Weibo
            Wheelchair
            Windows
            Wrench
            Xing
            XingSquare
            Youtube
            YoutubePlay
            YoutubeSquare
        End Enum

        Public Shared Icons As New Dictionary(Of IconsName, String) From { _
            {IconsName.Adjust, "fa-adjust"}, _
            {IconsName.Adn, "fa-adn"}, _
            {IconsName.AlignCenter, "fa-align-center"}, _
            {IconsName.AlignJustify, "fa-align-justify"}, _
            {IconsName.AlignLeft, "fa-align-left"}, _
            {IconsName.AlignRight, "fa-align-right"}, _
            {IconsName.Ambulance, "fa-ambulance"}, _
            {IconsName.Anchor, "fa-anchor"}, _
            {IconsName.Android, "fa-android"}, _
            {IconsName.AngleDoubleDown, "fa-angle-double-down"}, _
            {IconsName.AngleDoubleLeft, "fa-angle-double-left"}, _
            {IconsName.AngleDoubleRight, "fa-angle-double-right"}, _
            {IconsName.AngleDoubleUp, "fa-angle-double-up"}, _
            {IconsName.AngleDown, "fa-angle-down"}, _
            {IconsName.AngleLeft, "fa-angle-left"}, _
            {IconsName.AngleRight, "fa-angle-right"}, _
            {IconsName.AngleUp, "fa-angle-up"}, _
            {IconsName.Apple, "fa-apple"}, _
            {IconsName.Archive, "fa-archive"}, _
            {IconsName.ArrowCircleDown, "fa-arrow-circle-down"}, _
            {IconsName.ArrowCircleLeft, "fa-arrow-circle-left"}, _
            {IconsName.ArrowCircleODown, "fa-arrow-circle-o-down"}, _
            {IconsName.ArrowCircleOLeft, "fa-arrow-circle-o-left"}, _
            {IconsName.ArrowCircleORight, "fa-arrow-circle-o-right"}, _
            {IconsName.ArrowCircleOUp, "fa-arrow-circle-o-up"}, _
            {IconsName.ArrowCircleRight, "fa-arrow-circle-right"}, _
            {IconsName.ArrowCircleUp, "fa-arrow-circle-up"}, _
            {IconsName.ArrowDown, "fa-arrow-down"}, _
            {IconsName.ArrowLeft, "fa-arrow-left"}, _
            {IconsName.ArrowRight, "fa-arrow-right"}, _
            {IconsName.ArrowUp, "fa-arrow-up"}, _
            {IconsName.Arrows, "fa-arrows"}, _
            {IconsName.ArrowsAlt, "fa-arrows-alt"}, _
            {IconsName.ArrowsH, "fa-arrows-h"}, _
            {IconsName.ArrowsV, "fa-arrows-v"}, _
            {IconsName.Asterisk, "fa-asterisk"}, _
            {IconsName.Backward, "fa-backward"}, _
            {IconsName.Ban, "fa-ban"}, _
            {IconsName.BarChartO, "fa-bar-chart-o"}, _
            {IconsName.Barcode, "fa-barcode"}, _
            {IconsName.Bars, "fa-bars"}, _
            {IconsName.Beer, "fa-beer"}, _
            {IconsName.Bell, "fa-bell"}, _
            {IconsName.BellO, "fa-bell-o"}, _
            {IconsName.Bitbucket, "fa-bitbucket"}, _
            {IconsName.BitbucketSquare, "fa-bitbucket-square"}, _
            {IconsName.Bold, "fa-bold"}, _
            {IconsName.Bolt, "fa-bolt"}, _
            {IconsName.Book, "fa-book"}, _
            {IconsName.Bookmark, "fa-bookmark"}, _
            {IconsName.BookmarkO, "fa-bookmark-o"}, _
            {IconsName.Briefcase, "fa-briefcase"}, _
            {IconsName.Btc, "fa-btc"}, _
            {IconsName.Bug, "fa-bug"}, _
            {IconsName.BuildingO, "fa-building-o"}, _
            {IconsName.Bullhorn, "fa-bullhorn"}, _
            {IconsName.Bullseye, "fa-bullseye"}, _
            {IconsName.Calendar, "fa-calendar"}, _
            {IconsName.CalendarO, "fa-calendar-o"}, _
            {IconsName.Camera, "fa-camera"}, _
            {IconsName.CameraRetro, "fa-camera-retro"}, _
            {IconsName.CaretDown, "fa-caret-down"}, _
            {IconsName.CaretLeft, "fa-caret-left"}, _
            {IconsName.CaretRight, "fa-caret-right"}, _
            {IconsName.CaretSquareODown, "fa-caret-square-o-down"}, _
            {IconsName.CaretSquareOLeft, "fa-caret-square-o-left"}, _
            {IconsName.CaretSquareORight, "fa-caret-square-o-right"}, _
            {IconsName.CaretSquareOUp, "fa-caret-square-o-up"}, _
            {IconsName.CaretUp, "fa-caret-up"}, _
            {IconsName.Certificate, "fa-certificate"}, _
            {IconsName.ChainBroken, "fa-chain-broken"}, _
            {IconsName.Check, "fa-check"}, _
            {IconsName.CheckCircle, "fa-check-circle"}, _
            {IconsName.CheckCircleO, "fa-check-circle-o"}, _
            {IconsName.CheckSquare, "fa-check-square"}, _
            {IconsName.CheckSquareO, "fa-check-square-o"}, _
            {IconsName.ChevronCircleDown, "fa-chevron-circle-down"}, _
            {IconsName.ChevronCircleLeft, "fa-chevron-circle-left"}, _
            {IconsName.ChevronCircleRight, "fa-chevron-circle-right"}, _
            {IconsName.ChevronCircleUp, "fa-chevron-circle-up"}, _
            {IconsName.ChevronDown, "fa-chevron-down"}, _
            {IconsName.ChevronLeft, "fa-chevron-left"}, _
            {IconsName.ChevronRight, "fa-chevron-right"}, _
            {IconsName.ChevronUp, "fa-chevron-up"}, _
            {IconsName.Circle, "fa-circle"}, _
            {IconsName.CircleO, "fa-circle-o"}, _
            {IconsName.Clipboard, "fa-clipboard"}, _
            {IconsName.ClockO, "fa-clock-o"}, _
            {IconsName.Cloud, "fa-cloud"}, _
            {IconsName.CloudDownload, "fa-cloud-download"}, _
            {IconsName.CloudUpload, "fa-cloud-upload"}, _
            {IconsName.Code, "fa-code"}, _
            {IconsName.CodeFork, "fa-code-fork"}, _
            {IconsName.Coffee, "fa-coffee"}, _
            {IconsName.Cog, "fa-cog"}, _
            {IconsName.Cogs, "fa-cogs"}, _
            {IconsName.Columns, "fa-columns"}, _
            {IconsName.Comment, "fa-comment"}, _
            {IconsName.CommentO, "fa-comment-o"}, _
            {IconsName.Comments, "fa-comments"}, _
            {IconsName.CommentsO, "fa-comments-o"}, _
            {IconsName.Compass, "fa-compass"}, _
            {IconsName.Compress, "fa-compress"}, _
            {IconsName.CreditCard, "fa-credit-card"}, _
            {IconsName.Crop, "fa-crop"}, _
            {IconsName.Crosshairs, "fa-crosshairs"}, _
            {IconsName.Css3, "fa-css3"}, _
            {IconsName.Cutlery, "fa-cutlery"}, _
            {IconsName.Desktop, "fa-desktop"}, _
            {IconsName.DotCircleO, "fa-dot-circle-o"}, _
            {IconsName.Download, "fa-download"}, _
            {IconsName.Dribbble, "fa-dribbble"}, _
            {IconsName.Dropbox, "fa-dropbox"}, _
            {IconsName.Eject, "fa-eject"}, _
            {IconsName.EllipsisH, "fa-ellipsis-h"}, _
            {IconsName.EllipsisV, "fa-ellipsis-v"}, _
            {IconsName.Envelope, "fa-envelope"}, _
            {IconsName.EnvelopeO, "fa-envelope-o"}, _
            {IconsName.Eraser, "fa-eraser"}, _
            {IconsName.Eur, "fa-eur"}, _
            {IconsName.Exchange, "fa-exchange"}, _
            {IconsName.Exclamation, "fa-exclamation"}, _
            {IconsName.ExclamationCircle, "fa-exclamation-circle"}, _
            {IconsName.ExclamationTriangle, "fa-exclamation-triangle"}, _
            {IconsName.Expand, "fa-expand"}, _
            {IconsName.ExternalLink, "fa-external-link"}, _
            {IconsName.ExternalLinkSquare, "fa-external-link-square"}, _
            {IconsName.Eye, "fa-eye"}, _
            {IconsName.EyeSlash, "fa-eye-slash"}, _
            {IconsName.Facebook, "fa-facebook"}, _
            {IconsName.FacebookSquare, "fa-facebook-square"}, _
            {IconsName.FastBackward, "fa-fast-backward"}, _
            {IconsName.FastForward, "fa-fast-forward"}, _
            {IconsName.Female, "fa-female"}, _
            {IconsName.FighterJet, "fa-fighter-jet"}, _
            {IconsName.File, "fa-file"}, _
            {IconsName.FileO, "fa-file-o"}, _
            {IconsName.FileText, "fa-file-text"}, _
            {IconsName.FileTextO, "fa-file-text-o"}, _
            {IconsName.FilesO, "fa-files-o"}, _
            {IconsName.Film, "fa-film"}, _
            {IconsName.Filter, "fa-filter"}, _
            {IconsName.Fire, "fa-fire"}, _
            {IconsName.FireExtinguisher, "fa-fire-extinguisher"}, _
            {IconsName.Flag, "fa-flag"}, _
            {IconsName.FlagCheckered, "fa-flag-checkered"}, _
            {IconsName.FlagO, "fa-flag-o"}, _
            {IconsName.Flask, "fa-flask"}, _
            {IconsName.Flickr, "fa-flickr"}, _
            {IconsName.FloppyO, "fa-floppy-o"}, _
            {IconsName.Folder, "fa-folder"}, _
            {IconsName.FolderO, "fa-folder-o"}, _
            {IconsName.FolderOpen, "fa-folder-open"}, _
            {IconsName.FolderOpenO, "fa-folder-open-o"}, _
            {IconsName.Font, "fa-font"}, _
            {IconsName.Forward, "fa-forward"}, _
            {IconsName.Foursquare, "fa-foursquare"}, _
            {IconsName.FrownO, "fa-frown-o"}, _
            {IconsName.Gamepad, "fa-gamepad"}, _
            {IconsName.Gavel, "fa-gavel"}, _
            {IconsName.Gbp, "fa-gbp"}, _
            {IconsName.Gift, "fa-gift"}, _
            {IconsName.Github, "fa-github"}, _
            {IconsName.GithubAlt, "fa-github-alt"}, _
            {IconsName.GithubSquare, "fa-github-square"}, _
            {IconsName.Gittip, "fa-gittip"}, _
            {IconsName.Glass, "fa-glass"}, _
            {IconsName.Globe, "fa-globe"}, _
            {IconsName.GooglePlus, "fa-google-plus"}, _
            {IconsName.GooglePlusSquare, "fa-google-plus-square"}, _
            {IconsName.HSquare, "fa-h-square"}, _
            {IconsName.HandODown, "fa-hand-o-down"}, _
            {IconsName.HandOLeft, "fa-hand-o-left"}, _
            {IconsName.HandORight, "fa-hand-o-right"}, _
            {IconsName.HandOUp, "fa-hand-o-up"}, _
            {IconsName.HddO, "fa-hdd-o"}, _
            {IconsName.Headphones, "fa-headphones"}, _
            {IconsName.Heart, "fa-heart"}, _
            {IconsName.HeartO, "fa-heart-o"}, _
            {IconsName.Home, "fa-home"}, _
            {IconsName.HospitalO, "fa-hospital-o"}, _
            {IconsName.Html5, "fa-html5"}, _
            {IconsName.Inbox, "fa-inbox"}, _
            {IconsName.Indent, "fa-indent"}, _
            {IconsName.Info, "fa-info"}, _
            {IconsName.InfoCircle, "fa-info-circle"}, _
            {IconsName.Inr, "fa-inr"}, _
            {IconsName.Instagram, "fa-instagram"}, _
            {IconsName.Italic, "fa-italic"}, _
            {IconsName.Jpy, "fa-jpy"}, _
            {IconsName.Key, "fa-key"}, _
            {IconsName.KeyboardO, "fa-keyboard-o"}, _
            {IconsName.Krw, "fa-krw"}, _
            {IconsName.Laptop, "fa-laptop"}, _
            {IconsName.Leaf, "fa-leaf"}, _
            {IconsName.LemonO, "fa-lemon-o"}, _
            {IconsName.LevelDown, "fa-level-down"}, _
            {IconsName.LevelUp, "fa-level-up"}, _
            {IconsName.LightbulbO, "fa-lightbulb-o"}, _
            {IconsName.Link, "fa-link"}, _
            {IconsName.Linkedin, "fa-linkedin"}, _
            {IconsName.LinkedinSquare, "fa-linkedin-square"}, _
            {IconsName.Linux, "fa-linux"}, _
            {IconsName.List, "fa-list"}, _
            {IconsName.ListAlt, "fa-list-alt"}, _
            {IconsName.ListOl, "fa-list-ol"}, _
            {IconsName.ListUl, "fa-list-ul"}, _
            {IconsName.LocationArrow, "fa-location-arrow"}, _
            {IconsName.Lock, "fa-lock"}, _
            {IconsName.LongArrowDown, "fa-long-arrow-down"}, _
            {IconsName.LongArrowLeft, "fa-long-arrow-left"}, _
            {IconsName.LongArrowRight, "fa-long-arrow-right"}, _
            {IconsName.LongArrowUp, "fa-long-arrow-up"}, _
            {IconsName.Magic, "fa-magic"}, _
            {IconsName.Magnet, "fa-magnet"}, _
            {IconsName.MailReplyAll, "fa-mail-reply-all"}, _
            {IconsName.Male, "fa-male"}, _
            {IconsName.MapMarker, "fa-map-marker"}, _
            {IconsName.Maxcdn, "fa-maxcdn"}, _
            {IconsName.Medkit, "fa-medkit"}, _
            {IconsName.MehO, "fa-meh-o"}, _
            {IconsName.Microphone, "fa-microphone"}, _
            {IconsName.MicrophoneSlash, "fa-microphone-slash"}, _
            {IconsName.Minus, "fa-minus"}, _
            {IconsName.MinusCircle, "fa-minus-circle"}, _
            {IconsName.MinusSquare, "fa-minus-square"}, _
            {IconsName.MinusSquareO, "fa-minus-square-o"}, _
            {IconsName.Mobile, "fa-mobile"}, _
            {IconsName.Money, "fa-money"}, _
            {IconsName.MoonO, "fa-moon-o"}, _
            {IconsName.Music, "fa-music"}, _
            {IconsName.Outdent, "fa-outdent"}, _
            {IconsName.Pagelines, "fa-pagelines"}, _
            {IconsName.Paperclip, "fa-paperclip"}, _
            {IconsName.Pause, "fa-pause"}, _
            {IconsName.Pencil, "fa-pencil"}, _
            {IconsName.PencilSquare, "fa-pencil-square"}, _
            {IconsName.PencilSquareO, "fa-pencil-square-o"}, _
            {IconsName.Phone, "fa-phone"}, _
            {IconsName.PhoneSquare, "fa-phone-square"}, _
            {IconsName.PictureO, "fa-picture-o"}, _
            {IconsName.Pinterest, "fa-pinterest"}, _
            {IconsName.PinterestSquare, "fa-pinterest-square"}, _
            {IconsName.Plane, "fa-plane"}, _
            {IconsName.Play, "fa-play"}, _
            {IconsName.PlayCircle, "fa-play-circle"}, _
            {IconsName.PlayCircleO, "fa-play-circle-o"}, _
            {IconsName.Plus, "fa-plus"}, _
            {IconsName.PlusCircle, "fa-plus-circle"}, _
            {IconsName.PlusSquare, "fa-plus-square"}, _
            {IconsName.PowerOff, "fa-power-off"}, _
            {IconsName.Print, "fa-print"}, _
            {IconsName.PuzzlePiece, "fa-puzzle-piece"}, _
            {IconsName.Qrcode, "fa-qrcode"}, _
            {IconsName.Question, "fa-question"}, _
            {IconsName.QuestionCircle, "fa-question-circle"}, _
            {IconsName.QuoteLeft, "fa-quote-left"}, _
            {IconsName.QuoteRight, "fa-quote-right"}, _
            {IconsName.Random, "fa-random"}, _
            {IconsName.Refresh, "fa-refresh"}, _
            {IconsName.Renren, "fa-renren"}, _
            {IconsName.Repeat, "fa-repeat"}, _
            {IconsName.Reply, "fa-reply"}, _
            {IconsName.ReplyAll, "fa-reply-all"}, _
            {IconsName.Retweet, "fa-retweet"}, _
            {IconsName.Road, "fa-road"}, _
            {IconsName.Rocket, "fa-rocket"}, _
            {IconsName.Rss, "fa-rss"}, _
            {IconsName.RssSquare, "fa-rss-square"}, _
            {IconsName.Rub, "fa-rub"}, _
            {IconsName.Scissors, "fa-scissors"}, _
            {IconsName.Search, "fa-search"}, _
            {IconsName.SearchMinus, "fa-search-minus"}, _
            {IconsName.SearchPlus, "fa-search-plus"}, _
            {IconsName.Share, "fa-share"}, _
            {IconsName.ShareSquare, "fa-share-square"}, _
            {IconsName.ShareSquareO, "fa-share-square-o"}, _
            {IconsName.Shield, "fa-shield"}, _
            {IconsName.ShoppingCart, "fa-shopping-cart"}, _
            {IconsName.SignIn, "fa-sign-in"}, _
            {IconsName.SignOut, "fa-sign-out"}, _
            {IconsName.Signal, "fa-signal"}, _
            {IconsName.Sitemap, "fa-sitemap"}, _
            {IconsName.Skype, "fa-skype"}, _
            {IconsName.SmileO, "fa-smile-o"}, _
            {IconsName.Sort, "fa-sort"}, _
            {IconsName.SortAlphaAsc, "fa-sort-alpha-asc"}, _
            {IconsName.SortAlphaDesc, "fa-sort-alpha-desc"}, _
            {IconsName.SortAmountAsc, "fa-sort-amount-asc"}, _
            {IconsName.SortAmountDesc, "fa-sort-amount-desc"}, _
            {IconsName.SortAsc, "fa-sort-asc"}, _
            {IconsName.SortDesc, "fa-sort-desc"}, _
            {IconsName.SortNumericAsc, "fa-sort-numeric-asc"}, _
            {IconsName.SortNumericDesc, "fa-sort-numeric-desc"}, _
            {IconsName.Spinner, "fa-spinner"}, _
            {IconsName.Square, "fa-square"}, _
            {IconsName.SquareO, "fa-square-o"}, _
            {IconsName.StackExchange, "fa-stack-exchange"}, _
            {IconsName.StackOverflow, "fa-stack-overflow"}, _
            {IconsName.Star, "fa-star"}, _
            {IconsName.StarHalf, "fa-star-half"}, _
            {IconsName.StarHalfO, "fa-star-half-o"}, _
            {IconsName.StarO, "fa-star-o"}, _
            {IconsName.StepBackward, "fa-step-backward"}, _
            {IconsName.StepForward, "fa-step-forward"}, _
            {IconsName.Stethoscope, "fa-stethoscope"}, _
            {IconsName.[Stop], "fa-stop"}, _
            {IconsName.Strikethrough, "fa-strikethrough"}, _
            {IconsName.Subscript, "fa-subscript"}, _
            {IconsName.Suitcase, "fa-suitcase"}, _
            {IconsName.SunO, "fa-sun-o"}, _
            {IconsName.Superscript, "fa-superscript"}, _
            {IconsName.Table, "fa-table"}, _
            {IconsName.Tablet, "fa-tablet"}, _
            {IconsName.Tachometer, "fa-tachometer"}, _
            {IconsName.Tag, "fa-tag"}, _
            {IconsName.Tags, "fa-tags"}, _
            {IconsName.Tasks, "fa-tasks"}, _
            {IconsName.Terminal, "fa-terminal"}, _
            {IconsName.TextHeight, "fa-text-height"}, _
            {IconsName.TextWidth, "fa-text-width"}, _
            {IconsName.Th, "fa-th"}, _
            {IconsName.ThLarge, "fa-th-large"}, _
            {IconsName.ThList, "fa-th-list"}, _
            {IconsName.ThumbTack, "fa-thumb-tack"}, _
            {IconsName.ThumbsDown, "fa-thumbs-down"}, _
            {IconsName.ThumbsODown, "fa-thumbs-o-down"}, _
            {IconsName.ThumbsOUp, "fa-thumbs-o-up"}, _
            {IconsName.ThumbsUp, "fa-thumbs-up"}, _
            {IconsName.Ticket, "fa-ticket"}, _
            {IconsName.Times, "fa-times"}, _
            {IconsName.TimesCircle, "fa-times-circle"}, _
            {IconsName.TimesCircleO, "fa-times-circle-o"}, _
            {IconsName.Tint, "fa-tint"}, _
            {IconsName.TrashO, "fa-trash-o"}, _
            {IconsName.Trello, "fa-trello"}, _
            {IconsName.Trophy, "fa-trophy"}, _
            {IconsName.Truck, "fa-truck"}, _
            {IconsName.[Try], "fa-try "}, _
            {IconsName.Tumblr, "fa-tumblr"}, _
            {IconsName.TumblrSquare, "fa-tumblr-square"}, _
            {IconsName.Twitter, "fa-twitter"}, _
            {IconsName.TwitterSquare, "fa-twitter-square"}, _
            {IconsName.Umbrella, "fa-umbrella"}, _
            {IconsName.Underline, "fa-underline"}, _
            {IconsName.Undo, "fa-undo"}, _
            {IconsName.Unlock, "fa-unlock"}, _
            {IconsName.UnlockAlt, "fa-unlock-alt"}, _
            {IconsName.Upload, "fa-upload"}, _
            {IconsName.Usd, "fa-usd"}, _
            {IconsName.User, "fa-user"}, _
            {IconsName.UserMd, "fa-user-md"}, _
            {IconsName.Users, "fa-users"}, _
            {IconsName.VideoCamera, "fa-video-camera"}, _
            {IconsName.VimeoSquare, "fa-vimeo-square"}, _
            {IconsName.Vk, "fa-vk"}, _
            {IconsName.VolumeDown, "fa-volume-down"}, _
            {IconsName.VolumeOff, "fa-volume-off"}, _
            {IconsName.VolumeUp, "fa-volume-up"}, _
            {IconsName.Weibo, "fa-weibo"}, _
            {IconsName.Wheelchair, "fa-wheelchair"}, _
            {IconsName.Windows, "fa-windows"}, _
            {IconsName.Wrench, "fa-wrench"}, _
            {IconsName.Xing, "fa-xing"}, _
            {IconsName.XingSquare, "fa-xing-square"}, _
            {IconsName.Youtube, "fa-youtube"}, _
            {IconsName.YoutubePlay, "fa-youtube-play"}, _
            {IconsName.YoutubeSquare, "fa-youtube-square"}}

    End Class
End Namespace
























































































































































































































































































































































































