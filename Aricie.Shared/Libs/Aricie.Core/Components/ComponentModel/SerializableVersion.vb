Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Globalization

Namespace ComponentModel
    <Serializable> _
    <XmlType("Version")> _
    Public Class SerializableVersion
        Implements IComparable

        Public Sub New()
            Me.Version = Nothing
        End Sub

        Public Sub New(objVersion As Version)
            Me.Version = objVersion
        End Sub

        <XmlIgnore> _
        Public Property Version() As Version

        <XmlText> _
        <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
        Public Property StringValue() As String
            Get
                Return If(Me.Version Is Nothing, String.Empty, Me.Version.ToString())
            End Get
            Set(value As String)
                Dim temp As Version = Nothing
                TryParse(value, temp)
                Me.Version = temp
            End Set
        End Property

        Public Shared Widening Operator CType(objVersionXml As SerializableVersion) As Version
            Return objVersionXml.Version
        End Operator

        Public Shared Widening Operator CType(objVersion As Version) As SerializableVersion
            Return New SerializableVersion(objVersion)
        End Operator

        Public Overrides Function ToString() As String
            Return Me.StringValue
        End Function



        Public Shared Function TryParse(ByVal input As String, ByRef result As Version) As Boolean
            Dim versionResult As VersionResult = New VersionResult()
            versionResult.Init("input", False)
            Dim flag As Boolean = TryParseVersion(input, versionResult)
            result = versionResult.m_parsedVersion
            Return flag
        End Function


        Private Shared Function TryParseComponent(ByVal component As String, ByVal componentName As String, ByRef result As VersionResult, ByRef parsedComponent As Integer) As Boolean
            If (Not Integer.TryParse(component, NumberStyles.[Integer], CultureInfo.InvariantCulture, parsedComponent)) Then
                result.SetFailure(ParseFailureKind.FormatException, component)
                Return False
            End If
            If (parsedComponent >= 0) Then
                Return True
            End If
            result.SetFailure(ParseFailureKind.ArgumentOutOfRangeException, componentName)
            Return False
        End Function

        Private Shared Function TryParseVersion(ByVal version As String, ByRef result As VersionResult) As Boolean
            Dim num As Integer
            Dim num1 As Integer
            Dim num2 As Integer
            Dim num3 As Integer
            If (version Is Nothing) Then
                result.SetFailure(ParseFailureKind.ArgumentNullException)
                Return False
            End If
            Dim strArrays As String() = version.Split(New Char() {"."c})
            Dim length As Integer = CInt(strArrays.Length)
            If (length < 2 OrElse length > 4) Then
                result.SetFailure(ParseFailureKind.ArgumentException)
                Return False
            End If
            If (Not TryParseComponent(strArrays(0), "version", result, num)) Then
                Return False
            End If
            If (Not TryParseComponent(strArrays(1), "version", result, num1)) Then
                Return False
            End If
            length = length - 2
            If (length <= 0) Then
                result.m_parsedVersion = New System.Version(num, num1)
            Else
                If (Not TryParseComponent(strArrays(2), "build", result, num2)) Then
                    Return False
                End If
                length = length - 1
                If (length <= 0) Then
                    result.m_parsedVersion = New System.Version(num, num1, num2)
                Else
                    If (Not TryParseComponent(strArrays(3), "revision", result, num3)) Then
                        Return False
                    End If
                    result.m_parsedVersion = New System.Version(num, num1, num2, num3)
                End If
            End If
            Return True
        End Function

        Friend Enum ParseFailureKind
            ArgumentNullException
            ArgumentException
            ArgumentOutOfRangeException
            FormatException
        End Enum

        Friend Structure VersionResult
            Friend m_parsedVersion As Version

            Friend m_failure As ParseFailureKind

            Friend m_exceptionArgument As String

            Friend m_argumentName As String

            Friend m_canThrow As Boolean

            Friend Function GetVersionParseException() As System.Exception
                Select Case Me.m_failure
                    Case ParseFailureKind.ArgumentNullException
                        Return New ArgumentNullException(Me.m_argumentName)
                    Case ParseFailureKind.ArgumentException
                        Return New ArgumentException("Wrong version formating")
                    Case ParseFailureKind.ArgumentOutOfRangeException
                        Return New ArgumentOutOfRangeException(Me.m_exceptionArgument, "Version out of range")
                    Case ParseFailureKind.FormatException
                        Try
                            Integer.Parse(Me.m_exceptionArgument, CultureInfo.InvariantCulture)
                        Catch formatException As System.FormatException
                            Return formatException
                        Catch overflowException As System.OverflowException
                            Return overflowException
                        End Try
                End Select
                    Return New ArgumentException("Wrong version formating")
            End Function

            Friend Sub Init(ByVal argumentName As String, ByVal canThrow As Boolean)
                Me.m_canThrow = canThrow
                Me.m_argumentName = argumentName
            End Sub

            Friend Sub SetFailure(ByVal failure As ParseFailureKind)
                Me.SetFailure(failure, String.Empty)
            End Sub

            Friend Sub SetFailure(ByVal failure As ParseFailureKind, ByVal argument As String)
                Me.m_failure = failure
                Me.m_exceptionArgument = argument
                If (Me.m_canThrow) Then
                    Throw Me.GetVersionParseException()
                End If
            End Sub
        End Structure


        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            Dim toCompare As SerializableVersion = TryCast(obj, SerializableVersion)
            If toCompare Is Nothing Then
                Throw New ArgumentException("Can't compare serializable version with distinct object", "obj")
            End If
            Return Me.Version.CompareTo(toCompare)
        End Function
    End Class
End NameSpace