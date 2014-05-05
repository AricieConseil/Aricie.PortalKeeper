Imports System.Web.UI.Adapters
Imports Aricie.Collections
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services
Imports Aricie.Services
Imports System.Reflection
Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum ControlStep
        OnInit = -2
        CreateChildControls = -1
        OnLoad = 0
        ControlEvent = 1
        ChildControlEvent = 2
        OnPreRender = 3
        Render = 4
        RenderChildren = 5
        OnUnload = 6
    End Enum

    <Serializable()> _
    Public Class ControlAdaptersConfig
        Implements IEnabled

        Public Property Enabled As Boolean Implements IEnabled.Enabled

        Public Property Adapters As New List(Of ControlAdapterSettings)

        Private _AdaptersDictionary As Dictionary(Of Type, ControlAdapterSettings)

        Public ReadOnly Property AdaptersDictionary As Dictionary(Of Type, ControlAdapterSettings)
            Get
                If _AdaptersDictionary Is Nothing Then
                    Dim newDico As New Dictionary(Of Type, ControlAdapterSettings)
                    For Each objSettings As ControlAdapterSettings In Adapters
                        Dim objControlType As Type = objSettings.ResolvedAdaptedControlType
                        If objControlType IsNot Nothing Then
                            newDico(objControlType) = objSettings
                        End If
                    Next
                    _AdaptersDictionary = newDico
                End If
                Return _AdaptersDictionary
            End Get
        End Property

        Public Sub RegisterAdapters()
            If Me.Enabled Then
                Dim browser As HttpBrowserCapabilities = HttpContext.Current.Request.Browser
                For Each objTypeAdapter As KeyValuePair(Of Type, ControlAdapterSettings) In Me.AdaptersDictionary
                    If Not browser.Adapters.Contains(objTypeAdapter.Key.AssemblyQualifiedName) Then
                        browser.Adapters.Add(objTypeAdapter.Key.AssemblyQualifiedName, objTypeAdapter.Value.ResolvedAdapterControlType.AssemblyQualifiedName)
                    End If
                Next
            End If
        End Sub

    End Class

    Public Enum ControlBaseHandlerMode
        Before = -1
        Supressed = 0
        After = 1
    End Enum


    Public Enum AdaptedControlMode
        Type
        Path
    End Enum

    Public Enum AdapterControlMode
        Type
        DynamicAdapter
    End Enum

    <Serializable()> _
    Public Class ControlAdapterSettings
        Implements IProviderContainer

        Public Property AdaptedMode As AdaptedControlMode

        <ConditionalVisible("AdaptedControlMode", False, True, AdaptedControlMode.Path)> _
        Public Property AdaptedControlPath As String

        <ConditionalVisible("AdaptedControlMode", False, True, AdaptedControlMode.Type)> _
        Public Property AdaptedControlType As New DotNetType()


        Private _ResolvedAdaptedType As Type
        Public ReadOnly Property ResolvedAdaptedControlType As Type
            Get
                If _ResolvedAdaptedType Is Nothing Then
                    Try
                        Select Case Me.AdaptedMode
                            Case AdaptedControlMode.Type
                                _ResolvedAdaptedType = Me.AdaptedControlType.GetDotNetType()
                            Case AdaptedControlMode.Path
                                If DnnContext.Current.Page IsNot Nothing Then
                                    Dim tempUserControl As Control = DnnContext.Current.Page.LoadControl(Me.AdaptedControlPath)
                                    _ResolvedAdaptedType = tempUserControl.GetType()
                                End If
                        End Select
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
                Return _ResolvedAdaptedType
            End Get
        End Property


        Private _ResolvedAdapterType As Type
        Public ReadOnly Property ResolvedAdapterControlType As Type
            Get
                If _ResolvedAdapterType Is Nothing Then
                    Try
                        Select Case Me.AdapterMode
                            Case AdapterControlMode.Type
                                _ResolvedAdapterType = Me.AdapterControlType.GetDotNetType()
                            Case AdapterControlMode.DynamicAdapter
                                _ResolvedAdapterType = GetType(DynamicControlAdapter)
                        End Select
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
                Return _ResolvedAdapterType
            End Get
        End Property


        Public Property AdapterMode As AdapterControlMode

        <ConditionalVisible("AdapterControlMode", False, True, AdapterControlMode.Type)> _
        Public Property AdapterControlType As New DotNetType()


        Public Property DynamicHandlers As New List(Of DynamicHandlerSettings)


        Private _DynamicHandlersDictionary As Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)
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

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector

        End Function


        Public Function GetNewItem(collectionPropertyName As String, providerName As String) As Object Implements IProviderContainer.GetNewItem

        End Function
    End Class

    <Serializable()> _
    Public Class DynamicHandlerSettings

        Public Property ControlStep As ControlStep

        <ConditionalVisible("ControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)> _
        Public Property ControlEvent As String

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Property EventInfo As EventInfo

        Public Sub DynamicEventHandler(controlAdapter As DynamicControlAdapter, sender As Object, e As EventArgs)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("sender", sender)
            parameters.Add("e", e)
            controlAdapter.ProcessStep(parameters, Nothing, Me.GetStep(), False)
        End Sub

        <ConditionalVisible("ControlStep", False, True, ControlStep.ChildControlEvent)> _
        Public Property ChildControlId As String

        Public Property BaseHandlerMode As ControlBaseHandlerMode = ControlBaseHandlerMode.Before

        Public Property Engine As New SimpleRuleEngine()

        Public Function GetStep() As DynamicHandlerStep
            Return New DynamicHandlerStep(Me.ControlStep, Me.ChildControlId, Me.ControlEvent)
        End Function


    End Class





    <Serializable()> _
    Public Structure DynamicHandlerStep
        Implements IConvertible

        Sub New(objControlStep As ControlStep)
            Me.New(objControlStep, "", "")
        End Sub

        Sub New(objControlStep As ControlStep, strEvent As String)
            Me.New(objControlStep, "", strEvent)
        End Sub

        Sub New(objControlStep As ControlStep, strControl As String, strEvent As String)
            Me.ControlStep = objControlStep
            Me.ChildControlId = strControl
            Me.ControlEvent = strEvent
        End Sub

        Public Property ControlStep As ControlStep

        Public Property ChildControlId As String
        Public Property ControlEvent As String

        Public Overrides Function GetHashCode() As Integer
            Return ControlStep.GetHashCode() Xor ChildControlId.GetHashCode() Xor ControlEvent.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is DynamicHandlerStep Then
                Dim objDynamicHandlerStep As DynamicHandlerStep = DirectCast(obj, DynamicHandlerStep)
                Return ((Me.ControlStep = objDynamicHandlerStep.ControlStep) AndAlso (Me.ChildControlId = objDynamicHandlerStep.ChildControlId) AndAlso (Me.ControlEvent = objDynamicHandlerStep.ControlEvent))
            End If
            Return False
        End Function

        Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
            Return String.Empty.GetTypeCode()
        End Function

        Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
            Return Me.ControlEvent <> String.Empty
        End Function

        Private Const NotImplementedMessage As String = "IConvertible is only supported in DynamicHandlerStep for strings and int32"

        Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToDateTime(provider As IFormatProvider) As Date Implements IConvertible.ToDateTime
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
            Return GetHashCode()
        End Function

        Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
            Return GetHashCode()
        End Function

        Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
            Return GetHashCode()
        End Function

        Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
            Return GetHashCode()
        End Function

        Public Overloads Function ToString(provider As IFormatProvider) As String Implements IConvertible.ToString
            Return String.Format("{0};{1};{2})", Me.ControlStep.ToString(), Me.ChildControlId.ToString(provider), Me.ControlEvent.ToString(provider))
        End Function

        Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function
    End Structure

    Public Class DynamicControlAdapter '(Of T As Control)
        Inherits ControlAdapter 'Base(Of T)

        Private _Settings As ControlAdapterSettings

        Public ReadOnly Property Settings As ControlAdapterSettings
            Get
                If _Settings Is Nothing Then
                    If Not PortalKeeperConfig.Instance.ControlAdapters.AdaptersDictionary.TryGetValue(Me.Control.GetType(), _Settings) Then
                        Throw New ApplicationException(String.Format("Dynamic COntrol Adapter for control {0} was not found in the global confifuration", Me.Control.GetType.AssemblyQualifiedName))
                    End If
                End If
                Return _Settings
            End Get
        End Property



        Public Delegate Sub ControlEventHandler()


       

        Protected Overrides Sub OnInit(e As EventArgs)

            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("e", e)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.OnInit(e)
                                                               End Sub), New DynamicHandlerStep(ControlStep.OnInit), False)
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim parameters As New SerializableDictionary(Of String, Object)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.CreateChildControls()
                                                               End Sub), New DynamicHandlerStep(ControlStep.CreateChildControls), False)

        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("e", e)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.OnLoad(e)
                                                               End Sub), New DynamicHandlerStep(ControlStep.OnLoad), False)
            For Each objDynamicHandler As DynamicHandlerSettings In Me.Settings.DynamicHandlers
                If Not String.IsNullOrEmpty(objDynamicHandler.ControlEvent) Then
                    Dim ctrl As Control = Me.Control
                    If Not String.IsNullOrEmpty(objDynamicHandler.ChildControlId) Then
                        ctrl = ctrl.FindControl(objDynamicHandler.ChildControlId)
                    End If
                    If ctrl IsNot Nothing Then
                        If objDynamicHandler.EventInfo Is Nothing Then
                            objDynamicHandler.EventInfo = ctrl.GetType().GetEvent(objDynamicHandler.ControlEvent)
                        End If
                        If objDynamicHandler.EventInfo IsNot Nothing Then
                            If objDynamicHandler.EventInfo.EventHandlerType.IsAssignableFrom(GetType(EventHandler)) Then
                                Dim closureHandler As DynamicHandlerSettings = objDynamicHandler

                                objDynamicHandler.EventInfo.AddEventHandler(ctrl, Sub(sender As Object, ea As EventArgs)
                                                                                      closureHandler.DynamicEventHandler(Me, sender, ea)
                                                                                  End Sub)
                            End If
                        Else
                            Throw New ApplicationException(String.Format("Control Event was not found for dynamic handler {0}", objDynamicHandler.GetStep().ToString()))
                        End If
                    Else
                        Throw New ApplicationException(String.Format("Control was not found for dynamic handler {0}", objDynamicHandler.GetStep().ToString()))
                    End If
                End If
            Next
        End Sub

        Protected Overrides Sub OnPreRender(e As EventArgs)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("e", e)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.OnPreRender(e)
                                                               End Sub), New DynamicHandlerStep(ControlStep.OnInit), False)

        End Sub

        Protected Overrides Sub Render(writer As HtmlTextWriter)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("writer", writer)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.Render(writer)
                                                               End Sub), New DynamicHandlerStep(ControlStep.Render), False)

        End Sub

        Protected Overrides Sub RenderChildren(writer As HtmlTextWriter)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("writer", writer)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.RenderChildren(writer)
                                                               End Sub), New DynamicHandlerStep(ControlStep.RenderChildren), False)
        End Sub


        Protected Overrides Sub OnUnload(e As EventArgs)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("e", e)
            Me.ProcessStep(parameters, New ControlEventHandler(Sub()
                                                                   MyBase.OnUnload(e)
                                                               End Sub), New DynamicHandlerStep(ControlStep.OnUnload), True)
        End Sub

        Public Sub ProcessStep(parameters As IDictionary(Of String, Object), ByVal baseHandler As ControlEventHandler, ByVal newStep As DynamicHandlerStep, ByVal endSequence As Boolean)
            Dim keeperContext As PortalKeeperContext(Of Boolean) = PortalKeeperContext(Of Boolean).Instance(HttpContext.Current)
            If Not keeperContext.Disabled Then
                Dim dynamicHandler As DynamicHandlerSettings = Nothing
                If baseHandler IsNot Nothing AndAlso Not Settings.DynamicHandlersDictionary.TryGetValue(newStep, dynamicHandler) OrElse dynamicHandler.BaseHandlerMode = ControlBaseHandlerMode.Before Then
                    baseHandler.Invoke()
                End If
                If dynamicHandler IsNot Nothing Then
                    keeperContext.Init(dynamicHandler.Engine, parameters)
                    dynamicHandler.Engine.ProcessRules(keeperContext, True, endSequence)
                    If baseHandler IsNot Nothing AndAlso dynamicHandler.BaseHandlerMode = ControlBaseHandlerMode.After Then
                        baseHandler.Invoke()
                    End If
                End If

            End If
        End Sub


    End Class


End Namespace


