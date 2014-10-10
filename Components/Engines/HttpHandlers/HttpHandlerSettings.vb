Imports Aricie.DNN.Configuration
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.Entities
Imports Aricie.Collections
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.UI.Skins.Controls

Namespace Aricie.DNN.Modules.PortalKeeper

    <Serializable()> _
    Public Class HttpHandlerSettings
        Inherits HttpHandlerInfo

        <Browsable(False)> _
        Public Overrides ReadOnly Property FriendlyName As String
            Get
                If HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler Then
                    Return String.Format("{1}{0}{2}{0}{3}{0}{4}", UIConstants.TITLE_SEPERATOR, Me.DynamicHandler.Name, Me.FriendlyPathsAndVerbs, _
                                         IIf(Me.DynamicHandler.Enabled, "enabled", "disabled"), IIf(Me.IsInstalled, "registered", "unregistered"))
                Else
                    Return MyBase.FriendlyName
                End If
            End Get
        End Property




        <ExtendedCategory("MainHandler")> _
        Public Property HttpHandlerMode As HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler

        <ExtendedCategory("MainHandler")> _
        <ConditionalVisible("HttpHandlerMode", False, True, HttpHandlerMode.DynamicHandler)> _
        Public Property DynamicHandler As New SimpleRuleEngine()







        <ExtendedCategory("MainHandler")> _
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




        '<CollectionEditor(False, True, False, True, 20, CollectionDisplayStyle.Accordion, True, 0, "Key")> _
        '<ConditionalVisible("HttpHandlerMode", False, True, HttpHandlerMode.DynamicHandler)> _
        <ExtendedCategory("SubHandlers")> _
        Public Property SubHandlers As New HttpSubHandlersConfig



        Public Sub ProcessRequest(ByVal keeperContext As PortalKeeperContext(Of SimpleEngineEvent))
            If Not keeperContext.Disabled Then
                Dim objSubHandler As HttpSubHandlerSettings = Nothing
                If Me.SubHandlers.Enabled Then
                    objSubHandler = TryCast(Me.SubHandlers.MapDynamicHandler(keeperContext.DnnContext.HttpContext), HttpSubHandlerSettings)

                End If
                If objSubHandler Is Nothing OrElse objSubHandler.RunMainHandler Then
                    RunMainHandler(keeperContext)
                Else
                    If Me.HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler AndAlso objSubHandler.InitParamsToSubHandler Then
                        keeperContext.Init(Me.DynamicHandler)
                        keeperContext.SetVar("ParentHandler", Me)
                    End If
                End If
                If objSubHandler IsNot Nothing Then
                    objSubHandler.ProcessRequest(keeperContext)
                End If
            End If
        End Sub


        Private Sub RunMainHandler(keeperContext As PortalKeeperContext(Of SimpleEngineEvent))
            Select Case Me.HttpHandlerMode
                Case PortalKeeper.HttpHandlerMode.Type
                    Dim targetHandler As IHttpHandler = DirectCast(ReflectionHelper.CreateObject(Me.HttpHandlerType.GetDotNetType()), IHttpHandler)
                    targetHandler.ProcessRequest(keeperContext.DnnContext.HttpContext)
                Case PortalKeeper.HttpHandlerMode.DynamicHandler
                    If Me.DynamicHandler.Enabled Then
                        keeperContext.Init(Me.DynamicHandler)
                        keeperContext.SetVar("CurrentHandler", Me)
                        Me.DynamicHandler.ProcessRules(keeperContext, SimpleEngineEvent.Run, True)
                    End If
            End Select
        End Sub

        '<Browsable(True)> _
        'Public Overrides Sub Install(pe As AriciePropertyEditorControl)
        '    MyBase.Install(pe)
        'End Sub

        '<Browsable(True)> _
        'Public Overrides Sub Update(pe As AriciePropertyEditorControl)
        '    MyBase.Update(pe)
        'End Sub


        <ExtendedCategory("SubHandlers")> _
        <ActionButton(IconName.FloppyO, IconOptions.Normal)> _
        Public Overridable Overloads Sub AddTestFiddle(pe As AriciePropertyEditorControl)
            If pe.IsValid Then
                Dim fiddleHandler As SerializableList(Of HttpSubHandlerSettings) = ReflectionHelper.CloneObject(Of SerializableList(Of HttpSubHandlerSettings))(PortalKeeperConfig.Instance.HttpHandlers.DefaultFiddle)
                Me.SubHandlers.Handlers.AddRange(fiddleHandler.ToArray())
                pe.ItemChanged = True
                pe.DisplayLocalizedMessage("TestFiddleAdded.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
            Else
                pe.DisplayLocalizedMessage("InvalidConfigElement.Message", ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub



    End Class
End Namespace