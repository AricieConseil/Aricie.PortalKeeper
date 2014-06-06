Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee


    ''' <summary>
    ''' Non Generic version of the ObjectAction
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class GeneralObjectAction
        Inherits ObjectAction
        Implements ISelector(Of EventInfo)
        Implements ISelector(Of MethodInfo)
        Implements ISelector(Of PropertyInfo)




        Public Property DotNetType As New DotNetType()

        <Browsable(False)> _
        Public ReadOnly Property HasType As Boolean
            Get
                Return DotNetType.GetDotNetType() IsNot Nothing
            End Get
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property ObjectType As String
            Get
                Return DotNetType.TypeName
            End Get
        End Property

        <ConditionalVisible("HasType", False, True)> _
        Public Property ActionMode As ObjectActionMode


        ''' <summary>
        ''' Instance of the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        Public Property Instance() As New FleeExpressionInfo(Of Object)


        ''' <summary>
        ''' Gets or sets the property name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.SetProperty)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property PropertyName() As String = String.Empty

        ''' <summary>
        ''' Gets or sets the value of the property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.SetProperty)> _
        <LabelMode(LabelMode.Top)> _
        Public Property PropertyValue() As New FleeExpressionInfo(Of Object)


        ''' <summary>
        ''' Gets or sets the method name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.CallMethod)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property MethodName() As String = String.Empty

        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.CallMethod)> _
        Public Property MethodIndex As Integer = 1


        ''' <summary>
        ''' Parameters for the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.SetProperty, ObjectActionMode.CallMethod)> _
        Public Overridable Property Parameters() As New Variables


        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.AddEventHandler)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property EventName As String



        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("ActionMode", False, True, ObjectActionMode.AddEventHandler)> _
        Public Overridable Property DelegateExpression As New FleeExpressionInfo(Of [Delegate])



        '<XmlIgnore()> _
        '<Browsable(False)> _
        'Public Overrides Property Parameters As Variables
        '    Get
        '        Return Nothing
        '    End Get
        '    Set(value As Variables)
        '        'do nothing
        '    End Set
        'End Property

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "EventName"
                    Return DirectCast(GetSelectorEvents(propertyName), IList)
                Case "MethodName"
                    Return DirectCast(GetSelectorMethods(propertyName), IList)
                Case "PropertyName"
                    Return DirectCast(GetSelectorProperties(propertyName), IList)
            End Select
            Return Nothing
        End Function



        Public Overrides Sub Run(owner As Object, globalVars As IContextLookup)
            If Not Me.HasType Then
                Throw New ApplicationException("GeneralObjectAction has no Type Defined")
            End If
            Select Case Me.ActionMode
                Case Flee.ObjectActionMode.SetProperty
                    Dim args As New List(Of Object)
                    For Each objParam As KeyValuePair(Of String, Object) In Me.Parameters.EvaluateVariables(owner, globalVars)
                        args.Add(objParam.Value)
                    Next
                    Dim potentialsMembers As List(Of MemberInfo) = Nothing
                    Dim targetProp As PropertyInfo
                    If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType()).TryGetValue(Me._PropertyName, potentialsMembers) Then
                        For Each potentialMember As MemberInfo In potentialsMembers
                            If TypeOf potentialMember Is PropertyInfo Then
                                targetProp = DirectCast(potentialMember, PropertyInfo)
                                If targetProp.GetIndexParameters.Length = args.Count Then
                                    Dim objValue As Object = Me.PropertyValue.Evaluate(owner, globalVars)
                                    Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
                                    If Me.LockTarget Then
                                        SyncLock target
                                            targetProp.SetValue(Me.Instance.Evaluate(owner, globalVars), objValue, args.ToArray)
                                        End SyncLock
                                    Else
                                        targetProp.SetValue(Me.Instance.Evaluate(owner, globalVars), objValue, args.ToArray)
                                    End If
                                    Exit Sub
                                End If
                            End If
                        Next
                    End If
                    Throw New Exception(String.Format("Property {0} with {2} parameters was not found in type {1}", _
                                                      Me._PropertyName, args.Count, ReflectionHelper.GetSafeTypeName(Me.DotNetType.GetDotNetType())))
                Case Flee.ObjectActionMode.CallMethod
                    Dim args As New List(Of Object)
                    For Each objParam As KeyValuePair(Of String, Object) In Me.Parameters.EvaluateVariables(owner, globalVars)
                        args.Add(objParam.Value)
                    Next
                    Dim potentialsMembers As List(Of MemberInfo) = Nothing
                    Dim targetMethod As MethodInfo
                    Dim found As Boolean
                    If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType()).TryGetValue(Me._MethodName, potentialsMembers) Then
                        Dim index As Integer = 0
                        For Each potentialMember As MemberInfo In potentialsMembers
                            If TypeOf potentialMember Is MethodInfo Then
                                targetMethod = DirectCast(potentialMember, MethodInfo)
                                If targetMethod.GetParameters.Length = args.Count Then
                                    index += 1
                                    If index = MethodIndex Then
                                        found = True
                                        If targetMethod.IsStatic Then
                                            targetMethod.Invoke(Nothing, args.ToArray)
                                        Else
                                            Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
                                            If Me.LockTarget Then
                                                SyncLock target
                                                    targetMethod.Invoke(target, args.ToArray)
                                                End SyncLock
                                            Else
                                                targetMethod.Invoke(target, args.ToArray)
                                            End If
                                        End If

                                    End If
                                End If
                            End If
                        Next
                    End If
                    If Not found Then
                        Throw New Exception(String.Format("Method {0} with {2} parameters was not found in type {1}", _
                                                          Me._MethodName, args.Count, ReflectionHelper.GetSafeTypeName(Me.DotNetType.GetDotNetType())))
                    End If
                Case Flee.ObjectActionMode.AddEventHandler
                    Dim candidateEventMember As MemberInfo = ReflectionHelper.GetMember(Me.DotNetType.GetDotNetType(), EventName, True, True)
                    If candidateEventMember IsNot Nothing Then
                        Dim candidateEvent As EventInfo = TryCast(candidateEventMember, EventInfo)
                        If candidateEvent IsNot Nothing Then
                            Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
                            If target IsNot Nothing Then
                                AddEventHandler(owner, globalVars, target, candidateEvent)
                            Else
                                Throw New Exception(String.Format( _
                                    "Instance Expression {0} returned a null instance while adding event handler {1}  in type {2}", _
                                    Me.Instance.Expression, candidateEventMember.ToString(), Me.EventName, Me.DotNetType.GetDotNetType()))
                            End If
                        Else
                            Throw New Exception(String.Format( _
                                "Candidate Member {0} could not be converted to event {1} in type {2}", _
                                candidateEventMember.ToString(), Me.EventName, Me.DotNetType.GetDotNetType()))
                        End If
                    Else
                        Throw New Exception(String.Format("Event {0} was not found in type {1}", Me.EventName, ReflectionHelper.GetSafeTypeName(Me.DotNetType.GetDotNetType())))
                    End If
            End Select
        End Sub


        Protected Overridable Sub AddEventHandler(owner As Object, globalVars As IContextLookup, target As Object, candidateEvent As EventInfo)
            Dim objDelegate As [Delegate] = Me.DelegateExpression.Evaluate(owner, globalVars)
            If objDelegate IsNot Nothing Then
                If Me.LockTarget Then
                    SyncLock target
                        candidateEvent.AddEventHandler(target, objDelegate)
                    End SyncLock
                Else
                    candidateEvent.AddEventHandler(target, objDelegate)
                End If
            Else
                Throw New Exception(String.Format( _
                    "Delegate Expression {0} returned a null instance while adding event handler {1}  in type {2}", _
                    Me.DelegateExpression.Expression, candidateEvent.ToString(), Me.EventName, Me.DotNetType.GetDotNetType()))
            End If
        End Sub



        Public Function GetSelectorProperties(propertyName As String) As IList(Of PropertyInfo) Implements ISelector(Of PropertyInfo).GetSelectorG
            Dim toReturn As New List(Of PropertyInfo)()
            If Me.HasType Then
                For Each objProperty As PropertyInfo In ReflectionHelper.GetPropertiesDictionary(DotNetType.GetDotNetType()).Values
                    If objProperty.CanWrite Then
                        toReturn.Add(objProperty)
                    End If
                Next
                toReturn.Sort(Function(objProp1, objProp2) String.Compare(objProp1.Name, objProp2.Name, StringComparison.InvariantCultureIgnoreCase))
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' Returns a list of the methods on the generic type
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSelectorMethods(ByVal propertyName As String) As System.Collections.Generic.IList(Of System.Reflection.MethodInfo) Implements ComponentModel.ISelector(Of System.Reflection.MethodInfo).GetSelectorG
            Dim toReturn As New List(Of MethodInfo)
            If Me.HasType Then
                For Each objMember As MemberInfo In ReflectionHelper.GetMembersDictionary(Me.DotNetType.GetDotNetType(), True, False).Values
                    If TypeOf objMember Is MethodInfo Then
                        toReturn.Add(DirectCast(objMember, MethodInfo))
                    End If
                Next
                toReturn.Sort(Function(objMember1, objMember2) String.Compare(objMember1.Name, objMember2.Name, StringComparison.InvariantCultureIgnoreCase))
            End If
            Return toReturn
        End Function



        Public Function GetSelectorEvents(propertyName As String) As IList(Of EventInfo) Implements ISelector(Of EventInfo).GetSelectorG
            Dim toReturn As New List(Of EventInfo)
            If Me.HasType Then
                toReturn = ReflectionHelper.GetMembersDictionary(Me.DotNetType.GetDotNetType(), True, False).Values.OfType(Of EventInfo)().ToList()
                toReturn.Sort(Function(objMember1, objMember2) String.Compare(objMember1.Name, objMember2.Name, StringComparison.InvariantCultureIgnoreCase))
            End If
            Return toReturn
        End Function


    End Class
End Namespace