Imports Aricie.DNN.Configuration
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.Collections
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.Skins.Controls
Imports System.Linq

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.CloudDownload, IconOptions.Normal)> _
    <Serializable()> _
    Public Class HttpHandlerSettings
        Inherits HttpHandlerInfo

        <Browsable(False)> _
        Public Overrides ReadOnly Property FriendlyName As String
            Get
                Dim handlerName As String = String.Empty
                If HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler Then
                    handlerName = Me.DynamicHandler.Name
                Else
                    handlerName = Me.StaticHandlerName.Name
                End If
                Return String.Format("{1}{0}{2}{0}{3}{0}{4}", UIConstants.TITLE_SEPERATOR, handlerName, Me.FriendlyPathsAndVerbs, _
                                         IIf(Me.DynamicHandler.Enabled, "enabled", "disabled"), IIf(Me.IsInstalled, "registered", "unregistered"))
            End Get
        End Property




        <ExtendedCategory("MainHandler")> _
        Public Property HttpHandlerMode As HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler

        <ExtendedCategory("MainHandler")> _
        <ConditionalVisible("HttpHandlerMode", False, True, HttpHandlerMode.DynamicHandler)> _
        Public Property DynamicHandler As New SimpleRuleEngine()

        <ExtendedCategory("MainHandler")> _
        <ConditionalVisible("HttpHandlerMode", True, True, HttpHandlerMode.DynamicHandler)> _
        Public Property StaticHandlerName As New NamedEntity()




        <ExtendedCategory("MainHandler")> _
        <ConditionalVisible("HttpHandlerMode", False, True, HttpHandlerMode.Type)> _
        Public Property HttpHandlerType As New DotNetType()

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Overrides Property Type As Type
            Get
                Select Case HttpHandlerMode
                    Case PortalKeeper.HttpHandlerMode.DynamicHandler, PortalKeeper.HttpHandlerMode.Node
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


        Public Sub ProcessRequest(ByVal context As HttpContext)
            ProcessRequest(context, New List(Of SimpleRuleEngine))
        End Sub

        Public Sub ProcessRequest(ByVal context As HttpContext, parentHandlers As List(Of SimpleRuleEngine))

            Dim objSubHandler As HttpSubHandlerSettings = Nothing
            If Me.SubHandlers.Enabled Then
                objSubHandler = TryCast(Me.SubHandlers.MapDynamicHandler(context), HttpSubHandlerSettings)

            End If
            If objSubHandler Is Nothing OrElse objSubHandler.RunMainHandler Then
                RunMainHandler(context, parentHandlers)
            Else
                If Me.HttpHandlerMode = PortalKeeper.HttpHandlerMode.DynamicHandler AndAlso objSubHandler.InitParamsToSubHandler Then
                    parentHandlers.Add(Me.DynamicHandler)
                    'keeperContext.Init(Me.DynamicHandler)
                    'keeperContext.SetVar("ParentHandler", Me)
                End If
            End If
            If objSubHandler IsNot Nothing Then
                objSubHandler.ProcessRequest(context, parentHandlers)
            End If
        End Sub


        Private Sub RunMainHandler(context As HttpContext, parentHandlers As List(Of SimpleRuleEngine))
            Select Case Me.HttpHandlerMode
                Case PortalKeeper.HttpHandlerMode.Type
                    Dim targetHandler As IHttpHandler = DirectCast(ReflectionHelper.CreateObject(Me.HttpHandlerType.GetDotNetType()), IHttpHandler)
                    targetHandler.ProcessRequest(context)
                Case PortalKeeper.HttpHandlerMode.DynamicHandler
                    If Me.DynamicHandler.Enabled Then
                        Dim keeperContext As PortalKeeperContext(Of SimpleEngineEvent)
                        If parentHandlers.Count > 0 Then
                            keeperContext = New PortalKeeperContext(Of SimpleEngineEvent)
                            Dim key As String = "ParentHandler"
                            For Each objHandler As SimpleRuleEngine In Enumerable.Reverse(parentHandlers)
                                keeperContext.Init(objHandler)
                                keeperContext.SetVar(key, Me)
                                key = "Parent" & key
                            Next
                            keeperContext = Me.DynamicHandler.InitContext(keeperContext.Items)
                        Else
                            keeperContext = Me.DynamicHandler.InitContext()
                        End If

                        keeperContext.SetVar("CurrentHandler", Me)

                        Me.DynamicHandler.ProcessRules(keeperContext, SimpleEngineEvent.Run, True, True)
                        'keeperContext.LogEndEngine()
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