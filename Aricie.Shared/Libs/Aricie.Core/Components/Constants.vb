Imports System.Text
Imports System.Globalization
Imports System.Xml.Serialization


Public Class Constants

    Public Const glbPrefix As String = "Aricie"
    Public Const glbKeyPrefix As String = glbPrefix & "€"c


    Public Shared Function GetKey(Of T, H)(ByVal ParamArray args() As String) As String

        Array.Resize(args, args.Length + 1)
        args(args.Length - 1) = GetType(H).Name
        Return GetKey(Of T)(args)

    End Function

    Public Shared Function GetKey(Of T)(ByVal ParamArray args() As String) As String
        Array.Resize(Of String)(args, args.Length + 1)
        Dim myType As Type = GetType(T)
        args(args.Length - 1) = myType.FullName
        Dim toReturn As String = GetKey(args)

        Return toReturn

    End Function

    Public Shared Function GetKey(ByVal ParamArray args() As String) As String

        Dim toReturn As String = glbKeyPrefix & String.Join("€", args)
        If toReturn.Length > 46 Then
            toReturn = toReturn.Substring(0, 36) & GetStringHashCode(toReturn).ToString(CultureInfo.InvariantCulture)
        End If
        Return toReturn
    End Function

#Region "legacy versions"

    Public Shared Function GetKeyDotNet(Of T, H)(ByVal ParamArray args() As String) As String

        Array.Resize(args, args.Length + 1)
        args(args.Length - 1) = GetType(H).Name
        Return GetKeyDotNet(Of T)(args)

    End Function

    Public Shared Function GetKeyDotNet(Of T)(ByVal ParamArray args() As String) As String
        Array.Resize(Of String)(args, args.Length + 1)
        Dim myType As Type = GetType(T)
        args(args.Length - 1) = myType.FullName
        Dim toReturn As String = GetKeyDotNet(args)

        Return toReturn

    End Function

    Public Shared Function GetKeyDotNet(ByVal ParamArray args() As String) As String

        Dim toReturn As String = glbKeyPrefix & String.Join("€", args)
        If toReturn.Length > 46 Then
            toReturn = toReturn.Substring(0, 36) & toReturn.GetHashCode.ToString(CultureInfo.InvariantCulture)
        End If
        Return toReturn
    End Function

    Public Shared Function GetKey1(ByVal ParamArray args() As String) As String

        Dim prefix As String = "Aricie.Core"
        Dim toReturn As String = "--" & prefix

        If Not args Is Nothing Then
            For Each arg As String In args
                toReturn &= arg & "--" & toReturn

            Next
        End If
        Return DirectCast(IIf(toReturn.Length < 64, toReturn, toReturn.Substring(0, CInt(toReturn.Length / 4)) & toReturn.GetHashCode.ToString), String)

    End Function

    Public Shared Function GetKey1(Of T, H As Class)(ByVal ParamArray args() As String) As String

        Array.Resize(args, args.Length + 1)
        args(args.Length - 1) = GetType(H).Name
        Return GetKey1(Of T)(args)

    End Function

    Public Shared Function GetKey1(Of T)(ByVal ParamArray args() As String) As String

        Array.Resize(args, args.Length + 1)
        args(args.Length - 1) = GetType(T).FullName
        Dim toReturn As String = GetKey1(args)

        Return toReturn

    End Function

    Public Shared Function GetKey2(Of T)(Optional ByVal localVars As Dictionary(Of String, String) = Nothing) As String
        If localVars Is Nothing Then
            localVars = New Dictionary(Of String, String)
        End If

        Dim toReturn As String = GetKey2(localVars) & GetType(T).Name & "-"

        Return toReturn
    End Function

    Public Shared Function GetKey2(Optional ByVal localVars As Dictionary(Of String, String) = Nothing) As String
        Dim toReturn As String = "AricieMW-" & "-"
        If Not localVars Is Nothing Then
            For Each key As String In localVars.Keys
                toReturn &= key.ToString & "-" & localVars(key) & "-"
            Next
        End If

        Return IIf(toReturn.Length < 64, toReturn, toReturn.GetHashCode).ToString
    End Function

    Public Shared Function GetKey3(Of T)(ByVal ParamArray args() As String) As String

        Array.Resize(Of String)(args, args.Length + 1)
        Dim myType As Type = GetType(T)
        args(args.Length - 1) = myType.FullName

        ' ancienne version de GetKey buggué sur les longues clés
        Dim toReturnBuilder As New StringBuilder(glbPrefix)
        If Not args Is Nothing Then
            For Each arg As String In args
                toReturnBuilder.Append("€")
                toReturnBuilder.Append(arg)
            Next
        End If
        Dim toReturn As String = toReturnBuilder.ToString
        If toReturn.Length > 50 Then
            toReturn = toReturn.Substring(0, 38) & toReturn.GetHashCode.ToString(CultureInfo.InvariantCulture)
        End If
        Return toReturn

        Return toReturn

    End Function

