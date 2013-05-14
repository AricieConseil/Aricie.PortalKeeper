Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Reflection

Namespace Services.Flee
    ''' <summary>
    ''' Base class for flee actions
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public MustInherit Class ObjectAction
        Inherits NamedConfig

        ''' <summary>
        ''' Returns the name of the Object object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ObjectType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(Object))
            End Get
        End Property

        Public MustOverride Sub Run(ByVal owner As Object, ByVal globalVars As IContextLookup)
    End Class


    ''' <summary>
    ''' Type of flee actions
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ObjectActionType
        CallProcedure
        SetProperty
    End Enum


    ''' <summary>
    ''' Generic version of the ObjectAction
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public MustInherit Class ObjectAction(Of TObjectType)
        Inherits ObjectAction

        ''' <summary>
        ''' Returns the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        Public Overrides ReadOnly Property ObjectType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(TObjectType))
            End Get
        End Property

        ''' <summary>
        ''' Instance of the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <LabelMode(LabelMode.Top)> _
        <ExtendedCategory("Instance")> _
        Public Property Instance() As New SimpleExpression(Of TObjectType)

        ''' <summary>
        ''' Parameters for the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <LabelMode(LabelMode.Top)> _
        <ExtendedCategory("Parameters")> _
        Public Property Parameters() As New Variables

    End Class

    ''' <summary>
    ''' Set property flee action
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class SetObjectProperty(Of TObjectType)
        Inherits ObjectAction(Of TObjectType)
        Implements ISelector(Of PropertyInfo)

        ''' <summary>
        ''' Gets or sets the property name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
          <ProvidersSelector()> _
        Public Property PropertyName() As String = string.Empty

        ''' <summary>
        ''' Gets or sets the value of the property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        <LabelMode(LabelMode.Top)> _
        Public Property Value() As new FleeExpressionInfo(Of Object)

        ''' <summary>
        ''' Runs the evaluation against an object
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
            Dim targetProp As PropertyInfo
            If ReflectionHelper.GetFullMembersDictionary(GetType(TObjectType)).TryGetValue(Me._PropertyName, potentialsMembers) Then
                For Each potentialMember As MemberInfo In potentialsMembers
                    If TypeOf potentialMember Is PropertyInfo Then
                        targetProp = DirectCast(potentialMember, PropertyInfo)
                        If targetProp.GetIndexParameters.Length = args.Count Then
                            Dim objValue As Object = Me._Value.Evaluate(owner, globalVars)
                            targetProp.SetValue(Me.Instance.Evaluate(owner, globalVars), objValue, args.ToArray)
                            Exit Sub
                        End If
                    End If
                Next

            End If
            Throw New Exception(String.Format("Property {0} with {2} parameters was not found in type {1}", Me._PropertyName, args.Count, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
        End Sub

        ''' <summary>
        ''' Returns values for the generic Type properties that are writable
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSelector(ByVal propertyName As String) As System.Collections.IList Implements ComponentModel.ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        ''' <summary>
        ''' Returns the generic Type properties that are writable
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSelectorG(ByVal propertyName As String) As System.Collections.Generic.IList(Of System.Reflection.PropertyInfo) Implements ComponentModel.ISelector(Of System.Reflection.PropertyInfo).GetSelectorG
            Dim toReturn As New List(Of PropertyInfo)()
            For Each objProperty As PropertyInfo In ReflectionHelper.GetPropertiesDictionary(Of TObjectType)().Values
                If objProperty.CanWrite Then
                    toReturn.Add(objProperty)
                End If
            Next
            Return toReturn
        End Function
    End Class


    ''' <summary>
    ''' Runs method as flee action
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
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
        Public Property MethodName() As String = string.Empty

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
                For Each potentialMember As MemberInfo In potentialsMembers
                    If TypeOf potentialMember Is MethodInfo Then
                        targetMethod = DirectCast(potentialMember, MethodInfo)
                        If targetMethod.GetParameters.Length = args.Count Then
                            If targetMethod.IsStatic Then
                                targetMethod.Invoke(Nothing, args.ToArray)
                            Else
                                targetMethod.Invoke(Me.Instance.Evaluate(owner, globalVars), args.ToArray)
                            End If
                            Exit Sub
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
            For Each objMember As MemberInfo In ReflectionHelper.GetMembersDictionary(Of TObjectType)().Values
                If TypeOf objMember Is MethodInfo Then
                    toReturn.Add(DirectCast(objMember, MethodInfo))
                End If
            Next
            Return toReturn
        End Function


    End Class

End Namespace