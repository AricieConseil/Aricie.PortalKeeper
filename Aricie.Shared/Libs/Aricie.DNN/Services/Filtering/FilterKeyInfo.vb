Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls

Namespace Services.Filtering
    
    Public Class FilterKeyInfo

        Private _key As String = String.Empty
        Private _webControls As New List(Of FilterControlInfo)

        Public Property Key() As String
            Get
                Return _key
            End Get
            Set(ByVal value As String)
                _key = value
            End Set
        End Property

        <Editor(GetType(ListEditControl), GetType(EditControl)), _
            LabelMode(LabelMode.Top)> _
        Public Property WebControls() As List(Of FilterControlInfo)
            Get
                Return _webControls
            End Get
            Set(ByVal value As List(Of FilterControlInfo))
                _webControls = value
            End Set
        End Property

    End Class
End Namespace