#End Region


    Public Const ConstSiteAnchor As String = "<a href=""http://www.aricie.com"">www.aricie.com</a>"

    Public Shared ReadOnly UnlimitedTimeSpan As TimeSpan = TimeSpan.MaxValue

    Public Shared ReadOnly NoExpirationDate As Date = Date.MaxValue

    Public Shared ReadOnly UnlimitedAmount As Integer = Integer.MaxValue

    Public Shared ReadOnly LockTimeOut As TimeSpan = TimeSpan.FromMilliseconds(20)

    ' Image Request parameters

    Public Class HTTP
        Public Const ModuleID As String = "mid"
        Public Const CacheKey As String = "strHS"
        Public Const ShowUnapproved As String = "sna"
        Public Const DisplayStatus As String = "DisplayStatus"
        Public Const TargetModuleId As String = "TMID"
    End Class


    Public Class Business
        Public Const Version As String = "01.00.00"

        '############################# Limited Version
        Public Shared ReadOnly LimitedVersion As Boolean = False

        Public Shared ReadOnly ConstHostSettingsKey As String = "Aricie"
        Public Shared ReadOnly ConstLimitingKey As String = ConstHostSettingsKey & "Limit"
        Public Shared ReadOnly ConstNbofDays As Integer = 30

        '#############################

        Public Enum VersionTransition
            Minor
            Major
        End Enum
    End Class

    Public Class Layout
        Public Shared ReadOnly NbPageMax As Integer = 10
    End Class

    Class Settings
        Public Shared ReadOnly ProfileNamingContainer As String = glbPrefix
        Public Shared ReadOnly ProfileKey As String = glbPrefix & "Profile"
        Public Shared ReadOnly XmlRootAttributeType As Type = GetType(XmlRootAttribute)

        Public Shared Function GetXMLSettingsTerminator(Of T As Class)() As String
            Dim objType As Type = GetType(T)
            Dim tagName As String = objType.Name
            Dim customAttribute As XmlRootAttribute = DirectCast(Attribute.GetCustomAttribute(objType, XmlRootAttributeType), XmlRootAttribute)
            If (customAttribute IsNot Nothing) Then
                tagName = customAttribute.ElementName
            End If
            Return "</" & tagName & ">"
        End Function

        Public Shared ReadOnly DefaultColor As String = "OrangeRed"
    End Class

    Public Class Content

        Public Const EmailValidator As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"

    End Class

    Public Class Cache
        Public Shared ReadOnly GlobalExpiration As TimeSpan = TimeSpan.FromHours(1)
        Public Shared ReadOnly NoExpiration As TimeSpan = UnlimitedTimeSpan
        Public Const Dependency As String = glbPrefix & "Depend-"
    End Class

    Public Class Security
        Public Shared ReadOnly PermissionCode As String = glbPrefix
        Public Shared ReadOnly HostSettingsKeySuffix As String = glbPrefix & "PermissionsSet"
        Public Shared ReadOnly HostSettingsFlag As String = glbPrefix & Business.Version
    End Class
End Class


