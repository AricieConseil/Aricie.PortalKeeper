Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Sitemap, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Sub Conditions Provider")> _
    <Description("Allows to build a condition tree with sub conditions")> _
    Public Class SubConditionsProvider(Of TEngineEvents As IConvertible)
        Inherits DosEnabledConditionProvider(Of TEngineEvents)


        Private _KeeperCondition As New KeeperCondition(Of TEngineEvents)

        <ExtendedCategory("Condition")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property KeeperCondition() As KeeperCondition(Of TEngineEvents)
            Get
                Return _KeeperCondition
            End Get
            Set(ByVal value As KeeperCondition(Of TEngineEvents))
                _KeeperCondition = value
            End Set
        End Property


        Public Function MatchInternal(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return _KeeperCondition.Match(context)

        End Function

        Public Overrides Function FastGetKey(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal clue As Object) As String
            Dim objStr As String = ""
            Dim clueList As Dictionary(Of Integer, Object) = DirectCast(clue, Dictionary(Of Integer, Object))
            For Each entry As KeyValuePair(Of Integer, Object) In clueList
                objStr &= DirectCast(Me._KeeperCondition.Instances(entry.Key).GetProvider, IDoSEnabledConditionProvider(Of TEngineEvents)).FastGetKey(context, entry.Value)
            Next
            Return objStr
        End Function

        Public Overloads Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents), ByRef clue As Object, ByRef key As String) As Boolean
            If Me.DebuggerBreak Then
                CallDebuggerBreak()
            End If
            If Not Me.EnableDoSProtection Then
                Return Me.MatchInternal(context)
            Else
                Dim clueDico As New Dictionary(Of Integer, Object)
                key = ""
                Dim toReturn As Boolean = False
                Dim mandatoryMissing As Boolean
                Dim currProviderSettings As ConditionProviderSettings(Of TEngineEvents) = Nothing
                Dim currKey As String = Nothing
                Dim currClue As Object = Nothing
                Dim currMatch As Boolean
                For i As Integer = 0 To Me.KeeperCondition.Instances.Count - 1
                    currProviderSettings = Me.KeeperCondition.Instances(i)
                    Dim objProvider As IConditionProvider(Of TEngineEvents) = currProviderSettings.GetProvider
                    If TypeOf objProvider Is IDoSEnabledConditionProvider(Of TEngineEvents) Then
                        Dim dosProvider As IDoSEnabledConditionProvider(Of TEngineEvents) = DirectCast(objProvider, IDoSEnabledConditionProvider(Of TEngineEvents))
                        currMatch = dosProvider.Match(context, currClue, key) Xor currProviderSettings.Negate
                        key &= currKey
                        clueDico.Add(i, currClue)
                        If Not mandatoryMissing Then
                            If currProviderSettings.IsMandatory AndAlso Not currMatch Then
                                toReturn = False
                                mandatoryMissing = True
                            ElseIf (Not toReturn) AndAlso currMatch Then
                                toReturn = True
                            End If
                        End If
                    End If
                Next
                clue = clueDico
                Return toReturn
            End If
        End Function
    End Class
End Namespace