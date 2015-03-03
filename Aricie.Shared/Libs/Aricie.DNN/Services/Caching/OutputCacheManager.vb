Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports System.Web
Imports System.Web.Configuration
Imports System.Web.Hosting

Namespace Services.Caching

    Public Class OutputCacheManager
        Implements IRegisteredObject
        ' Methods



        ' Properties
        Friend Shared ReadOnly Property FileRemovalDelay() As TimeSpan
            Get
                Return OutputCacheManager._theCache._fileRemovalDelay
            End Get
        End Property

        Friend Shared ReadOnly Property FileValidationDelay() As TimeSpan
            Get
                Return OutputCacheManager._theCache._fileValidationDelay
            End Get
        End Property

        Friend Shared ReadOnly Property Location() As String
            Get
                Return OutputCacheManager._theCache._location
            End Get
        End Property

        Friend Shared ReadOnly Property ShuttingDown() As Boolean
            Get
                Return CType(Thread.VolatileRead(OutputCacheManager._theCache._shuttingDown), Boolean)
            End Get
        End Property

        Friend Shared ReadOnly Property VaryByLimit() As Integer
            Get
                Return OutputCacheManager._theCache._varyByLimit
            End Get
        End Property


        ' Fields
        Private Shared _initError As Exception
        Private Shared _initLock As Object = New Object
        Private Shared _theCache As OutputCacheManager

        Private _fileRemovalDelay As TimeSpan
        Private _fileRemovalList As LinkedList(Of ScavangerEntry) = New LinkedList(Of ScavangerEntry)
        Private _fileScavangingDelay As TimeSpan
        Private _fileValidationDelay As TimeSpan

        Private _location As String
        Private _scavangingTimer As Timer
        Private _shuttingDown As Integer

        Private _trackers As Dictionary(Of String, FileOutputTracker) = New Dictionary(Of String, FileOutputTracker)(StringComparer.OrdinalIgnoreCase)
        Private _varyByLimit As Integer

#Region "Public methods"

        Public Shared Sub EnsureInitialized()
            If (OutputCacheManager._theCache Is Nothing) Then
                SyncLock OutputCacheManager._initLock
                    If (OutputCacheManager._theCache Is Nothing) Then
                        Dim tempOutputManager As New OutputCacheManager
                        Try
                            tempOutputManager.Startup()
                        Catch exception As Exception
                            OutputCacheManager._initError = exception
                            Throw
                        End Try
                        HostingEnvironment.RegisterObject(tempOutputManager)
                        OutputCacheManager._theCache = tempOutputManager
                    End If
                End SyncLock
            End If
            OutputCacheManager.CheckInitialized()
        End Sub

        Public Shared Function Lookup(ByVal context As HttpContext) As OutputTrackerBase
            OutputCacheManager.CheckInitialized()
            Return OutputCacheManager._theCache.LookupTracker(context)
        End Function

        Public Shared Sub ScheduleFileDeletion(ByVal filename As String)
            OutputCacheManager._theCache.AddToRemovalList(filename)
            OutputCacheManager._theCache.ScheduleScavanger()
        End Sub


#End Region

