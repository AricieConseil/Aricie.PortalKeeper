Imports System.Xml.Serialization
Imports System.Xml.Schema
Imports System.Xml
Imports System.Web.Configuration

Namespace Configuration

    ' <XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    '<XmlInclude(GetType(TrustAddInfo))> _
    <Serializable()> _
    Public Class CustomAddInfo
        Inherits AddInfo

        Public Sub New()

        End Sub

        Public Sub New(ByVal elementName As String)
            Me._ElementName = elementName
        End Sub


        Private _ElementName As String = ""


        Public Property ElementName() As String
            Get
                Return _ElementName
            End Get
            Set(ByVal value As String)
                _ElementName = value
            End Set
        End Property



    End Class


    ''' <summary>
    ''' Base Class for a add merge node
    ''' </summary>
    <XmlRoot("add")> _
    <XmlInclude(GetType(CustomAddInfo))> _
    <XmlInclude(GetType(CustomErrorAddInfo))> _
    <XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlInclude(GetType(ProviderAddInfo))> _
        <XmlInclude(GetType(AppSettingAddInfo))> _
        <XmlInclude(GetType(HttpModuleAddInfo))> _
        <XmlInclude(GetType(HttpHandlerAddInfo))> _
        <XmlInclude(GetType(WebServerAddInfo))> _
        <XmlInclude(GetType(TrustAddInfo))> _
        <Serializable()> _
    Public Class AddInfo
        Implements IXmlSerializable


        Private _Attributes As New Dictionary(Of String, String)

        Public Property Attributes() As Dictionary(Of String, String)
            Get
                Return _Attributes
            End Get
            Set(ByVal value As Dictionary(Of String, String))
                _Attributes = value
            End Set
        End Property

        Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            'reader.ReadStartElement("add")
            If reader.HasAttributes Then
                Dim attributeCount As Integer = reader.AttributeCount
                Dim i As Integer
                For i = 0 To attributeCount - 1
                    reader.MoveToAttribute(i)
                    Me._Attributes(reader.Name) = reader.GetAttribute(i)
                Next i
            End If
            'reader.ReadEndElement()
        End Sub

        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            'writer.WriteStartElement("add")

            For Each elt As String In Me.Attributes.Keys
                writer.WriteAttributeString(elt, Me.Attributes(elt))
            Next
            'writer.WriteEndElement()
        End Sub
    End Class





End Namespace