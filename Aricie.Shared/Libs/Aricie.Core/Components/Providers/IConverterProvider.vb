Imports System.ComponentModel

Namespace Providers
    ''' <summary>
    ''' Interface to provide a series of conversion capabilities
    ''' </summary>
    Public Interface IConverterProvider
        ReadOnly Property SourceTypes() As List(Of Type)

        Function GetDestinationTypes(ByVal sourceType As Type) As List(Of Type)

        Function GetConverter(ByVal sourceType As Type, ByVal destType As Type) As TypeConverter

        Function GetConverter(Of T As Class, Y As Class)() As Converter(Of T, Y)
    End Interface
End Namespace
