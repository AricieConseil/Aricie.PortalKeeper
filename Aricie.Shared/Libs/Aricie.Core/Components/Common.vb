Imports System.Collections.Specialized
Imports System.Reflection
Imports Aricie.Services
Imports System.Web
Imports System.Web.UI
Imports System.Text
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Xml
Imports System.Linq
Imports System.Net.Sockets
Imports Aricie.Security.Cryptography
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Devices
Imports System.Globalization

''' <summary>
''' Collection of general purpose helper functions
''' </summary>
Public Module Common

#Region " Public Methods "


#Region "Generics"

    <Extension()> _
    Public Function IsAssignableToGenericType(givenType As Type, genericType As Type) As Boolean
        Dim interfaceTypes = givenType.GetInterfaces()

        For Each it As Type In interfaceTypes
            If it.IsGenericType AndAlso it.GetGenericTypeDefinition() Is genericType Then
                Return True
            End If
        Next

        If givenType.IsGenericType AndAlso givenType.GetGenericTypeDefinition() Is genericType Then
            Return True
        End If

        Dim baseType As Type = givenType.BaseType
        If baseType Is Nothing Then
            Return False
        End If

        Return IsAssignableToGenericType(baseType, genericType)
    End Function

    <Extension()> _
    Public Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

#End Region


#Region " Enum methods "

    <Extension()> _
    Public Function GetEnum(Of T)(strEnum As String) As T
        Return DirectCast([Enum].Parse(GetType(T), strEnum), T)
    End Function

    Public Function GetEnumMembers(Of T)() As List(Of T)
        Return (From objEnum In [Enum].GetValues(GetType(T)) Select DirectCast(objEnum, T)).ToList()
    End Function

    Public Function GetNextCyclicFlag(Of T As {IConvertible})(current As T, availableValues As T) As T
        Dim toReturn As T
        Dim foundNext As Boolean
        Dim foundBefore = False
        Dim firstAvailable As T
        For Each flagOption As T In GetEnumMembers(Of T)()
            If Convert.ToInt32(flagOption) <> 0 Then
                If Convert.ToInt32(current) < Convert.ToInt32(flagOption) Then
                    If ((Convert.ToInt32(availableValues) And Convert.ToInt32(flagOption)) = Convert.ToInt32(flagOption)) Then
                        foundNext = True
                        toReturn = flagOption
                        Exit For
                    End If
                ElseIf Not foundBefore Then
                    If ((Convert.ToInt32(availableValues) And Convert.ToInt32(flagOption)) = Convert.ToInt32(flagOption)) Then
                        firstAvailable = flagOption
                        foundBefore = True
                    End If
                End If
            End If
        Next
        If Not foundNext Then
            toReturn = firstAvailable
        End If
        Return toReturn
    End Function

#End Region

#Region "String methods "

    <Extension>
    Public Function IsNullOrEmpty(value As String) As Boolean
        Return String.IsNullOrEmpty(value)
    End Function


    ''' <summary>
    ''' Reverses the specified string.
    ''' </summary>
    ''' <param name="input">The string to reverse.</param>
    ''' <returns>The input string, reversed.</returns>
    ''' <remarks>This method correctly reverses strings containing supplementary characters
    ''' (which are encoded with two surrogate code units).</remarks>
    <System.Runtime.CompilerServices.Extension>
    Public Function Reverse(input As String) As String
        If input Is Nothing Then
            Throw New ArgumentNullException("input")
        End If

        ' allocate a buffer to hold the output
        Dim output As Char() = New Char(input.Length - 1) {}
        Dim outputIndex As Integer = 0, inputIndex As Integer = input.Length - 1
        While outputIndex < input.Length
            ' check for surrogate pair
            If AscW( input(inputIndex)) >= &HDC00 _
                AndAlso AscW(input(inputIndex)) <= &HDFFF _
                AndAlso inputIndex > 0 _
                AndAlso AscW(input(inputIndex - 1)) >= &HD800 _
                AndAlso AscW(input(inputIndex - 1)) <= &HDBFF Then
                ' preserve the order of the surrogate pair code units
                output(outputIndex + 1) = input(inputIndex)
                output(outputIndex) = input(inputIndex - 1)
                outputIndex += 1
                inputIndex -= 1
            Else
                output(outputIndex) = input(inputIndex)
            End If
            outputIndex += 1
            inputIndex -= 1
        End While

        Return New String(output)
    End Function

    Public Function StringToByteArray(hexInput As String) As Byte()
        Return Enumerable.Range(0, hexInput.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(hexInput.Substring(x, 2), 16)).ToArray()
    End Function

    Public Function BitConverterStringToByteArray(hexBitConverter As String) As Byte()
        Dim hexInput As String = hexBitConverter.Replace("-"c, "").Trim()
        Return StringToByteArray(hexInput)
    End Function




    <Extension> _
    Public Function ToStream(str As String) As Stream
        Dim stream As New MemoryStream()
        Dim writer As New StreamWriter(stream)
        writer.Write(str)
        writer.Flush()
        stream.Position = 0
        Return stream
    End Function

    <Extension> _
    Public Function FromUtf8(stream As MemoryStream) As String
        Return stream.ToArray().FromUTF8()
    End Function

    <Extension> _
    Public Function GetBase64FromUtf8(strUtf8 As String) As String
        Return Convert.ToBase64String(Encoding.UTF8.GetBytes(strUtf8))
    End Function

    <Extension> _
    Public Function GetBase64FromEncoding(strToEncode As String, objEncoding As Encoding) As String
        Return Convert.ToBase64String(objEncoding.GetBytes(strToEncode))
    End Function

    <Extension> _
    Public Function GetFromBase64(strBase64 As String, objEncoding As Encoding) As String
        Return objEncoding.GetString(Convert.FromBase64String(strBase64))
    End Function

    <Extension> _
    Public Function GetUtf8FromBase64(strBase64 As String) As String
        Return Encoding.UTF8.GetString(Convert.FromBase64String(strBase64))
    End Function

    <Extension> _
    Public Function GetUtf8FromBase64(objBytes As Byte()) As Byte()
        Return Encoding.UTF8.GetBytes(Convert.ToBase64String(objBytes))
    End Function

    <Extension> _
    Public Function GetBase64FromUtf8(objBytes As Byte()) As Byte()
        Return Convert.FromBase64String(Encoding.UTF8.GetString(objBytes))
    End Function

    <Extension> _
    Public Function ToBase64(objBytes As Byte()) As String
        Return Convert.ToBase64String(objBytes)
    End Function

    <Extension> _
    Public Function FromBase64(value As String) As Byte()
        Return Convert.FromBase64String(value)
    End Function

    <Extension> _
    Public Function FromUTF8(objBytes As Byte()) As String
        Return Encoding.UTF8.GetString(objBytes)
    End Function

    <Extension> _
    Public Function ToUTF8(value As String) As Byte()
        Return Encoding.UTF8.GetBytes(value)
    End Function

    <Extension> _
    Public Function AppendBytes(first As Byte(), second As Byte()) As Byte()
        Dim ret = New Byte(first.Length + (second.Length - 1)) {}
        Buffer.BlockCopy(first, 0, ret, 0, first.Length)
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length)
        Return ret
    End Function

    <Extension()> _
    Public Function OnlyAlphaNumericChars(objString As String) As String

        objString = Trim(objString)
        Dim lLen As Integer
        Dim sAns As New StringBuilder(objString.Length)
        Dim lCtr As Integer
        Dim sChar As String


        For lCtr = 1 To lLen
            sChar = Mid(objString, lCtr, 1)
            If sChar.IsAlphaNumeric() Then
                sAns.Append(sChar)
            End If
        Next

        OnlyAlphaNumericChars = Replace(sAns.ToString, " ", "-")

    End Function

    <Extension> _
    Public Function IsAlphaNumeric(sChr As String) As Boolean
        IsAlphaNumeric = sChr Like "[0-9A-Za-z ]"
    End Function

    <Extension> _
    Public Function IsNumber(sChr As String) As Boolean
        For i = 0 To sChr.Length - 1
            If Not Char.IsDigit(sChr, i) Then
                Return False
            End If
        Next
        Return True
    End Function

    <Extension> _
    Public Function PathHasInvalidChars(path As String) As Boolean
        Return (Not String.IsNullOrEmpty(path) AndAlso path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
    End Function

    Public Function BytesToString(byteCount As Long) As [String]
        Dim suf As String() = {"B", "KB", "MB", "GB", "TB", "PB", "EB"}
        'Longs run out around EB
        If byteCount = 0 Then
            Return "0" + suf(0)
        End If
        Dim bytes As Long = Math.Abs(byteCount)
        Dim place As Integer = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)))
        Dim num As Double = Math.Round(bytes / Math.Pow(1024, place), 1)
        Return (Math.Sign(byteCount) * num).ToString() + suf(place)
    End Function

    Public WordRegex As New Regex("\p{Lu}\p{Ll}+|\p{Lu}+(?!\p{Ll})|\p{Ll}+|\d+", RegexOptions.Compiled)
    Public SplitRegex As New Regex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", RegexOptions.Compiled)

    <Extension> _
    Public Function ToPascalCase(input As String) As String
        Return WordRegex.Replace(input, AddressOf EvaluatePascal)
    End Function

    <Extension> _
    Public Function ToCamelCase(input As String) As String
        Dim pascal As String = input.ToPascalCase()
        Return WordRegex.Replace(pascal, AddressOf EvaluateFirstCamel, 1)
    End Function

    <Extension> _
    Public Function ToTitleCase(input As String) As String
        Dim pascal As String = input.ToPascalCase()
        Return SplitRegex.Replace(pascal, " $1")
    End Function

    Private Function EvaluateFirstCamel(match As Match) As String
        Return match.Value.ToLower()
    End Function

    Private Function EvaluatePascal(match As Match) As String
        Dim value As String = match.Value
        Dim valueLength As Integer = value.Length

        If valueLength = 1 Then
            Return value.ToUpper()
        Else
            If valueLength <= 2 AndAlso value.IsWordUpper() Then
                Return value
            Else
                Return value.Substring(0, 1).ToUpper() + value.Substring(1, valueLength - 1).ToLower()
            End If
        End If
    End Function

    <Extension> _
    Public Function IsWordUpper(word As String) As Boolean
        Return word.All(Function(c) Not [Char].IsLower(c))
    End Function

    <Extension> _
    Public Function Compress(source As String, method As CompressionMethod) As String
        Dim sourceBytes As Byte() = Encoding.UTF8.GetBytes(source)
        Dim returnBytes As Byte() = sourceBytes.Compress(method)
        Return Convert.ToBase64String(returnBytes)
    End Function

    <Extension> _
    Public Function Decompress(source As String, method As CompressionMethod) As String
        Dim sourceBytes As Byte() = Convert.FromBase64String(source)
        Dim returnBytes As Byte() = sourceBytes.Decompress(method)
        Return Encoding.UTF8.GetString(returnBytes)
    End Function




    <Extension> _
    Public Function Compress(bytes As Byte(), method As CompressionMethod) As Byte()
        'Using ms = New MemoryStream
        '    Select Case method
        '        Case CompressionMethod.Deflate
        '            Using gzs = New DeflateStream(ms, CompressionMode.Compress, False)
        '                gzs.Write(bytes, 0, bytes.Length)
        '            End Using
        '        Case CompressionMethod.Gzip
        '            Using gzs = New GZipStream(ms, CompressionMode.Compress, False)
        '                gzs.Write(bytes, 0, bytes.Length)
        '            End Using
        '    End Select

        '    ms.Close()
        '    Return ms.ToArray
        'End Using
        Using ms = New MemoryStream(bytes, False)
            Using destStream As MemoryStream = ms.Compress(method)
                Return destStream.ToArray()
            End Using
        End Using
    End Function

    <Extension> _
    Public Function Compress(objStream As Stream, method As CompressionMethod) As MemoryStream
        Dim toreturn As New MemoryStream()
        'Dim read As Integer
        'Dim tmp = New Byte(CInt(objStream.Length - 1)) {}
        Select Case method
            Case CompressionMethod.Deflate
                Using gzs As New DeflateStream(toreturn, CompressionMode.Compress, True)
                    objStream.CopyStream(gzs)
                    'read = objStream.Read(tmp, 0, tmp.Length)
                    'Do While (read <> 0)
                    '    gzs.Write(tmp, 0, read)
                    '    read = objStream.Read(tmp, 0, tmp.Length)
                    'Loop
                    'gzs.Close()
                End Using
            Case CompressionMethod.Gzip
                Using gzs As New GZipStream(toreturn, CompressionMode.Compress, True)
                    objStream.CopyStream(gzs)
                    'read = objStream.Read(tmp, 0, tmp.Length)
                    'Do While (read <> 0)
                    '    gzs.Write(tmp, 0, read)
                    '    read = objStream.Read(tmp, 0, tmp.Length)
                    'Loop
                    'gzs.Close()
                End Using
        End Select

        toreturn.Seek(0, SeekOrigin.Begin)
        Return toreturn
    End Function


    <Extension> _
    Public Function Decompress(bytes As Byte(), method As CompressionMethod) As Byte()
        Using ms = New MemoryStream(bytes, False)
            Using destStream As MemoryStream = ms.Decompress(method)
                Return destStream.ToArray()
            End Using
        End Using
    End Function



    <Extension> _
    Public Function Decompress(objStream As Stream, method As CompressionMethod) As MemoryStream
        Dim toreturn As New MemoryStream()
        'Dim read As Integer
        'Dim tmp = New Byte(CInt(objStream.Length - 1)) {}
        Select Case method
            Case CompressionMethod.Deflate
                Using gzs = New DeflateStream(objStream, CompressionMode.Decompress, False)
                    'read = gzs.Read(tmp, 0, tmp.Length)
                    'Do While (read <> 0)
                    '    toreturn.Write(tmp, 0, read)
                    '    read = gzs.Read(tmp, 0, tmp.Length)
                    'Loop
                    gzs.CopyStream(toreturn)
                    'gzs.Close()
                End Using
            Case CompressionMethod.Gzip
                Using gzs = New GZipStream(objStream, CompressionMode.Decompress, False)
                    'read = gzs.Read(tmp, 0, tmp.Length)
                    'Do While (read <> 0)
                    '    toreturn.Write(tmp, 0, read)
                    '    read = gzs.Read(tmp, 0, tmp.Length)
                    'Loop
                    gzs.CopyStream(toreturn)
                    'gzs.Close()
                End Using
        End Select
        toreturn.Seek(0, SeekOrigin.Begin)
        Return toreturn
    End Function





    <Extension> _
    Public Function LinesCount(s As String) As Integer
        Dim count As Integer
        Dim position As Integer
        While (InlineAssignHelper(position, s.IndexOf(ControlChars.Lf, position))) <> -1
            count += 1
            ' Skip this occurrence!
            position += 1
        End While
        Return count
    End Function

    Public Function RestrictedLineCount(value As String, minLines As Integer, maxLines As Integer) As Integer
        Return Math.Max(minLines, Math.Min( _
                        Math.Max(value.Length \ 50, _
                        value.LinesCount()) _
                        , maxLines))
    End Function

    Public Function GetHtmlLineBreaks(source As String) As String
        Return source.Replace(vbCrLf, "<br/>").Replace(vbLf, "<br/>")
    End Function

    <Extension()> _
    Public Function ParseStringList(listAsString As String, Optional ByVal separator As Char = ","c) As List(Of String)
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

    Public Function ParsePairs(listAsString As String, invertPairs As Boolean, Optional ByVal itemSeparator As Char = ";"c, Optional ByVal pairSeparator As Char = "="c) As Dictionary(Of String, String)
        Dim fieldNames As New Dictionary(Of String, String)
        Dim dumpVars As List(Of String) = ParseStringList(listAsString, itemSeparator)
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

    <Extension> _
    Public Function Beautify(doc As XmlDocument) As String
        Return doc.Beautify(False)
    End Function

    <Extension> _
    Public Function Beautify(doc As XmlDocument, omitDeclaration As Boolean) As String
        Dim sb As New StringBuilder()
        Dim settings As New XmlWriterSettings()
        settings.Indent = True
        settings.IndentChars = "  "
        settings.NewLineChars = vbCr & vbLf
        settings.NewLineHandling = NewLineHandling.Replace
        settings.OmitXmlDeclaration = omitDeclaration
        Using writer As XmlWriter = XmlWriter.Create(sb, settings)
            doc.Save(writer)
        End Using
        Return sb.ToString()
    End Function


