
Imports Aricie.Security.Cryptography
Imports DotNetNuke.Security
Imports System.Web.Configuration
Imports System.Text
Imports System.IO
Imports Aricie.ComponentModel
Imports System.Xml
Imports Aricie.Services

Namespace Security

    <Obsolete("Use Aricie.Security.CryptoHelper From Aricie.Core")> _
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

            CryptoHelper.AddRandomDelay()


        End Sub

        Public Shared Function GetNewSalt(ByVal length As Integer) As Byte()
            Return CryptoHelper.GetNewSalt(length)

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


        Public Shared Function SignXmlString(xmlString As String, encrypter As IEncrypter) As String
            Dim doc As New XmlDocument()
            doc.LoadXml(xmlString)
            encrypter.Sign(doc)
            Dim sb As New StringBuilder()
            Using sw As New StringWriter(sb)
                'Using sw As New StreamWriter(ms)
                Dim objXmlSettings As XmlWriterSettings = ReflectionHelper.GetStandardXmlWriterSettings()
                Using writer As XmlWriter = XmlWriter.Create(sw, objXmlSettings)
                    doc.WriteTo(writer)
                End Using
                Return sb.ToString
            End Using
        End Function

        Public Shared Function RemoveSignatureFromXmlString(signedXmlString As String) As String
            Dim doc As New XmlDocument()
            doc.LoadXml(signedXmlString)
            Dim sigNodes As XmlNodeList = doc.GetElementsByTagName("Signature")
            If sigNodes.Count > 0 Then
                sigNodes.Item(0).ParentNode.RemoveChild(sigNodes.Item(0))
            End If
            'Me.PayLoad = doc.OuterXml
            Dim sb As New StringBuilder()
            Using sw As New StringWriter(sb)
                'Using sw As New StreamWriter(ms)
                Dim objXmlSettings As XmlWriterSettings = ReflectionHelper.GetStandardXmlWriterSettings()
                Using writer As XmlWriter = XmlWriter.Create(sw, objXmlSettings)
                    doc.WriteTo(writer)
                End Using
                Return sb.ToString
            End Using
        End Function


    End Class


End Namespace
