Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports System.Threading

Namespace Services.Flee

    Public Enum ObjectActionMode
        SetProperty
        CallMethod
        AddEventHandler
    End Enum



    ''' <summary>
    ''' Non Generic version of the ObjectAction
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public MustInherit Class GeneralObjectAction
        Inherits ObjectAction
        Implements ISelector(Of EventInfo)
        Implements ISelector(Of MethodInfo)





        ''' <summary>
        ''' Returns the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        Public Overrides ReadOnly Property ObjectType As String
            Get
                Return DotNetType.TypeName
            End Get
        End Property

        Public Property DotNetType As New DotNetType()


        Public ObjectActionMode As ObjectActionMode


        ''' <summary>
        ''' Instance of the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        Public Property Instance() As New FleeExpressionInfo(Of Object)


        ''' <summary>
        ''' Gets or sets the method name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property MethodName() As String = String.Empty

        <ExtendedCategory("Instance")> _
        Public MethodIndex As Integer = 1


        ' ''' <summary>
        ' ''' Runs the method
        ' ''' </summary>
        ' ''' <param name="owner"></param>
        ' ''' <param name="globalVars"></param>
        ' ''' <remarks></remarks>
        'Public Overrides Sub Run(ByVal owner As Object, ByVal globalVars As IContextLookup)
        '    Dim args As New List(Of Object)
        '    For Each objParam As KeyValuePair(Of String, Object) In Me.Parameters.EvaluateVariables(owner, globalVars)
        '        args.Add(objParam.Value)
        '    Next
        '    Dim potentialsMembers As List(Of MemberInfo) = Nothing
        '    Dim targetMethod As MethodInfo
        '    If ReflectionHelper.GetFullMembersDictionary(GetType(TObjectType)).TryGetValue(Me._MethodName, potentialsMembers) Then
        '        Dim index As Integer = 0
        '        For Each potentialMember As MemberInfo In potentialsMembers
        '            If TypeOf potentialMember Is MethodInfo Then
        '                targetMethod = DirectCast(potentialMember, MethodInfo)
        '                If targetMethod.GetParameters.Length = args.Count Then
        '                    index += 1
        '                    If index = MethodIndex Then
        '                        If targetMethod.IsStatic Then
        '                            targetMethod.Invoke(Nothing, args.ToArray)
        '                        Else
        '                            Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
        '                            If Me.LockTarget Then
        '                                SyncLock target
        '                                    targetMethod.Invoke(target, args.ToArray)
        '                                End SyncLock
        '                            Else
        '                                targetMethod.Invoke(target, args.ToArray)
        '                            End If
        '                        End If
        '                        Exit Sub
        '                    End If
        '                End If
        '            End If
        '        Next

        '    End If
        '    Throw New Exception(String.Format("Method {0} with {2} parameters was not found in type {1}", Me._MethodName, args.Count, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
        'End Sub


        ''' <summary>
        ''' Parameters for the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Parameters")> _
        Public Overridable Property Parameters() As New Variables


        <ExtendedCategory("Instance")> _
       <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
       <ProvidersSelector()> _
        Public Property EventName As String


        Public Property DelegateExpression As New FleeExpressionInfo(Of [Delegate])

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

            End Select
            Return Nothing
        End Function



        ''' <summary>
        ''' Returns a list of the methods on the generic type
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSelectorMethods(ByVal propertyName As String) As System.Collections.Generic.IList(Of System.Reflection.MethodInfo) Implements ComponentModel.ISelector(Of System.Reflection.MethodInfo).GetSelectorG
            Dim toReturn As New List(Of MethodInfo)
            For Each objMember As MemberInfo In ReflectionHelper.GetMembersDictionary(Me.DotNetType.GetDotNetType(), True, False).Values
                If TypeOf objMember Is MethodInfo Then
                    toReturn.Add(DirectCast(objMember, MethodInfo))
                End If
            Next
            toReturn.Sort(Function(objMember1, objMember2) String.Compare(objMember1.Name, objMember2.Name, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function



        Public Function GetSelectorEvents(propertyName As String) As IList(Of EventInfo) Implements ISelector(Of EventInfo).GetSelectorG
            Dim toReturn As List(Of EventInfo) = ReflectionHelper.GetMembersDictionary(Me.DotNetType.GetDotNetType(), True, False).Values.OfType(Of EventInfo)().ToList()
            toReturn.Sort(Function(objMember1, objMember2) String.Compare(objMember1.Name, objMember2.Name, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function

        Public Overrides Sub Run(owner As Object, globalVars As IContextLookup)
            Select Case Me.ObjectActionMode
                Case Flee.ObjectActionMode.AddEventHandler
                    Dim candidateEventMember As MemberInfo = ReflectionHelper.GetMember(Me.DotNetType.GetDotNetType(), EventName, True, True)
                    If candidateEventMember IsNot Nothing Then
                        Dim candidateEvent As EventInfo = TryCast(candidateEventMember, EventInfo)
                        If candidateEvent IsNot Nothing Then
                            Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
                            If target IsNot Nothing Then
                                Dim objDelegate As [Delegate] = Me.DelegateExpression.Evaluate(owner, globalVars)
                                If objDelegate IsNot Nothing Then
                                    If Me.LockTarget Then
                                        SyncLock target
                                            candidateEvent.AddEventHandler(target, objDelegate)
                                        End SyncLock
                                    Else
                                        candidateEvent.AddEventHandler(target, objDelegate)
                                    End If
                                End If
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
                Case Flee.ObjectActionMode.CallMethod
                    Dim args As New List(Of Object)
                    For Each objParam As KeyValuePair(Of String, Object) In Me.Parameters.EvaluateVariables(owner, globalVars)
                        args.Add(objParam.Value)
                    Next
                    Dim potentialsMembers As List(Of MemberInfo) = Nothing
                    Dim targetMethod As MethodInfo
                    If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType()).TryGetValue(Me._MethodName, potentialsMembers) Then
                        Dim index As Integer = 0
                        For Each potentialMember As MemberInfo In potentialsMembers
                            If TypeOf potentialMember Is MethodInfo Then
                                targetMethod = DirectCast(potentialMember, MethodInfo)
                                If targetMethod.GetParameters.Length = args.Count Then
                                    index += 1
                                    If index = MethodIndex Then
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
                                        Exit Sub
                                    End If
                                End If
                            End If
                        Next

                    End If
                    Throw New Exception(String.Format("Method {0} with {2} parameters was not found in type {1}", _
                                                      Me._MethodName, args.Count, ReflectionHelper.GetSafeTypeName(Me.DotNetType.GetDotNetType())))
                Case Flee.ObjectActionMode.SetProperty

            End Select
           
        End Sub



    End Class


    ''' <summary>
    ''' Runs method as flee action
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    <DisplayName("Call Method")> _
    <Serializable()> _
    Public Class CallObjectMethod(Of TObjectType)
        Inherits ObjectAction(Of TObjectType)
        Implements ISelector(Of MethodInfo)

        ''' <summary>
        ''' Gets or sets the method name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property MethodName() As String = String.Empty

        <ExtendedCategory("Instance")> _
        Public MethodIndex As Integer = 1


        ''' <summary>
        ''' Runs the method
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <remarks></remarks>
        Public Overrides Sub Run(ByVal owner As Object, ByVal globalVars As IContextLookup)
            Dim args As New List(Of Object)
            For Each objParam As KeyValuePair(Of String, Object) In Me.Parameters.EvaluateVariables(owner, globalVars)
                args.Add(objParam.Value)
            Next
            Dim potentialsMembers As List(Of MemberInfo) = Nothing
            Dim targetMethod As MethodInfo
            If ReflectionHelper.GetFullMembersDictionary(GetType(TObjectType)).TryGetValue(Me._MethodName, potentialsMembers) Then
                Dim index As Integer = 0
                For Each potentialMember As MemberInfo In potentialsMembers
                    If TypeOf potentialMember Is MethodInfo Then
                        targetMethod = DirectCast(potentialMember, MethodInfo)
                        If targetMethod.GetParameters.Length = args.Count Then
                            index += 1
                            If index = MethodIndex Then
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
                                Exit Sub
                            End If
                        End If
                    End If
                Next

            End If
            Throw New Exception(String.Format("Method {0} with {2} parameters was not found in type {1}", Me._MethodName, args.Count, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
        End Sub

        ''' <summary>
        ''' Returns a list of the methods on the generic type
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSelector(ByVal propertyName As String) As System.Collections.IList Implements ComponentModel.ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        ''' <summary>
        ''' Returns a list of the methods on the generic type
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSelectorG(ByVal propertyName As String) As System.Collections.Generic.IList(Of System.Reflection.MethodInfo) Implements ComponentModel.ISelector(Of System.Reflection.MethodInfo).GetSelectorG
            Dim toReturn As New List(Of MethodInfo)
            For Each objMember As MemberInfo In ReflectionHelper.GetMembersDictionary(GetType(TObjectType), True, False).Values
                If TypeOf objMember Is MethodInfo Then
                    toReturn.Add(DirectCast(objMember, MethodInfo))
                End If
            Next
            toReturn.Sort(Function(objMember1, objMember2) String.Compare(objMember1.Name, objMember2.Name, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function


    End Class
End Namespace