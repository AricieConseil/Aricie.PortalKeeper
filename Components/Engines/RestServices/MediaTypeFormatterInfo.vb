Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class MediaTypeFormatterInfo
        Implements ISelector

        <Selector("Text", "Value", False, True, "Custom Formatter Type", "", False, True)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        Public Property KnownFormatter As String = "JsonMediaTypeFormatterTracer"

        <Required(True)> _
       <ConditionalVisible("KnownFormatter", False, True, "")> _
        Public Property FormatterExpression As New FleeExpressionInfo(Of Object)




        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection()
            Select Case propertyName
                Case "KnownFormatter"
                    For Each formatterName As String In ObsoleteDotNetProvider.Instance.GetFormatterNames()
                        toReturn.Add(New ListItem(formatterName))
                    Next
            End Select

            Return toReturn
        End Function
    End Class
End Namespace