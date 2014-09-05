Imports DotNetNuke.UI.WebControls

Namespace Configuration
    ''' <summary>
    ''' Base class for Xml nodes with a name attribute.
    ''' </summary>
    <Serializable()> _
    Public MustInherit Class XmlNamedConfigElementInfo
        Inherits XmlConfigElementInfo


        Private _Name As String = ""


        Public Sub New()

        End Sub


        Public Sub New(ByVal name As String)
            Me._Name = name
        End Sub

        <Required(True)> _
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

    End Class
End Namespace