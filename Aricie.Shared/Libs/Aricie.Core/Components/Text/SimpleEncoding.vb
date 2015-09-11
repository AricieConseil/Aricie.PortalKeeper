
Imports System.Text.RegularExpressions

Namespace Text
    ''' <summary>
    ''' Enumeration wrapped around common encodings
    ''' </summary>
    Public Enum SimpleEncoding
        [Default]
        UTF8
        ASCII
        Unicode
        UTF7
        UTF32
        BigEndianUnicode
    End Enum


    Public Class MatchEvaluatorEventArgs
        Inherits EventArgs

        Public Sub New()

        End Sub

        Public Sub New(objMatch As Match)
            Me.Match = objMatch
        End Sub

        Public Property Match As Match

        Public Property Replacement As String

    End Class
End Namespace





