Imports DotNetNuke.UI.WebControls
Imports Aricie.Services


Namespace UI.WebControls.EditorInfos
    Public Class AricieEditorInfo
        Inherits EditorInfo

        'Protected _hierarchy As AricieWebControlEntity
        Protected _autoPostBack As Boolean
        Private _length As Integer

        'Public Property Hierarchy() As AricieWebControlEntity
        '    Get
        '        Return _hierarchy
        '    End Get
        '    Set(ByVal value As AricieWebControlEntity)
        '        _hierarchy = value
        '    End Set
        'End Property

        Public Property AutoPostBack() As Boolean
            Get
                Return _autoPostBack
            End Get
            Set(ByVal value As Boolean)
                _autoPostBack = value
            End Set
        End Property

        Public Property Length() As Integer
            Get
                Return _length
            End Get
            Set(ByVal value As Integer)
                _length = value
            End Set
        End Property

        Private _PropertyType As Type

        Public Property PropertyType As Type
            Get
                If _PropertyType Is Nothing Then
                    _PropertyType = ReflectionHelper.CreateType(Me.Type, True)
                End If
                Return _PropertyType
            End Get
            Set(value As Type)
                _PropertyType = value
                Me.Type = value.AssemblyQualifiedName
            End Set
        End Property
           


    End Class
End Namespace

