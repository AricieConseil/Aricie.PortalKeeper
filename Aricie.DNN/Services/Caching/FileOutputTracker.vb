Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.Text
Imports System.IO

Namespace Services.Caching
    Public Class FileOutputTracker
        Inherits OutputTrackerBase


        Private _FileOutputSettings As FileOutputSettings



        Private Shared _scavangingCompleted As Integer

        Private Const DataFileExt As String = ".data"
        Private Const InfoFileExt As String = ".info"
        Private Const InfoTagName As String = "diskOutputCacheItem"
        Private Const TempFileExt As String = ".temp"

        ' Methods
        Public Sub New(ByVal parent As OutputTrackerBase, ByVal varyById As String)
            MyBase.New(parent, varyById)
            Me._FileOutputSettings = DirectCast(parent, FileOutputTracker)._FileOutputSettings
        End Sub

        'Public Sub New(ByVal path As String, ByVal filePrefix As String, ByVal duration As TimeSpan, ByVal verbs As String(), ByVal varyBy As String(), ByVal emptyQueryStringOnly As Boolean, ByVal emptyPathInfoOnly As Boolean, ByVal serveFromMemory As Boolean)
        '    'Me._path = path
        '    'Me._duration = duration
        '    'Me._verbs = DirectCast(verbs.Clone, String())
        '    'Me._varyBy = DirectCast(varyBy.Clone, String())
        '    'Me._emptyQueryStringOnly = emptyQueryStringOnly
        '    'Me._emptyPathInfoOnly = emptyPathInfoOnly
        '    Me._serveFromMemory = serveFromMemory
        '    Me._varyById = String.Empty
        '    Me._filenamePrefix = filePrefix
        '    Me._capturingEvent = New ManualResetEvent(True)
        '    Me._varyByTrackers = New Dictionary(Of String, Tracker)
        'End Sub

        Public Sub New(ByVal path As String, ByVal objStrategy As OutputCachingStrategy, ByVal fileOutputSettings As FileOutputSettings)
            MyBase.New(path, objStrategy)
            Me._FileOutputSettings = fileOutputSettings
        End Sub

        'Public Property FileOutputSettings() As FileOutputSettings
        '    Get
        '        Return _FileOutputSettings
        '    End Get
        '    Set(ByVal value As FileOutputSettings)
        '        _FileOutputSettings = value
        '    End Set
        'End Property

        ' Properties
        Friend Shared ReadOnly Property ScavangingCompleted() As Boolean
            Get
                Return Thread.VolatileRead(FileOutputTracker._scavangingCompleted) = 1
            End Get
        End Property






#Region "Public Methods"




        'Public Function FindTrackerForRequest(ByVal request As HttpRequest) As FileOutputTracker
        '    Dim toReturn As FileOutputTracker = Nothing
        '    If (String.Compare(Me.Path, request.FilePath, StringComparison.OrdinalIgnoreCase) = 0) Then
        '        For i = 0 To Me.Strategy.VerbsList.Count - 1
        '            If (String.Compare(Me.Strategy.VerbsList(i), request.HttpMethod, StringComparison.OrdinalIgnoreCase) = 0) Then
        '                If (Not Me.Strategy.EmptyPathInfoOnly OrElse String.IsNullOrEmpty(request.PathInfo)) Then
        '                    If (Not Me.Strategy.EmptyQueryStringOnly OrElse (request.QueryString.Count > 0)) Then
        '                        If (Me.Strategy.VaryByList.Count = 0) Then
        '                            toReturn = Me
        '                        Else
        '                            Dim key As String = Me.Strategy.CalculateVaryByKey(request)
        '                            If (Not Me.VaryByTrackers.TryGetValue(key, toReturn) AndAlso (Me.VaryByTrackers.Count < OutputCacheManager.VaryByLimit)) Then
        '                                SyncLock Me.VaryByTrackers
        '                                    If (Not Me.VaryByTrackers.TryGetValue(key, toReturn) AndAlso (Me.VaryByTrackers.Count < OutputCacheManager.VaryByLimit)) Then
        '                                        toReturn = New FileOutputTracker(Me, key)
        '                                        Me.VaryByTrackers.Add(key, toReturn)
        '                                    End If
        '                                End SyncLock
        '                            End If
        '                        End If
        '                    End If
        '                End If
        '                Exit For
        '            End If
        '        Next i
        '    End If
        '    Return toReturn
        'End Function





        Friend Shared Sub ScavangeFilesOnAppDomainStartup(ByVal location As String)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf FileOutputTracker.ScavangeFiles))
        End Sub



