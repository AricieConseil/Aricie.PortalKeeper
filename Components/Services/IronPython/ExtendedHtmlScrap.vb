Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ExtendedHtmlScrap
        Inherits HtmlPageScrapInfo

        <ExtendedCategory("Custom")> _
        Public Property Python As New EnabledFeature(Of IronPython)

    End Class
End NameSpace