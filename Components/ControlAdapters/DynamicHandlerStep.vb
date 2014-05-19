Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Structure DynamicHandlerStep
        Implements IConvertible


        Sub New(objControlStep As ControlStep)
            Me.MainControlStep = objControlStep
            Me.ChildControlId = String.Empty
            Me.ControlEvent = String.Empty
        End Sub

        Sub New(objControlStep As ControlStep, strEvent As String)
            Me.MainControlStep = objControlStep
            Me.ChildControlId = String.Empty
            Me.ControlEvent = strEvent
        End Sub

        Sub New(objControlStep As ControlStep, strControl As String, strEvent As String)
            Me.MainControlStep = objControlStep
            Me.ChildControlId = strControl
            Me.ControlEvent = strEvent
        End Sub

        Public MainControlStep As ControlStep

        Public ChildControlId As String

        Public ControlEvent As String

        Public Overrides Function GetHashCode() As Integer
            Return MainControlStep.GetHashCode() Xor ChildControlId.GetHashCode() Xor ControlEvent.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is DynamicHandlerStep Then
                Dim objDynamicHandlerStep As DynamicHandlerStep = DirectCast(obj, DynamicHandlerStep)
                Return ((Me.MainControlStep = objDynamicHandlerStep.MainControlStep) AndAlso (Me.ChildControlId = objDynamicHandlerStep.ChildControlId) AndAlso (Me.ControlEvent = objDynamicHandlerStep.ControlEvent))
            End If
            Return False
        End Function

        Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
            Return String.Empty.GetTypeCode()
        End Function

        Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
            Return Me.ControlEvent <> String.Empty
        End Function

        Private Const NotImplementedMessage As String = "IConvertible is only supported in DynamicHandlerStep for strings and int32"

        Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToDateTime(provider As IFormatProvider) As Date Implements IConvertible.ToDateTime
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
            Return GetHashCode()
        End Function

        Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
            Return GetHashCode()
        End Function

        Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
            Return GetHashCode()
        End Function

        Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
            Return GetHashCode()
        End Function

        Public Overloads Function ToString(provider As IFormatProvider) As String Implements IConvertible.ToString
            Return String.Format("{0};{1};{2}", Me.MainControlStep.ToString(), Me.ChildControlId.ToString(provider), Me.ControlEvent.ToString(provider))
        End Function

        Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function

        Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
            Throw (New NotImplementedException(NotImplementedMessage))
        End Function



    End Structure
End NameSpace