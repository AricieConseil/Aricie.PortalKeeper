Imports System.Security.Cryptography
Imports System.Security
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Linq

Namespace Security


    Public Enum ProtectedMode
        Memory
        Data
    End Enum

    ''' <summary>
    ''' Provides extension methods that deal with
    ''' string encryption/decryption and
    ''' <see cref="SecureString"/> encapsulation.
    ''' </summary>
    Public Module SecurityExtensions

        ''' <summary>
        ''' Specifies the data protection scope of the DPAPI.
        ''' </summary>
        Private Const Scope As DataProtectionScope = DataProtectionScope.LocalMachine

        ''' <summary>
        ''' Encrypts a given password and returns the encrypted data
        ''' as a base64 string.
        ''' </summary>
        ''' <param name="plainText">An unencrypted string that needs
        ''' to be secured.</param>
        ''' <returns>A base64 encoded string that represents the encrypted
        ''' binary data.
        ''' </returns>
        ''' <remarks>This solution is not really secure as we are
        ''' keeping strings in memory. If runtime protection is essential,
        ''' <see cref="SecureString"/> should be used.</remarks>
        ''' <exception cref="ArgumentNullException">If <paramref name="plainText"/>
        ''' is a null reference.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function Encrypt(plainText As String, mode As ProtectedMode) As String
            If plainText Is Nothing Then
                Throw New ArgumentNullException("plainText")
            End If

            'encrypt data
            Dim encrypted As Byte() = Encoding.Unicode.GetBytes(plainText)

            Select Case mode
                Case ProtectedMode.Data
                    encrypted = ProtectedData.Protect(encrypted, Nothing, Scope)
                Case ProtectedMode.Memory

                    ProtectedMemory.Protect(encrypted, MemoryProtectionScope.SameProcess)
            End Select


            'return as base64 string
            Return Convert.ToBase64String(encrypted)
        End Function

        ''' <summary>
        ''' Decrypts a given string.
        ''' </summary>
        ''' <param name="cipher">A base64 encoded string that was created
        ''' through the "CryptoHelper.Encrypt(string,ProtectedMode)" or
        ''' "Encrypt(SecureString)" extension methods.</param>
        ''' <returns>The decrypted string.</returns>
        ''' <remarks>Keep in mind that the decrypted string remains in memory
        ''' and makes your application vulnerable per se. If runtime protection
        ''' is essential, <see cref="SecureString"/> should be used.</remarks>
        ''' <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        ''' is a null reference.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function Decrypt(cipher As String, mode As ProtectedMode) As String
            If cipher Is Nothing Then
                Throw New ArgumentNullException("cipher")
            End If

            'parse base64 string
            Dim decrypted As Byte() = Convert.FromBase64String(cipher)

            Select Case mode
                Case ProtectedMode.Data
                    decrypted = ProtectedData.Unprotect(decrypted, Nothing, Scope)
                Case ProtectedMode.Memory

                    ProtectedMemory.Unprotect(decrypted, MemoryProtectionScope.SameProcess)
            End Select

            'decrypt data
            Return Encoding.Unicode.GetString(decrypted)
        End Function

        ''' <summary>
        ''' Encrypts the contents of a secure string.
        ''' </summary>
        ''' <param name="value">An unencrypted string that needs
        ''' to be secured.</param>
        ''' <returns>A base64 encoded string that represents the encrypted
        ''' binary data.
        ''' </returns>
        ''' <exception cref="ArgumentNullException">If <paramref name="value"/>
        ''' is a null reference.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function Encrypt(value As SecureString, mode As ProtectedMode) As String
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If

            Dim ptr As IntPtr = Marshal.SecureStringToCoTaskMemUnicode(value)
            Try
                Dim buffer As Char() = New Char(value.Length - 1) {}
                Marshal.Copy(ptr, buffer, 0, value.Length)

                Dim encrypted As Byte() = Encoding.Unicode.GetBytes(buffer)

                Select Case mode
                    Case ProtectedMode.Data
                        encrypted = ProtectedData.Protect(encrypted, Nothing, Scope)
                    Case ProtectedMode.Memory

                        ProtectedMemory.Protect(encrypted, MemoryProtectionScope.SameProcess)
                End Select

                'return as base64 string
                Return Convert.ToBase64String(encrypted)
            Finally
                Marshal.ZeroFreeCoTaskMemUnicode(ptr)
            End Try
        End Function

        ''' <summary>
        ''' Decrypts a base64 encrypted string and returns the decrpyted data
        ''' wrapped into a <see cref="SecureString"/> instance.
        ''' </summary>
        ''' <param name="cipher">A base64 encoded string that was created
        ''' through the Encrypt(string) or
        ''' "Encrypt(SecureString)" extension methods.</param>
        ''' <returns>The decrypted string, wrapped into a
        ''' <see cref="SecureString"/> instance.</returns>
        ''' <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        ''' is a null reference.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function DecryptSecure(cipher As String, mode As ProtectedMode) As SecureString
            If cipher Is Nothing Then
                Throw New ArgumentNullException("cipher")
            End If

            'parse base64 string
            Dim decrypted As Byte() = Convert.FromBase64String(cipher)

            Select Case mode
                Case ProtectedMode.Data
                    decrypted = ProtectedData.Unprotect(decrypted, Nothing, Scope)
                Case ProtectedMode.Memory

                    ProtectedMemory.Unprotect(decrypted, MemoryProtectionScope.SameProcess)
            End Select


            Dim ss As New SecureString()

            'parse characters one by one - doesn't change the fact that
            'we have them in memory however...
            Dim count As Integer = Encoding.Unicode.GetCharCount(decrypted)
            Dim bc As Integer = decrypted.Length \ count
            For i As Integer = 0 To count - 1
                ss.AppendChar(Encoding.Unicode.GetChars(decrypted, i * bc, bc)(0))
            Next

            'mark as read-only
            ss.MakeReadOnly()
            Return ss
        End Function

        ''' <summary>
        ''' Wraps a managed string into a <see cref="SecureString"/> 
        ''' instance.
        ''' </summary>
        ''' <param name="value">A string or char sequence that 
        ''' should be encapsulated.</param>
        ''' <returns>A <see cref="SecureString"/> that encapsulates the
        ''' submitted value.</returns>
        ''' <exception cref="ArgumentNullException">If <paramref name="value"/>
        ''' is a null reference.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function ToSecureString(value As IEnumerable(Of Char), makeReadonly As Boolean) As SecureString
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If

            Dim secured = New SecureString()

            Dim charArray = value.ToArray()
            For i As Integer = 0 To charArray.Length - 1
                secured.AppendChar(charArray(i))
            Next
            If makeReadonly Then
                secured.MakeReadOnly()
            End If

            Return secured
        End Function

        ''' <summary>
        ''' Unwraps the contents of a secured string and
        ''' returns the contained value.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks>Be aware that the unwrapped managed string can be
        ''' extracted from memory.</remarks>
        ''' <exception cref="ArgumentNullException">If <paramref name="value"/>
        ''' is a null reference.</exception>
        <System.Runtime.CompilerServices.Extension> _
        Public Function UnwrapToString(value As SecureString) As String
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If

            Dim ptr As IntPtr = IntPtr.Zero
            Try
                ptr = Marshal.SecureStringToCoTaskMemUnicode(value)
                Return Marshal.PtrToStringUni(ptr)
            Finally
                Marshal.ZeroFreeCoTaskMemUnicode(ptr)
            End Try
        End Function

        ''' <summary>
        ''' Checks whether a <see cref="SecureString"/> is either
        ''' null or has a <see cref="SecureString.Length"/> of 0.
        ''' </summary>
        ''' <param name="value">The secure string to be inspected.</param>
        ''' <returns>True if the string is either null or empty.</returns>
        <System.Runtime.CompilerServices.Extension> _
        Public Function IsNullOrEmpty(value As SecureString) As Boolean
            Return value Is Nothing OrElse value.Length = 0
        End Function

        ''' <summary>
        ''' Performs bytewise comparison of two secure strings.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="other"></param>
        ''' <returns>True if the strings are equal.</returns>
        <System.Runtime.CompilerServices.Extension> _
        Public Function Matches(value As SecureString, other As SecureString) As Boolean
            If value Is Nothing AndAlso other Is Nothing Then
                Return True
            End If
            If value Is Nothing OrElse other Is Nothing Then
                Return False
            End If
            If value.Length <> other.Length Then
                Return False
            End If
            If value.Length = 0 AndAlso other.Length = 0 Then
                Return True
            End If

            Dim ptrA As IntPtr = Marshal.SecureStringToCoTaskMemUnicode(value)
            Dim ptrB As IntPtr = Marshal.SecureStringToCoTaskMemUnicode(other)
            Try
                'parse characters one by one - doesn't change the fact that
                'we have them in memory however...
                Dim byteA As Byte = 1
                Dim byteB As Byte = 1

                Dim index As Integer = 0
                While Convert.ToChar(byteA) <> ControlChars.NullChar AndAlso Convert.ToChar(byteB) <> ControlChars.NullChar
                    byteA = Marshal.ReadByte(ptrA, index)
                    byteB = Marshal.ReadByte(ptrB, index)
                    If byteA <> byteB Then
                        Return False
                    End If
                    index += 2
                End While

                Return True
            Finally
                Marshal.ZeroFreeCoTaskMemUnicode(ptrA)
                Marshal.ZeroFreeCoTaskMemUnicode(ptrB)
            End Try
            Return False
        End Function

    End Module
End Namespace