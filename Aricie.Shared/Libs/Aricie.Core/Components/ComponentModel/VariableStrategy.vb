Imports System.Text.RegularExpressions

Namespace ComponentModel
    <Serializable()> _
    Public Class VariableStrategy(Of T)

        Public Sub New(ByVal strRegex As RegexMapping, ByVal strategy As T)
            Me._Mapping = strRegex
            Me._Strategy = strategy
        End Sub

        Private _Mapping As RegexMapping
        Private _Strategy As T

        Private _Regex As Regex
        Private _Match As Match


        Public ReadOnly Property Mapping() As RegexMapping
            Get
                Return _Mapping
            End Get
        End Property

        Public ReadOnly Property Strategy() As T
            Get
                Return _Strategy
            End Get
        End Property


        Public ReadOnly Property Regex() As Regex
            Get
                If _Regex Is Nothing Then
                    _Regex = New Regex(Me._Mapping.Pattern, RegexOptions.CultureInvariant Or RegexOptions.Compiled Or RegexOptions.IgnoreCase)
                End If
                Return _Regex
            End Get
        End Property

        Public ReadOnly Property Match() As Match
            Get
                If Me._Match Is Nothing Then
                    Me._Match = Me.Regex.Match(Me._Mapping.Sample)
                End If
                Return Me._Match
            End Get
        End Property


        Public ReadOnly Property Prefix() As String
            Get
                Return Me.Mapping.Sample.Substring(0, Me.Match.Groups(1).Index)
            End Get
        End Property


    End Class
End Namespace