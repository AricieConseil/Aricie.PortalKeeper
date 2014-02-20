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

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Sitemap, IconOptions.Normal)> _
    <DefaultProperty("Summary")> _
    <Serializable()> _
    Public Class KeeperRule(Of TEngineEvents As IConvertible)
        Inherits NamedConfig

        Public Sub New()

        End Sub

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property Summary As String
            Get
                Return Me.Name.PadRight(50) & " - " & IIf(Me.Enabled, "Enabled", "Disabled").ToString.PadRight(30)
            End Get
        End Property

        <ExtendedCategory("Policy")> _
       <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
       <LabelMode(LabelMode.Top)> _
        Public Property Action() As KeeperAction(Of TEngineEvents) = New KeeperAction(Of TEngineEvents)

        <ExtendedCategory("Condition")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
                <LabelMode(LabelMode.Top)> _
                <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        Public Property Condition() As KeeperCondition(Of TEngineEvents) = New KeeperCondition(Of TEngineEvents)

        <ExtendedCategory("RuleSettings")> _
        Public Property MatchingLifeCycleEvent() As TEngineEvents

        <ExtendedCategory("RuleSettings")> _
        Public Property StopRule() As Boolean

        Public Function RunActions(ByVal keeperContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            keeperContext.CurrentRule = Me
            Return Me.Action.Run(keeperContext)
        End Function

    End Class

    
End Namespace


