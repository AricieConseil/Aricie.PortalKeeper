Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
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
        Public Property PropertyName() As String = String.Empty

        ''' <summary>
        ''' Gets or sets the value of the property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        <LabelMode(LabelMode.Top)> _
        Public Property Value() As New FleeExpressionInfo(Of Object)

        ''' <summary>
        ''' Runs the evaluation against an object
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <remarks></remarks>
        Public Overrides Function Run(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
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
                            Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
                            If Me.LockTarget Then
                                SyncLock target
                                    targetProp.SetValue(Me.Instance.Evaluate(owner, globalVars), objValue, args.ToArray)
                                End SyncLock
                            Else
                                targetProp.SetValue(Me.Instance.Evaluate(owner, globalVars), objValue, args.ToArray)
                            End If
                            Return Nothing
                        End If
                    End If
                Next
            End If
            Throw New Exception(String.Format("Property {0} with {2} parameters was not found in type {1}", Me._PropertyName, args.Count, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
        End Function

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
            toReturn.Sort(Function(objProp1, objProp2) String.Compare(objProp1.Name, objProp2.Name, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function
    End Class
End Namespace