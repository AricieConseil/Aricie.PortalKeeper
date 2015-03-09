Imports System.Xml.Serialization

Namespace Configuration

    '<XmlRoot("add")> _
    ''' <summary>
    ''' HttpModule Add merge node
    ''' </summary>
    <Serializable()> _
    Public Class HttpModuleAddInfo
        Inherits WebServerAddInfo

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal name As String, ByVal type As Type, Optional ByVal preCondition As String = "")
            MyBase.New(name, type, preCondition)
        End Sub

    End Class

End Namespace