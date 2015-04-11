Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.Services

Namespace Services.Flee




   





    ''' <summary>
    ''' Runs method as flee action
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    <DisplayName("Add Event Handler")> _
     <DefaultProperty("FriendlyName")> _
    <Serializable()> _
    Public Class AddEventHandler(Of TObjectType)
        Inherits ObjectAction(Of TObjectType)
        Implements ISelector(Of EventInfo)


        <Browsable(False)> _
        Public Overridable ReadOnly Property FriendlyName As String
            Get
                Return String.Format("Add Event Handler{0}{1}", UIConstants.TITLE_SEPERATOR, EventName.ToString())
            End Get
        End Property



        <ExtendedCategory("Instance")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector()> _
        Public Property EventName As String = ""


        Public Property DelegateExpression As New FleeExpressionInfo(Of [Delegate])

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Parameters As Variables
            Get
                Return Nothing
            End Get
            Set(value As Variables)
                'do nothing
            End Set
        End Property

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        Public Function GetSelectorG(propertyName As String) As IList(Of EventInfo) Implements ISelector(Of EventInfo).GetSelectorG
            Dim toReturn As List(Of EventInfo) = ReflectionHelper.GetMembersDictionary(GetType(TObjectType), True, False).Values.OfType(Of EventInfo)().ToList()
            toReturn.Sort(Function(objMember1, objMember2) String.Compare(objMember1.Name, objMember2.Name, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function

        Public Overrides Function Run(owner As Object, globalVars As IContextLookup) As Object
            Dim candidateEventMember As MemberInfo = ReflectionHelper.GetMember(GetType(TObjectType), EventName, True, True)
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
                        Throw New Exception(String.Format("Instance Expression {0} returned a null instance while adding event handler {1}  in type {2}", Me.Instance.Expression, candidateEventMember.ToString(), Me.EventName, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
                    End If
                Else
                    Throw New Exception(String.Format("Candidate Member {0} could not be converted to event {1} in type {2}", candidateEventMember.ToString(), Me.EventName, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
                End If
            Else
                Throw New Exception(String.Format("Event {0} was not found in type {1}", Me.EventName, ReflectionHelper.GetSafeTypeName(GetType(TObjectType))))
            End If
            Return Nothing
        End Function

        Public Overrides Function GetOutputType() As Type
            Return Nothing
        End Function
    End Class
End Namespace