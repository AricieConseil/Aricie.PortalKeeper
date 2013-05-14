Imports DotNetNuke.UI.WebControls

Namespace Services.Filtering
    <Serializable()> _
    Public Class FilterControlInfo

        Private _controlId As String = String.Empty
        Private _propertyName As String = String.Empty

        <Required(True)> _
        Public Property ControlId() As String
            Get
                Return _controlId
            End Get
            Set(ByVal value As String)
                _controlId = value
            End Set
        End Property

        <Required(True)> _
        Public Property PropertyName() As String
            Get
                Return _propertyName
            End Get
            Set(ByVal value As String)
                _propertyName = value
            End Set
        End Property
    End Class
End Namespace