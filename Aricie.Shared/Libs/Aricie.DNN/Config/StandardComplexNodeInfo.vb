Imports System.Xml.Serialization

Namespace Configuration


    '<XmlRoot("node", Namespace:="StandardComplexNodeInfo")> _
    '<XmlRoot("node")> _
    <Serializable()> _
    Public Class StandardComplexNodeInfo
        Inherits ComplexNodeInfo

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal key As String, ByVal collision As NodeCollision)
            MyBase.New(path, action, key, collision)
        End Sub

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal collision As NodeCollision, ByVal targetPath As String)
            MyBase.New(path, action, collision, targetPath)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        '<XmlArrayItemAttribute("add", GetType(AddInfo))> _
        '<XmlArrayItemAttribute("trust", GetType(TrustAddInfo))> _
        '<XmlArrayItemAttribute("customErrors", GetType(CustomErrorsAddInfo))> _
        '<XmlArrayItemAttribute("error", GetType(CustomErrorAddInfo))> _
        '<XmlArrayAttribute()> _
        '<XmlArrayItemAttribute("add", GetType(ProviderAddInfo))> _
        '<XmlArrayItemAttribute("add", GetType(AppSettingAddInfo))> _
        '<XmlArrayItemAttribute("add", GetType(HttpModuleAddInfo))> _
        '<XmlArrayItemAttribute("add", GetType(HttpHandlerAddInfo))> _
        '<XmlArrayItemAttribute("add", GetType(WebServerAddInfo))> _
        '<XmlArrayItemAttribute("add", GetType(CustomAddInfo))> _
        '<XmlElement("add")> _
        '<XmlChoiceIdentifier("ChildrenTypes")> _
        '<XmlElement("add", GetType(ProviderAddInfo))> _
        '<XmlElement("add", GetType(AppSettingAddInfo))> _
        '<XmlElement("add", GetType(HttpModuleAddInfo))> _
        '<XmlElement("add", GetType(HttpHandlerAddInfo))> _
        '<XmlElement("add", GetType(WebServerAddInfo))> _
        '<XmlElement("add", GetType(CustomAddInfo))> _
        '<XmlElement("add", GetType(AddInfo))> _
        <XmlElement("customErrors", GetType(CustomErrorsAddInfo))> _
        <XmlElement("error", GetType(CustomErrorAddInfo))> _
        <XmlElement("trust", GetType(TrustAddInfo))> _
        <XmlElement("add", GetType(AddInfo))> _
        Public Property Children As New AddNodes()



    End Class


    Public Class AddNodes
        Inherits List(Of AddInfoBase)

    End Class


End Namespace