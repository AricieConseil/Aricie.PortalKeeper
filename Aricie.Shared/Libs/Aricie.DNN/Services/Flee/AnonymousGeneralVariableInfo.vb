Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.ComponentModel
Imports Newtonsoft.Json

Namespace Services.Flee
    Public Class AnonymousGeneralVariableInfo
        Inherits GeneralVariableInfo

        <JsonIgnore()> _
        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <JsonIgnore()> _
        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Decription As CData
            Get
                Return MyBase.Decription
            End Get
            Set(value As CData)
                MyBase.Decription = value
            End Set
        End Property

    End Class

        Public Class AnonymousGeneralVariableInfo(Of T)
        Inherits GeneralVariableInfo(Of T)

        <JsonIgnore()> _
        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <JsonIgnore()> _
        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Decription As CData
            Get
                Return MyBase.Decription
            End Get
            Set(value As CData)
                MyBase.Decription = value
            End Set
        End Property

    End Class

End NameSpace