Imports DotNetNuke.UI.WebControls

Namespace UI
    <Serializable()> _
    Public Class Field
        ' Methods
        Public Sub New()
            Me.Enabled = True
            Me.LabelMode = LabelMode.Left
        End Sub

        Public Sub New(ByVal fieldName As String)
            Me.FieldName = fieldName
        End Sub


        ' Properties
        Public Property Enabled() As Boolean
            Get
                Return Me._enabled
            End Get
            Set(ByVal value As Boolean)
                Me._enabled = value
            End Set
        End Property

        Public Property FieldName() As String
            Get
                Return Me._fieldName
            End Get
            Set(ByVal value As String)
                Me._fieldName = value
            End Set
        End Property

        Public Property LabelMode() As LabelMode
            Get
                Return Me._labelMode
            End Get
            Set(ByVal value As LabelMode)
                Me._labelMode = value
            End Set
        End Property

        Public Property Type() As String
            Get
                Return Me._type
            End Get
            Set(ByVal value As String)
                Me._type = value
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return _required
            End Get
            Set(ByVal value As Boolean)
                _required = value
            End Set
        End Property

        ' Fields
        Private _enabled As Boolean
        Private _fieldName As String
        Private _labelMode As LabelMode
        Private _type As String
        Private _required As Boolean
    End Class
End Namespace
