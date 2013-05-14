


Namespace Collections

    ''' <summary>
    ''' Interface for a dicitionary with automated init capabilities
    ''' </summary>
    Public Interface IAutoInitDictionary(Of TKey, TValue)
        Inherits System.Collections.Generic.IDictionary(Of TKey, TValue)


        ReadOnly Property AutoInitOnLookup() As Boolean

        Function GetNewValue() As TValue


    End Interface

    ''' <summary>
    ''' Interface for a Dictionary with a reversed value/key lookup
    ''' </summary>
    Public Interface IReverseLookupDictionary(Of TKey, TValue)
        Inherits IAutoInitDictionary(Of TValue, IList(Of TKey))



    End Interface


    ''' <summary>
    ''' Interface for a double indexed dictionary
    ''' </summary>
    Public Interface IDualDictionary(Of TKey, TValue)
        Inherits System.Collections.Generic.IDictionary(Of TKey, TValue)

        ReadOnly Property Reverse() As IReverseLookupDictionary(Of TKey, TValue)

    End Interface

End Namespace

