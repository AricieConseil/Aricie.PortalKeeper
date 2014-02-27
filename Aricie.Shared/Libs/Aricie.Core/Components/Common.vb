Imports System.Reflection
Imports Aricie.Services
Imports System.Web
Imports System.Web.UI
Imports System.Text
Imports System.Security.Cryptography
Imports System.Threading
Imports System.IO
Imports System.Globalization
Imports System.IO.Compression
Imports System.Net
Imports System.Web.Configuration
Imports System.Xml
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Xml
Imports Aricie.Cryptography




''' <summary>
''' Collection of general purpose helper functions
''' </summary>
Public Module Common

#Region " Public Methods "

#Region " Enum methods "

    Public Function GetEnum(Of T)(ByVal strEnum As String) As T
        Return DirectCast([Enum].Parse(GetType(T), strEnum), T)
    End Function

    Public Function GetEnumMembers(Of T)() As List(Of T)
        Dim toreturn As New List(Of T)
        Dim stringEnums() As String = [Enum].GetNames(GetType(T))
        For Each objEnum As String In stringEnums
            toreturn.Add(GetEnum(Of T)(objEnum))
        Next
        Return toreturn
    End Function

#End Region

#Region "String methods "

    Public Function OnlyAlphaNumericChars(ByVal objString As String) As String

        objString = Trim(objString)
        Dim lLen As Integer
        Dim sAns As New StringBuilder(objString.Length)
        Dim lCtr As Integer
        Dim sChar As String


        For lCtr = 1 To lLen
            sChar = Mid(objString, lCtr, 1)
            If IsAlphaNumeric(sChar) Then
                sAns.Append(sChar)
            End If
        Next

        OnlyAlphaNumericChars = Replace(sAns.ToString, " ", "-")

    End Function

    Public Function IsAlphaNumeric(ByVal sChr As String) As Boolean
        IsAlphaNumeric = sChr Like "[0-9A-Za-z ]"
    End Function

    Public Function IsNumber(ByVal sChr As String) As Boolean
        For i As Integer = 0 To sChr.Length - 1
            If Not Char.IsDigit(sChr, i) Then
                Return False
            End If
        Next
        Return True
    End Function


#Region "Crypto"


    Public Function GetBase64FromUtf8(ByVal strUtf8 As String) As String
        Return Convert.ToBase64String(Encoding.UTF8.GetBytes(strUtf8))
    End Function

    Public Function GetUtf8FromBase64(ByVal strBase64 As String) As String
        Return Encoding.UTF8.GetString(Convert.FromBase64String(strBase64))
    End Function


    Public Sub AddRandomDelay()

        Thread.Sleep(GetNewSalt(0)(0))

    End Sub

    Public Function GetNewSalt(ByVal length As Integer) As Byte()
        ' Create a buffer
        Dim randBytes() As Byte

        If length >= 1 Then
            randBytes = New Byte(length) {}
        Else
            randBytes = New Byte(0) {}
        End If

        ' Create a new RNGCryptoServiceProvider.
        Dim rand As New RNGCryptoServiceProvider()

        ' Fill the buffer with random bytes.
        rand.GetBytes(randBytes)

        Dim disposable As IDisposable = TryCast(rand, IDisposable)
        If Not disposable Is Nothing Then
            disposable.Dispose()
        End If


        Return randBytes

    End Function





    <Obsolete("Use version with byref byte array salt")>
    Public Function Encrypt(ByVal plainText As String, _
                         ByVal key As String, _
                               Optional ByVal initVector As String = defaultInitVector, _
                              Optional ByVal salt As String = defaultSalt) _
                       As String


        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.Unicode.GetBytes(salt)

        Return Encrypt(plainText, key, initVector, saltValueBytes)

    End Function


    Public Function Encrypt(ByVal plainText As String, ByVal key As String, ByVal initVector As String, ByRef salt As Byte()) As String

        If salt Is Nothing OrElse salt.Length = 0 Then
            salt = GetNewSalt(30)
        End If

        Dim password As New Rfc2898DeriveBytes(initVector, salt, 10)
        Dim initVectorBytes As Byte() = password.GetBytes(16)
        'initVectorBytes = Encoding.ASCII.GetBytes(initVector)

        ' Convert our plaintext into a byte array.
        ' Let us assume that plaintext contains UTF8-encoded characters.
        Dim plainTextBytes As Byte()
        plainTextBytes = Encoding.Unicode.GetBytes(plainText)

        ' First, we must create a password, from which the key will be derived.
        ' This password will be generated from the specified passphrase and 
        ' salt value. The password will be created using the specified hash 
        ' algorithm. Password creation can be done in several iterations.
        password = New Rfc2898DeriveBytes(key, salt, 10)

        ' Use the password to generate pseudo-random bytes for the encryption
        ' key. Specify the size of the key in bytes (instead of bits).
        Dim keyBytes As Byte()
        keyBytes = password.GetBytes(CInt(256 / 8))

        ' Create uninitialized Rijndael encryption object.
        Dim symmetricKey As RijndaelManaged
        symmetricKey = New RijndaelManaged()

        ' It is reasonable to set encryption mode to Cipher Block Chaining
        ' (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC

        ' Generate encryptor from the existing key bytes and initialization 
        ' vector. Key size will be defined based on the number of the key 
        ' bytes.
        Dim encryptor As ICryptoTransform
        encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim memoryStream As MemoryStream
        memoryStream = New MemoryStream()

        ' Define cryptographic stream (always use Write mode for encryption).
        Dim cryptoStream As CryptoStream
        cryptoStream = New CryptoStream(memoryStream, _
                                        encryptor, _
                                        CryptoStreamMode.Write)
        ' Start encrypting.
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)

        ' Finish encrypting.
        cryptoStream.FlushFinalBlock()

        ' Convert our encrypted data from a memory stream into a byte array.
        Dim cipherTextBytes As Byte()
        cipherTextBytes = memoryStream.ToArray()

        ' Close both streams.
        memoryStream.Close()
        cryptoStream.Close()

        ' Convert encrypted data into a base64-encoded string.
        Dim cipherText As String
        cipherText = Convert.ToBase64String(cipherTextBytes)

        ' Return encrypted string.
        Return cipherText
    End Function

    'Public Sub SignDocument(doc As XmlDocument, cert As RSACryptoServiceProvider, referencePath As String)
    '    Dim objSignedXml As New SignedXml(doc)
    '    objSignedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl
    '    objSignedXml.SigningKey = cert

    '    ' Retrieve the value of the "ID" attribute on the root assertion element.
    '    Dim reference As New Reference(referencePath)

    '    reference.AddTransform(New XmlDsigEnvelopedSignatureTransform())
    '    reference.AddTransform(New XmlDsigExcC14NTransform())

    '    objSignedXml.AddReference(reference)

    '    ' Include the public key of the certificate in the assertion.
    '    objSignedXml.KeyInfo = New KeyInfo()
    '    objSignedXml.KeyInfo.AddClause(New RSAKeyValue(cert))

    '    objSignedXml.ComputeSignature()
    '    ' Append the computed signature. The signature must be placed as the sibling of the Issuer element.
    '    Dim nodes As XmlNodeList = doc.DocumentElement.GetElementsByTagName("Issuer", Saml20Constants.ASSERTION)
    '    ' doc.DocumentElement.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), nodes[0]);            
    '    nodes(0).ParentNode.InsertAfter(doc.ImportNode(objSignedXml.GetXml(), True), nodes(0))
    'End Sub




    Public Function GetRSAProvider(privateKey As String) As RSACryptoServiceProvider
        Dim toReturn As New RSACryptoServiceProvider()
        toReturn.ImportCspBlob(Convert.FromBase64String(privateKey))
        Return toReturn
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

        'ADD THE RSA KEY TO THE SIGNED DOCUMENT
        signedXml.SigningKey = privateKey

        'CREATE A REFERENCE TO BE SIGNED
        For Each objPath As String In paths
            Dim reference As New Reference()
            reference.Uri = objPath

            'CREATE AN ENVELOPED SIGNATURE WHICH
            Dim env As New XmlDsigEnvelopedSignatureTransform(True)
            reference.AddTransform(env)

            'ADD THE REFERENCE TO THE SIGNED DOCUMENT
            signedXml.AddReference(reference)
        Next

        'COMPUTE THE DOCUMENTS SIGNATURE
        signedXml.ComputeSignature()

        'RETRIEVE THE XML SIGNATURE FROM THE DOCUMENT
        Dim xmlDigitalSignature As XmlElement = signedXml.GetXml()

        'APPEND THE SIGNATURE TO THE END OF THE DOCUMENT
        'Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));
        doc.DocumentElement.AppendChild(xmlDigitalSignature)
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





    <Obsolete("Use version with byte array salt")>
    Public Function Decrypt(ByVal encryptedText As String, _
                                Optional ByVal key As String = defaultKey, _
                                Optional ByVal initVector As String = defaultInitVector, _
                                Optional ByVal salt As String = defaultSalt) As String


        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.Unicode.GetBytes(salt)

        Return Decrypt(encryptedText, key, initVector, saltValueBytes)

    End Function

    Public Function Decrypt(ByVal encryptedText As String, ByVal key As String, ByVal initVector As String, ByVal salt As Byte()) As String

        ' Convert strings defining encryption key characteristics into byte
        ' arrays. Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8
        ' encoding.


        Dim password As New Rfc2898DeriveBytes(initVector, salt, 10)
        Dim initVectorBytes As Byte() = password.GetBytes(16)

        ' Convert our ciphertext into a byte array.
        Dim cipherTextBytes As Byte()
        cipherTextBytes = Convert.FromBase64String(encryptedText)

        ' First, we must create a password, from which the key will be 
        ' derived. This password will be generated from the specified 
        ' passphrase and salt value. The password will be created using
        ' the specified hash algorithm. Password creation can be done in
        ' several iterations.
        password = New Rfc2898DeriveBytes(key, salt, 10)

        ' Use the password to generate pseudo-random bytes for the encryption
        ' key. Specify the size of the key in bytes (instead of bits).
        Dim keyBytes As Byte()
        keyBytes = password.GetBytes(CInt(256 / 8))

        ' Create uninitialized Rijndael encryption object.
        Dim symmetricKey As RijndaelManaged
        symmetricKey = New RijndaelManaged()

        ' It is reasonable to set encryption mode to Cipher Block Chaining
        ' (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC

        ' Generate decryptor from the existing key bytes and initialization 
        ' vector. Key size will be defined based on the number of the key 
        ' bytes.
        Dim decryptor As ICryptoTransform
        decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim memoryStream As MemoryStream
        memoryStream = New MemoryStream(cipherTextBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim cryptoStream As CryptoStream
        cryptoStream = New CryptoStream(memoryStream, _
                                        decryptor, _
                                        CryptoStreamMode.Read)

        ' Since at this point we don't know what the size of decrypted data
        ' will be, allocate the buffer long enough to hold ciphertext;
        ' plaintext is never longer than ciphertext.
        Dim plainTextBytes As Byte()
        ReDim plainTextBytes(cipherTextBytes.Length)

        ' Start decrypting.
        Dim decryptedByteCount As Integer
        decryptedByteCount = cryptoStream.Read(plainTextBytes, _
                                               0, _
                                               plainTextBytes.Length)

        ' Close both streams.
        memoryStream.Close()
        cryptoStream.Close()

        ' Convert decrypted data into a string. 
        ' Let us assume that the original plaintext string was UTF8-encoded.
        Dim plainText As String
        plainText = Encoding.Unicode.GetString(plainTextBytes, _
                                            0, _
                                            decryptedByteCount)

        ' Return decrypted string.
        Return plainText
    End Function




    Public Function Hash(s As [String], hashProvider As HashProvider) As String
        Dim objBuffer As Byte() = Encoding.UTF8.GetBytes(s)
        Select Case hashProvider
            Case hashProvider.MD5
                Using hasher As MD5 = MD5.Create()
                    Dim strHash As Byte() = hasher.ComputeHash(objBuffer)
                    Return BitConverter.ToString(strHash).Replace("-", "").ToLowerInvariant().Trim()
                End Using
            Case hashProvider.SHA256
                Using hasher As New SHA256CryptoServiceProvider()
                    Dim strHash As Byte() = hasher.ComputeHash(objBuffer)
                    Return BitConverter.ToString(strHash).Replace("-", "").ToLowerInvariant().Trim()
                End Using
        End Select
    End Function

#End Region



    Public Function DoCompress(ByVal source As String, ByVal method As CompressionMethod) As String
        Dim sourceBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(source)
        Dim returnBytes As Byte() = DoCompress(sourceBytes, method)
        Return System.Convert.ToBase64String(returnBytes)
    End Function

    Public Function DoDeCompress(ByVal source As String, ByVal method As CompressionMethod) As String
        Dim sourceBytes As Byte() = System.Convert.FromBase64String(source)
        Dim returnBytes As Byte() = DoDeCompress(sourceBytes, method)
        Return Encoding.UTF8.GetString(returnBytes)
    End Function

    Public Function DoCompress(ByVal bytes As Byte(), ByVal method As CompressionMethod) As Byte()
        Using ms As MemoryStream = New MemoryStream
            Select Case method
                Case CompressionMethod.Deflate
                    Using gzs As DeflateStream = New DeflateStream(ms, CompressionMode.Compress, False)
                        gzs.Write(bytes, 0, bytes.Length)
                    End Using
                Case CompressionMethod.Gzip
                    Using gzs As GZipStream = New GZipStream(ms, CompressionMode.Compress, False)
                        gzs.Write(bytes, 0, bytes.Length)
                    End Using
            End Select

            ms.Close()
            Return ms.ToArray
        End Using
    End Function

    Public Function DoDecompress(ByVal bytes As Byte(), ByVal method As CompressionMethod) As Byte()
        Dim toReturn As Byte() = Nothing
        Using ms As MemoryStream = New MemoryStream(bytes, False)
            Select Case method
                Case CompressionMethod.Deflate
                    Using gzs As DeflateStream = New DeflateStream(ms, CompressionMode.Decompress, False)
                        Using dest As MemoryStream = New MemoryStream
                            Dim read As Integer
                            Dim tmp As Byte() = New Byte(bytes.Length - 1) {}
                            read = gzs.Read(tmp, 0, tmp.Length)
                            Do While (read <> 0)
                                dest.Write(tmp, 0, read)
                                read = gzs.Read(tmp, 0, tmp.Length)
                            Loop
                            dest.Close()
                            toReturn = dest.ToArray
                        End Using
                    End Using
                Case CompressionMethod.Gzip
                    Using gzs As GZipStream = New GZipStream(ms, CompressionMode.Decompress, False)
                        Using dest As MemoryStream = New MemoryStream
                            Dim read As Integer
                            Dim tmp As Byte() = New Byte(bytes.Length - 1) {}
                            read = gzs.Read(tmp, 0, tmp.Length)
                            Do While (read <> 0)
                                dest.Write(tmp, 0, read)
                                read = gzs.Read(tmp, 0, tmp.Length)
                            Loop
                            dest.Close()
                            toReturn = dest.ToArray
                        End Using
                    End Using
            End Select
        End Using
        Return toReturn
    End Function



    Public Function ParseStringList(ByVal listAsString As String, Optional ByVal separator As Char = ","c) As List(Of String)
        Dim strArray As String() = listAsString.Trim.Trim(separator).Split(New String() {separator}, StringSplitOptions.RemoveEmptyEntries)
        Dim list As New List(Of String)(strArray.Length)
        For Each str As String In strArray
            Dim item As String = str.Trim
            If (item.Length > 0) Then
                list.Add(item)
            End If
        Next
        Return list
    End Function

    Public Function ParsePairs(ByVal listAsString As String, invertPairs As Boolean, Optional ByVal itemSeparator As Char = ";"c, Optional ByVal pairSeparator As Char = "="c) As Dictionary(Of String, String)
        Dim fieldNames As New Dictionary(Of String, String)
        Dim dumpVars As List(Of String) = Common.ParseStringList(listAsString, itemSeparator)
        For Each dumpVar As String In dumpVars
            Dim splitVar() As String = dumpVar.Split("="c)
            If splitVar.Length = 2 Then
                If invertPairs Then
                    fieldNames(splitVar(1)) = splitVar(0)
                Else
                    fieldNames(splitVar(0)) = splitVar(1)
                End If
            End If
        Next
        Return fieldNames
    End Function


#End Region

#Region "Date / TimeSpan methods"

    Public ReadOnly UnixEpoch As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    Public Function ConvertToUnixTimestamp(inputDate As DateTime) As Long
        Return CType((inputDate - UnixEpoch).TotalSeconds, Long)
    End Function

    Public Function ConvertFromUnixTimestamp(unixtimestamp As Long) As DateTime
        Return UnixEpoch.AddSeconds(unixtimestamp)
    End Function

    Public Function GetMicroSeconds(ByVal span As TimeSpan) As Integer
        Return Convert.ToInt32(span.Ticks Mod TimeSpan.TicksPerMillisecond) \ 10
    End Function

    Public Function FormatTimeSpan(ByVal duration As TimeSpan) As String
        Return FormatTimeSpan(duration, False)
    End Function

    Public Function FormatTimeSpan(ByVal duration As TimeSpan, ByVal smartFormat As Boolean) As String
        Dim toReturn As String = ""
        If duration <> TimeSpan.MaxValue Then
            Dim stopNb As Integer = 0
            If Not smartFormat OrElse duration.Days > 0 Then
                toReturn &= duration.Days.ToString & " d "
                stopNb += 1
            End If
            If Not smartFormat OrElse duration.Hours > 0 Then
                toReturn &= duration.Hours.ToString & " h "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse duration.Minutes > 0 Then
                toReturn &= duration.Minutes.ToString & " mn "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse duration.Seconds > 0 Then
                toReturn &= duration.Seconds.ToString & " s "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse (stopNb < 2 AndAlso duration.Milliseconds > 0) Then
                toReturn &= duration.Milliseconds.ToString & " ms "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse stopNb < 2 Then
                toReturn &= Aricie.Common.GetMicroSeconds(duration).ToString & " µs "
            End If
        Else
            toReturn = "Not Available"
        End If

        Return toReturn
    End Function

#End Region

    Public Sub CopyStream(ByVal source As Stream, ByVal destination As Stream)
        Dim num As Integer
        Dim buffer As Byte() = New Byte(&H1000 - 1) {}
        num = source.Read(buffer, 0, buffer.Length)
        While num <> 0
            destination.Write(buffer, 0, num)
            num = source.Read(buffer, 0, buffer.Length)
        End While
    End Sub


    Public Function ReadStream(input As Stream) As Byte()
        Dim buffer As Byte() = New Byte(16 * 1024 - 1) {}
        Using ms As New MemoryStream()
            Dim read As Integer = input.Read(buffer, 0, buffer.Length)
            While (read > 0)
                ms.Write(buffer, 0, read)
                read = input.Read(buffer, 0, buffer.Length)
            End While
            Return ms.ToArray()
        End Using
    End Function


#Region "Collection methods"


    Public Sub ShuffleList(Of T)(ByRef listToShuffle As IList(Of T))
        If listToShuffle IsNot Nothing AndAlso listToShuffle.Count > 1 Then
            Dim tempSwap As T
            Dim randomIdx As Integer
            Dim random As New Random
            For i As Integer = listToShuffle.Count - 1 To 0 Step -1
                tempSwap = listToShuffle(i)
                randomIdx = random.Next(i + 1)
                listToShuffle(i) = listToShuffle(randomIdx)
                listToShuffle(randomIdx) = tempSwap
            Next
        End If
    End Sub

    Public Function GetUnifiedInstances(ByVal referenceList As IList, ByVal idPropertyName As String, ByVal externalList As IList) As IList

        Dim toReturn As New ArrayList

        If referenceList.Count > 0 Then
            Dim propInfoDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(referenceList(0).GetType())
            Dim propInfo As PropertyInfo = propInfoDico(idPropertyName)
            Dim compareProps As New Dictionary(Of Object, Boolean)(externalList.Count)
            For Each externalObject As Object In externalList
                compareProps(propInfo.GetValue(externalObject, Nothing)) = True
            Next

            For Each safeObj As Object In referenceList
                If compareProps.ContainsKey(propInfo.GetValue(safeObj, Nothing)) Then
                    toReturn.Add(safeObj)
                End If
            Next
        End If

        Return toReturn

    End Function

    Public Function GetUnifiedInstances(Of T As Class)(ByVal referenceList As IList(Of T), _
                                                         ByVal idPropertyName As String, _
                                                         ByVal externalList As IList(Of T)) As IList(Of T)

        Dim toReturn As New List(Of T)

        Dim propInfoDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(Of T)()
        Dim propInfo As PropertyInfo = propInfoDico(idPropertyName)
        Dim compareProps As New Dictionary(Of Object, Boolean)(externalList.Count)
        For Each externalObject As T In externalList
            compareProps(propInfo.GetValue(externalObject, Nothing)) = True
        Next

        For Each safeObj As T In referenceList
            If compareProps.ContainsKey(propInfo.GetValue(safeObj, Nothing)) Then
                toReturn.Add(safeObj)
            End If
        Next

        Return toReturn

    End Function

    Public Function GetUnifiedInstance(Of T As Class)(ByVal referenceList As IList(Of T), _
                                                        ByVal idPropertyName As String, ByVal externalObject As T) As T

        Return DirectCast(GetUnifiedInstance(referenceList, idPropertyName, externalObject), T)

    End Function

    Public Function GetUnifiedInstance(ByVal referenceList As IList, ByVal idPropertyName As String, ByVal externalObject As Object) As Object

        Dim toReturn As Object = Nothing

        Dim propInfoDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(externalObject.GetType)
        Dim propInfo As PropertyInfo = propInfoDico(idPropertyName)
        Dim compareProp As Object = propInfo.GetValue(externalObject, Nothing)

        For Each safeObj As Object In referenceList
            If compareProp.Equals(propInfo.GetValue(safeObj, Nothing)) Then
                toReturn = safeObj
                Exit For
            End If
        Next

        Return toReturn

    End Function

    Public Function GetComplementList(Of Y As Class)(ByVal sourceList As IList(Of Y), ByVal complement As IList(Of Y), _
                                                       ByVal unifyingPropertyName As String) As IList(Of Y)
        Dim innerComplement As IList(Of Y) = GetUnifiedInstances(Of Y)(sourceList, unifyingPropertyName, complement)
        Dim toReturn As New List(Of Y)(sourceList)
        For Each complementItem As Y In innerComplement
            toReturn.Remove(complementItem)
        Next
        Return toReturn

    End Function

    Public Function GetComplementList(ByVal sourceList As IList, ByVal complement As IList, ByVal unifyingPropertyName As String) As IList
        Dim innerComplement As IList = GetUnifiedInstances(sourceList, unifyingPropertyName, complement)
        Dim toReturn As New ArrayList(sourceList)
        For Each complementItem As Object In innerComplement
            toReturn.Remove(complementItem)
        Next
        Return toReturn

    End Function

    Public Sub AddToArray(Of T As New)(ByRef targetArray As T(), newItem As T)
        If targetArray Is Nothing Then
            targetArray = New T() {}
        End If
        Array.Resize(targetArray, targetArray.Length + 1)

        Array.Copy(New T() {newItem}, 0, targetArray, targetArray.Length - 1, 1)
    End Sub

    Public Sub ConcatArrays(Of T As New)(ByRef firstArray As T(), newArray As T())
        If newArray IsNot Nothing Then
            If firstArray Is Nothing Then
                firstArray = New T() {}
            End If
            Dim initialLength As Integer = firstArray.Length
            Array.Resize(firstArray, initialLength + newArray.Length)
            Array.Copy(newArray, 0, firstArray, initialLength, newArray.Length)
        End If
    End Sub

#End Region



    Public Sub ClearBytes(ByVal buffer() As Byte)
        ' Check arguments.
        If buffer Is Nothing Then
            Throw New ArgumentException("buffer")
        End If

        ' Set each byte in the buffer to 0.
        Dim x As Integer
        For x = 0 To buffer.Length - 1
            buffer(x) = 0
        Next x

    End Sub

    Private Const defaultKey As String = "Emergence"
    Private Const defaultSalt As String = "Serendipity"
    Private Const defaultInitVector As String = "Similarity"

    Public Sub CopyDirectory(sourceDir As String, targetDir As String, overwrite As Boolean)
        Dim cmp As New Microsoft.VisualBasic.Devices.Computer()
        cmp.FileSystem.CopyDirectory(sourceDir, targetDir, overwrite)
    End Sub

#Region " HTML methods "

    Public Function HtmlEncode(ByVal text As String) As String

        Return HttpUtility.HtmlEncode(text)

    End Function

    Public Function HtmlDecode(ByVal text As String) As String

        Return HttpUtility.HtmlDecode(text)

    End Function

    ''' <summary>
    ''' Get The cookie value
    ''' </summary>
    ''' <param name="CookieName">The Name of the cookie</param>
    ''' <returns>The Value of the cookie</returns>
    ''' <remarks></remarks>
    Public Function GetCookieValue(ByVal CookieName As String) As Object
        If HttpContext.Current.Request.Cookies(CookieName) IsNot Nothing Then
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(CookieName)
            If cookie IsNot Nothing Then
                Return cookie.Value
            End If
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Create or update a cookie
    ''' </summary>
    ''' <param name="CookieName">The Name of the cookie</param>
    ''' <param name="CookieValue">The value of the cookie</param>
    ''' <param name="Duration">The duration of the cookie in days</param>
    ''' <remarks></remarks>
    Public Sub SetCookieValue(ByVal CookieName As String, ByVal CookieValue As Object, _
                               Optional ByVal Duration As Integer = 0)
        Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(CookieName)
        cookie = New HttpCookie(CookieName)
        cookie.Value = CookieValue.ToString
        cookie.Name = CookieName
        If Duration <> 0 Then
            cookie.Expires = DateTime.Today.AddDays(Duration)
        Else
            cookie.Expires = DateTime.Today.AddYears(10)
        End If
        HttpContext.Current.Response.Cookies.Add(cookie)
    End Sub

    ''' <summary>
    ''' Open the URL in a new window
    ''' </summary>
    ''' <param name="CurrentPage">Current Page</param>
    ''' <param name="Url">Url to open</param>
    ''' <remarks></remarks>
    Public Sub ShowHasPopup(ByVal CurrentPage As Page, ByVal Url As String)

        Dim Script As String = String.Format("window.open(""{0}"", ""_Blank"");", Url)
        CurrentPage.ClientScript.RegisterStartupScript(CurrentPage.GetType(), "Redirect", Script, True)

    End Sub

