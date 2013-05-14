Imports System.Xml.Serialization

Namespace Configuration

    ''' <summary>
    ''' HttpHandler Add merge node
    ''' </summary>
    ''' <XmlInclude(GetType(CustomErrorAddInfo))> _
    '''   <XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlRoot("add")> _
    <Serializable()> _
    Public Class HttpHandlerAddInfo
        Inherits WebServerAddInfo

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal name As String, ByVal type As Type, ByVal path As String, ByVal verb As String)
            Me.New(name, type, path, verb, "")
            Me.Attributes.Remove("name")
        End Sub

        Public Sub New(ByVal name As String, ByVal type As Type, ByVal path As String, ByVal verb As String, ByVal preCondition As String)
            MyBase.New(name, type, preCondition)
            Me.Attributes("path") = path
            Me.Attributes("verb") = verb
        End Sub

    End Class
End Namespace