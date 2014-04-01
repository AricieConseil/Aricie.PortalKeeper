Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
    <DisplayName("Delegate")> _
    <Serializable()> _
    Public Class DelegateVariableInfo(Of TParentType)
        Inherits VariableInfo(Of [Delegate])
        Implements ISelector(Of MethodInfo)

        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property MethodName As String

        Public Function GetMethodInfo() As MethodInfo
            Dim toReturn As MethodInfo
            Dim potentialsMembers As List(Of MemberInfo) = Nothing
            Dim targetMethod As MethodInfo
            If ReflectionHelper.GetFullMembersDictionary(GetType(TParentType)).TryGetValue(Me._MethodName, potentialsMembers) Then
                Dim index As Integer = 0
                For Each potentialMember As MemberInfo In potentialsMembers
                    If TypeOf potentialMember Is MethodInfo Then
                        index += 1
                        If index = MethodIndex Then
                            toReturn = DirectCast(potentialMember, MethodInfo)
                        End If
                    End If
                Next
            End If
            Return toReturn
        End Function

        Public MethodIndex As Integer = 1

        <Browsable(False)> _
        Public ReadOnly Property RequiresInstance As Boolean
            Get
                Dim targetMethod As MethodInfo = Me.GetMethodInfo()
                If targetMethod IsNot Nothing Then
                    Return targetMethod.IsStatic
                End If
                Return False
            End Get
        End Property

        <ConditionalVisible("RequiresInstance", False, True)>
        Public Property Instance As New SimpleExpression(Of Object)

        Public Overrides Function EvaluateOnce(owner As Object, globalVars As IContextLookup) As Object
            Dim toReturn As [Delegate]
            Dim potentialsMembers As List(Of MemberInfo) = Nothing
            Dim targetMethod As MethodInfo = Me.GetMethodInfo()
            If targetMethod IsNot Nothing Then
                If RequiresInstance Then
                    Dim target As Object = Me.Instance.Evaluate(owner, globalVars)
                    toReturn = [Delegate].CreateDelegate(GetType(TParentType), target, targetMethod)
                Else
                    toReturn = [Delegate].CreateDelegate(GetType(TParentType), targetMethod)
                End If

            Else
                Throw New Exception(String.Format("Can't create Delegate for method {0} in type {1}", targetMethod.Name, GetType(TParentType).FullName))
            End If
            Return toReturn
        End Function


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        Public Function GetSelectorG(propertyName As String) As IList(Of MethodInfo) Implements ISelector(Of MethodInfo).GetSelectorG
            Dim toReturn As New List(Of MethodInfo)
            For Each objMember As MemberInfo In ReflectionHelper.GetMembersDictionary(Of TParentType)().Values
                If TypeOf objMember Is MethodInfo Then
                    toReturn.Add(DirectCast(objMember, MethodInfo))
                End If
            Next
            Return toReturn
        End Function
    End Class
End Namespace