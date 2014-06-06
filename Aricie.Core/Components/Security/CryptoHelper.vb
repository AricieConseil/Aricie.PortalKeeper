Imports Aricie.ComponentModel
Imports System.Xml
Imports System.Text
Imports System.IO
Imports Aricie.Services
Imports System.Threading
Imports System.Security.Cryptography
Imports System.Security
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography.Xml
Imports Aricie.Cryptography
Imports System.Linq

Namespace Security.Cryptography

    Public Enum RijndelKeySizes
        Key128 = 128
        Key256 = 256
    End Enum

    Public Enum RsaKeySize
        Key384 = 384
        Key512 = 512
        Key1024 = 1024
        Key2048 = 2048
        Key4096 = 4096
        Key8192 = 8192
        Key16384 = 16384
    End Enum

    <Flags()> _
    Public Enum EncryptionType
        Symmetric = 1
        Asymmetric = 2
        AsymmetricKeyExchange = 4
        AsymmetricByBlock = 8
    End Enum

    Public Enum AsymmetricEncryptionType
        Simple = 2
        KeyExchange = 4
        ByBlock = 8
    End Enum


    <Flags()> _
    Public Enum KeyProtectionMode
        None = 0
        Application = 2
        ProtectData = 4
        KeyContainer = 8
    End Enum

    Public Enum CryptoTransformDirection
        Encrypt
        Decrypt
    End Enum


    Public Module CryptoHelper

        Private Const BaseInitVector As String = "Aricie"

        Private _StaticRandomNumberGenerator As RNGCryptoServiceProvider
        Public ReadOnly Property StaticRandomNumberGenerator As RNGCryptoServiceProvider
            Get
                If _StaticRandomNumberGenerator Is Nothing Then
                    _StaticRandomNumberGenerator = New RNGCryptoServiceProvider()
                End If
                Return _StaticRandomNumberGenerator
            End Get
        End Property

        Public Sub AddRandomDelay()

            Thread.Sleep(GetNewSalt(0)(0))

        End Sub



        Public Function GetNewSalt() As Byte()
            Return GetNewSalt(16)
        End Function

        Public Function GetNewSalt(ByVal length As Integer) As Byte()
            ' Create a buffer
            Dim objBytes() As Byte

            If length >= 1 Then
                objBytes = New Byte(length) {}
            Else
                objBytes = New Byte(0) {}
            End If

            objBytes.FillRandom()


            Return objBytes

        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Sub FillRandom(ByRef objBytes As Byte())
            ' Create a new RNGCryptoServiceProvider.


            ' Fill the buffer with random bytes.
            StaticRandomNumberGenerator.GetBytes(objBytes)


        End Sub

        <System.Runtime.CompilerServices.Extension> _
        Public Sub ClearBytes(ByRef objBytes As Byte(), fillRandom As Boolean)
            For i As Integer = 0 To objBytes.Length - 1
                If fillRandom Then
                    objBytes.FillRandom()
                Else
                    objBytes(i) = 0
                End If
            Next
        End Sub


