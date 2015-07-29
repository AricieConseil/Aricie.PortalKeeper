Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class MediaTypeHeaderInfo
        Implements ISelector

        <Selector("Text", "Value", False, True, "Custom Media Type", "", False, True)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        Public Property KnownHeader As String = "text/json"

        <Required(True)> _
        <ConditionalVisible("KnownHeader", False, True, "")> _
        Public Property CustomHeader As New SimpleOrExpression(Of String)("text/html")

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection()
            Select Case propertyName
                Case "KnownHeader"
                    For Each mediatTypeHeader As String In ObsoleteDotNetProvider.Instance.GetMediaTypeHeaders()
                        toReturn.Add(New ListItem(mediatTypeHeader))
                    Next
            End Select

            Return toReturn
        End Function

    End Class
End NameSpace