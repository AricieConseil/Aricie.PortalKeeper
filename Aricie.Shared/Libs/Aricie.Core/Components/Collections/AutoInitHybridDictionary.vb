Namespace Collections
    ''' <summary>
    ''' Base class for a Hybrid Dictionary with an automated init hook
    ''' </summary>
    Public MustInherit Class AutoInitHybridDictionary(Of Tkey, TValue)
        Inherits HybridDictionary(Of Tkey, TValue)
        Implements IAutoInitDictionary(Of Tkey, TValue)


        Private _AutoInitOnLookup As Boolean

        Public ReadOnly Property AutoInitOnLookup() As Boolean Implements IAutoInitDictionary(Of Tkey, TValue).AutoInitOnLookup
            Get
                Return Me._AutoInitOnLookup
            End Get
        End Property

        Public Sub New()
            Me.New(False, 0, Nothing, 0)
        End Sub

        Public Sub New(ByVal autoInitOnLookup As Boolean, ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of Tkey))
            Me.New(autoInitOnLookup, capacity, comparer, 0)
        End Sub

        Public Sub New(ByVal autoInitOnLookup As Boolean, ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of Tkey), ByVal dictionaryCreationThreshold As Integer)
            MyBase.New(capacity, comparer, dictionaryCreationThreshold)
            Me._AutoInitOnLookup = True
        End Sub

        Public Overrides Function ContainsKey(ByVal key As Tkey) As Boolean
            If MyBase.ContainsKey(key) Then
                Return True
            End If
            If Me._AutoInitOnLookup Then
                Me.Add(key, Me.GetNewValue)
            End If
            Return False
        End Function

        Protected MustOverride Function GetNewValue() As TValue Implements IAutoInitDictionary(Of Tkey, TValue).GetNewValue



    End Class
End NameSpace