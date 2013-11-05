


Namespace UI.Attributes


    Public Class EnableExportsAttribute
        Inherits Attribute

        Public Sub New()

        End Sub

        Public Sub New(enable As Boolean)
            Me.Enabled = enable
        End Sub

        Public Property Enabled As Boolean

    End Class


End Namespace


