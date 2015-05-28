Imports DotNetNuke.Services.Exceptions

Imports System.IO

Imports Aricie.Documents.Services.IFilter
Imports Aricie.Documents.Services.Pdf
Imports System.Text.RegularExpressions
Imports org.apache.pdfbox.pdmodel

Imports iTextSharp.text.pdf
Imports iTextSharp.text.xml
Imports System.Text
Imports iTextSharp.text


Namespace Services

    Public Class DocumentFile
        Implements IDisposable


#Region "Private Attributes"
        Private _fileName As String = String.Empty

        Private _tempFile As Boolean

        Private _dicoReadFilters As Dictionary(Of String, IDocumentReader) = New Dictionary(Of String, IDocumentReader)()

        Private _pageNumber As Integer = -1

        Private firstPageDim As Rectangle

#End Region

#Region "Public Properties"

        Public Property PageNumber() As Integer
            Get

                If _pageNumber = -1 AndAlso New FileInfo(FileName).Extension.TrimStart("."c).ToLower() = "pdf" Then
                    'Dim pdf As PDDocument = Nothing
                    'Try
                    '    pdf = PDDocument.load(_fileName)
                    '    _pageNumber = pdf.getNumberOfPages()
                    'Finally
                    '    If pdf IsNot Nothing Then
                    '        pdf.close()
                    '    End If
                    'End Try
                    Dim pdfReader As PdfReader = New PdfReader(FileName)
                    Me._pageNumber = pdfReader.NumberOfPages

                End If

                Return _pageNumber
            End Get
            Set(ByVal value As Integer)
                _pageNumber = value
            End Set
        End Property

        Public ReadOnly Property FileName As String
            Get
                Return _fileName
            End Get
        End Property

#End Region

#Region "Cstors"
        Public Sub New()

        End Sub

        Public Sub New(ByVal fileName As String)
            _fileName = fileName
            InitDicoReadFilters()
        End Sub

        Public Sub New(ByVal fileExtension As String, ByVal fileStream As Stream)
            Dim tempFileName As String = Path.GetTempFileName()
            _tempFile = True
            _fileName = tempFileName.Replace(".tmp", "."c & fileExtension)
            System.IO.File.Move(tempFileName, FileName)
            System.IO.File.WriteAllBytes(FileName, ReadStream(fileStream))
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

            If Not File.Exists(FileName) Then
                Exceptions.LogException(New Exception("An error occured when trying to read a file content: it seems that the file " & FileName & " doesn't exist!"))
            Else
                '1) Read with IFilter
                Dim iFiltReader As New IFilterReader()
                toReturn = iFiltReader.GetDocumentText(FileName)

                '2) Read with Other Methods si pas de IFilter
                If Not iFiltReader.HasFilter Then

                    Dim file As New FileInfo(FileName)

                    Dim fileExtension As String = file.Extension.TrimStart(Convert.ToChar(".")).ToLower()

                    If _dicoReadFilters.ContainsKey(fileExtension) Then
                        toReturn = _dicoReadFilters(fileExtension).GetDocumentText(FileName)
                        'Else
                        'Exceptions.LogException(New Exception("Error trying reading file content, Filter for " + _fileName + " doesn't exist"))
                    End If

                End If

                If toReturn <> String.Empty Then
                    ' Process the content

                    'Clean string
                    toReturn = CleanDocumentText(toReturn)

                    'Limit string Length
                    If maxLengthChars > 0 Then
                        toReturn = toReturn.Substring(0, (If((toReturn.Length > maxLengthChars), maxLengthChars, toReturn.Length)))
                    End If

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
        End Function

        Public Function getPageNumber() As Integer
            Return PageNumber
        End Function

        Public Function getBookRatio() As Double
            Dim pdfReader As PdfReader = New PdfReader(_fileName)

            Me.firstPageDim = pdfReader.GetPageSize(1)
            Dim rotation = pdfReader.GetPageRotation(1)
            If rotation = 90 Or rotation = 270 Then
                Return (Me.firstPageDim.Width / Me.firstPageDim.Height)
            End If
            Return (Me.firstPageDim.Height / Me.firstPageDim.Width)
        End Function

        Private Sub InitDicoReadFilters()
            ' /!\ Keys must be ToLower /!\
            _dicoReadFilters.Add("pdf", New PdfBoxReader())
            '_dicoReadFilters.Add("docx", New Word2007Reader())
            '_dicoReadFilters.Add("xlsx", New Excel2007Reader())
        End Sub

#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                If _tempFile Then
                    If System.IO.File.Exists(FileName) Then
                        System.IO.File.Delete(FileName)
                    End If

                End If


            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace
