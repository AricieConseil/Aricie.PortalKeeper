Namespace Entities

    ''' <summary>
    ''' Advanced URI class that support TLD parsing
    ''' </summary>
    <Serializable()> _
    Public Class ParsedUri
        Inherits Uri

        Public Sub New(uriString As String)
            MyBase.New(uriString)
            Me.ParseDomain()
        End Sub

        Private _DomainName As DomainName.Library.DomainName

        Private Sub ParseDomain()
            DomainName.Library.DomainName.TryParse(Me.Host, Me._DomainName)
        End Sub


        Public ReadOnly Property TLD As String
            Get
                If _DomainName IsNot Nothing Then
                    Return _DomainName.TLD
                End If
                Return String.Empty
            End Get
        End Property


        Public ReadOnly Property Domain As String
            Get
                If _DomainName IsNot Nothing Then
                    Return _DomainName.Domain
                End If
                Return Me.Host
            End Get
        End Property

        Public ReadOnly Property SubDomain As String
            Get
                If _DomainName IsNot Nothing Then
                    Return _DomainName.SubDomain
                End If
                Return String.Empty
            End Get
        End Property

    End Class

End Namespace