#End Region


#Region "Crypto"



    <Obsolete("Use methods in the CryptoHelper module")>
    Public Function Encrypt(plainText As String, _
                         key As String, _
                               Optional ByVal initVector As String = defaultInitVector, _
                              Optional ByVal salt As String = defaultSalt) _
                       As String


        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.Unicode.GetBytes(salt)

        Return Convert.ToBase64String(CryptoHelper.Encrypt(Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(initVector), saltValueBytes))

    End Function



    <Obsolete("Use methods in the CryptoHelper module")>
    Public Function Decrypt(encryptedText As String, _
                                Optional ByVal key As String = defaultKey, _
                                Optional ByVal initVector As String = defaultInitVector, _
                                Optional ByVal salt As String = defaultSalt) As String


        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.Unicode.GetBytes(salt)



        Return Encoding.UTF8.GetString(CryptoHelper.Decrypt(Convert.FromBase64String(encryptedText), Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(initVector), saltValueBytes))

    End Function







#End Region



#Region "Date / TimeSpan methods"

    Public ReadOnly UnixEpoch As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    <Extension> _
    Public Function ConvertToUnixTimestamp(inputDate As DateTime) As Long
        Return CType((inputDate - UnixEpoch).TotalSeconds, Long)
    End Function

    <Extension> _
    Public Function ConvertFromUnixTimestamp(unixtimestamp As Long) As DateTime
        Return UnixEpoch.AddSeconds(unixtimestamp)
    End Function

    <Extension> _
    Public Function GetMicroSeconds(span As TimeSpan) As Integer
        Return Convert.ToInt32(span.Ticks Mod TimeSpan.TicksPerMillisecond) \ 10
    End Function

    <Extension> _
    Public Function FormatTimeSpan(duration As TimeSpan) As String
        Return duration.FormatTimeSpan(True)
    End Function

    <Extension> _
    Public Function FormatTimeSpan(duration As TimeSpan, smartFormat As Boolean) As String
        Dim toReturn = ""
        If duration <> TimeSpan.MaxValue Then
            Dim stopNb = 0
            If Not smartFormat OrElse duration.Days > 0 Then
                toReturn &= duration.Days.ToString(CultureInfo.InvariantCulture) & " d "
                stopNb += 1
            End If
            If Not smartFormat OrElse duration.Hours > 0 Then
                toReturn &= duration.Hours.ToString(CultureInfo.InvariantCulture) & " h "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse duration.Minutes > 0 Then
                toReturn &= duration.Minutes.ToString(CultureInfo.InvariantCulture) & " mn "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse duration.Seconds > 0 Then
                toReturn &= duration.Seconds.ToString(CultureInfo.InvariantCulture) & " s "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse (stopNb < 2 AndAlso duration.Milliseconds > 0) Then
                toReturn &= duration.Milliseconds.ToString(CultureInfo.InvariantCulture) & " ms "
                stopNb += 1
            ElseIf stopNb > 0 Then
                stopNb += 1
            End If
            If Not smartFormat OrElse stopNb < 2 Then
                toReturn &= duration.GetMicroSeconds().ToString(CultureInfo.InvariantCulture) & " µs "
            End If
        Else
            toReturn = "Not Available"
        End If

        Return toReturn
    End Function

