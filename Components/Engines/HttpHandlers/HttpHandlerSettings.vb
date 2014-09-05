Imports Aricie.DNN.Configuration
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.Xml.Serialization

Namespace Aricie.DNN.Modules.PortalKeeper
    <DefaultProperty("FriendlyName")> _
    <Serializable()> _
    Public Class HttpHandlerSettings
        Inherits HttpHandlerInfo

        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                If HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler Then
                    Return Me.DynamicHandler.Name
                Else
                    Return Me.Name
                End If
            End Get
        End Property

        Public Property HttpHandlerMode As HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler

        <ConditionalVisible("HttpHandlerMode", False, True, HttpHandlerMode.DynamicHandler)> _
        Public Property DynamicHandler As New SimpleRuleEngine()

        <ConditionalVisible("HttpHandlerMode", False, True, HttpHandlerMode.Type)> _
        Public Property HttpHandlerType As New DotNetType()

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Overrides Property Type As Type
            Get
                Select Case HttpHandlerMode
                    Case PortalKeeper.HttpHandlerMode.DynamicHandler
                        Return GetType(DynamicHttpHandler)
                    Case PortalKeeper.HttpHandlerMode.Type
                        Return HttpHandlerType.GetDotNetType()
                End Select
                Return Nothing
            End Get
            Set(value As Type)
                'dot nothing
            End Set
        End Property


    End Class
End NameSpace