#Region "Secure Strings"

        <System.Runtime.CompilerServices.Extension> _
        Public Function WriteSecureString(source As Byte(), makeReadOnly As Boolean, encodingFormat As System.Text.Encoding, clearSource As Boolean) As SecureString
            Dim toReturn As New SecureString()
            For Each objByte As Byte In source
                toReturn.AppendChar(encodingFormat.GetChars({objByte})(0))
            Next
            If clearSource Then
                source.FillRandom()
            End If
            If makeReadOnly Then
                toReturn.MakeReadOnly()
            End If

            Return toReturn
        End Function


        <System.Runtime.CompilerServices.Extension> _
        Public Function WriteSecureString(source As String, makeReadOnly As Boolean) As SecureString
            Dim toReturn As New SecureString()
            For Each objChar As Char In source
                toReturn.AppendChar(objChar)
            Next
            If makeReadOnly Then
                toReturn.MakeReadOnly()
            End If
            Return toReturn
        End Function


        <System.Runtime.CompilerServices.Extension> _
        Public Function ReadSecureString(source As SecureString) As String
            Dim result As String = Nothing
            Dim pointer As IntPtr = IntPtr.Zero
            'Dim length As Integer = source.Length

            'Dim chars As Char() = New Char(length - 1) {}

            Try
                pointer = Marshal.SecureStringToBSTR(source)
                result = Marshal.PtrToStringBSTR(pointer)
                'Marshal.Copy(pointer, chars, 0, length)

                'result = String.Join("", chars.Select(Function(objChar) objChar.ToString()).ToArray())
            Catch ex As Exception
                ExceptionHelper.LogException(ex)
            Finally
                If pointer <> IntPtr.Zero Then
                    Marshal.FreeBSTR(pointer)
                End If
            End Try
            Return result
        End Function


        ''' <summary>
        ''' Retrieves, (and then zero's,) a character array from the SecureString.
        ''' Then optionally URL encodes the characters and returns an encoded byte array.
        ''' Make sure to zero out the byte array when you're done with it.
        ''' </summary>
        ''' <param name="secure">The SecureString to be retrieved.</param>
        ''' <param name="encodingFormat">The encoding format used to retrieve the byte array. The default is UTF-8.</param>
        ''' <returns></returns>
        <System.Runtime.CompilerServices.Extension> _
        Public Function ReadSecureStringToBytes(secure As SecureString, encodingFormat As System.Text.Encoding) As Byte()
            If secure Is Nothing Then
                Throw New ArgumentNullException("secure")
            End If

            If encodingFormat Is Nothing Then
                encodingFormat = System.Text.Encoding.UTF8
            End If

            Dim chars As Char() = New Char(secure.Length - 1) {}


            ' copy the SecureString data into an unmanaged char array and get a pointer to it
            Dim ptr As IntPtr = Marshal.SecureStringToCoTaskMemUnicode(secure)

            Try
                ' copy the unmanaged char array into a managed char array
                Marshal.Copy(ptr, chars, 0, secure.Length)
            Finally
                Marshal.ZeroFreeCoTaskMemUnicode(ptr)
            End Try
            Dim objBytes As Byte()
            objBytes = encodingFormat.GetBytes(chars)


            ' zero out chars
            For i As Integer = 0 To chars.Length - 1
                chars(i) = ControlChars.NullChar
            Next

            Return objBytes
        End Function

#End Region



#Region "Symmetric encryption"




        <System.Runtime.CompilerServices.Extension> _
        Public Function Encrypt(ByVal plainText As Byte(), ByVal key As Byte(), ByVal initVector As Byte(), ByRef salt As Byte(), Optional keysize As RijndelKeySizes = RijndelKeySizes.Key256) As Byte()

            Dim encryptor As ICryptoTransform = GetRijndaelManaged(CryptoTransformDirection.Encrypt, key, initVector, salt, keysize)

            Dim cipherText As Byte() = plainText.Encrypt(encryptor)

            Return cipherText
        End Function

        Public Sub GetNewRijndaelManaged(keySize As RijndelKeySizes, ByRef key As Byte(), ByRef iv As Byte())

            Dim intKeySize As Integer = CInt(keySize)

            Dim symmetricKey As New RijndaelManaged()
            symmetricKey.KeySize = intKeySize
            symmetricKey.GenerateIV()
            symmetricKey.GenerateKey()
            key = symmetricKey.Key
            iv = symmetricKey.IV
        End Sub


        Public Function GetRijndaelManaged(direction As CryptoTransformDirection, ByVal key As Byte(), ByVal initVector As Byte(), ByRef salt As Byte(), Optional keysize As RijndelKeySizes = RijndelKeySizes.Key256) As ICryptoTransform
            If salt Is Nothing OrElse salt.Length = 0 Then
                salt = GetNewSalt(30)
            End If
            If initVector Is Nothing Then
                initVector = New Byte() {}
            End If

            Dim intKeySize As Integer = CInt(keysize)

            ' First, we must create a password, from which the key will be derived.
            ' This password will be generated from the specified passphrase and 
            ' salt value. The password will be created using the specified hash 
            ' algorithm. Password creation can be done in several iterations.

            Dim password As New Rfc2898DeriveBytes(initVector, salt, 10)


            Dim initVectorBytes As Byte() = password.GetBytes(intKeySize \ 16)

            password = New Rfc2898DeriveBytes(key, salt, 10)

            ' Use the password to generate pseudo-random bytes for the encryption
            ' key. Specify the size of the key in bytes (instead of bits).
            Dim keyBytes As Byte()
            keyBytes = password.GetBytes(intKeySize \ 8)

            ' Create uninitialized Rijndael encryption object.
            Dim symmetricKey As RijndaelManaged
            symmetricKey = New RijndaelManaged()
            symmetricKey.KeySize = intKeySize
            ' It is reasonable to set encryption mode to Cipher Block Chaining
            ' (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC

            ' Generate encryptor from the existing key bytes and initialization 
            ' vector. Key size will be defined based on the number of the key 
            ' bytes.
            Dim toReturn As ICryptoTransform
            If direction = CryptoTransformDirection.Encrypt Then
                toReturn = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
            Else
                toReturn = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)
            End If

            Return toReturn
        End Function







        <System.Runtime.CompilerServices.Extension> _
        Public Function Encrypt(ByVal plainText As Byte(), encryptor As ICryptoTransform) As Byte()
            Dim toReturn As Byte()


            ' Define memory stream which will be used to hold encrypted data.
            Using objMemStream As New MemoryStream()
                ' Define cryptographic stream (always use Write mode for encryption).
                Using objCryptoStream As New CryptoStream(objMemStream, _
                                                encryptor, _
                                                CryptoStreamMode.Write)
                    ' Start encrypting.
                    objCryptoStream.Write(plainText, 0, plainText.Length)

                    ' Finish encrypting.
                    objCryptoStream.FlushFinalBlock()

                    ' Convert our encrypted data from a memory stream into a byte array.

                    toReturn = objMemStream.ToArray()

                    ' Close both streams.

                End Using
            End Using
            Return toReturn

        End Function



        <System.Runtime.CompilerServices.Extension> _
        Public Function Decrypt(ByVal encrypted As Byte(), ByVal key As Byte(), ByVal initVector As Byte(), ByVal salt As Byte(), Optional keysize As RijndelKeySizes = RijndelKeySizes.Key256) As Byte()

            Dim decryptor As ICryptoTransform = GetRijndaelManaged(CryptoTransformDirection.Decrypt, key, initVector, salt, keysize)

            Dim plainText As Byte()
            plainText = Decrypt(encrypted, decryptor)

            ' Return decrypted string.
            Return plainText
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function Decrypt(ByVal encrypted As Byte(), decryptor As ICryptoTransform) As Byte()
            ' Since at this point we don't know what the size of decrypted data
            ' will be, allocate the buffer long enough to hold ciphertext;
            ' plaintext is never longer than ciphertext.
            Dim toReturn() As Byte
            ReDim toReturn(encrypted.Length)

            ' Define memory stream which will be used to hold encrypted data.
            Using objMemStream As New MemoryStream(encrypted)
                ' Define cryptographic stream (always use Write mode for encryption).
                Using objCryptoStream As New CryptoStream(objMemStream, _
                                                decryptor, _
                                                CryptoStreamMode.Read)


                    ' Start decrypting.
                    Dim decryptedByteCount As Integer
                    decryptedByteCount = objCryptoStream.Read(toReturn, _
                                                           0, _
                                                           toReturn.Length)

                    ReDim Preserve toReturn(decryptedByteCount - 1)
                    'toReturn = Encoding.UTF8.GetString(toReturn, 0, decryptedByteCount).ToUTF8()

                End Using
            End Using
            ' Return decrypted string.
            Return toReturn
        End Function





