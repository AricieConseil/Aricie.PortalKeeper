Imports System.ComponentModel
Imports System.Globalization
Imports System.Threading
Imports System.Web.UI.WebControls
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls

Namespace Entities
    <ActionButton(IconName.Flag, IconOptions.Normal)> _
    Public Class CulturePicker
        Implements ISelector


        Public Property CultureMode() As CultureInfoMode

        <Selector("Text", "Value", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("CultureMode", False, True, CultureInfoMode.Custom)> _
        Public Property CustomCulture() As String = ""

        Public Function GetCulture() As CultureInfo
            Select Case Me.CultureMode
                Case CultureInfoMode.Invariant
                    Return CultureInfo.InvariantCulture
                Case CultureInfoMode.Current
                    Return DirectCast(Thread.CurrentThread.CurrentCulture.Clone, CultureInfo)
                Case CultureInfoMode.CurrentUI
                    Return DirectCast(Thread.CurrentThread.CurrentUICulture.Clone, CultureInfo)
                Case Else
                    Return New CultureInfo(Me.CustomCulture)
            End Select
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection
            Select Case propertyName
                Case "CustomCulture"
                    For Each objCulture As CultureInfo In CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                        toReturn.Add(New ListItem(objCulture.DisplayName, objCulture.Name))
                    Next
            End Select
            Return toReturn
        End Function
    End Class
End NameSpace