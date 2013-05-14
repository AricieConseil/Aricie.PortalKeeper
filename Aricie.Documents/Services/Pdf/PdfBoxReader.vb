Imports DotNetNuke.Services.Exceptions
Imports System.IO
Imports Org.apache.pdfbox.pdmodel
Imports Org.apache.pdfbox.util

Namespace Services.Pdf

    Public Class PdfBoxReader
        Implements IDocumentReader

        Public Function GetDocumentText(ByVal fileName As String) As String Implements IDocumentReader.GetDocumentText
            Dim toReturn As String = String.Empty

            Dim objPDF As PDDocument = Nothing
            Try
                If File.Exists(fileName) Then
                    objPDF = PDDocument.load(fileName)
                    Dim stripper As New PDFTextStripper()
                    toReturn = stripper.getText(objPDF)
                End If
            Catch ex As Exception
                Dim exc As New Exception(" Détail : " & fileName, ex)
                Exceptions.LogException(exc)
                toReturn = String.Empty
            Finally
                If objPDF IsNot Nothing Then
                    objPDF.close()
                End If
            End Try

            Return toReturn.Trim()
        End Function

    End Class

End Namespace
