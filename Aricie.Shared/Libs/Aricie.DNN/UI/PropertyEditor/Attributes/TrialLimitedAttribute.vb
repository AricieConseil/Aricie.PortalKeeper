Imports Aricie.DNN.Security.Trial
Namespace UI.Attributes


    <AttributeUsage(AttributeTargets.Property)> _
    Public Class TrialLimitedAttribute
        Inherits Attribute

        Public Shared ReadOnly TrialModeLimitedKey As String = "TrialMode.Disabled"

        Private _TrialPropertyMode As TrialPropertyMode

        Public Sub New()
        End Sub

        Public Sub New(ByVal trialPropertyMode As TrialPropertyMode)
            Me._TrialPropertyMode = TrialPropertyMode
        End Sub

        Public Property TrialPropertyMode() As TrialPropertyMode
            Get
                Return _TrialPropertyMode
            End Get
            Set(ByVal value As TrialPropertyMode)
                _TrialPropertyMode = value
            End Set
        End Property

    End Class
End Namespace