Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Globalization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Sitemap, IconOptions.Normal)> _
    <DefaultProperty("Summary")> _
    <Serializable()> _
    Public Class KeeperRule(Of TEngineEvents As IConvertible)
        Inherits NamedConfig
        Implements IExpressionVarsProvider



        Public Sub New()

        End Sub

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property Summary As String
            Get
                Return String.Format("{0}{1}{2}", Me.Name.PadRight(50), ComponentModel.UIConstants.TITLE_SEPERATOR, IIf(Me.Enabled, "Enabled", "Disabled").ToString.PadRight(30))
            End Get
        End Property

        <ExtendedCategory("Policy")> _
       <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
       <LabelMode(LabelMode.Top)> _
        Public Property Action() As KeeperAction(Of TEngineEvents) = New KeeperAction(Of TEngineEvents)

        <ExtendedCategory("Condition")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
                <LabelMode(LabelMode.Top)> _
                <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        Public Property Condition() As KeeperCondition(Of TEngineEvents) = New KeeperCondition(Of TEngineEvents)

        <Browsable(False)> _
        Public ReadOnly Property HasEvent As Boolean
            Get
                Return GetType(TEngineEvents) IsNot GetType(Boolean)
            End Get
        End Property

        <ConditionalVisible("HasEvent", False, True)> _
        Public Property MatchingLifeCycleEvent() As TEngineEvents

        <ExtendedCategory("RuleSettings")> _
        Public Property StopRule() As Boolean

        Public Function RunActions(ByVal keeperContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            keeperContext.CurrentRule = Me
            Return Me.Action.Run(keeperContext)
        End Function

        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            Me.Condition.AddVariables(currentProvider, existingVars)
            Me.Action.AddVariables(currentProvider, existingVars)
        End Sub

    End Class


End Namespace


