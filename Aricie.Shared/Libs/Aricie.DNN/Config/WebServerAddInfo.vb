Imports System.Xml.Serialization
Imports Aricie.Services

Namespace Configuration

    ''' <summary>
    ''' Base Webserver Add merge node
    ''' </summary>
    <XmlInclude(GetType(HttpModuleAddInfo))> _
        <XmlInclude(GetType(HttpHandlerAddInfo))> _
        <XmlRoot("add")> _
        <Serializable()> _
    Public Class WebServerAddInfo
        Inherits AddInfo

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal name As String, ByVal type As Type, Optional ByVal preCondition As String = "")
            Me.Attributes("name") = name
            Me.Attributes("type") = ReflectionHelper.GetSafeTypeName(type)
            If preCondition <> "" Then
                Me.Attributes("preCondition") = preCondition
            End If
        End Sub

    End Class
End Namespace