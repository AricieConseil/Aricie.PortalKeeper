Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.ComponentModel
Imports Aricie.DNN.Configuration
Imports Aricie.DNN.Services.Errors
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class ApplicationSettings
        Inherits RuleEngineSettings(Of ApplicationEvent)

        Private _UrlCompression As UrlCompressionInfo

        Private _CustomErrorsConfig As VirtualCustomErrorsInfo

        <SortOrder(0)> _
        <ExtendedCategory("Rules")> _
        Public Property EnableCriticalChangesHandler As Boolean

        <ExtendedCategory("Compression")> _
        Public Property UrlCompression As UrlCompressionInfo
            Get
                If _UrlCompression Is Nothing Then
                     _UrlCompression = New UrlCompressionInfo()
                End If
                Return _UrlCompression
            End Get
            Set(value As UrlCompressionInfo)
                _UrlCompression = value
            End Set
        End Property

        <ExtendedCategory("CustomErrors")> _
        Public Property CustomErrorsConfig() As VirtualCustomErrorsInfo
            Get
                If _CustomErrorsConfig Is Nothing Then
                    _CustomErrorsConfig = New VirtualCustomErrorsInfo()
                End If
                Return _CustomErrorsConfig
            End Get
            Set(value As VirtualCustomErrorsInfo)
                _CustomErrorsConfig = value
            End Set
        End Property

        <XmlIgnore()> _
       <Browsable(False)> _
        Public Overrides Property Mode As RuleEngineMode
            Get
                Return RuleEngineMode.Rules
            End Get
            Set(value As RuleEngineMode)
                'do nothing
            End Set
        End Property

        <XmlIgnore()> _
       <Browsable(False)> _
        Public Overrides Property Actions As KeeperAction(Of ApplicationEvent)
            Get
                Return Nothing
            End Get
            Set(value As KeeperAction(Of ApplicationEvent))
                'do nothing
            End Set
        End Property

        <Browsable(False)> _
       <XmlIgnore()> _
        Public Overrides Property Name As String
            Get
                Return "Application"
            End Get
            Set(value As String)
            End Set
        End Property

        <Browsable(False)> _
         <XmlIgnore()> _
        Public Overrides Property Decription As CData
            Get
                Return MyBase.Decription
            End Get
            Set(value As CData)
                MyBase.Decription = value
            End Set
        End Property

    End Class
End Namespace