#Region "Private methods"

        Private Shared Sub CheckInitialized()
            If (OutputCacheManager._theCache Is Nothing) Then
                Throw New InvalidOperationException("Cache is not initialized")
            End If
            If (Not OutputCacheManager._initError Is Nothing) Then
                Throw New InvalidOperationException("Cache failed to initialize", OutputCacheManager._initError)
            End If
        End Sub

        Private Sub Startup()
            Dim webApplicationSection As DiskOutputCacheSettingsSection = DirectCast(WebConfigurationManager.GetWebApplicationSection("diskOutputCacheSettings"), DiskOutputCacheSettingsSection)
            If (Not webApplicationSection Is Nothing) Then
                Me._fileRemovalDelay = webApplicationSection.FileRemovalDelay
                Me._fileValidationDelay = webApplicationSection.FileValidationDelay
                Me._fileScavangingDelay = webApplicationSection.FileScavangingDelay
                Me._varyByLimit = webApplicationSection.VaryByLimitPerUrl
                If String.IsNullOrEmpty(webApplicationSection.Location) Then
                    Me._location = Path.Combine(HttpRuntime.CodegenDir, "DiskOutputCache")
                    If Not Directory.Exists(Me._location) Then
                        Directory.CreateDirectory(Me._location)
                    End If
                Else
                    Me._location = webApplicationSection.Location
                    If Not Directory.Exists(Me._location) Then
                        Throw New InvalidDataException(String.Format("Invalid location '{0}'", Me._location))
                    End If
                End If
                Try
                    Dim path As String = System.IO.Path.Combine(Me._location, ("test" & DateTime.UtcNow.ToFileTime & ".txt"))
                    File.WriteAllText(path, "test")
                    File.Delete(path)
                Catch
                    Throw New InvalidDataException(String.Format("Invalid location '{0}' -- failed to write", Me._location))
                End Try
                Dim element As CachedUrlsElement
                For Each element In webApplicationSection.CachedUrls
                    Dim virtualPath As String = element.Path
                    If Not (VirtualPathUtility.IsAppRelative(virtualPath) OrElse VirtualPathUtility.IsAbsolute(virtualPath)) Then
                        Throw New InvalidDataException(String.Format("Invalid path '{0}', absolute or app-relative path expected", virtualPath))
                    End If
                    virtualPath = VirtualPathUtility.ToAbsolute(virtualPath)
                    Dim filePrefix As String = VirtualPathUtility.ToAppRelative(virtualPath)
                    If filePrefix.StartsWith("~/") Then
                        filePrefix = filePrefix.Substring(2)
                    End If
                    filePrefix = filePrefix.Replace("."c, "_"c).Replace("/"c, "_"c)
                    Dim objStrategy As New OutputCachingStrategy()
                    objStrategy.Enabled = True
                    objStrategy.Duration = element.Duration
                    objStrategy.Verbs = element.Verbs
                    objStrategy.VaryBy = element.VaryBy
                    objStrategy.EmptyPathInfoOnly = element.EmptyPathInfoOnly
                    objStrategy.EmptyQueryStringOnly = element.EmptyQueryStringOnly
                    Dim objFileSettings As New FileOutputSettings
                    objFileSettings.ServeFromMemory = element.ServeFromMemory
                    objFileSettings.FileNamePrefix = Path.Combine(Me._location, filePrefix)
                    Me._trackers.Item(virtualPath) = New FileOutputTracker(virtualPath, objStrategy, objFileSettings)
                    'Me._trackers.Item(virtualPath) = New Tracker(virtualPath, Path.Combine(Me._location, str3), element.Duration, verbs, varyBy, element.EmptyQueryStringOnly, element.EmptyPathInfoOnly, element.ServeFromMemory)
                Next
                If Me.CheckIfNeedToScavangeFilesThisTimeOnAppDomainStarted Then
                    FileOutputTracker.ScavangeFilesOnAppDomainStartup(Me._location)
                End If
            End If
        End Sub

        Private Sub [Stop](ByVal immediate As Boolean) Implements IRegisteredObject.Stop
            If immediate Then
                Do While Not FileOutputTracker.ScavangingCompleted
                    Thread.Sleep(50)
                Loop
                HostingEnvironment.UnregisterObject(Me)
            Else
                Thread.VolatileWrite(Me._shuttingDown, 1)
                Me.Cleanup()
                If FileOutputTracker.ScavangingCompleted Then
                    HostingEnvironment.UnregisterObject(Me)
                End If
            End If
        End Sub

        Private Function LookupTracker(ByVal context As HttpContext) As OutputTrackerBase
            Dim tracker As FileOutputTracker = Nothing
            Dim request As HttpRequest = context.Request
            If Me._trackers.TryGetValue(request.FilePath, tracker) Then
                Return tracker.FindTrackerForRequest(request)
            End If
            Return Nothing
        End Function



        Private Sub AddToRemovalList(ByVal filename As String)
            Dim entry As New ScavangerEntry(filename, DateTime.UtcNow.Add(Me._fileRemovalDelay))
            SyncLock Me._fileRemovalList
                Me._fileRemovalList.AddLast(entry)
            End SyncLock
        End Sub

        Private Function CheckIfNeedToScavangeFilesThisTimeOnAppDomainStarted() As Boolean
            Dim path As String = System.IO.Path.Combine(Me._location, "scavanger.timestamp")
            Try
                If File.Exists(path) Then
                    Dim time As DateTime = DateTime.FromBinary(Long.Parse(File.ReadAllText(path)))
                    If (DateTime.UtcNow < (time + Me._fileScavangingDelay)) Then
                        Return False
                    End If
                End If
            Catch
            End Try
            Try
                File.WriteAllText(path, DateTime.UtcNow.ToBinary.ToString)
            Catch
            End Try
            Return True
        End Function



        Private Sub Cleanup()
            Dim timer As Timer = Me._scavangingTimer
            If (Not timer Is Nothing) Then
                timer.Dispose()
            End If
        End Sub


        Private Shared Function ParseStringList(ByVal listAsString As String) As String()
            Dim strArray As String() = listAsString.Trim.Split(New Char() {","c})
            Dim list As New List(Of String)(strArray.Length)
            Dim str As String
            For Each str In strArray
                Dim item As String = str.Trim
                If (item.Length > 0) Then
                    list.Add(item)
                End If
            Next
            Return list.ToArray
        End Function

        Private Sub ScavangerCallback(ByVal state As Object)
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Try
                Dim utcNow As DateTime = DateTime.UtcNow
                Dim list As New List(Of String)
                SyncLock Me._fileRemovalList
                    Dim first As LinkedListNode(Of ScavangerEntry) = Me._fileRemovalList.First
                    Do While (Not first Is Nothing)
                        Dim node As LinkedListNode(Of ScavangerEntry) = first
                        first = node.Next
                        If (utcNow > node.Value.UtcDelete) Then
                            list.Add(node.Value.Filename)
                            Me._fileRemovalList.Remove(node)
                            num += 1
                        Else
                            num2 += 1
                        End If
                    Loop
                End SyncLock
                Dim str As String
                For Each str In list
                    Try
                        File.Delete(str)
                    Catch
                    End Try
                Next
            Finally
                Me._scavangingTimer = Nothing
            End Try
            If (num2 > 0) Then
                Me.ScheduleScavanger()
            End If
        End Sub


        Private Sub ScheduleScavanger()
            If (Me._scavangingTimer Is Nothing) Then
                Dim dueTime As Integer = CInt((Me._fileRemovalDelay.TotalMilliseconds * 1.1))
                SyncLock Me
                    If (Me._scavangingTimer Is Nothing) Then
                        Me._scavangingTimer = New Timer(New TimerCallback(AddressOf Me.ScavangerCallback), Nothing, dueTime, -1)
                    End If
                End SyncLock
            End If
        End Sub





#End Region


        ' Nested Types

    End Class
End Namespace

