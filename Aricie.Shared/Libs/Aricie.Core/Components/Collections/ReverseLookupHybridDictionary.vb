Namespace Collections
    ''' <summary>
    ''' Complete Dual Dictionary with init mechanism
    ''' </summary>
    <Serializable()> _
    Public Class ReverseLookupHybridDictionary(Of TKey, TValue)
        Inherits AutoInitHybridDictionary(Of TValue, IList(Of TKey))
        Implements IReverseLookupDictionary(Of TKey, TValue)

        Private _ForwardDictionary As IDualDictionary(Of TKey, TValue)

        Public Sub New(ByVal forwardDictionary As IDualDictionary(Of TKey, TValue))
            Me.New(forwardDictionary, False, Nothing, 0)
        End Sub

        Public Sub New(ByVal forwardDictionary As IDualDictionary(Of TKey, TValue), ByVal autoInitOnLookup As Boolean, ByVal comparer As IEqualityComparer(Of TValue))
            Me.New(forwardDictionary, autoInitOnLookup, comparer, 0)
        End Sub

        Public Sub New(ByVal forwardDictionary As IDualDictionary(Of TKey, TValue), ByVal autoInitOnLookup As Boolean, ByVal comparer As IEqualityComparer(Of TValue), ByVal dictionaryCreationThreshold As Integer)
            MyBase.New(autoInitOnLookup, 0, comparer, dictionaryCreationThreshold)
            Me._ForwardDictionary = forwardDictionary
        End Sub

        Protected Overrides Function GetNewValue() As System.Collections.Generic.IList(Of TKey)
            Return New List(Of TKey)
        End Function

        Public Overrides Function Remove(ByVal value As TValue) As Boolean

            If Me.ContainsKey(value) Then
                Dim toReturn As Boolean = True
                For Each key As TKey In Me(value)
                    toReturn = toReturn And Me._ForwardDictionary.Remove(key)
                Next
                Me.Item(value).Clear()
                Return toReturn
            End If
            Return False
        End Function



    End Class
End NameSpace