Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.Services
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Linq
Imports System.Web.Compilation
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Entities
Imports DotNetNuke.UI.Skins.Controls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum DynamicHandlerType
        Typed
        UnTyped
    End Enum

    Public Enum AdatedPathMode
        SelectPath
        EnterPath
    End Enum

   

   


    Public Delegate Sub ControlEventHandler()

    <ActionButton(IconName.Clipboard, IconOptions.Normal)> _
    <Serializable()> _
    Public Class ControlAdapterSettings
        Inherits NamedConfig
        Implements IProviderContainer
        Implements IExpressionVarsProvider


        Public Overrides Function GetFriendlyDetails() As String
            Dim toReturn As String = ""
            Select Case Me.AdaptedMode
                Case AdaptedControlMode.Type
                    If Me.ResolvedAdaptedControlType IsNot Nothing Then
                        toReturn &= ResolvedAdaptedControlType.FullName
                    End If
                Case AdaptedControlMode.Path
                    toReturn &= Me.AdaptedControlPath
            End Select
            Return String.Format("{1} {0} {2}", UIConstants.TITLE_SEPERATOR, MyBase.GetFriendlyDetails(), toReturn)
        End Function

        '<Browsable(False)> _
        'Public ReadOnly Property FriendlyName As String
        '    Get
        '        If ResolvedAdaptedControlType IsNot Nothing Then
        '            Return ResolvedAdaptedControlType.Name
        '        End If
        '        Return "Select Adapted Control Type"
        '    End Get
        'End Property
        

        Public Property VersionRange As New EnabledFeature(Of Range(Of SerializableVersion))(New Range(Of SerializableVersion)(New SerializableVersion(New Version(4, 8, 1)), New SerializableVersion(NukeHelper.DnnVersion)))

        <Browsable(False)> _
        Public ReadOnly Property IsEnabled As Boolean
            Get
                Return Me.Enabled AndAlso ((Not Me.VersionRange.Enabled) OrElse Me.VersionRange.Entity.Validate(NukeHelper.DnnVersion))
            End Get
        End Property

        Public Property AdaptedMode As AdaptedControlMode

        <ConditionalVisible("AdaptedMode", False, True, AdaptedControlMode.Path)> _
        Public Property AdatedPathMode As AdatedPathMode

        <Required(True)> _
        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("AdatedPathMode", False, True, AdatedPathMode.SelectPath)> _
       <ConditionalVisible("AdaptedMode", False, True, AdaptedControlMode.Path)> _
       <Selector("Text", "Value", False, True, "<Select control path>", "", False, True)> _
        Public Property AdaptedSelectedPath As String
            Get
                Return AdaptedControlPath
            End Get
            Set(value As String)
                If AdaptedControlPath <> value Then
                    AdaptedControlPath = value
                    _ResolvedAdaptedType = Nothing
                End If
            End Set
        End Property

        <Required(True)> _
        <ConditionalVisible("AdatedPathMode", False, True, AdatedPathMode.EnterPath)> _
        <ConditionalVisible("AdaptedMode", False, True, AdaptedControlMode.Path)> _
        Public Property AdaptedControlPath As String = ""

        <ConditionalVisible("AdaptedMode", False, True, AdaptedControlMode.Type)> _
        Public Property AdaptedControlType As New DotNetType()


        Private _ResolvedAdaptedType As Type

        <Browsable(False)> _
        Public ReadOnly Property ResolvedAdaptedControlType As Type
            Get
                If _ResolvedAdaptedType Is Nothing Then
                    _ResolvedAdaptedType = GetResolvedAdaptedControlType()
                End If
                Return _ResolvedAdaptedType
            End Get
        End Property

        Public ReadOnly Property ResolvedAdaptedControlTypeName As String
            Get
                Dim toReturn As String = String.Empty
                Dim objType As Type = GetResolvedAdaptedControlType()
                If objType IsNot Nothing Then
                    toReturn = ReflectionHelper.GetSafeTypeName(objType)
                End If
                Return toReturn
            End Get
        End Property

        Public Property AdapterMode As AdapterControlMode

        <ConditionalVisible("AdapterMode", False, True, AdapterControlMode.Type)> _
        Public Property AdapterControlType As New DotNetType()

        Private _ResolvedAdapterType As Type

       <Browsable(False)> _
        Public ReadOnly Property ResolvedAdapterControlType As Type
            Get
                If _ResolvedAdapterType Is Nothing Then
                    _ResolvedAdapterType = GetResolvedAdapterControlType()
                End If
                Return _ResolvedAdapterType
            End Get
        End Property


        <ProvidersSelector("Text", "Value")> _
        <ConditionalVisible("AdapterMode", False, True, AdapterControlMode.DynamicAdapter)> _
        Public Property DynamicHandlers As New SerializableList(Of DynamicHandlerSettings)

        <ConditionalVisible("AdapterMode", False, True, AdapterControlMode.DynamicAdapter)> _
        Public Property GlobalParameters As New Variables()

        Private _DynamicHandlersDictionary As Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)

        <XmlIgnore()> _
       <Browsable(False)> _
        Public ReadOnly Property DynamicHandlersDictionary As Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)
            Get
                If _DynamicHandlersDictionary Is Nothing Then
                    Dim newDico As New Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)
                    For Each objSettings As DynamicHandlerSettings In Me.DynamicHandlers
                        newDico(objSettings.GetStep()) = objSettings
                    Next
                    Me._DynamicHandlersDictionary = newDico
                End If
                Return _DynamicHandlersDictionary
            End Get
        End Property


        Public ReadOnly Property ResolvedAdapterControlTypeName As String
            Get
                Dim toReturn As String = String.Empty
                Dim objType As Type = GetResolvedAdapterControlType()
                If objType IsNot Nothing Then
                    toReturn = ReflectionHelper.GetSafeTypeName(objType)
                End If
                Return toReturn
            End Get
        End Property



        <ActionButton(IconName.Anchor, IconOptions.Normal)> _
        Public Sub UpgradeDynamicHandlers(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            Dim genericDynamicHandlerType As Type = Me.GetGenericDynamicHandlerType()
            SyncLock Me
                Dim tempList As New SerializableList(Of DynamicHandlerSettings)

                For Each objDynamicHandler As DynamicHandlerSettings In Me.DynamicHandlers

                    Dim objHandlerType As Type = objDynamicHandler.GetType()

                    If objHandlerType Is GetType(DynamicHandlerSettings) Then
                        tempList.Add(DirectCast(Activator.CreateInstance(genericDynamicHandlerType, objDynamicHandler), DynamicHandlerSettings))
                    Else
                        tempList.Add(objDynamicHandler)
                    End If
                Next
                Me.DynamicHandlers = tempList
            End SyncLock
            ape.ItemChanged = True
            ape.DisplayLocalizedMessage("DynamicHandlersUpgraded.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ActionButton(IconName.Suitcase, IconOptions.Normal)> _
        Public Sub DowngradeDynamicHandlers(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            SyncLock Me
                Dim tempList As New SerializableList(Of DynamicHandlerSettings)

                For Each objDynamicHandler As DynamicHandlerSettings In Me.DynamicHandlers
                    Dim objHandlerType As Type = objDynamicHandler.GetType()
                    If objHandlerType IsNot GetType(DynamicHandlerSettings) Then
                        tempList.Add(objDynamicHandler.Downgrade())
                    Else
                        tempList.Add(objDynamicHandler)
                    End If
                Next
                Me.DynamicHandlers = tempList
            End SyncLock
            ape.ItemChanged = True
            ape.DisplayLocalizedMessage("DynamicHandlersDowngraded.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        Private Shared _ascxList As ListItemCollection

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection
            Select Case propertyName
                Case "DynamicHandlers"
                    toReturn.Add(New ListItem("Typed Handler", "Typed"))
                    toReturn.Add(New ListItem("Portable Handler", "Portable"))

                Case "AdaptedSelectedPath"
                    If _ascxList Is Nothing Then
                        Dim objFiles As IEnumerable(Of String) = Aricie.DNN.Services.FileHelper.LoadFiles("*.as?x")
                        Dim fileList As List(Of String) = objFiles.Where(Function(file) file.ToLower().EndsWith("aspx") OrElse file.ToLower().EndsWith("ascx")).ToList()
                        fileList.Sort()
                        For Each objFile As String In fileList
                            toReturn.Add(Aricie.DNN.Services.FileHelper.GetPathFromMapPath(objFile))
                        Next
                        _ascxList = toReturn
                    Else
                        toReturn = _ascxList
                    End If
            End Select

            Return toReturn
        End Function


        Public Function GetNewItem(collectionPropertyName As String, providerName As String) As Object Implements IProviderContainer.GetNewItem
            If collectionPropertyName = "DynamicHandlers" Then
                If providerName = "Portable" Then
                    Return New DynamicHandlerSettings()
                Else
                    Return ReflectionHelper.CreateObject(GetGenericDynamicHandlerType())
                End If
            End If
            Return Nothing
        End Function

        'Private _GenericDynamicHandlerType As Type

        Public Function GetGenericDynamicHandlerType() As Type
            'If _GenericDynamicHandlerType Is Nothing Then
            Return GetType(DynamicHandlerSettings(Of )).MakeGenericType(Me.ResolvedAdaptedControlType)
            'End If
            'Return _GenericDynamicHandlerType
        End Function

        Private Function GetResolvedAdaptedControlType() As Type
            Try
                Select Case Me.AdaptedMode
                    Case AdaptedControlMode.Type
                        Return Me.AdaptedControlType.GetDotNetType()
                    Case AdaptedControlMode.Path
                        If Not String.IsNullOrEmpty(Me.AdaptedControlPath) AndAlso DnnContext.Current.Page IsNot Nothing Then
                            'Dim tempUserControl As Control = DnnContext.Current.Page.LoadControl(Me.AdaptedControlPath)
                            'Return tempUserControl.GetType()
                            Return BuildManager.GetCompiledType(Me.AdaptedControlPath)
                        End If
                End Select
            Catch ex As Exception
                ExceptionHelper.LogException(ex)
            End Try
            Return Nothing
        End Function

        Private Function GetResolvedAdapterControlType() As Type
            Try
                Select Case Me.AdapterMode
                    Case AdapterControlMode.Type
                        Return Me.AdapterControlType.GetDotNetType()
                    Case AdapterControlMode.DynamicAdapter
                        Return DynamicControlAdapter.GetGenericType(Me.GetResolvedAdaptedControlType())
                End Select
            Catch ex As Exception
                ExceptionHelper.LogException(ex)
            End Try
            Return Nothing
        End Function


        Public Sub ProcessStep(adapter As DynamicControlAdapter, parameters As IDictionary(Of String, Object), ByVal baseHandler As ControlEventHandler, ByVal newStep As DynamicHandlerStep)
            'Dim keeperContext As PortalKeeperContext(Of SimpleEngineEvent) = PortalKeeperContext(Of SimpleEngineEvent).Instance(HttpContext.Current)

            'If Not keeperContext.Disabled Then
            Dim dynamicHandler As DynamicHandlerSettings = Nothing
            If (Not DynamicHandlersDictionary.TryGetValue(newStep, dynamicHandler) OrElse dynamicHandler.BaseHandlerMode = ControlBaseHandlerMode.Before) AndAlso baseHandler IsNot Nothing Then
                baseHandler.Invoke()
            End If
            If dynamicHandler IsNot Nothing AndAlso dynamicHandler.Enabled Then
                If (Not adapter.GeneralAdaptedControl.Page.IsPostBack AndAlso Not dynamicHandler.NotOnFirstLoad) OrElse (adapter.GeneralAdaptedControl.Page.IsPostBack AndAlso Not dynamicHandler.NotOnPostBacks) Then
                    parameters("Adapter") = adapter
                    parameters("HandlerStep") = newStep
                    Dim keeperContext As PortalKeeperContext(Of SimpleEngineEvent) = dynamicHandler.InitContext()
                    keeperContext.InitParams(Me.GlobalParameters)
                    keeperContext.InitParams(parameters)
                    dynamicHandler.ProcessRules(keeperContext, SimpleEngineEvent.Run, True, True)
                End If
                If baseHandler IsNot Nothing AndAlso dynamicHandler.BaseHandlerMode = ControlBaseHandlerMode.After Then
                    baseHandler.Invoke()
                End If
            End If
            'End If
        End Sub





        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            existingVars.Add("Adapter", GetResolvedAdapterControlType())
        End Sub
    End Class

End Namespace