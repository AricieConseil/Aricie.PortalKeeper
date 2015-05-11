Imports Aricie.DNN.UI.Attributes

Namespace Entities

    Public Class EnabledFeature(Of T As {New})

        Private _Entity As T

        Public Sub New()
        End Sub

        Public Sub New(objSimple As T)
            Me._Entity = objSimple
        End Sub

        <AutoPostBack()> _
        Public Property Enabled As Boolean

        <ConditionalVisible("Enabled", False, True)>
        Public Property Entity As T
            Get
                If _Entity Is Nothing AndAlso _Enabled Then
                    _Entity = New T
                End If
                Return _Entity
            End Get
            Set(value As T)
                _Entity = value
            End Set
        End Property
    End Class
End Namespace