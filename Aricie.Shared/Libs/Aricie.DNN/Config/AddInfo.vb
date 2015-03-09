Imports System.Xml.Serialization
Imports System.Xml

Namespace Configuration

    '<XmlRoot("add", Namespace:="CustomAddInfo")> _
    <XmlInclude(GetType(ProviderAddInfo))> _
        <XmlInclude(GetType(AppSettingAddInfo))> _
        <XmlInclude(GetType(HttpModuleAddInfo))> _
        <XmlInclude(GetType(HttpHandlerAddInfo))> _
        <XmlInclude(GetType(WebServerAddInfo))> _
        <XmlInclude(GetType(CustomAddInfo))> _
    <XmlRoot("add")> _
    <Serializable()> _
    Public Class AddInfo
        Inherits AddInfoBase

    End Class

    ''' <summary>
    ''' Base Class for a add merge node
    ''' </summary>
    <XmlInclude(GetType(TrustAddInfo))> _
    <XmlInclude(GetType(CustomErrorAddInfo))> _
    <XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlInclude(GetType(AddInfo))> _
    <Serializable()> _
    Public Class AddInfoBase
        Implements IXmlSerializable

        Public Sub New()

        End Sub


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