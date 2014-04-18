Imports Aricie.Collections
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls


Namespace Aricie.DNN.Modules.PortalKeeper


    Public Enum ActionTreeMode
        ListSubTrees
        DecisionTree
        SynonymBranch
    End Enum

    <ActionButton(IconName.Sitemap, IconOptions.Normal)> _
    <Serializable()> _
    Public Class ActionTree(Of TEngineEvents As IConvertible)
        Inherits NamedConfig



        <ExtendedCategory("SubTrees")> _
        Public Property Mode As ActionTreeMode


        <ConditionalVisible("Mode", False, True, ActionTreeMode.ListSubTrees)> _
        <ExtendedCategory("SubTrees")> _
        Public Property MainList As New List(Of ActionTree(Of TEngineEvents))


        <ConditionalVisible("Mode", False, True, ActionTreeMode.DecisionTree)> _
        <CollectionEditor(False, True, False, True, 20, CollectionDisplayStyle.Accordion, True, 0, "Key")> _
        <Orientation(Orientation.Vertical)> _
        <ExtendedCategory("SubTrees")> _
        Public Property SubTrees As New SerializableSortedDictionary(Of String, ActionTree(Of TEngineEvents))

        <ConditionalVisible("Mode", False, True, ActionTreeMode.SynonymBranch)> _
        <ExtendedCategory("SubTrees")> _
        Public Property Synonym As String = String.Empty



        Public Class SubTreesAttributes
            Implements IAttributesProvider


            Public Function GetAttributes() As IEnumerable(Of Attribute) Implements IAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                toReturn.Add(New WidthAttribute(100))
                Return toReturn
            End Function
        End Class

        <ConditionalVisible("Mode", False, True, ActionTreeMode.DecisionTree)> _
        <ExtendedCategory("SubTrees")> _
        Public Property KeyExpression As New FleeExpressionInfo(Of String)

        <ExtendedCategory("CurrentNode")> _
        Public Property Conditions As New KeeperCondition(Of TEngineEvents)

        <ExtendedCategory("CurrentNode")> _
        Public Property Actions As New KeeperAction(Of TEngineEvents)


        Public Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Me.Enabled Then
                Return Run(actionContext, Me)
            End If
            Return True
        End Function


        Protected Shared Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), objActionTree As ActionTree(Of TEngineEvents)) As Boolean
            Dim toReturn As Boolean = True
            If objActionTree.Enabled Then
                If objActionTree.Conditions.Match(actionContext) Then
                    toReturn = toReturn And objActionTree.Actions.Run(actionContext)
                End If
                Select Case objActionTree.Mode
                    Case ActionTreeMode.ListSubTrees
                        If objActionTree.MainList.Count > 0 Then
                            For Each objSubActionTree As ActionTree(Of TEngineEvents) In objActionTree.MainList
                                toReturn = toReturn And Run(actionContext, objSubActionTree)
                            Next
                        End If
                    Case ActionTreeMode.DecisionTree
                        If objActionTree.SubTrees.Count > 0 Then
                            Dim key As String = objActionTree.KeyExpression.Evaluate(actionContext, actionContext)
                            Dim subTree As ActionTree(Of TEngineEvents) = Nothing
                            If objActionTree.SubTrees.TryGetValue(key, subTree) Then
                                toReturn = toReturn And Run(actionContext, subTree)
                            End If
                        End If
                    Case ActionTreeMode.SynonymBranch
                        Dim targetTree As ActionTree(Of TEngineEvents) = Nothing
                        If objActionTree.Synonym IsNot Nothing AndAlso objActionTree.SubTrees.TryGetValue(objActionTree.Synonym, targetTree) Then
                            toReturn = toReturn And Run(actionContext, targetTree)
                        End If
                End Select
            End If
            Return toReturn
        End Function

    End Class



    'Public Class WebRequestCondition(Of TEngineEvents As IConvertible)

    '    Public ReadOnly Property FriendlyName As String
    '        Get
    '            Return String.Format("{0} - {1}", Me.WebPart, Me.Value)
    '        End Get
    '    End Property

    '    Public Property WebPart As String

    '    Public Property Value As String




    '    Public Overrides Function GetHashCode() As Integer
    '        Return (WebPart & Value).GetHashCode
    '    End Function

    'End Class


End Namespace

