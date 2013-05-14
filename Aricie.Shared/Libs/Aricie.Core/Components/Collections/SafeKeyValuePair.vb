Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Xml.Schema
Imports System.Xml
Imports Aricie.Services
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Collections


    ''' <summary>
    ''' Interface for a item with a key generating mechanism
    ''' </summary>
    Public Interface IKeyedItem(Of TKey)


        Function GetKey() As TKey


    End Interface


    ''' <summary>
    ''' Fixed KeyValuePair Structure with consistent operator overloads
    ''' </summary>
    <Serializable()> _
    Public Structure SafeKeyValuePair(Of TKey, TValue)
        Implements IKeyedItem(Of TKey)



        Public Key As TKey
        Public Value As TValue
        Public KeyComparer As IEqualityComparer(Of TKey)
        Public ValueComparer As IEqualityComparer(Of TValue)

        Public Sub New(ByVal key As TKey, ByVal value As TValue)
            Me.New(key, value, Nothing, Nothing)
        End Sub

        Public Sub New(ByVal key As TKey, ByVal value As TValue, ByVal keyComparer As IEqualityComparer(Of TKey))
            Me.New(key, value, keyComparer, Nothing)
        End Sub

        Public Sub New(ByVal key As TKey, ByVal value As TValue, ByVal keyComparer As IEqualityComparer(Of TKey), ByVal valueComparer As IEqualityComparer(Of TValue))
            Me.Key = key
            Me.Value = value
            If keyComparer Is Nothing Then
                Me.KeyComparer = EqualityComparer(Of TKey).Default
            Else
                Me.KeyComparer = keyComparer
            End If
            If valueComparer Is Nothing Then
                Me.ValueComparer = EqualityComparer(Of TValue).Default
            Else
                Me.ValueComparer = valueComparer
            End If
        End Sub

        Public Function GetKey() As TKey Implements IKeyedItem(Of TKey).GetKey
            Return Me.Key
        End Function


        Public Overrides Function ToString() As String
            Dim builder As New StringBuilder
            builder.Append("["c)
            If (Not Me.Key Is Nothing) Then
                builder.Append(Me.Key.ToString)
            End If
            builder.Append(", ")
            If (Not Me.Value Is Nothing) Then
                builder.Append(Me.Value.ToString)
            End If
            builder.Append("]"c)
            Return builder.ToString
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is SafeKeyValuePair(Of TKey, TValue) Then
                Return False
            End If
            Dim toCompare As SafeKeyValuePair(Of TKey, TValue) = DirectCast(obj, SafeKeyValuePair(Of TKey, TValue))
            Return Me.KeyComparer.Equals(Me.Key, toCompare.Key) AndAlso Me.ValueComparer.Equals(Me.Value, toCompare.Value)
            'Return (Not (Me.Key Is Nothing Xor toCompare.Key Is Nothing)) AndAlso (Me.Key Is Nothing OrElse Me.KeyComparer.Equals(Me.Key, toCompare.Key)) _
            '            AndAlso _
            '        (Not (Me.Value Is Nothing Xor obj.Value Is Nothing)) AndAlso (Me.Value Is Nothing OrElse Me.ValueComparer.Equals(Me.Value, toCompare.Value))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim toReturn As Integer = 0
            If Me.Key IsNot Nothing Then
                toReturn = toReturn Xor Me.KeyComparer.GetHashCode(Me.Key)
            End If
            If Me.Value IsNot Nothing Then
                toReturn = toReturn Xor Me.ValueComparer.GetHashCode(Me.Value)
            End If
            Return toReturn
        End Function

        Public Shared Function Convert(ByVal item As SafeKeyValuePair(Of TKey, TValue)) As KeyValuePair(Of TKey, TValue)
            Return New KeyValuePair(Of TKey, TValue)(item.Key, item.Value)
        End Function


    End Structure

End Namespace








