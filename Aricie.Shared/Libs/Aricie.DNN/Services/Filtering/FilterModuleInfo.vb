Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Services.Filtering
    
    Public Class FilterModuleInfo

        Private _friendlyName As String = String.Empty
        Private _keys As New List(Of FilterKeyInfo)

        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
            <Selector(GetType(DesktopModuleSelector), "FriendlyName", "ModuleName", True, False, "", "", False, False)> _
            <Required(True)> _
        Public Property FriendlyName() As String
            Get
                Return _friendlyName
            End Get
            Set(ByVal value As String)
                _friendlyName = value
            End Set
        End Property



        <Editor(GetType(ListEditControl), GetType(EditControl)), _
            LabelMode(LabelMode.Top)> _
        Public Property Keys() As List(Of FilterKeyInfo)
            Get
                Return _keys
            End Get
            Set(ByVal value As List(Of FilterKeyInfo))
                _keys = value
            End Set
        End Property

    End Class
End Namespace