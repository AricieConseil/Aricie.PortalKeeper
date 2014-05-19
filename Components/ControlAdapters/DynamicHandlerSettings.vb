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


        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Property EditChildControl As Boolean

        '<Selector("Text", "Value", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
         <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
         <ConditionalVisible("EditChildControl", True, True)>
        <AutoPostBack> _
        <XmlIgnore()> _
        Public Property SelectedChildControlId() As String
            Get
                Return ChildControlId
            End Get
            Set(value As String)
                ChildControlId = value
            End Set
        End Property

        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Property EditEventName As Boolean

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

        <ConditionalVisible("EditChildControl", False, True)>
        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Overrides Property ChildControlId As String = String.Empty

        <ConditionalVisible("EditEventName", False, True)> _
        <ConditionalVisible("MainControlStep", False, True, ControlStep.ControlEvent, ControlStep.ChildControlEvent)>
        Public Overrides Property ControlEventName As String = String.Empty


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection()
            Select Case propertyName
                Case "SelectedChildControlId"
                    Dim objField As FieldInfo
                    Dim typeMembers As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(GetType(TAdaptedType))
                    For Each objMembersByName As KeyValuePair(Of String, List(Of MemberInfo)) In typeMembers
                        For Each objMember As MemberInfo In objMembersByName.Value
                            objField = TryCast(objMember, FieldInfo)
                            If objField IsNot Nothing AndAlso GetType(Control).IsAssignableFrom(objField.FieldType) Then
                                toReturn.Add(objField.Name)
                            End If
                        Next
                    Next
                Case "SelectedControlEventName"
                    Dim objEvent As EventInfo
                    Dim targetControlType As Type = GetType(TAdaptedType)
                    If Me.MainControlStep = ControlStep.ChildControlEvent Then
                        Dim objMember As MemberInfo = ReflectionHelper.GetMember(targetControlType, ChildControlId)
                        If objMember IsNot Nothing AndAlso TypeOf objMember Is FieldInfo Then
                            targetControlType = DirectCast(objMember, FieldInfo).FieldType
                        Else
                            targetControlType = GetType(Control)
                        End If
                    End If
                    Dim typeMembers As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(targetControlType)
                    For Each objMembersByName As KeyValuePair(Of String, List(Of MemberInfo)) In typeMembers
                        For Each objMember As MemberInfo In objMembersByName.Value
                            objEvent = TryCast(objMember, EventInfo)
                            If objEvent IsNot Nothing Then
                                toReturn.Add(objEvent.Name)
                            End If
                        Next
                    Next
            End Select
            Return toReturn
        End Function


    End Class


    <Serializable()> _
    Public Class DynamicHandlerSettings
        Inherits SimpleRuleEngine


        Public Property MainControlStep As ControlStep

        <ConditionalVisible("MainControlStep", False, True, ControlStep.ChildControlEvent)>
        Public Overridable Property ChildControlId As String = String.Empty


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


        Public Sub RegisterEvent(objCtl As Control)
            Me._EventInfo = objCtl.GetType().GetEvent(Me.ControlEventName)
        End Sub

        Public Sub DynamicEventHandler(controlAdapter As DynamicControlAdapter, sender As Object, e As EventArgs)
            Dim parameters As New SerializableDictionary(Of String, Object)
            parameters.Add("sender", sender)
            parameters.Add("e", e)
            controlAdapter.ProcessStep(parameters, Nothing, Me.GetStep(), False)
        End Sub



        Public Property BaseHandlerMode As ControlBaseHandlerMode = ControlBaseHandlerMode.Before

        Public Function GetStep() As DynamicHandlerStep
            If MainControlStep = ControlStep.ChildControlEvent Then
                Return New DynamicHandlerStep(Me.MainControlStep, Me.ChildControlId, Me.ControlEventName)
            ElseIf MainControlStep = ControlStep.ControlEvent Then
                Return New DynamicHandlerStep(Me.MainControlStep, Me.ControlEventName)
            Else
                Return New DynamicHandlerStep(Me.MainControlStep)
            End If

        End Function


    End Class
End Namespace