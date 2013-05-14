Imports System.Xml.Serialization

Namespace Configuration

    '<XmlInclude(GetType(CustomErrorAddInfo))> _
    '    <XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlRoot("node")> _
    <Serializable()> _
    Public Class SimpleNodeInfo
        Inherits NodeInfo

        '<node path="/configuration/system.web/httpModules" action="update" Name="name" Value="overwrite">
        '     <add name="Compression" type="DotNetNuke.HttpModules.Compression.CompressionModule, DotNetNuke.HttpModules" />
        '</node>

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal name As String, ByVal value As String)
            MyBase.New(path, action)
            Me._Name = name
            Me._Value = value
        End Sub


        Private _Name As String = ""
        Private _Value As String = ""



        <XmlAttribute("name")> _
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property


        <XmlAttribute("value")> _
        Public Property Value() As String
            Get
                Return _Value
            End Get
            Set(ByVal value As String)
                _Value = value
            End Set
        End Property




    End Class
End Namespace