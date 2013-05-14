Imports System.ComponentModel
Imports System.Globalization

Namespace Business
    ''' <summary>
    ''' adapter between IConverter and TypeConverter
    ''' </summary>
    Public Class SimpleTypeConverter(Of T As {Class}, Y As {Class})
        Inherits TypeConverter

        Private _Converter As Converter(Of T, Y)

        Public Sub New(ByVal converter As Converter(Of T, Y))
            MyBase.New()
            _Converter = converter
        End Sub

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, _
                                                  ByVal sourceType As Type) As Boolean
            Return (sourceType Is GetType(T)) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, _
                                                ByVal destinationType As Type) As Boolean
            Return (destinationType Is GetType(Y)) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, _
                                               ByVal culture As CultureInfo, ByVal value As Object) As Object
            Return IIf(value Is GetType(T), _Converter.Invoke(DirectCast(value, T)), MyBase.ConvertFrom(value))
        End Function

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, _
                                             ByVal culture As CultureInfo, _
                                             ByVal value As Object, _
                                             ByVal destinationType As Type) As Object
            Return _Converter.Invoke(DirectCast(value, T))
            Return _
                IIf(value Is GetType(T) And destinationType.FullName = GetType(Y).FullName, _
                     _Converter.Invoke(DirectCast(value, T)), MyBase.ConvertFrom(value))
        End Function
    End Class
End Namespace
