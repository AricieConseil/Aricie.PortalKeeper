Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Xml.Schema
Imports System.Xml
Imports Aricie.Services
Imports System.Runtime.InteropServices
Imports System.Text



Namespace Collections


    ''' <summary>
    ''' Dictionary with double key/value/key index
    ''' </summary>
    <Serializable()> _
    Public Class DualDictionary(Of TKey, TValue)
        Inherits HybridDictionary(Of TKey, TValue)
        Implements IDualDictionary(Of TKey, TValue)


        'Implements IDictionary(Of TValue, IList(Of TKey))




        Private _Reverse As IReverseLookupDictionary(Of TKey, TValue)



#Region "cTors"

        Public Sub New()
            Me.New(False, Nothing, 0)
        End Sub

        Public Sub New(ByVal preInitOnReverseLookup As Boolean, ByVal keysComparer As IEqualityComparer(Of TKey))
            Me.New(preInitOnReverseLookup, keysComparer, 8)
        End Sub

        Public Sub New(ByVal preInitOnReverseLookup As Boolean, ByVal keysComparer As IEqualityComparer(Of TKey), ByVal keysDictionaryCreationThreshold As Integer)
            Me.New(preInitOnReverseLookup, keysComparer, keysDictionaryCreationThreshold, Nothing, 8)
        End Sub

        Public Sub New(ByVal preInitOnReverseLookup As Boolean, ByVal keysComparer As IEqualityComparer(Of TKey), ByVal keysDictionaryCreationThreshold As Integer, _
                          ByVal valuesComparer As IEqualityComparer(Of TValue), ByVal valuesDictionaryCreationThreshold As Integer)
            MyBase.New(0, keysComparer, keysDictionaryCreationThreshold)
            Me._Reverse = New ReverseLookupHybridDictionary(Of TKey, TValue)(Me, preInitOnReverseLookup, valuesComparer, valuesDictionaryCreationThreshold)
        End Sub





#End Region

        'todo, uncomment and update those methods

        Public Overrides Sub Add(ByVal key As TKey, ByVal value As TValue)
            MyBase.Add(key, value)
            AddReverseLookup(key, value, False)
        End Sub

        Public Overrides Function Remove(ByVal key As TKey) As Boolean
            Dim tempkeys As IList(Of TKey) = Nothing
            Dim toReturn As Boolean = True
            If Me._Reverse.TryGetValue(Me(key), tempkeys) Then
                toReturn = tempkeys.Remove(key)
            End If
            Return toReturn And MyBase.Remove(key)
        End Function

        Public Overrides Sub Clear()
            Me._Reverse.Clear()
            MyBase.Clear()
        End Sub

        Protected Overrides Sub SetItem(ByVal key As TKey, ByVal value As TValue)
            If Me.ContainsKey(key) Then
                Me.Remove(key)
            End If
            Me.Add(key, value)
            'MyBase.[Set](key, value)
        End Sub

        'Protected Overrides Sub SetItem(ByVal index As Integer, ByVal item As KeyValuePair(Of TKey, TValue))
        '    'If Not Me.Items(index).Equals(item) Then
        '    '    Me._Reverse(Me.Items(index).Value).Remove(Me.Items(index).Key)
        '    'End If
        '    'If Me._Reverse.ContainsKey(item.Value) AndAlso Me._Reverse(item.Value).Contains(item.Key) Then
        '    '    Me._Reverse(item.Value).Remove(item.Key)
        '    'End If
        '    'MyBase.SetItem(index, item)
        '    'AddReverseLookup(item, True)
        '    Throw New NotImplementedException
        'End Sub

#Region "Private methods"

        Private Sub AddReverseLookup(ByVal key As TKey, ByVal value As TValue, ByVal checkForExistence As Boolean)
            If (Not Me._Reverse.ContainsKey(value)) AndAlso (Not Me._Reverse.AutoInitOnLookup) Then
                InitValue(value)
            End If
            If (Not checkForExistence) OrElse Not Me._Reverse(value).Contains(key) Then
                Me._Reverse(value).Add(key)
            End If
        End Sub

        Private Sub InitValue(ByVal value As TValue)
            Me._Reverse.Add(value, New List(Of TKey))
        End Sub

#End Region


#Region "Implementations"



        Public ReadOnly Property Reverse() As IReverseLookupDictionary(Of TKey, TValue) Implements IDualDictionary(Of TKey, TValue).Reverse
            Get
                Return Me._Reverse
            End Get
        End Property


#End Region









    End Class

End Namespace




