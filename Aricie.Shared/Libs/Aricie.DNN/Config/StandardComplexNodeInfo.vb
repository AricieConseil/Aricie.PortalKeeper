Imports System.Xml.Serialization

Namespace Configuration

    <XmlRoot("node")> _
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

        <XmlElement("trust", GetType(TrustAddInfo))> _
        <XmlElement("customErrors", GetType(CustomErrorsAddInfo))> _
        <XmlElement("error", GetType(CustomErrorAddInfo))> _
        <XmlElement("add", GetType(AddInfo))> _
        Public Property Children As New List(Of AddInfo)
           
    End Class
End Namespace