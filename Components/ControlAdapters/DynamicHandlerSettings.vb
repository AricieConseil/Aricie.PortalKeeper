Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Reflection
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper

    <Serializable()> _
    Public Class DynamicHandlerSettings(Of TAdaptedType)
        Inherits DynamicHandlerSettings
        Implements ISelector

        Public Sub New()

        End Sub

        Public Sub New(untyped As DynamicHandlerSettings)
            ReflectionHelper.MergeObjects(untyped, Me)
        End Sub

        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Property EditChildControl As Boolean

        <Required(True)> _
        <ConditionalVisible("EditChildControl", False, True)>
       <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Overrides Property ChildControlId As String = String.Empty

        '<Selector("Text", "Value", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
         <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
         <ConditionalVisible("EditChildControl", True, True)>
        <AutoPostBack()> _
        <XmlIgnore()> _
        Public Property SelectedChildControlId() As String
            Get
                Return ChildControlId
            End Get
            Set(value As String)
                ChildControlId = value
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property ChildControlMember As MemberInfo
            Get
                Dim targetControlType As Type = GetType(TAdaptedType)
                Return ReflectionHelper.GetMember(targetControlType, ChildControlId, True, True)
            End Get
        End Property


        <Browsable(False)> _
        Public ReadOnly Property EventControlType As Type
            Get
                Dim targetControlType As Type = GetType(TAdaptedType)
                If Me.MainControlStep = ControlStep.ChildControlEvent Then
                    Dim objMember As MemberInfo = ChildControlMember
                    If objMember IsNot Nothing AndAlso TypeOf objMember Is FieldInfo Then
                        targetControlType = DirectCast(objMember, FieldInfo).FieldType
                    Else
                        targetControlType = GetType(Control)
                    End If
                End If
                Return targetControlType
            End Get
        End Property

        <ConditionalVisible("MainControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
        Public ReadOnly Property EventControlTypeAsString As String
            Get
                Return ReflectionHelper.GetSafeTypeName(EventControlType)
            End Get
        End Property

        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public ReadOnly Property ChildControlDeclaringType As String
            Get
                Dim objMember As MemberInfo = ChildControlMember
                If objMember IsNot Nothing AndAlso TypeOf objMember Is FieldInfo Then
                    Return ReflectionHelper.GetSafeTypeName(DirectCast(objMember, FieldInfo).DeclaringType)
                Else
                    Return ReflectionHelper.GetSafeTypeName(GetType(TAdaptedType))
                End If
            End Get
        End Property

        <ConditionalVisible("MainControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
        Public Property EditEventName As Boolean

        <Required(True)> _
        <ConditionalVisible("EditEventName", False, True)> _
         <ConditionalVisible("MainControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
        Public Overrides Property ControlEventName As String = String.Empty

        '<Selector("Text", "Value", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
         <ConditionalVisible("MainControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
         <ConditionalVisible("EditEventName", True, True)>
        <AutoPostBack> _
        <XmlIgnore()> _
        Public Property SelectedControlEventName() As String
            Get
                Return ControlEventName
            End Get
            Set(value As String)
                ControlEventName = value
            End Set
        End Property




        <LabelMode(LabelMode.Left)> _
        <XmlIgnore()> _
        <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List)> _
        Public ReadOnly Property AvailableParameters As SerializableDictionary(Of String, String)
            Get
                Dim toReturn As New SerializableDictionary(Of String, String)
                Select Case Me.MainControlStep
                    Case ControlStep.ControlEvent, ControlStep.ChildControlEvent
                        Dim objEvent As EventInfo = EventControlType.GetEvent(ControlEventName)
                        If objEvent IsNot Nothing Then
                            Dim objParams As ParameterInfo() = ReflectionHelper.GetEventParameters(objEvent)
                            For Each objParam As ParameterInfo In objParams
                                Dim objParamName As String = objParam.Name
                                If objParamName = "e" Then
                                    objParamName = DynamicControlAdapter.EventArgsVarName
                                End If
                                toReturn.Add(objParamName, ReflectionHelper.GetSafeTypeName(objParam.ParameterType))
                            Next
                        End If
                    Case ControlStep.Render, ControlStep.RenderChildren
                        toReturn.Add("writer", ReflectionHelper.GetSafeTypeName(GetType(HtmlTextWriter)))
                    Case ControlStep.CreateChildControls
                        Exit Select
                    Case Else
                        toReturn.Add(DynamicControlAdapter.EventArgsVarName, ReflectionHelper.GetSafeTypeName(GetType(EventArgs)))
                End Select
                Return toReturn
            End Get
        End Property
        

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection()
            Select Case propertyName
                Case "SelectedChildControlId"
                    Dim objField As FieldInfo
                    Dim typeMembers As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(GetType(TAdaptedType), True, True)
                    For Each objMembersByName As KeyValuePair(Of String, MemberInfo) In typeMembers
                        objField = TryCast(objMembersByName.Value, FieldInfo)
                        If objField IsNot Nothing AndAlso GetType(Control).IsAssignableFrom(objField.FieldType) Then
                            toReturn.Add(objField.Name)
                        End If
                    Next
                Case "SelectedControlEventName"
                    Dim objEvent As EventInfo
                    Dim targetControlType As Type = EventControlType
                    Dim typeMembers As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(targetControlType, True, True)
                    For Each objMembersByName As KeyValuePair(Of String, MemberInfo) In typeMembers
                        objEvent = TryCast(objMembersByName.Value, EventInfo)
                        If objEvent IsNot Nothing Then
                            toReturn.Add(objEvent.Name)
                        End If
                    Next
            End Select
            Return toReturn
        End Function

        


    End Class


    <Serializable()> _
    Public Class DynamicHandlerSettings
        Inherits SimpleRuleEngine


        Public Property MainControlStep As ControlStep

        <Required(True)> _
        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Overridable Property ChildControlId As String = String.Empty

        <Required(True)> _
        <ConditionalVisible("MainControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
        Public Overridable Property ControlEventName As String = String.Empty


        Private _EventInfo As EventInfo

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property EventInfo As EventInfo
            Get
                Return _EventInfo
            End Get
        End Property

        <ConditionalVisible("MainControlStep", True, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
        Public Property BaseHandlerMode As ControlBaseHandlerMode = ControlBaseHandlerMode.Before


        Public Sub RegisterEvent(objCtl As Control)
            Me._EventInfo = objCtl.GetType().GetEvent(Me.ControlEventName)
        End Sub

        Public Sub DynamicEventHandler(controlAdapter As DynamicControlAdapter, sender As Object, e As EventArgs)
            Dim parameters As New SerializableDictionary(Of String, Object)
            Dim objParams As ParameterInfo() = ReflectionHelper.GetEventParameters(Me.EventInfo)
            parameters(objParams(0).Name) = sender
            Dim objParamName As String = objParams(1).Name
            If objParamName = "e" Then
                objParamName = DynamicControlAdapter.EventArgsVarName
            End If
            parameters(objParamName) = e
            controlAdapter.ProcessStep(parameters, Nothing, Me.GetStep(), False)
        End Sub


        Public Function GetStep() As DynamicHandlerStep
            If MainControlStep = ControlStep.ChildControlEvent Then
                Return New DynamicHandlerStep(Me.MainControlStep, Me.ChildControlId, Me.ControlEventName)
            ElseIf MainControlStep = ControlStep.ControlEvent Then
                Return New DynamicHandlerStep(Me.MainControlStep, Me.ControlEventName)
            Else
                Return New DynamicHandlerStep(Me.MainControlStep)
            End If

        End Function

        Public Function Downgrade() As DynamicHandlerSettings
            Dim toReturn As New DynamicHandlerSettings()
            ReflectionHelper.MergeObjects(Me, toReturn)
            Return toReturn
        End Function

    End Class
End Namespace