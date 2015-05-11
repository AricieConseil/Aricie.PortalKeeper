Imports Aricie.DNN.UI.Attributes

Namespace Entities

    <Flags()> _
    Public Enum BoundsMode
        UnBounded = 0
        SetMinimum = 1
        SetMaximum = 2
    End Enum

    <Serializable()> _
    Public Class Range(Of T)

        Private Shared ReadOnly DefaultComparer As Comparer(Of T) = Generic.Comparer(Of T).Default

        Public Sub New()

        End Sub

        Public Sub New(initMin As T, initMax As T)
            Me.Minimum = initMin
            Me.Maximum = initMax
        End Sub

        Public Property Mode As BoundsMode = BoundsMode.UnBounded

        '<Editor("DotNetNuke.UI.WebControls.VersionEditControl, DotNetNuke", GetType(DotNetNuke.UI.WebControls.EditControl))> _
        <ConditionalVisible("Mode", False, True, BoundsMode.SetMinimum)> _
        Public Property Minimum As T

        <ConditionalVisible("Mode", False, True, BoundsMode.SetMaximum)> _
        Public Property Maximum As T

        Public Function Validate(value As T) As Boolean

            Return (((Me.Mode And BoundsMode.SetMaximum) = BoundsMode.UnBounded) _
                    OrElse DefaultComparer.Compare(value, Maximum) <= 0) _
                   AndAlso (((Me.Mode And BoundsMode.SetMinimum) = BoundsMode.UnBounded) _
                            OrElse DefaultComparer.Compare(value, Minimum) >= 0)

        End Function


    End Class
End Namespace