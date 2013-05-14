Imports System.Text

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


    ''' <summary>
    ''' Helper to retrieve and encoding based on the dedicated enumeration
    ''' </summary>
    Public Module EncodingHelper

        Public Function GetEncoding(objSimpleEncoding As SimpleEncoding) As Encoding
            Select Case objSimpleEncoding
                Case SimpleEncoding.Default
                    Return Encoding.Default
                Case SimpleEncoding.UTF8
                    Return Encoding.UTF8
                Case SimpleEncoding.ASCII
                    Return Encoding.ASCII
                Case SimpleEncoding.Unicode
                    Return Encoding.Unicode
                Case SimpleEncoding.UTF7
                    Return Encoding.UTF7
                Case SimpleEncoding.UTF32
                    Return Encoding.UTF32
                Case SimpleEncoding.BigEndianUnicode
                    Return Encoding.BigEndianUnicode
                Case Else
                    Return Encoding.Default
            End Select
        End Function


    End Module

End Namespace





