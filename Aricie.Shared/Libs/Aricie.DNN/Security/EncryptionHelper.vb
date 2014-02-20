Imports System.Security.Cryptography
Imports DotNetNuke
Imports DotNetNuke.Security
Imports System.Web.Configuration
Imports System.Threading
Imports System.Text
Imports System.IO

''' <summary>
''' Helper for encryption/decryption routines
''' </summary>
''' <remarks></remarks>
Public Class EncryptionHelper

    Private Const defaultKey As String = "Emergence"
    Private Const defaultSalt As String = "Serendipity"
    Private Const defaultInitVector As String = "Similarity"


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

    Public Shared Sub AddRandomDelay()

        Common.AddRandomDelay()

    End Sub

    Public Shared Function GetNewSalt(ByVal length As Integer) As Byte()
        Return Common.GetNewSalt(length)

    End Function

    Public Shared Function Encrypt(ByVal plainText As String, _
                         ByVal key As String, _
                               Optional ByVal initVector As String = defaultInitVector, _
                              Optional ByVal salt As String = defaultSalt) _
                       As String


        Return Common.Encrypt(plainText, key, initVector, salt)
    End Function


    Public Shared Function Decrypt(ByVal encryptedText As String, _
                                Optional ByVal key As String = defaultKey, _
                                Optional ByVal initVector As String = defaultInitVector, _
                                Optional ByVal salt As String = defaultSalt) As String

        Return Common.Decrypt(encryptedText, key, initVector, salt)
    End Function



End Class
