Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class DataSourceAttribute
        Inherits Attribute

        Private _dataSource As String


        Public Sub New(ByVal datasource As String)
            _dataSource = datasource
        End Sub

        Public Property DataSource() As String
            Get
                Return _dataSource
            End Get
            Set(ByVal value As String)
                _dataSource = value
            End Set
        End Property

    End Class

End Namespace
