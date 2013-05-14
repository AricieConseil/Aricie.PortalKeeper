Imports DotNetNuke.Services.Exceptions
Imports System.IO
Imports System.IO.Packaging
Imports System.Xml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Spreadsheet

Namespace Services.OpenXML

    Public Class Excel2007Reader
        Implements IDocumentReader

        Public Function GetDocumentText(ByVal fileName As String) As String Implements IDocumentReader.GetDocumentText
            Dim toReturn As String = String.Empty

            'TODO A revoir

            Using myDoc As SpreadsheetDocument = SpreadsheetDocument.Open(fileName, True)
                Dim workbookPart As WorkbookPart = myDoc.WorkbookPart

                'For Each worksheetPart As WorksheetPart In workbookPart.WorksheetParts
                Dim worksheetPart As WorksheetPart = workbookPart.WorksheetParts.First()

                Dim reader As OpenXmlReader = OpenXmlReader.Create(WorksheetPart)

                While reader.Read()
                    If reader.ElementType Is GetType(CellValue) Then
                        toReturn += reader.GetText() + " "
                    End If
                End While
                'Next
            End Using

            Return toReturn
        End Function

    End Class
End Namespace