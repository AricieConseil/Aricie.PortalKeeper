

'Namespace Aricie.Pdf

'    Public Class PdfCreator

'        Private _PageSize As iTextSharp.text.Rectangle
'        Private _footer As iTextSharp.text.HeaderFooter

'        ''' <summary>
'        ''' Ctor
'        ''' Initialize object with default parameters :
'        ''' - PageSize=A4
'        ''' - Footer=Nothing
'        ''' </summary>
'        Public Sub New()
'            Me.new(iTextSharp.text.PageSize.A4, Nothing)
'        End Sub
'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pageSize">Size of the page</param>
'        ''' <param name="footer">Footer of the document, null is allowed</param>
'        ''' <remarks></remarks>
'        Public Sub New(ByVal pageSize As iTextSharp.text.Rectangle, ByVal footer As iTextSharp.text.HeaderFooter)
'            _PageSize = pageSize
'            _footer = footer
'        End Sub

'        ''' <summary>
'        ''' Genere un fichier pdf a partir d'un xml
'        ''' </summary>
'        ''' <param name="Xml">Document XML à enregistrer en pdf</param>
'        ''' <param name="nomFichier">Nom complet du fichier pdf à générer</param>
'        Public Sub GeneratePdfFromXml(ByVal xml As String, ByVal nomFichier As String)
'            Dim fs As New IO.FileStream(nomFichier, IO.FileMode.Create)
'            GeneratePdfFromXml(xml, fs)
'            fs.Close()
'        End Sub

'        ''' <summary>
'        ''' Genere un fichier pdf a partir d'un xml
'        ''' </summary>
'        ''' <param name="xml">Document XMLà enregistrer en pdf</param>
'        ''' <param name="Stream">Flux dans lequel sera enregistrer le pdf</param>
'        ''' <remarks></remarks>
'        Public Sub GeneratePdfFromXml(ByVal xml As String, ByVal stream As IO.Stream)
'            Dim documentA As New iTextSharp.text.Document(_PageSize, 80, 50, 30, 65)
'            If Not _footer Is Nothing Then
'                'on a un footer de defini
'                documentA.Footer = _footer
'            End If

'            Dim pdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(documentA, stream)
'            pdfWriter.Footer = _footer
'            Dim xmlHandler As New iTextSharp.text.xml.ITextHandler(documentA)
'            Dim xmldoc As New System.Xml.XmlDocument()
'            'Dim xmlToParse As New System.Text.StringBuilder
'            'xmlToParse.Append("<itext author=""Author "" title=""curriculum vitae"">")
'            'xmlToParse.Append("<chapter numberdepth=""0"">")
'            'xmlToParse.Append("<paragraph font=""Verdana"" size=""12"" leading=""15"" align=""Justify""> ")
'            'xmlToParse.Append("JB")
'            'xmlToParse.Append("<newline /> ")
'            'xmlToParse.Append("</paragraph> ")
'            'xmlToParse.Append("</chapter> ")
'            'xmlToParse.Append("</itext>")
'            'xmldoc.LoadXml(xmlToParse.ToString)
'            xmldoc.LoadXml(xml)
'            xmlHandler.Parse(xmldoc)

'        End Sub
'    End Class

'End Namespace