#End Region

    <Extension> _
    Public Sub CopyStream(source As Stream, destination As Stream)
        Dim num As Integer
        Dim buffer = New Byte(&H1000 - 1) {}
        num = source.Read(buffer, 0, buffer.Length)
        While num <> 0
            destination.Write(buffer, 0, num)
            num = source.Read(buffer, 0, buffer.Length)
        End While
    End Sub

    <Extension> _
    Public Function ReadStream(input As Stream) As Byte()
        Dim buffer = New Byte(16 * 1024 - 1) {}
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

    <Extension> _
    Public Function TryGetValue(Of T)(collection As IDictionary(Of String, Object), key As String, ByRef value As T) As Boolean
        If collection Is Nothing Then
            Throw New ArgumentNullException("collection")
        End If
        Dim obj As Object = Nothing
        If collection.TryGetValue(key, obj) AndAlso TypeOf obj Is T Then
            value = DirectCast(obj, T)
            Return True
        Else
            value = Nothing
            Return False
        End If
    End Function

    <Extension> _
    Public Function FindKeysWithPrefix(Of TValue)(dictionary As IDictionary(Of String, TValue), prefix As String) As IEnumerable(Of KeyValuePair(Of String, TValue))
        Dim toReturn As New List(Of KeyValuePair(Of String, TValue))
        If dictionary Is Nothing Then
            Throw New ArgumentNullException("dictionary")
        End If
        If prefix Is Nothing Then
            Throw New ArgumentNullException("prefix")
        End If
        Dim exactMatchValue As TValue
        If dictionary.TryGetValue(prefix, exactMatchValue) Then
            toReturn.Add(New KeyValuePair(Of String, TValue)(prefix, exactMatchValue))
        End If
        For Each keyValuePair As KeyValuePair(Of String, TValue) In DirectCast(dictionary, IEnumerable(Of KeyValuePair(Of String, TValue)))
            Dim key As String = keyValuePair.Key
            If key.Length > prefix.Length AndAlso key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                If prefix.Length = 0 Then
                    toReturn.Add(keyValuePair)
                Else
                    Dim charAfterPrefix As Char = key(prefix.Length)
                    Select Case charAfterPrefix
                        Case "."c, "["c
                            toReturn.Add(keyValuePair)
                        Case Else
                    End Select
                End If
            End If
        Next
        Return toReturn
    End Function

    <Extension> _
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

    Public Function GetUnifiedInstances(referenceList As IList, idPropertyName As String, externalList As IList) As IList

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

    Public Function GetUnifiedInstances(Of T As Class)(referenceList As IList(Of T), _
                                                         idPropertyName As String, _
                                                         externalList As IList(Of T)) As IList(Of T)

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

    Public Function GetUnifiedInstance(Of T As Class)(referenceList As IList(Of T), _
                                                        idPropertyName As String, externalObject As T) As T

        Return DirectCast(GetUnifiedInstance(referenceList, idPropertyName, externalObject), T)

    End Function

    Public Function GetUnifiedInstance(referenceList As IList, idPropertyName As String, externalObject As Object) As Object

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

    Public Function GetComplementList(Of Y As Class)(sourceList As IList(Of Y), complement As IList(Of Y), _
                                                       unifyingPropertyName As String) As IList(Of Y)
        Dim innerComplement As IList(Of Y) = GetUnifiedInstances(Of Y)(sourceList, unifyingPropertyName, complement)
        Dim toReturn As New List(Of Y)(sourceList)
        For Each complementItem As Y In innerComplement
            toReturn.Remove(complementItem)
        Next
        Return toReturn

    End Function

    Public Function GetComplementList(sourceList As IList, complement As IList, unifyingPropertyName As String) As IList
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


    <Extension> _
    Public Sub ClearBytes(buffer() As Byte)
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
        Dim cmp As New Computer()
        cmp.FileSystem.CopyDirectory(sourceDir, targetDir, overwrite)
    End Sub

