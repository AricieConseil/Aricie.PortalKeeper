Imports System.Text.RegularExpressions

Namespace Services
    ''' <summary>
    ''' SubToken information holder
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure SubToken

        Public Sub New(ByVal type As SubTokenType)
            Me.SubTokenType = type
        End Sub

        Public Sub New(ByVal match As Match)
            Me.New(SubTokenType.Match)
            Me.Match = match
        End Sub

        Public Sub New(ByVal subProcessor As LoopTokenProcessor)
            Me.New(SubTokenType.SubProcessor)
            Me.SubProcessor = subProcessor
        End Sub

        Public SubTokenType As SubTokenType
        Public Match As Match
        Public SubProcessor As LoopTokenProcessor

    End Structure
End Namespace