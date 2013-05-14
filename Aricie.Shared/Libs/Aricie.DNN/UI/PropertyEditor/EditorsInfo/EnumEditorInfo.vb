Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditorInfos
    Public Class EnumEditorInfo
        Inherits EditorInfo

        Private _enumType As Type

        Public Property EnumType() As Type
            Get
                Return _enumType
            End Get
            Set(ByVal value As Type)
                _enumType = value
            End Set
        End Property
    End Class
End Namespace