#Region " HTML methods "

    <Extension> _
    Public Function HtmlEncode(text As String) As String

        Return HttpUtility.HtmlEncode(text)

    End Function

    <Extension> _
    Public Function HtmlDecode(text As String) As String

        Return HttpUtility.HtmlDecode(text)

    End Function

    ''' <summary>
    ''' Get The cookie value
    ''' </summary>
    ''' <param name="CookieName">The Name of the cookie</param>
    ''' <returns>The Value of the cookie</returns>
    ''' <remarks></remarks>
    Public Function GetCookieValue(CookieName As String) As Object
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
    Public Sub SetCookieValue(CookieName As String, CookieValue As Object, _
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
    Public Sub ShowHasPopup(CurrentPage As Page, Url As String)

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
    Public Function GetStringHashCode(source As String) As Integer

        Dim num = &H15051505
        Dim num2 As Integer = num

        Dim sourceBytes As Byte() = Encoding.UTF8.GetBytes(source)

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

    <Extension> _
    Public Function GetExternalIPAddress(request As HttpRequest) As String

        Dim objServerVariables As NameValueCollection = request.ServerVariables

        Return GetExternalIPAddress(objServerVariables)
    End Function


    Public Function GetExternalIPAddress(ByVal objServerVariables As NameValueCollection) As String
        Dim fallBack As String = objServerVariables.Get("REMOTE_ADDR")
        Dim toReturn As String
        toReturn = objServerVariables.Get("HTTP_CLIENT_IP")
        Dim addressList As String()
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        Dim strAddressList As String = objServerVariables.Get("HTTP_X_FORWARDED_FOR")
        If Not String.IsNullOrEmpty(strAddressList) Then
            addressList = strAddressList.Split(","c)
            For Each address As String In addressList
                toReturn = address.Trim
                If IsExternalIPAddress(toReturn) Then
                    Return toReturn
                End If
            Next
        End If
        strAddressList = objServerVariables.Get("X_FORWARDED_FOR")
        If Not String.IsNullOrEmpty(strAddressList) Then
            addressList = strAddressList.Split(","c)
            For Each address As String In addressList
                toReturn = address.Trim
                If IsExternalIPAddress(toReturn) Then
                    Return toReturn
                End If
            Next
        End If
        toReturn = objServerVariables.Get("HTTP_X_FORWARDED")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        toReturn = objServerVariables.Get("HTTP_X_CLUSTER_CLIENT_IP")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        toReturn = objServerVariables.Get("HTTP_FORWARDED_FOR")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        toReturn = objServerVariables.Get("HTTP_FORWARDED")
        If IsExternalIPAddress(toReturn) Then
            Return toReturn
        End If
        Return fallBack
    End Function

    Public Sub CallDebuggerBreak()
        CallDebuggerBreak(True)
    End Sub

    Private debuggerCalled As Boolean
    Public Sub CallDebuggerBreak(onceOnly As Boolean)
        If Not debuggerCalled Then
            debuggerCalled = True
            If Not Debugger.IsAttached Then
                Debugger.Launch()
            End If
            Debugger.Break()
        End If
    End Sub

#End Region


#Region "Private methods"


    Private Function IsExternalIPAddress(address As String) As Boolean
        If Not String.IsNullOrEmpty(address) Then
            address = address.Trim
            Dim ipAdd As IPAddress = Nothing
            If IPAddress.TryParse(address, ipAdd) Then
                If ipAdd.AddressFamily = AddressFamily.InterNetwork Then
                    Return PrivateIps.All(Function(privateIpRange) _
                                              Not CompareIPs.IsGreaterOrEqual(ipAdd, privateIpRange(0)) _
                                              OrElse Not CompareIPs.IsLessOrEqual(ipAdd, privateIpRange(0)))
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
                        For i = 0 To PrivateIpStringArray.GetUpperBound(0)
                            Dim ipPair As New List(Of IPAddress)
                            Dim lowerBoundIp As String = PrivateIpStringArray(i, 0)
                            Dim upperBoundIp As String = PrivateIpStringArray(i, 1)
                            ipPair.Add(IPAddress.Parse(lowerBoundIp))
                            ipPair.Add(IPAddress.Parse(upperBoundIp))
                            tempList.Add(ipPair)
                        Next
                        _PrivateIps = tempList
                    End If
                End SyncLock
            End If
            Return _PrivateIps
        End Get
    End Property





    Private ReadOnly PrivateIpStringArray As String(,) = {{"0.0.0.0", "2.255.255.255"}, _
                                                          {"10.0.0.0", "10.255.255.255"}, _
                                                          {"127.0.0.0", "127.255.255.255"}, _
                                                          {"169.254.0.0", "169.254.255.255"}, _
                                                          {"172.16.0.0", "172.31.255.255"}, _
                                                          {"192.0.2.0", "192.0.2.255"}, _
                                                          {"192.168.0.0", "192.168.255.25"}, _
                                                          {"255.255.255.0", "255.255.255.255"}}

    ''' <summary>
    ''' converti un long en int en récupérant les 4 octets de droite du long
    ''' </summary>
    ''' <param name="longValue">un long</param>
    ''' <returns>un int constitué des 4 premiers octets du long</returns>
    ''' <remarks></remarks>
    Private Function Long2Int(longValue As Long) As Integer
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
    Private Function GetPtr(sourceBytes As Byte(), indexNumPtr As Integer) As Integer
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