Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
    ''' <summary>
    ''' Set property flee action
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    
    <DefaultProperty("FriendlyName")> _
    Public Class SetObjectProperty(Of TObjectType)
        Inherits ObjectAction(Of TObjectType)
        Implements ISelector(Of PropertyInfo)

        <Browsable(False)> _
        Public Overridable ReadOnly Property FriendlyName As String
            Get
                Return String.Format("Set Property{0}{1}", UIConstants.TITLE_SEPERATOR, PropertyName.ToString())
            End Get
        End Property


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


        <Browsable(False)> _
        Public Property Value() As FleeExpressionInfo(Of Object)
            Get
                Return Nothing
            End Get
            Set(objValue As FleeExpressionInfo(Of Object))
                Me.PropertyValue.Expression = objValue
                Me.PropertyValue.Mode = SimpleOrExpressionMode.Expression
            End Set
        End Property


        <ExtendedCategory("Instance")> _
        Public Property PropertyValue As New SimpleOrExpression(Of Object)

        ''' <summary>
        ''' Runs the evaluation against an object
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <remarks></remarks>
        Public Overrides Function Run(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            Dim args As List(Of Object) = (From objParam In Me.Parameters.EvaluateVariables(owner, globalVars) Select objParam.Value).ToList()
            Dim potentialsMembers As List(Of MemberInfo) = Nothing
            Dim targetProp As PropertyInfo
            If ReflectionHelper.GetFullMembersDictionary(GetType(TObjectType)).TryGetValue(Me._PropertyName, potentialsMembers) Then
                For Each potentialMember As MemberInfo In potentialsMembers
                    If TypeOf potentialMember Is PropertyInfo Then
                        targetProp = DirectCast(potentialMember, PropertyInfo)
                        If targetProp.GetIndexParameters.Length = args.Count Then
                            Dim objValue As Object = Me.PropertyValue.GetValue(owner, globalVars)
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
            Dim toReturn As List(Of PropertyInfo) = (From objProperty In ReflectionHelper.GetPropertiesDictionary(Of TObjectType)().Values Where objProperty.CanWrite).ToList()
            toReturn.Sort(Function(objProp1, objProp2) String.Compare(objProp1.Name, objProp2.Name, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function

        Public Overrides Function GetOutputType() As Type
            Return Nothing
        End Function
    End Class
End Namespace