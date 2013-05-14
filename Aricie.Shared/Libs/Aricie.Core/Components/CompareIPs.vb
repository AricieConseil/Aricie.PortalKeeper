Imports System.Net

''' <summary>
''' Helper function that can parse propertly IPAdresses for comparison
''' </summary>
Public Class CompareIPs
    ' Methods
    Public Shared Function AreEqual(ByVal IPAddress1 As String, ByVal IPAddress2 As String) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(IPAddress1) = CompareIPs.IPAddressToLongBackwards(IPAddress2))
    End Function

    Public Shared Function IPAddressToLong(ByVal strIPAddress As String) As UInt32
        Return IPAddressToLong(IPAddress.Parse(strIPAddress))
    End Function

    Public Shared Function IPAddressToLong(ByVal objIPAddress As IPAddress) As UInt32
        Dim addressBytes As Byte() = objIPAddress.GetAddressBytes
        Dim num As UInt32 = Convert.ToUInt32(addressBytes(3) << &H18)
        num = (num + Convert.ToUInt32(addressBytes(2) << &H10))
        num = (num + Convert.ToUInt32(addressBytes(1) << 8))
        Return (num + addressBytes(0))
    End Function

    Private Shared Function IPAddressToLongBackwards(ByVal strIPAddress As String) As UInt32
        Return IPAddressToLongBackwards(IPAddress.Parse(strIPAddress))
    End Function


    Private Shared Function IPAddressToLongBackwards(ByVal objIPAddress As IPAddress) As UInt32
        Dim addressBytes As Byte() = objIPAddress.GetAddressBytes
        Dim num As UInt32 = Convert.ToUInt32(addressBytes(0) << &H18)
        num = (num + Convert.ToUInt32(addressBytes(1) << &H10))
        num = (num + Convert.ToUInt32(addressBytes(2) << 8))
        Return (num + addressBytes(3))
    End Function

    Public Shared Function IsGreater(ByVal ToCompare As IPAddress, ByVal CompareAgainst As IPAddress) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) > CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function IsGreaterOrEqual(ByVal ToCompare As IPAddress, ByVal CompareAgainst As IPAddress) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) >= CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function IsLess(ByVal ToCompare As IPAddress, ByVal CompareAgainst As IPAddress) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) < CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function IsLessOrEqual(ByVal ToCompare As IPAddress, ByVal CompareAgainst As IPAddress) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) <= CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function



    Public Shared Function IsGreater(ByVal ToCompare As String, ByVal CompareAgainst As String) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) > CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function IsGreaterOrEqual(ByVal ToCompare As String, ByVal CompareAgainst As String) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) >= CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function IsLess(ByVal ToCompare As String, ByVal CompareAgainst As String) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) < CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function IsLessOrEqual(ByVal ToCompare As String, ByVal CompareAgainst As String) As Boolean
        Return (CompareIPs.IPAddressToLongBackwards(ToCompare) <= CompareIPs.IPAddressToLongBackwards(CompareAgainst))
    End Function

    Public Shared Function LongToIPAddress(ByVal IPAddress As UInt32) As String
        Return New IPAddress(CLng(IPAddress)).ToString
    End Function

End Class