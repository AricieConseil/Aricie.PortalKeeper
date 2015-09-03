Imports Aricie.Collections

Namespace Services
    <Obsolete("User IContextLookup")> _
    Public Interface IContextItems

        Property Items() As Dictionary(Of String, Object)

    End Interface

    ''' <summary>
    ''' Interface for dictionary collection of items in a context
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IContextLookup

        ReadOnly Property Items() As IDictionary(Of String, Object)


    End Interface

    Public Interface IContextOwnerProvider

        Property ContextOwner As Object

    End Interface

    ''' <summary>
    ''' Simple context implementation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SimpleContextLookup
        Implements IContextLookup

        Public Sub New()
            Me._Items = New SerializableDictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        End Sub

        Public Sub New(existingLookup As IContextLookup)
            If existingLookup IsNot Nothing Then
                Me._Items = New SerializableDictionary(Of String, Object)(existingLookup.Items)
            Else
                Me._Items = New SerializableDictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            End If
        End Sub

        Private _Items As SerializableDictionary(Of String, Object)

        Public Property Items() As SerializableDictionary(Of String, Object)
            Get
                Return _Items
            End Get
            Set(ByVal value As SerializableDictionary(Of String, Object))
                _Items = value
            End Set
        End Property


        Public ReadOnly Property ReadOnlyItems As System.Collections.Generic.IDictionary(Of String, Object) Implements IContextLookup.Items
            Get
                Return _Items
            End Get
        End Property
    End Class


End Namespace