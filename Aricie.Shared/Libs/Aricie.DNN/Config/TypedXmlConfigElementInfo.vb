Imports System.Xml
Imports System.ComponentModel

Namespace Configuration

    ''' <summary>
    ''' Base class for configuration elements with a type attribute
    ''' </summary>
    <Serializable()> _
    Public MustInherit Class TypedXmlConfigElementInfo
        Inherits XmlNamedConfigElementInfo


        Private _Type As Type


        Public Sub New(ByVal name As String, ByVal objType As Type)
            MyBase.New(name)
            Me._Type = objType
        End Sub


        <Browsable(False)> _
        Public Property Type() As Type
            Get
                Return _Type
            End Get
            Set(ByVal value As Type)
                _Type = value
            End Set
        End Property

        Public MustOverride Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean

        Public MustOverride Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)

    End Class


End Namespace


