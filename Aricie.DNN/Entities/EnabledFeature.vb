Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Services

Namespace Entities

    Public Class EnabledFeature(Of T)
        Implements IEnabled

        Private _Entity As T

        Public Sub New()
        End Sub

        Public Sub New(objSimple As T)
            Me._Entity = objSimple
        End Sub

        Public Sub New(objSimple As T, enabled As Boolean)
            Me.New(objSimple)
            Me.Enabled = enabled
        End Sub

        <AutoPostBack()> _
        Public Property Enabled As Boolean Implements IEnabled.Enabled

        <ConditionalVisible("Enabled", False, True)>
        Public Property Entity As T
            Get
                If _Entity Is Nothing AndAlso _Enabled Then
                    _Entity = ReflectionHelper.CreateObject(Of T)()
                End If
                Return _Entity
            End Get
            Set(value As T)
                _Entity = value
            End Set
        End Property
    End Class
End Namespace