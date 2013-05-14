Imports System.ComponentModel

Namespace Providers
    ''' <summary>
    ''' Base class for a init based implementation of IConverterProvider
    ''' </summary>
    Public MustInherit Class ConverterProviderBase
        Implements IConverterProvider


        Private defaultValues As New Dictionary(Of String, Object)

        Private _Conversions As Dictionary(Of Type, Dictionary(Of Type, TypeConverter))

        Public ReadOnly Property Conversions() As Dictionary(Of Type, Dictionary(Of Type, TypeConverter))
            Get
                If Me._Conversions Is Nothing Then
                    Me._Conversions = New Dictionary(Of Type, Dictionary(Of Type, TypeConverter))
                    Me.InitConversions()
                End If
                Return Me._Conversions
            End Get
        End Property


        Public MustOverride Sub InitConversions()


        Public Sub AddConversion(ByVal sourceType As Type, ByVal destType As Type, ByVal converter As TypeConverter)
            If Not Conversions.ContainsKey(sourceType) Then
                Conversions.Add(sourceType, New Dictionary(Of Type, TypeConverter))
            End If
            Conversions(sourceType)(destType) = converter
        End Sub

        Public Function GetConverter(ByVal sourceType As Type, ByVal destType As Type) As TypeConverter _
            Implements IConverterProvider.GetConverter
            If Conversions.ContainsKey(sourceType) AndAlso Conversions(sourceType).ContainsKey(destType) Then
                Return Conversions(sourceType)(destType)
            End If
            Return New TypeConverter
        End Function

        Public Function GetDestinationTypes(ByVal sourceType As Type) As List(Of Type) _
            Implements IConverterProvider.GetDestinationTypes
            If Conversions.ContainsKey(sourceType) Then
                Return New List(Of Type)(Conversions(sourceType).Keys)
            End If
            Return New List(Of Type)
        End Function

        Public ReadOnly Property SourceTypes() As List(Of Type) Implements IConverterProvider.SourceTypes
            Get
                Return New List(Of Type)(Conversions.Keys)
            End Get
        End Property

        Public Function Convert(Of T As Class, Y As Class)(ByVal sourceObj As T) As Y
            Return DirectCast(Me.Conversions(GetType(T))(GetType(Y)).ConvertTo(sourceObj, GetType(Y)), Y)
        End Function


        Public Function GetConverter(Of T As Class, Y As Class)() As Converter(Of T, Y) _
            Implements IConverterProvider.GetConverter
            Return New Converter(Of T, Y)(AddressOf Me.Convert(Of T, Y))
        End Function
    End Class
End Namespace
