Imports DotNetNuke
Imports DotNetNuke.Security
Imports System.Web.Configuration

''' <summary>
''' Helper for encryption/decryption routines
''' </summary>
''' <remarks></remarks>
Public Class EncryptionHelper

    ''' <summary>
    ''' Encrypt the string passed as parameter using DotNetNuke portal settings
    ''' </summary>
    ''' <param name="str">String to encrypt</param>
    ''' <returns>Encrypted string</returns>
    ''' <remarks></remarks>
    Shared Function EncryptString(ByVal str As String) As String
        Dim toReturn As String = ""
        Dim obj As PortalSecurity = New PortalSecurity
        Dim objMachine As MachineKeySection = New MachineKeySection
        toReturn = obj.Encrypt(objMachine.DecryptionKey, str)
        Return toReturn
    End Function

    ''' <summary>
    ''' Decrypt the string passed as parameter using DotNetNuke portal settings
    ''' </summary>
    ''' <param name="str">String to decrypt</param>
    ''' <returns>Decrypted string</returns>
    ''' <remarks></remarks>
    Shared Function DecryptString(ByVal str As String) As String
        Dim toReturn As String = ""
        Dim obj As PortalSecurity = New PortalSecurity
        Dim objMachine As MachineKeySection = New MachineKeySection
        toReturn = obj.Decrypt(objMachine.DecryptionKey, str)
        Return toReturn
    End Function

End Class
