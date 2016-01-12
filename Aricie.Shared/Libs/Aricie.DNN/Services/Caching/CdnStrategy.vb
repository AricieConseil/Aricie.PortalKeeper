Imports Aricie.ComponentModel
Imports Aricie.DNN.Entities

Namespace Services.Caching
    
    Public Class CdnStrategy

        Public Property PortalAlias As New DnnPortalAlias()

        Public Property XPathQuery As CData = "//*[self::img or self::link or self::script or (self::a and contains(@href,'ortals/'))]/@*[(local-name() = 'href' or local-name() = 'src') and starts-with(.,'/')]"

        Public Property CachingStrategy As New OutputCachingStrategy()


    End Class
End NameSpace