#End Region




#Region "Asymmetric encryption"





        <System.Runtime.CompilerServices.Extension> _
        Public Function EncryptByKeyExchange(rsa As RSACryptoServiceProvider, input As Byte()) As Byte()
            Dim sa As SymmetricAlgorithm = New RijndaelManaged()
            sa.KeySize = 256
            Dim ct As ICryptoTransform = sa.CreateEncryptor()
            Dim objBytes As Byte() = ct.TransformFinalBlock(input, 0, input.Length)

            Dim fmt As New RSAPKCS1KeyExchangeFormatter(rsa)
            Dim keyex As Byte() = fmt.CreateKeyExchange(sa.Key)

            ' return the key exchange, the IV (public) and encrypted data
            Dim result As Byte() = New Byte(keyex.Length + sa.IV.Length + (objBytes.Length - 1)) {}
            Buffer.BlockCopy(keyex, 0, result, 0, keyex.Length)
            Buffer.BlockCopy(sa.IV, 0, result, keyex.Length, sa.IV.Length)
            Buffer.BlockCopy(objBytes, 0, result, keyex.Length + sa.IV.Length, objBytes.Length)
            Return result
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function DecryptFromKeyExchange(rsa As RSACryptoServiceProvider, input As Byte()) As Byte()
            Dim sa As SymmetricAlgorithm = New RijndaelManaged()
            sa.KeySize = 256

            Dim keyex As Byte() = New Byte((rsa.KeySize >> 3) - 1) {}
            Buffer.BlockCopy(input, 0, keyex, 0, keyex.Length)

            Dim def As New RSAPKCS1KeyExchangeDeformatter(rsa)
            Dim key As Byte() = def.DecryptKeyExchange(keyex)

            Dim iv As Byte() = New Byte(sa.IV.Length - 1) {}
            Buffer.BlockCopy(input, keyex.Length, iv, 0, iv.Length)

            Dim ct As ICryptoTransform = sa.CreateDecryptor(key, iv)
            Dim objBytes As Byte() = ct.TransformFinalBlock(input, keyex.Length + iv.Length, input.Length - (keyex.Length + iv.Length))
            Return objBytes
        End Function

        'todo: does not seem to work
        <System.Runtime.CompilerServices.Extension> _
        Public Function EncryptByBlocks(provider As RSACryptoServiceProvider, objBytes As Byte()) As Byte()
            ' TODO: Add Proper Exception Handlers
            Dim keySize As Integer = provider.KeySize \ 8
            ' The hash function in use by the .NET RSACryptoServiceProvider here is SHA1
            Dim maxLength As Integer = (keySize) - 2 - (2 * SHA1.Create().ComputeHash(objBytes).Length)
            'Dim maxLength As Integer = 117
            Dim dataLength As Integer = objBytes.Length
            Dim iterations As Integer = dataLength \ maxLength

            Dim result As Byte() = New Byte(-1) {}
            For i As Integer = 0 To iterations
                Dim tempBytes As Byte() = New Byte(If((dataLength - maxLength * i > maxLength), maxLength, dataLength - maxLength * i) - 1) {}
                Buffer.BlockCopy(objBytes, maxLength * i, tempBytes, 0, tempBytes.Length)
                Dim encryptedBytes As Byte() = provider.Encrypt(tempBytes, True)
                ' Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after encryption and before decryption.
                ' If you do not require compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors.
                ' Comment out the next line and the corresponding one in the DecryptString function.
                Array.Reverse(encryptedBytes)

                result = result.AppendBytes(BitConverter.GetBytes(encryptedBytes.Length))
                result = result.AppendBytes(encryptedBytes)
            Next
            Return result
        End Function



        <System.Runtime.CompilerServices.Extension> _
        Public Function DecryptBlocks(provider As RSACryptoServiceProvider, objBytes As Byte()) As Byte()
            Dim list As New List(Of Byte)()
            Dim i As Integer = 0
            While i < objBytes.Length
                Dim length As Integer = BitConverter.ToInt32(objBytes, i)
                i += 4

                Dim encryptedBytes As Byte() = New Byte(length - 1) {}
                Array.Copy(objBytes, i, encryptedBytes, 0, length)

                Array.Reverse(encryptedBytes)
                list.AddRange(provider.Decrypt(encryptedBytes, True))

                i += length
            End While
            Return list.ToArray()
        End Function



        ' ''' Encrypts data.
        ' '''
        ' ''' A byte array with data to be encrypted. /// Returns encrypted data into a array of bytes.
        '<System.Runtime.CompilerServices.Extension> _
        'Public Function EncryptByBlocks(provider As RSACryptoServiceProvider, objBytes As Byte()) As Byte()
        '    Dim toReturn As New List(Of Byte)


        '    'Dim maxLength As Integer = (provider.KeySize \ 8) - (SHA1.Create().ComputeHash(objBytes).Length * 2 + 2)
        '    Dim maxLength As Integer = provider.KeySize \ 8 - 42

        '    Dim blocksCount As Integer = objBytes.Length \ maxLength
        '    For i As Integer = 0 To blocksCount - 1
        '        Dim bytesCount As Integer = objBytes.Length - maxLength * i
        '        If bytesCount > maxLength Then
        '            bytesCount = maxLength
        '        End If
        '        Dim block As Byte() = New Byte(bytesCount) {}
        '        Buffer.BlockCopy(objBytes, maxLength * i, block, 0, block.Length)
        '        Dim encryptedData As Byte() = provider.Encrypt(block, True)
        '        Array.Reverse(encryptedData)
        '        toReturn.AddRange(encryptedData)
        '    Next

        '    Return toReturn.ToArray()
        'End Function





        ' '''

        ' ''' Decrypts data.
        ' '''
        ' ''' A byte array with encryoted information to be decrypted. /// Decrypted data into array of bytes.
        '<System.Runtime.CompilerServices.Extension> _
        'Public Function DecryptBlocks(provider As RSACryptoServiceProvider, objBytes As Byte()) As Byte()

        '    Dim toReturn As New List(Of Byte)
        '    Dim keySize As Integer = provider.KeySize \ 8
        '    Dim blocksCount As Integer = objBytes.Length \ keySize
        '    For i As Integer = 0 To blocksCount - 1
        '        Dim bytesCount As Integer = objBytes.Length - keySize * i
        '        If bytesCount > keySize Then
        '            bytesCount = keySize
        '        End If
        '        Dim block As Byte() = New Byte(bytesCount - 1) {}
        '        Buffer.BlockCopy(objBytes, keySize * i, block, 0, block.Length)
        '        Array.Reverse(block)
        '        Dim decryptedData As Byte() = provider.Decrypt(block, True)
        '        toReturn.AddRange(decryptedData)
        '    Next
        '    Return toReturn.ToArray()
        'End Function




        ''' <summary>
        ''' Creates a new key/pair in the RSA key container at the machine level with the specified key size.
        ''' </summary>
        ''' <param name="keyContainerName">The name of the key to create.</param>
        ''' <param name="keySize">The size of the key to create.</param>
        Public Function CSPCreateNewKey(keyContainerName As String, keySize As RsaKeySize, notExportable As Boolean, ephemeral As Boolean) As CspParameters
            Dim cspp As New CspParameters()
            cspp.Flags = CspProviderFlags.NoPrompt Or CspProviderFlags.UseMachineKeyStore
            If ephemeral Then
                cspp.Flags = CType(cspp.Flags Or 128, CspProviderFlags)
            End If
            If Not String.IsNullOrEmpty(keyContainerName) Then
                cspp.KeyContainerName = keyContainerName
            ElseIf ephemeral Then
                cspp.KeyContainerName = CryptoHelper.GetNewSalt(20).Hash(HashProvider.SHA256)
            End If
            If notExportable Then
                cspp.Flags = cspp.Flags Or CspProviderFlags.UseNonExportableKey
            Else
                cspp.Flags = cspp.Flags Or CspProviderFlags.UseArchivableKey
            End If

            Using rsa As New RSACryptoServiceProvider(CInt(keySize), cspp)
                If Not ephemeral Then
                    rsa.PersistKeyInCsp = True
                End If
            End Using
            Return cspp
        End Function

        ''' <summary>
        ''' Exports the key/pair specified.
        ''' </summary>
        ''' <param name="keyContainerName">The key container name to export the public key from.</param>
        ''' <returns>Returns the public key for the specified key container name.</returns>
        Public Function CSPExportPublicKeyToXML(keyContainerName As String) As String
            Dim cspp As New CspParameters()
            cspp.KeyContainerName = keyContainerName
            cspp.Flags = CspProviderFlags.NoPrompt Or CspProviderFlags.UseArchivableKey Or CspProviderFlags.UseMachineKeyStore
            Using rsa As New RSACryptoServiceProvider(cspp)
                rsa.PersistKeyInCsp = True
                Return rsa.ToXmlString(False)
            End Using
        End Function

        ''' <summary>
        ''' Export Public and Private Key to XML
        ''' </summary>
        ''' <param name="keyContainerName">The key container to export.</param>
        ''' <returns>Returns the xml which can be shared between PCs. ** Warning ** It is best to keep the private key secure and never expose this!</returns>
        Public Function CSPExportPublicAndPrivateKeyToXML(keyContainerName As String) As String
            Dim cspp As New CspParameters()
            cspp.KeyContainerName = keyContainerName
            cspp.Flags = CspProviderFlags.NoPrompt Or CspProviderFlags.UseArchivableKey Or CspProviderFlags.UseMachineKeyStore
            Using rsa As New RSACryptoServiceProvider(cspp)
                rsa.PersistKeyInCsp = True
                Return rsa.ToXmlString(True)
            End Using
        End Function

        ''' <summary>
        ''' On another computer this can be used to create the key/pair used to decrypt information. Note this will not allow you
        ''' to "encrypt" unless you are importing the full data. It will allow the target PC to decrypt any data encrypted by the original
        ''' generator of the key.
        ''' </summary>
        ''' <param name="keyContainerName">The key container name to inject this key into.</param>
        Public Sub CSPImportFromXml(keyContainerName As String, xml As String, notExportable As Boolean)
            Dim cspp As New CspParameters()
            If Not String.IsNullOrEmpty(keyContainerName) Then
                cspp.KeyContainerName = keyContainerName
            Else
                cspp.KeyContainerName = Nothing
                cspp.Flags = CType(cspp.Flags Or 128, CspProviderFlags)
            End If
            cspp.Flags = CspProviderFlags.NoPrompt Or CspProviderFlags.UseMachineKeyStore
            If notExportable Then
                cspp.Flags = cspp.Flags Or CspProviderFlags.UseNonExportableKey
            Else
                cspp.Flags = cspp.Flags Or CspProviderFlags.UseArchivableKey
            End If
            Using rsa As New RSACryptoServiceProvider(cspp)
                rsa.FromXmlString(xml)
                rsa.PersistKeyInCsp = True
            End Using
        End Sub

        ''' <summary>
        ''' Encrypted the specified data.
        ''' </summary>
        ''' <param name="objBytes">The bytes to encrypt</param>
        ''' <param name="keyContainerName">The key name to use</param>
        ''' <returns>Returns the encrypted bytes.</returns>
        <System.Runtime.CompilerServices.Extension> _
        Public Function CSPEncrypt(objBytes As Byte(), keyContainerName As String, objEncryptionType As AsymmetricEncryptionType) As Byte()
            ' Configure the parameters to indicate the name of the
            ' key container which contains our public/private key pair
            Dim csp As New CspParameters()
            csp.KeyContainerName = keyContainerName

            ' Create an instance of the RSA encryption algorithm
            ' and perform the actual encryption

            Using rsa As New RSACryptoServiceProvider(csp)
                Select Case objEncryptionType
                    Case AsymmetricEncryptionType.Simple
                        objBytes = rsa.Encrypt(objBytes, True)
                    Case AsymmetricEncryptionType.KeyExchange
                        objBytes = rsa.EncryptByKeyExchange(objBytes)
                    Case AsymmetricEncryptionType.ByBlock
                        objBytes = rsa.EncryptByBlocks(objBytes)
                End Select
            End Using
            Return objBytes
        End Function

        ''' <summary>
        ''' Decrypts the bytes specified.
        ''' </summary>
        ''' <param name="objBytes">The bytes to decrypt</param>
        ''' <param name="keyContainerName">The key name to use</param>
        ''' <returns>Returns the decrypted bytes.</returns>
        <System.Runtime.CompilerServices.Extension> _
        Public Function CSPDecrypt(objBytes As Byte(), keyContainerName As String, objEncryptionType As AsymmetricEncryptionType) As Byte()
            ' Configure the parameters to indicate the name of the
            ' key container which contains our public/private key pair
            Dim csp As New CspParameters()
            csp.KeyContainerName = keyContainerName

            ' Create an instance of the RSA encryption algorithm
            ' and perform the actual decryption
            Using rsa As New RSACryptoServiceProvider(csp)
                Select Case objEncryptionType
                    Case AsymmetricEncryptionType.Simple
                        objBytes = rsa.Decrypt(objBytes, True)
                    Case AsymmetricEncryptionType.KeyExchange
                        objBytes = rsa.DecryptFromKeyExchange(objBytes)
                    Case AsymmetricEncryptionType.ByBlock
                        objBytes = rsa.DecryptBlocks(objBytes)
                End Select
            End Using
            Return objBytes
        End Function

        ''' <summary>
        ''' Encryptes a string using the supplied key. Encoding is done using RSA encryption.
        ''' </summary>
        ''' <param name="data">String that must be encrypted.</param>
        ''' <param name="keyContainerName">Encryptionkey.</param>
        ''' <returns>A string representing a byte array separated by a minus sign.</returns>
        ''' <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function CSPEncryptString(data As String, keyContainerName As String, objEncryptionType As AsymmetricEncryptionType) As String
            ' Encrypt a string using the private key stored within the
            ' specified key container
            Dim dataToEncrypt As Byte() = Encoding.UTF8.GetBytes(data)
            Dim encryptedData As Byte() = CSPEncrypt(dataToEncrypt, keyContainerName, objEncryptionType)
            Return Convert.ToBase64String(encryptedData)
        End Function

        ''' <summary>
        ''' Decrypts a string using the supplied key. Decoding is done using RSA encryption.
        ''' </summary>
        ''' <param name="data">String that must be decrypted.</param>
        ''' <param name="keyContainerName">Decryptionkey.</param>
        ''' <returns>The decrypted string or null if decryption failed.</returns>
        ''' <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function CSPDecryptString(data As String, keyContainerName As String, objEncryptionType As AsymmetricEncryptionType) As String
            ' Decrypt a string using the public key stored within the
            ' specified key container
            Dim dataToDecrypt As Byte() = Convert.FromBase64String(data)
            Dim decryptedData As Byte() = CSPDecrypt(dataToDecrypt, keyContainerName, objEncryptionType)
            Return Encoding.UTF8.GetString(decryptedData)
        End Function



        ''' <summary>
        ''' SIGN AN XML DOCUMENT USING THE PRIVATE KEY
        ''' </summary>
        ''' <param name="Doc">XML DOCUMENT TO BE SHOULD SIGNED</param>
        ''' <param name="PrivateKey">PRIVATE RSA KEY USED TO SIGN XML</param>
        Public Sub SignXml(doc As XmlDocument, privateKey As AsymmetricAlgorithm, ParamArray paths As String())

            'VERIFY ALL ARGUMENTS HAVE BEEN PASSED IN
            If doc Is Nothing Then
                Throw New ArgumentException("Doc")
            End If
            If privateKey Is Nothing Then
                Throw New ArgumentException("privateKey")
            End If

            'CREATE A SIGNED XML DOCUMENT
            Dim signedXml As New SignedXml(doc)
            signedXml.SignedInfo.CanonicalizationMethod = signedXml.XmlDsigExcC14NTransformUrl

            'ADD THE RSA KEY TO THE SIGNED DOCUMENT
            signedXml.SigningKey = privateKey

            If paths Is Nothing OrElse paths.Length = 0 Then
                paths = {""}
            End If

            'CREATE A REFERENCE TO BE SIGNED
            For Each objPath As String In paths
                Dim reference As New Reference()
                'If Not String.IsNullOrEmpty(objPath) Then
                reference.Uri = objPath
                'End If

                ''CREATE AN ENVELOPED SIGNATURE WHICH
                Dim env As New XmlDsigEnvelopedSignatureTransform()
                reference.AddTransform(env)
                'reference.AddTransform(New XmlDsigExcC14NTransform())

                'ADD THE REFERENCE TO THE SIGNED DOCUMENT
                signedXml.AddReference(reference)
            Next


            'COMPUTE THE DOCUMENTS SIGNATURE
            signedXml.ComputeSignature()

            'RETRIEVE THE XML SIGNATURE FROM THE DOCUMENT
            Dim xmlDigitalSignature As XmlElement = signedXml.GetXml()

            'APPEND THE SIGNATURE TO THE END OF THE DOCUMENT
            'Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, True))
        End Sub

        ''' <summary>
        ''' VERIFY A SIGNED XML DOCUMENT
        ''' </summary>
        ''' <param name="Doc">SIGNED XML DOCUMENT</param>
        ''' <param name="PublicKey">PUBLIC KEY TO VERIFY SIGNATURE AGAINST</param>
        ''' <returns></returns>
        Public Function VerifyXml(doc As XmlDocument, publicKey As AsymmetricAlgorithm) As Boolean

            'VERIFY ALL ARGUMENTS HAVE BEEN PASSED IN
            If doc Is Nothing Then
                Throw New ArgumentException("Doc")
            End If
            If publicKey Is Nothing Then
                Throw New ArgumentException("Key")
            End If

            'HOLD THE SIGNED DOCUMENT
            Dim signedXml As New SignedXml(doc)

            'LOCATE THE SIGNATURE NODE IN THE DOCUMENT
            Dim nodeList As XmlNodeList = doc.GetElementsByTagName("Signature")

            'IF WE CANT FIND THE NODE THEN THIS DOCUMENT IS NOT SIGNED
            If nodeList.Count <= 0 Then
                Throw New CryptographicException("Verification failed: No Signature was found in the document.")
            End If

            'IF THERE ARE MORE THEN ONE SIGNATURES THEN FAIL
            If nodeList.Count >= 2 Then
                Throw New CryptographicException("Verification failed: More that one signature was found for the document.")
            End If

            'LOAD THE SIGNATURE NODE INTO THE SIGNEDXML DOCUMENT
            signedXml.LoadXml(DirectCast(nodeList(0), XmlElement))

            'CHECK THE SIGNATURE AND SEND THE RESULT
            Return signedXml.CheckSignature(publicKey)
        End Function

        ''' <summary>
        ''' Encrypts a Xml Document
        ''' </summary>
        ''' <remarks>Example of use: 
        ''' Encrypt(xmlDoc, "creditcard", "EncryptedElement1", rsaKey, "rsaKey");
        ''' </remarks>
        Public Sub EncryptXml(doc As XmlDocument, elementToEncrypt As String, encryptionElementId As String, alg As RSA, keyName As String)
            ' Check the arguments.
            If doc Is Nothing Then
                Throw New ArgumentNullException("doc")
            End If
            If elementToEncrypt Is Nothing Then
                Throw New ArgumentNullException("elementToEncrypt")
            End If
            If encryptionElementId Is Nothing Then
                Throw New ArgumentNullException("encryptionElementId")
            End If
            If alg Is Nothing Then
                Throw New ArgumentNullException("alg")
            End If
            If keyName Is Nothing Then
                Throw New ArgumentNullException("keyName")
            End If

            '/////////////////////////////////////////////
            ' Find the specified element in the XmlDocument
            ' object and create a new XmlElemnt object.
            '/////////////////////////////////////////////
            Dim xmlElementToEncrypt As XmlElement = TryCast(doc.GetElementsByTagName(elementToEncrypt)(0), XmlElement)

            ' Throw an XmlException if the element was not found.
            If xmlElementToEncrypt Is Nothing Then

                Throw New XmlException("The specified element was not found")
            End If
            Dim sessionKey As RijndaelManaged = Nothing

            Try
                '///////////////////////////////////////////////
                ' Create a new instance of the EncryptedXml class
                ' and use it to encrypt the XmlElement with the
                ' a new random symmetric key.
                '///////////////////////////////////////////////

                ' Create a 256 bit Rijndael key.
                sessionKey = New RijndaelManaged()
                sessionKey.KeySize = 256

                Dim eXml As New EncryptedXml()

                Dim encryptedElement As Byte() = eXml.EncryptData(xmlElementToEncrypt, sessionKey, False)
                '/////////////////////////////////////////////
                ' Construct an EncryptedData object and populate
                ' it with the desired encryption information.
                '/////////////////////////////////////////////

                Dim edElement As New EncryptedData()
                edElement.Type = EncryptedXml.XmlEncElementUrl
                edElement.Id = encryptionElementId
                ' Create an EncryptionMethod element so that the
                ' receiver knows which algorithm to use for decryption.

                edElement.EncryptionMethod = New EncryptionMethod(EncryptedXml.XmlEncAES256Url)
                ' Encrypt the session key and add it to an EncryptedKey element.
                Dim ek As New EncryptedKey()

                Dim encryptedKey As Byte() = EncryptedXml.EncryptKey(sessionKey.Key, alg, False)

                ek.CipherData = New CipherData(encryptedKey)

                ek.EncryptionMethod = New EncryptionMethod(EncryptedXml.XmlEncRSA15Url)

                ' Create a new DataReference element
                ' for the KeyInfo element.  This optional
                ' element specifies which EncryptedData
                ' uses this key.  An XML document can have
                ' multiple EncryptedData elements that use
                ' different keys.
                Dim dRef As New DataReference()

                ' Specify the EncryptedData URI.
                dRef.Uri = Convert.ToString("#") & encryptionElementId

                ' Add the DataReference to the EncryptedKey.
                ek.AddReference(dRef)
                ' Add the encrypted key to the
                ' EncryptedData object.

                edElement.KeyInfo.AddClause(New KeyInfoEncryptedKey(ek))
                ' Set the KeyInfo element to specify the
                ' name of the RSA key.

                ' Create a new KeyInfo element.
                edElement.KeyInfo = New KeyInfo()

                ' Create a new KeyInfoName element.
                Dim kin As New KeyInfoName()

                ' Specify a name for the key.
                kin.Value = keyName

                ' Add the KeyInfoName element to the
                ' EncryptedKey object.
                ek.KeyInfo.AddClause(kin)
                ' Add the encrypted element data to the
                ' EncryptedData object.
                edElement.CipherData.CipherValue = encryptedElement
                '/////////////////////////////////////////////////
                ' Replace the element from the original XmlDocument
                ' object with the EncryptedData element.
                '////////////////////////////////////////////////
                EncryptedXml.ReplaceElement(xmlElementToEncrypt, edElement, False)
            Catch e As Exception
                ' re-throw the exception.
                Throw e
            Finally
                If sessionKey IsNot Nothing Then
                    sessionKey.Clear()

                End If
            End Try
        End Sub


        ''' <summary>
        ''' Decrypts a Xml Document
        ''' </summary>
        ''' <remarks>Example of use: 
        ''' Decrypt(xmlDoc, rsaKey, "rsaKey");
        ''' </remarks>
        Public Sub DecryptXml(doc As XmlDocument, keyObject As Object, keyName As String)


            ' Check the arguments.
            If doc Is Nothing Then
                Throw New ArgumentNullException("doc")
            End If
            If keyObject Is Nothing Then
                Throw New ArgumentNullException("keyObject")
            End If
            If keyName Is Nothing Then
                Throw New ArgumentNullException("KeyName")
            End If
            ' Create a new EncryptedXml object.
            Dim exml As New EncryptedXml(doc)

            ' Add a key-name mapping.
            ' This method can only decrypt documents
            ' that present the specified key name.
            exml.AddKeyNameMapping(keyName, keyObject)

            ' Decrypt the element.
            exml.DecryptDocument()

        End Sub


        Public Function SignXmlBytes(objBytes As Byte(), encrypter As IEncrypter) As Byte()

            Using inputStream As New MemoryStream(objBytes)
                Dim doc = New XmlDocument()
                doc.Load(inputStream)
                encrypter.Sign(doc)
                Using outStream As New MemoryStream()
                    doc.Save(outStream)
                    Return outStream.ToArray()
                End Using
            End Using
        End Function


        Public Sub RemoveSignatureFromXmlDocument(ByRef doc As XmlDocument)
            Dim sigNodes As XmlNodeList = doc.GetElementsByTagName("Signature")
            If sigNodes.Count > 0 Then
                sigNodes.Item(0).ParentNode.RemoveChild(sigNodes.Item(0))
            End If
        End Sub

        Public Function RemoveSignatureFromXmlBytes(objBytes As Byte()) As Byte()

            Using inputStream As New MemoryStream(objBytes)
                Dim doc = New XmlDocument()
                doc.Load(inputStream)
                RemoveSignatureFromXmlDocument(doc)
                Using outStream As New MemoryStream()
                    doc.Save(outStream)
                    Return outStream.ToArray()
                End Using
            End Using
        End Function

