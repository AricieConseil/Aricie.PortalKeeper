Imports System.Xml.Serialization
Imports System.Xml.Schema
Imports System.Xml
Imports System.Reflection
Imports System.Security.Permissions
Imports System.ComponentModel
Imports System.ComponentModel.Design.Serialization
Imports System.Globalization

Namespace ComponentModel
    ''' <summary>
    ''' A class to represent a bloc of CData while being serialize.
    ''' </summary>
    ''' <remarks>Implements IConvertible for safe conversion to value types</remarks>
    <Serializable()> _
    <DefaultMember("Value")> _
    <TypeConverter(GetType(CData.CDataConverter))> _
    Public Class CData
        Implements IXmlSerializable
        Implements IConvertible


        ' Methods
        Public Sub New()
        End Sub

        Public Sub New(ByVal value As String)
            Me._value = value
        End Sub

        Public Shared Widening Operator CType(ByVal element As CData) As String
            If (Not element Is Nothing) Then
                Return element.Value
            End If
            Return String.Empty
        End Operator

        Public Shared Widening Operator CType(ByVal [text] As String) As CData
            Return New CData([text])
        End Operator

        Private Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Private Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            Me._value = reader.ReadElementContentAsString
        End Sub

        Private Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            writer.WriteCData(Me.Value)
        End Sub

        Public Overloads Overrides Function ToString() As String
            Return Me.Value
        End Function


        ' Properties
        Public Property Value() As String
            Get
                If Me._value IsNot Nothing Then
                    Return Me._value
                End If
                Return String.Empty
            End Get
            Set(ByVal value As String)
                Me._value = value
            End Set
        End Property


        ' Fields
        Private _value As String

        Public Function GetTypeCode() As System.TypeCode Implements System.IConvertible.GetTypeCode

            Return Type.GetTypeCode(GetType(String))
        End Function

        Public Function ToBoolean(ByVal provider As System.IFormatProvider) As Boolean Implements System.IConvertible.ToBoolean
            Return Convert.ToBoolean(Me._value, provider)
        End Function

        Public Function ToByte(ByVal provider As System.IFormatProvider) As Byte Implements System.IConvertible.ToByte
            Return Convert.ToByte(Me._value, provider)
        End Function

        Public Function ToChar(ByVal provider As System.IFormatProvider) As Char Implements System.IConvertible.ToChar
            Return Convert.ToChar(Me._value, provider)
        End Function

        Public Function ToDateTime(ByVal provider As System.IFormatProvider) As Date Implements System.IConvertible.ToDateTime
            Return Convert.ToDateTime(Me._value, provider)
        End Function

        Public Function ToDecimal(ByVal provider As System.IFormatProvider) As Decimal Implements System.IConvertible.ToDecimal
            Return Convert.ToDecimal(Me._value, provider)
        End Function

        Public Function ToDouble(ByVal provider As System.IFormatProvider) As Double Implements System.IConvertible.ToDouble
            Return Convert.ToDouble(Me._value, provider)
        End Function

        Public Function ToInt16(ByVal provider As System.IFormatProvider) As Short Implements System.IConvertible.ToInt16
            Return Convert.ToInt16(Me._value, provider)
        End Function

        Public Function ToInt32(ByVal provider As System.IFormatProvider) As Integer Implements System.IConvertible.ToInt32
            Return Convert.ToInt32(Me._value, provider)
        End Function

        Public Function ToInt64(ByVal provider As System.IFormatProvider) As Long Implements System.IConvertible.ToInt64
            Return Convert.ToInt64(Me._value, provider)
        End Function

        Public Function ToSByte(ByVal provider As System.IFormatProvider) As SByte Implements System.IConvertible.ToSByte
            Return Convert.ToSByte(Me._value, provider)
        End Function

        Public Function ToSingle(ByVal provider As System.IFormatProvider) As Single Implements System.IConvertible.ToSingle
            Return Convert.ToSingle(Me._value, provider)
        End Function

        Public Overloads Function ToString(ByVal provider As System.IFormatProvider) As String Implements System.IConvertible.ToString
            Return Me.Value
        End Function

        Public Function ToType(ByVal conversionType As System.Type, ByVal provider As System.IFormatProvider) As Object Implements System.IConvertible.ToType
            'Return Convert.DefaultToType(Me._value, conversionType, provider)
            If conversionType Is GetType(String) Then
                Return Me.Value
            End If
            Throw New InvalidCastException("CData is not configured to support that kind of conversion. You may want to edit the class")
        End Function

        Public Function ToUInt16(ByVal provider As System.IFormatProvider) As UShort Implements System.IConvertible.ToUInt16
            Return Convert.ToUInt16(Me._value, provider)
        End Function

        Public Function ToUInt32(ByVal provider As System.IFormatProvider) As UInteger Implements System.IConvertible.ToUInt32
            Return Convert.ToUInt32(Me._value, provider)
        End Function

        Public Function ToUInt64(ByVal provider As System.IFormatProvider) As ULong Implements System.IConvertible.ToUInt64
            Return Convert.ToUInt64(Me._value, provider)
        End Function







        <HostProtection(SecurityAction.LinkDemand, SharedState:=True)> _
        Public Class CDataConverter
            Inherits TypeConverter
            ' Methods
            Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
                Return ((sourceType Is GetType(String)) OrElse MyBase.CanConvertFrom(context, sourceType))
            End Function

            Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
                Return ((destinationType Is GetType(InstanceDescriptor)) _
                        OrElse (destinationType Is GetType(String)) _
                        OrElse (destinationType Is GetType(CData)) _
                        OrElse MyBase.CanConvertTo(context, destinationType))
            End Function

            Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
                If TypeOf value Is String Then
                    Return New CData(CStr(value).Trim)
                End If
                Return MyBase.ConvertFrom(context, culture, value)
            End Function

            Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
                If (destinationType Is Nothing) Then
                    Throw New ArgumentNullException("destinationType")
                End If
                If TypeOf value Is CData Then
                    If (destinationType Is GetType(InstanceDescriptor)) Then
                        Dim constructor As ConstructorInfo = GetType(CData).GetConstructor(New Type() {GetType(String)})
                        If (Not constructor Is Nothing) Then
                            Return New InstanceDescriptor(constructor, New Object() {value.ToString})
                        End If
                    ElseIf destinationType Is GetType(String) Then
                        Return DirectCast(value, CData).Value
                    End If
                End If
                Return MyBase.ConvertTo(context, culture, value, destinationType)
            End Function

        End Class







    End Class
End Namespace