#End Region

#Region "Overrides"

        Protected Overrides Function LoadCaptureFilter(ByVal response As System.Web.HttpResponse) As CaptureStream
            Dim captureFilename As String = String.Format("{0}_{1:x8}{2}", Me._FileOutputSettings.GetPrefix(Me.VaryById), Guid.NewGuid.ToString.GetHashCode, ".temp")
            Return New FileCaptureFilter(response.Filter, captureFilename)
        End Function

#End Region

#Region "Private methods"



        'Private Shared Function CalculateVaryByKey(ByVal varyBy As String(), ByVal request As HttpRequest) As String
        '    If ((varyBy Is Nothing) OrElse (varyBy.Length = 0)) Then
        '        Return String.Empty
        '    End If
        '    If ((varyBy.Length = 1) AndAlso (varyBy(0) = "*")) Then
        '        Return request.QueryString.ToString
        '    End If
        '    Dim builder As New StringBuilder
        '    Dim i As Integer
        '    For i = 0 To varyBy.Length - 1
        '        builder.Append(request.QueryString.Item(varyBy(i)))
        '        builder.Append("-"c)
        '    Next i
        '    Return HttpUtility.UrlEncodeUnicode(builder.ToString)
        'End Function



        Protected Overrides Sub FinishCapture(ByVal response As System.Web.HttpResponse)
            Dim captureFilename As String = DirectCast(Me.CapturingFilter, FileCaptureFilter).CaptureFilename
            Me._FileOutputSettings.InfoFileName = captureFilename.Replace(".temp", ".info")
            Me._FileOutputSettings.DataFileName = captureFilename.Replace(".temp", ".data")
            Dim utcNow As DateTime = DateTime.UtcNow

            Me.NextResponseValidationTime = (utcNow + OutputCacheManager.FileValidationDelay)
            Dim cachedOutput As New CachedOutput
            cachedOutput.ResponseHash = OutputTrackerBase.CalculateHandlerHash(Me.Path)
            cachedOutput.ResponseExpiry = (utcNow + Me.Strategy.Duration)
            Dim contents As String = String.Format("<{0} path=""{1}"" vary=""{2}"" timestamp=""{3}"" expiry=""{4}"" hash=""{5}"" />", New Object() {"diskOutputCacheItem", Me.Path, Me.VaryById, utcNow.ToBinary, cachedOutput.ResponseExpiry.ToBinary, cachedOutput.ResponseHash})
            File.WriteAllText(Me._FileOutputSettings.InfoFileName, contents, Encoding.UTF8)
            File.Move(captureFilename, Me._FileOutputSettings.DataFileName)
            If Me._FileOutputSettings.ServeFromMemory Then
                cachedOutput.ResponseBytes = File.ReadAllBytes(Me._FileOutputSettings.DataFileName)
            Else
                cachedOutput.ResponseBytes = New Byte(0 - 1) {}
            End If
            Me.CachedOutput = cachedOutput
        End Sub

        Private Shared Function TryReadInfoFile(ByVal infoFilename As String, <Out()> ByRef path As String, <Out()> ByRef vary As String, <Out()> ByRef timestamp As DateTime, <Out()> ByRef expiry As DateTime, <Out()> ByRef hash As Long) As Boolean
            path = String.Empty
            vary = String.Empty
            timestamp = DateTime.MinValue
            expiry = DateTime.MinValue
            hash = 0
            Dim document As New XmlDocument
            document.Load(infoFilename)
            Dim node As XmlNode = Nothing
            Dim node2 As XmlNode = document.FirstChild
            Do While (Not node2 Is Nothing)
                If (node2.NodeType = XmlNodeType.Element) Then
                    node = node2
                    Exit Do
                End If
                node2 = node2.NextSibling
            Loop
            If ((Not node Is Nothing) AndAlso (node.Name = "diskOutputCacheItem")) Then
                path = node.Attributes.ItemOf("path").Value
                vary = node.Attributes.ItemOf("vary").Value
                timestamp = DateTime.FromBinary(Long.Parse(node.Attributes.ItemOf("timestamp").Value))
                expiry = DateTime.FromBinary(Long.Parse(node.Attributes.ItemOf("expiry").Value))
                hash = Long.Parse(node.Attributes.ItemOf("hash").Value)
                Return True
            End If
            Return False
        End Function


        Private Shared Sub DeleteFiles(ByVal infoFilename As String, ByVal dataFilename As String)
            Try
                File.Delete(infoFilename)
            Catch
            End Try
            OutputCacheManager.ScheduleFileDeletion(dataFilename)
        End Sub


        Protected Overrides Sub InvalidateCachedResponse()
            MyBase.InvalidateCachedResponse()
            FileOutputTracker.DeleteFiles(Me._FileOutputSettings.InfoFileName, Me._FileOutputSettings.DataFileName)
        End Sub

        Protected Overrides Sub TryLoadCachedResponse()
            Dim fileNamePrefix As String = Me._FileOutputSettings.GetPrefix(Me.VaryById)
            Dim directoryName As String = System.IO.Path.GetDirectoryName(fileNamePrefix)
            Dim searchPattern As String = (System.IO.Path.GetFileName(fileNamePrefix) & "_????????.info")
            Dim strArray As String() = Directory.GetFiles(directoryName, searchPattern, SearchOption.TopDirectoryOnly)
            Dim infoFileName As String
            For Each infoFileName In strArray
                Try
                    Dim strPath As String = Nothing
                    Dim vary As String = Nothing
                    Dim timeStamp As DateTime
                    Dim expiry As DateTime
                    Dim hash As Long
                    If ((Not FileOutputTracker.TryReadInfoFile(infoFileName, strPath, vary, timeStamp, expiry, hash) OrElse (String.Compare(strPath, Me.Path, StringComparison.OrdinalIgnoreCase) <> 0)) OrElse (String.Compare(vary, Me.VaryById, StringComparison.OrdinalIgnoreCase) <> 0)) Then
                        Continue For
                    End If
                    Dim path As String = infoFileName.Replace(".info", ".data")
                    If Not File.Exists(path) Then
                        Continue For
                    End If
                    Dim newExpiry As DateTime = (timeStamp + Me.Strategy.Duration)
                    If (newExpiry > expiry) Then
                        newExpiry = expiry
                    End If
                    Dim utcNow As DateTime = DateTime.UtcNow
                    If (expiry <= utcNow) Then
                        FileOutputTracker.DeleteFiles(infoFileName, path)
                        Continue For
                    End If
                    Me._FileOutputSettings.InfoFileName = infoFileName
                    Me._FileOutputSettings.DataFileName = path
                    Dim cachedOutput As New CachedOutput
                    cachedOutput.ResponseHash = hash
                    cachedOutput.ResponseExpiry = newExpiry
                    Me.NextResponseValidationTime = utcNow

                    If Me._FileOutputSettings.ServeFromMemory Then
                        cachedOutput.ResponseBytes = File.ReadAllBytes(path)
                    Else
                        cachedOutput.ResponseBytes = New Byte(0 - 1) {}
                    End If
                    Me.CachedOutput = cachedOutput
                    'Thread.VolatileWrite(Me._cachedResponseLoaded, 1)
                Catch ex As Exception
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                End Try
                Exit For
            Next
        End Sub

        Private Shared Sub ScavangeFiles(ByVal state As Object)
            Try
                Dim searchPattern As String = "*.data"
                Dim strArray As String() = Directory.GetFiles(OutputCacheManager.Location, searchPattern, SearchOption.TopDirectoryOnly)
                Dim str2 As String
                For Each str2 In strArray
                    Try
                        If Not File.Exists(str2.Replace(".data", ".info")) Then
                            OutputCacheManager.ScheduleFileDeletion(str2)
                        End If
                    Catch
                    End Try
                    If OutputCacheManager.ShuttingDown Then
                        Return
                    End If
                Next
                searchPattern = "*.info"
                strArray = Directory.GetFiles(OutputCacheManager.Location, searchPattern, SearchOption.TopDirectoryOnly)
                Dim str3 As String
                For Each str3 In strArray
                    Try
                        Dim str4 As String = ""
                        Dim str5 As String = ""
                        Dim time As DateTime
                        Dim time2 As DateTime
                        Dim num As Long
                        If (FileOutputTracker.TryReadInfoFile(str3, str4, str5, time, time2, num) AndAlso (time2 < DateTime.UtcNow)) Then
                            str2 = str3.Replace(".info", ".data")
                            FileOutputTracker.DeleteFiles(str3, str2)
                        End If
                    Catch
                    End Try
                    If OutputCacheManager.ShuttingDown Then
                        Return
                    End If
                Next
            Finally
                Thread.VolatileWrite(FileOutputTracker._scavangingCompleted, 1)
            End Try
        End Sub




#End Region

    End Class
End Namespace