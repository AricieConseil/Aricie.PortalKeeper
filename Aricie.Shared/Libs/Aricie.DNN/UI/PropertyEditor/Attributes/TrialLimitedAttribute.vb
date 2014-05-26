Imports Aricie.DNN.Security.Trial
Imports DotNetNuke.Security
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services

Namespace UI.Attributes

    Public Class AccessLimitationAttribute
        Inherits LimitationAttribute

        Public Sub New()
        End Sub

        Public Sub New(ByVal valUnlimitedAccessLevel As SecurityAccessLevel, ByVal valLimitationMode As TrialPropertyMode)
            Me.UnlimitedAccessLevel = valUnlimitedAccessLevel
            Me.LimitationMode = valLimitationMode
        End Sub

        Public Property UnlimitedAccessLevel As SecurityAccessLevel


        Public Overrides Function IsLimited(ape As AriciePropertyEditorControl) As Boolean
            Return Not PortalSecurity.HasNecessaryPermission(Me.UnlimitedAccessLevel, ape.ParentModule.PortalSettings, ape.ParentModule.ModuleConfiguration)
        End Function
    End Class


    Public MustInherit Class LimitationAttribute
        Inherits Attribute


        Public Sub New()
        End Sub

        Public Sub New(ByVal valLimitationMode As TrialPropertyMode)
            Me._LimitationMode = valLimitationMode
        End Sub

        Public Property LimitationMode() As TrialPropertyMode

        Public MustOverride Function IsLimited(ape As AriciePropertyEditorControl) As Boolean


    End Class



    <AttributeUsage(AttributeTargets.Property)> _
    Public Class TrialLimitedAttribute
        Inherits LimitationAttribute


        Public Shared ReadOnly TrialModeLimitedKey As String = "TrialMode.Disabled"

        Private _TrialPropertyMode As TrialPropertyMode

        Public Sub New()
        End Sub

        Public Sub New(ByVal trialPropertyMode As TrialPropertyMode)
            MyBase.New(trialPropertyMode)
        End Sub

        Public Property TrialPropertyMode() As TrialPropertyMode
            Get
                Return Me.LimitationMode
            End Get
            Set(ByVal value As TrialPropertyMode)
                Me.LimitationMode = value
            End Set
        End Property

        Public Overrides Function IsLimited(ape As AriciePropertyEditorControl) As Boolean
            Return ape.TrialStatus.IsLimited()
        End Function
    End Class
End Namespace