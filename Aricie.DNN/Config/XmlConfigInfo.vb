Imports System.Xml.Serialization


Namespace Configuration




    ' <XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlRoot("configuration")> _
    <Serializable()> _
    Public Class XmlConfigInfo
        'Inherits List(Of NodesInfo)

        Private _NodesList As New List(Of NodesInfo)


        <XmlElement("nodes")> _
        Public Property NodesList() As List(Of NodesInfo)
            Get
                Return _NodesList
            End Get
            Set(ByVal value As List(Of NodesInfo))
                _NodesList = value
            End Set
        End Property


    End Class
End Namespace
