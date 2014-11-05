Imports System.Web.UI.HtmlControls

Namespace UI.WebControls.EditControls
    Public Class IndexedDiv
        Inherits HtmlGenericControl
        Implements IPathProvider

        Private _Path As String = Nothing
        Private _Index As String = ""

        Public Sub New(index As String)
            MyBase.New("div")
            _Index = index
        End Sub



        Public Function GetPath() As String Implements IPathProvider.GetPath
            If _Path Is Nothing Then
                Dim parentPath As String = CollectionEditControl.GetParentPath(Me)
                If Not parentPath.IsNullOrEmpty() Then
                    Dim indexPath As String = CollectionEditControl.GetIndexPath(_Index)
                    _Path = parentPath & indexPath
                End If
            End If
            Return _Path
        End Function




    End Class
End NameSpace