#End Region



#Region "Hashing"

        <System.Runtime.CompilerServices.Extension> _
        Public Function Hash(content As Byte(), hashProvider As HashProvider) As String
            Select Case hashProvider
                Case hashProvider.SHA1
                    Using hasher As SHA1 = SHA1.Create()
                        Dim strHash As Byte() = hasher.ComputeHash(content)
                        Return BitConverter.ToString(strHash).Replace("-", "").ToLowerInvariant().Trim()
                    End Using
                Case hashProvider.MD5
                    Using hasher As MD5 = MD5.Create()
                        Dim strHash As Byte() = hasher.ComputeHash(content)
                        Return BitConverter.ToString(strHash).Replace("-", "").ToLowerInvariant().Trim()
                    End Using
                Case hashProvider.SHA256
                    Using hasher As New SHA256CryptoServiceProvider()
                        Dim strHash As Byte() = hasher.ComputeHash(content)
                        Return BitConverter.ToString(strHash).Replace("-", "").ToLowerInvariant().Trim()
                    End Using
            End Select
            Return Nothing
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function Hash(s As [String], hashProvider As HashProvider) As String
            Dim objBuffer As Byte() = Encoding.UTF8.GetBytes(s)
            Return Hash(objBuffer, hashProvider)
        End Function

#End Region








    End Module
End Namespace