Imports DotNetNuke.Services.Exceptions

Imports System.IO

Imports Aricie.Documents.Services.IFilter
Imports Aricie.Documents.Services.Pdf
Imports System.Text.RegularExpressions
Imports Org.apache.pdfbox.pdmodel

Imports iTextSharp.text.pdf
Imports iTextSharp.text.xml
Imports System.Text


Namespace Services

    Public Class DocumentFile

#Region "Private Attributes"
        Private _fileName As String = String.Empty

        Private _dicoReadFilters As Dictionary(Of String, IDocumentReader) = New Dictionary(Of String, IDocumentReader)()

        Private _pageNumber As Integer = -1
#End Region

#Region "Public Properties"

        Public Property PageNumber() As Integer
            Get

                If _pageNumber = -1 AndAlso New FileInfo(_fileName).Extension.TrimStart("."c).ToLower() = "pdf" Then
                    'Dim pdf As PDDocument = Nothing
                    'Try
                    '    pdf = PDDocument.load(_fileName)
                    '    _pageNumber = pdf.getNumberOfPages()
                    'Finally
                    '    If pdf IsNot Nothing Then
                    '        pdf.close()
                    '    End If
                    'End Try
                    Dim pdfReader As PdfReader = New PdfReader(_fileName)
                    Me._pageNumber = pdfReader.NumberOfPages

                End If

                Return _pageNumber
            End Get
            Set(ByVal value As Integer)
                _pageNumber = value
            End Set
        End Property

#End Region

#Region "Cstors"
        Public Sub New()

        End Sub

        Public Sub New(ByVal fileName As String)
            _fileName = fileName
            InitDicoReadFilters()



        End Sub
#End Region

#Region "Public Methods"

        Public Function ReadDocumentText() As String
            Return ReadDocumentText(-1)
        End Function

        ''' <summary>
        ''' Read File Document Text
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadDocumentText(ByVal maxLengthChars As Integer) As String
            Dim toReturn As String = String.Empty

            If Not File.Exists(_fileName) Then
                Exceptions.LogException(New Exception("Error trying reading file content, File " & _fileName & "doesn't exist"))
            Else
                '1) Read with IFilter
                Dim iFiltReader As New IFilterReader()
                toReturn = iFiltReader.GetDocumentText(_fileName)

                '2) Read with Other Methods si pas de IFilter
                If Not iFiltReader.HasFilter Then

                    Dim file As New FileInfo(_fileName)

                    Dim fileExtension As String = file.Extension.TrimStart(Convert.ToChar(".")).ToLower()

                    If _dicoReadFilters.ContainsKey(fileExtension) Then
                        toReturn = _dicoReadFilters(fileExtension).GetDocumentText(_fileName)
                    Else
                        Exceptions.LogException(New Exception("Error trying reading file content, Filter for " + _fileName + " doesn't exist"))
                    End If

                End If


                'Clean string
                toReturn = CleanDocumentText(toReturn)

                'Limit string Length
                If maxLengthChars > 0 Then
                    toReturn = toReturn.Substring(0, (If((toReturn.Length > maxLengthChars), maxLengthChars, toReturn.Length)))
                End If

            End If

            Return toReturn
        End Function

        Private Shared DocumentTextCleanerRegex As New Regex("(?:/[rn]|[\s;]|---|\\[rnozuh]|&\#)+", RegexOptions.Compiled Or RegexOptions.IgnoreCase)
        ''' <summary>
        ''' Clean File Document Text
        ''' </summary>
        ''' <param name="content"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CleanDocumentText(ByVal content As String) As String
            Dim toReturn As String = content

            ' modification pour éviter le replace à répétition
            Return DocumentTextCleanerRegex.Replace(content, " ").Trim()


            'PDFs CLEAN
            toReturn = toReturn.Replace(" ", " ") 'Espace Insécable
            toReturn = toReturn.Replace("/r", " ").Replace("\r", " ")
            toReturn = toReturn.Replace("/n", " ").Replace("\n", " ")
            toReturn = toReturn.Replace(";", " ")
            toReturn = toReturn.Replace("&#", " ")

            toReturn = toReturn.Replace("---", " ")

            'OFFICEs CLEAN
            toReturn = toReturn.Replace("\o", " ")
            toReturn = toReturn.Replace("\z", " ")
            toReturn = toReturn.Replace("\u", " ")
            toReturn = toReturn.Replace("\h", " ")

            'Blanks CLEAN
            Dim reg As New Regex("\s{2,}", RegexOptions.IgnoreCase)
            toReturn = reg.Replace(toReturn, " ")
            toReturn = toReturn.Trim()

            Return toReturn
        End Function

        Public Function getPageNumber() As Integer
            Return PageNumber
        End Function

        Private Sub InitDicoReadFilters()
            ' /!\ Keys must be ToLower /!\
            _dicoReadFilters.Add("pdf", New PdfBoxReader())
            '_dicoReadFilters.Add("docx", New Word2007Reader())
            '_dicoReadFilters.Add("xlsx", New Excel2007Reader())
        End Sub

#End Region

    End Class

End Namespace