#End Region

    ''' <summary>
    ''' constitu le hashcode d'une string
    ''' </summary>
    ''' <param name="source">La string dont on veux le hashcode</param>
    ''' <returns>un Entier représentant le hashcode de la string</returns>
    ''' <remarks>Adaptation en VB de la méthode GetHashCode de la classe String de .Net 32 Bits</remarks>
    Public Function GetStringHashCode(ByVal source As String) As Integer

        Dim num As Integer = &H15051505
        Dim num2 As Integer = num

        Dim sourceBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(source)

        Dim indexNumPtr = 0
        For index As Int32 = source.Length To 0 Step -4
            num = Long2Int((((num << 5) + CLng(num)) + (num >> 27)) Xor GetPtr(sourceBytes, indexNumPtr))

            If (index <= 2) Then
                Exit For
            End If

            num2 = Long2Int((((num2 << 5) + CLng(num2)) + (num2 >> 27)) Xor GetPtr(sourceBytes, indexNumPtr + 2))

            indexNumPtr += 4
            If indexNumPtr >= sourceBytes.Length Then
                Exit For
            End If
        Next

        Return Long2Int(num + (CLng(num2) * &H5D588B65))

    End Function


    Public Function GetExternalIPAddress(request As HttpRequest) As String

        Dim toReturn As String
        toReturn = request.ServerVariables("HTTP_CLIENT_IP")
        Dim addressList As String()
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        Dim strAddressList As String = request.ServerVariables("HTTP_X_FORWARDED_FOR")
        If Not String.IsNullOrEmpty(strAddressList) Then
            addressList = strAddressList.Split(","c)
            For Each address As String In addressList
                toReturn = address.Trim
                If IsExternalIPAddress(toReturn) Then
                    Return toReturn
                End If
            Next
        End If
        strAddressList = request.ServerVariables("X_FORWARDED_FOR")
        If Not String.IsNullOrEmpty(strAddressList) Then
            addressList = strAddressList.Split(","c)
            For Each address As String In addressList
                toReturn = address.Trim
                If IsExternalIPAddress(toReturn) Then
                    Return toReturn
                End If
            Next
        End If
        toReturn = request.ServerVariables("HTTP_X_FORWARDED")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        toReturn = request.ServerVariables("HTTP_X_CLUSTER_CLIENT_IP")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        toReturn = request.ServerVariables("HTTP_FORWARDED_FOR")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        toReturn = request.ServerVariables("HTTP_FORWARDED")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        Return request.UserHostAddress
    End Function


