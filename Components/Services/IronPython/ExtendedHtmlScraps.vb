Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class ExtendedHtmlScraps
        Inherits HtmlPageScrapsInfo(Of ExtendedHtmlScrap)

        <ExtendedCategory("Advanced")> _
        Public Property Python As New EnabledFeature(Of IronPython)

    End Class
End NameSpace