Namespace Aricie.DNN.Modules.PortalKeeper
    <Flags()> _
    Public Enum RequestSourceType
        Any = 0
        Country = 1
        IPAddress = 2
        Session = 4
        UrlPath = 8
        Url = 16
        XForwardedIP = 32
    End Enum
End NameSpace