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

       
        <XmlElement("urlCompression", GetType(UrlCompressionAddInfo))> _
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