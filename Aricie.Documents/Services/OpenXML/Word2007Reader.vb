Imports DotNetNuke.Services.Exceptions
Imports System.IO
Imports System.IO.Packaging
Imports System.Xml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing

Namespace Services.OpenXML

    Public Class Word2007Reader
        Implements IDocumentReader

        Public Function GetDocumentText(ByVal fileName As String) As String Implements IDocumentReader.GetDocumentText
            'Return ReadWordXML(fileName)
            Return ReadWordOpenXML(fileName)
        End Function

        Private Const officeDocRelType As [String] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"

        Private Function ReadWordXML(ByVal fileName As String) As String
            Dim toReturn As String = String.Empty

            Dim officePackage As Package = Nothing
            Try
                'Ouverture du package OpenXML
                officePackage = Package.Open(fileName, FileMode.Open, FileAccess.Read)

                Dim mainPart As PackagePart = Nothing
                Dim documentUri As Uri = Nothing
                'on récupère la partie contenant les propriétés
                For Each relationship As PackageRelationship In officePackage.GetRelationshipsByType(officeDocRelType)
                    ' Il n'y a qu'une seule partie de type partType dans le package
                    documentUri = PackUriHelper.ResolvePartUri(New Uri("/", UriKind.Relative), relationship.TargetUri)
                    mainPart = officePackage.GetPart(documentUri)
                    Exit For
                Next

                Dim doc As New XmlDocument()
                doc.Load(mainPart.GetStream())

                'sensible à la casse
                toReturn = doc.DocumentElement.InnerText

                officePackage.Close()
            Catch ex As Exception
                Dim exc As New Exception("Error reading file : " & fileName, ex)
                Exceptions.LogException(exc)
                toReturn = ""
                If officePackage IsNot Nothing Then
                    officePackage.Close()
                End If
            End Try

            Return toReturn
        End Function

        Private Function ReadWordOpenXML(ByVal fileName As String) As String
            Dim toReturn As String = String.Empty

            Using wdDoc As WordprocessingDocument = WordprocessingDocument.Open(fileName, True)
                ' Get the main document part.
                toReturn = wdDoc.MainDocumentPart.Document.InnerText
            End Using

            Return toReturn
        End Function

    End Class

End Namespace
