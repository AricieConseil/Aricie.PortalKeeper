Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports Aricie.DNN.Services
Imports System.Text

Namespace UI.WebControls
    Public Class SubPathContainer
        Implements ISelector(Of KeyValuePair(Of String, String))

        Private _SubEntities As New Dictionary(Of String, Object)

        <IsReadOnly(True)> _
        Public Property OriginalPath As String = ""

        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Key", "Value", False, True, "Root", "Root", True, True)> _
        Public Property Path As String = "Root"

        '<ConditionalVisible("Path", False, True, "Root")> _
        <Browsable(False)> _
        Public Property OriginalEntity As Object


        '<ConditionalVisible("Path", True, True, "Root")>
        <XmlIgnore()> _
        Public Property SubEntity As Object
            Get
                Return GetSubEntity()
            End Get
            Set(value As Object)
                '_SubEntity = value
            End Set
        End Property

        <ActionButton("~/images/folderup.gif")> _
        Public Sub CloseSubEditor(pe As AriciePropertyEditorControl)
            pe.RootEditor.CloseSubEditor()
        End Sub


        Private Function GetSubEntity() As Object
            Dim toReturn As Object
            If Not _SubEntities.TryGetValue(Path, toReturn) Then
                If Not "Root" = Path Then
                    Dim propAccess As New DeepObjectPropertyAccess(Me.OriginalEntity)
                    Dim tokenPath As String = Path.Replace("."c, ":"c).Replace("["c, ":").Replace("]"c, "")
                    toReturn = propAccess.GetValue(tokenPath)
                Else
                    toReturn = Me.OriginalEntity
                End If
                Me._SubEntities(Path) = toReturn
            End If
            Return toReturn
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        Public Function GetSelectorG(propertyName As String) As IList(Of KeyValuePair(Of String, String)) Implements ISelector(Of KeyValuePair(Of String, String)).GetSelectorG
            Dim toReturn As New List(Of KeyValuePair(Of String, String))
            Dim dicoBuilder As New StringBuilder()
            Dim split As String() = Me.OriginalPath.Split("."c)
            Select Case propertyName
                Case "Path"
                    For i As Integer = LBound(split) To UBound(split) Step 1
                        Dim segment As String = split(i)
                        dicoBuilder.Append(segment)
                        toReturn.Add(New KeyValuePair(Of String, String)(dicoBuilder.ToString(), dicoBuilder.ToString()))
                        If i <> UBound(split) Then
                            dicoBuilder.Append("."c)
                        End If
                    Next
            End Select
            Return toReturn
        End Function
    End Class
End NameSpace