#End Region


#Region "Private methods"


    Private Function IsExternalIPAddress(ByVal address As String) As Boolean
        If Not String.IsNullOrEmpty(address) Then
            address = address.Trim
            Dim ipAdd As IPAddress = Nothing
            If IPAddress.TryParse(address, ipAdd) Then
                If ipAdd.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                    For Each privateIPRange As List(Of IPAddress) In PrivateIps
                        If CompareIPs.IsGreaterOrEqual(ipAdd, privateIPRange(0)) AndAlso CompareIPs.IsLessOrEqual(ipAdd, privateIPRange(0)) Then
                            Return False
                        End If
                    Next
                    Return True
                End If
            End If
        End If
        Return False
    End Function


    'Private ReadOnly Property PrivateIPRanges As Long(,)
    '    Get

    '    End Get
    'End Property

    Private _PrivateIps As New List(Of List(Of IPAddress))

    Private ReadOnly Property PrivateIps As List(Of List(Of IPAddress))
        Get
            If _PrivateIps.Count = 0 Then
                SyncLock _PrivateIps
                    If _PrivateIps.Count = 0 Then
                        Dim tempList As New List(Of List(Of IPAddress))
                        For i As Integer = 0 To _PrivateIPStringArray.GetUpperBound(0)
                            Dim ipPair As New List(Of IPAddress)
                            Dim lowerBoundIP As String = _PrivateIPStringArray(i, 0)
                            Dim upperBoundIP As String = _PrivateIPStringArray(i, 1)
                            ipPair.Add(IPAddress.Parse(lowerBoundIP))
                            ipPair.Add(IPAddress.Parse(upperBoundIP))
                            tempList.Add(ipPair)
                        Next
                        _PrivateIps = tempList
                    End If
                End SyncLock
            End If
            Return _PrivateIps
        End Get
    End Property





    Private _PrivateIPStringArray As String(,) = {{"0.0.0.0", "2.255.255.255"}, {"10.0.0.0", "10.255.255.255"}, {"127.0.0.0", "127.255.255.255"}, {"169.254.0.0", "169.254.255.255"}, {"172.16.0.0", "172.31.255.255"}, {"192.0.2.0", "192.0.2.255"}, {"192.168.0.0", "192.168.255.25"}, {"255.255.255.0", "255.255.255.255"}}

    ''' <summary>
    ''' converti un long en int en récupérant les 4 octets de droite du long
    ''' </summary>
    ''' <param name="longValue">un long</param>
    ''' <returns>un int constitué des 4 premiers octets du long</returns>
    ''' <remarks></remarks>
    Private Function Long2Int(ByVal longValue As Long) As Integer
        Dim numByte As Byte() = BitConverter.GetBytes(longValue)
        Return (CInt(numByte(3)) << 24) + (CInt(numByte(2)) << 16) + (CInt(numByte(1)) << 8) + numByte(0)
    End Function

    ''' <summary>
    ''' Retourne un entier selon la position demandé
    ''' </summary>
    ''' <param name="sourceBytes">Un tableau d'octets</param>
    ''' <param name="indexNumPtr">La position désiré</param>
    ''' <returns>Un entier constitué de des 2 octets à partir de l'index</returns>
    ''' <remarks></remarks>
    Private Function GetPtr(ByVal sourceBytes As Byte(), ByVal indexNumPtr As Integer) As Integer
        If indexNumPtr >= sourceBytes.Length Then
            Return 0
        Else
            Dim numPtr As Integer = sourceBytes(indexNumPtr)
            If indexNumPtr + 1 < sourceBytes.Length Then
                numPtr += (CInt(sourceBytes(indexNumPtr + 1)) << 16)
            End If
            Return numPtr
        End If
    End Function



#End Region

End Module