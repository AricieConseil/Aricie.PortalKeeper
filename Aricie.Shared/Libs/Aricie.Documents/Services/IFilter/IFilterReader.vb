Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports EPocalipse.IFilter

Namespace Services.IFilter
    Public Class IFilterReader
        Implements IDocumentReader

        Private _hasFilter As Boolean = False

        Public Function GetDocumentText(ByVal fileName As String) As String Implements IDocumentReader.GetDocumentText
            Dim toReturn As String = String.Empty

            Try
                Dim reader As TextReader = New FilterReader(fileName)
                Using (reader)
                    toReturn = reader.ReadToEnd()
                End Using
                'Check String.IsNullOrEmpty(toReturn) because sometimes PDF IFilter is installed
                ' but can't success to read document whereas PdfBox success
                _hasFilter = Not String.IsNullOrEmpty(toReturn)
            Catch ex As Exception
                _hasFilter = False
            End Try

            Return toReturn
        End Function

        Public ReadOnly Property HasFilter() As Boolean
            Get
                Return _hasFilter
            End Get
        End Property

    End Class
End Namespace