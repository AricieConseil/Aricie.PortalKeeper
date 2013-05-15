Imports System.ComponentModel
Imports Aricie.Collections
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Threading
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class DosEnabledConditionProvider(Of TEngineEvents As IConvertible)
        Inherits ConditionProvider(Of TEngineEvents)
        Implements IDoSEnabledConditionProvider(Of TEngineEvents)

        <NonSerialized()> _
        Private Shared _LockProviders As New ReaderWriterLock()
        Friend Shared DosProviders As New List(Of DosEnabledConditionProvider(Of TEngineEvents))
        Private _BroadcastTaskQueueInfo As New TaskQueueInfo(1, True, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(100))

        <NonSerialized()> _
        Private WithEvents _BroadcastTaskQueue As WaitQueue

        <NonSerialized()> _
        Private _LockDico As New ReaderWriterLock()

        Public Shared Sub BeginReadProviders()
            _LockProviders.AcquireReaderLock(Aricie.Constants.LockTimeOut)
        End Sub

        Public Shared Sub EndReadProviders()
            _LockProviders.ReleaseReaderLock()
        End Sub

        Private Function BeginReadInternal() As Boolean
            Try
                _LockDico.AcquireReaderLock(Aricie.Constants.LockTimeOut)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        Private Sub EndReadInternal()
            _LockDico.ReleaseReaderLock()
        End Sub

        Private Function BeginWriteInternal() As Boolean
            Try
                _LockDico.AcquireWriterLock(Aricie.Constants.LockTimeOut)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        Private Sub EndWriteInternal()
            _LockDico.ReleaseWriterLock()
        End Sub


        Friend ReadOnly Property DosDico() As SerializableDictionary(Of Object, SerializableDictionary(Of String, DateTime))
            Get
                Dim toReturn As SerializableDictionary(Of Object, SerializableDictionary(Of String, DateTime)) = _
                    Aricie.Services.CacheHelper.GetGlobal(Of SerializableDictionary(Of Object, SerializableDictionary(Of String, DateTime)))("DosDico", Me.Name)
                If toReturn Is Nothing Then
                    _LockProviders.AcquireReaderLock(Constants.LockTimeOut)
                    Dim contains As Boolean = DosProviders.Contains(Me)
                    _LockProviders.ReleaseReaderLock()
                    If Not contains Then
                        _LockProviders.AcquireWriterLock(Constants.LockTimeOut)
                        If Not DosProviders.Contains(Me) Then
                            DosProviders.Add(Me)
                        End If
                        _LockProviders.ReleaseWriterLock()
                    End If
                    toReturn = New SerializableDictionary(Of Object, SerializableDictionary(Of String, DateTime))
                    Aricie.Services.CacheHelper.SetCacheDependant(Of SerializableDictionary(Of Object, SerializableDictionary(Of String, DateTime)))(toReturn, _
                                                                        PortalKeeperConfig.GetFilePath(True), Constants.Cache.GlobalExpiration, "DosDico", Me.Name)
                End If
                Return toReturn
            End Get
        End Property

        Friend Property DosEntries(ByVal objClue As Object) As SerializableDictionary(Of String, DateTime)
            Get
                Dim toReturn As SerializableDictionary(Of String, DateTime) = Nothing
                If BeginReadInternal() Then
                    Try
                        toReturn = DosDico.Item(objClue)
                    Catch ex As Exception
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                    Finally
                        EndReadInternal()
                    End Try
                End If
                Return toReturn
            End Get
            Set(ByVal value As SerializableDictionary(Of String, DateTime))
                'SyncLock lockDoS
                If BeginWriteInternal() Then
                    Try
                        If value Is Nothing Then
                            DosDico.Remove(objClue)
                        Else
                            DosDico.Item(objClue) = value
                        End If
                    Catch ex As Exception
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                    Finally
                        EndWriteInternal()
                    End Try
                    BroadcastTaskQueue.WaitOne()
                End If
            End Set
        End Property





        Public MustOverride Function FastGetKey(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal clue As Object) As String Implements IDoSEnabledConditionProvider(Of TEngineEvents).FastGetKey
        Public MustOverride Overloads Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents), ByRef clue As Object, ByRef key As String) As Boolean Implements IDoSEnabledConditionProvider(Of TEngineEvents).Match


        Public NotOverridable Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return MatchDoS(context, Me)
        End Function

        <ExtendedCategory("DenialOfService")> _
        <SortOrder(1000)> _
        Public Property EnableDoSProtection() As Boolean Implements IDoSEnabledConditionProvider(Of TEngineEvents).EnableDoSProtection

        <ExtendedCategory("DenialOfService")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ConditionalVisible("EnableDoSProtection", False, True)> _
            <SortOrder(1000)> _
        Public Property DoSProtectionDuration() As STimeSpan = New STimeSpan(TimeSpan.FromMinutes(5)) _
                                                               Implements IDoSEnabledConditionProvider(Of TEngineEvents).DoSProtectionDuration


        <ExtendedCategory("DenialOfService")> _
           <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
           <LabelMode(LabelMode.Top)> _
           <ConditionalVisible("EnableDoSProtection", False, True)> _
           <SortOrder(1000)> _
        Public Property BroadcastTaskQueueInfo() As TaskQueueInfo
            Get
                Return _BroadcastTaskQueueInfo
            End Get
            Set(ByVal value As TaskQueueInfo)
                _BroadcastTaskQueueInfo = value
            End Set
        End Property

        Private ReadOnly Property BroadcastTaskQueue() As WaitQueue
            Get
                If _BroadcastTaskQueue Is Nothing Then
                    _BroadcastTaskQueue = New WaitQueue(Me._BroadcastTaskQueueInfo)
                    AddHandler _BroadcastTaskQueue.ActionPerformed, AddressOf Me.BroadcastTaskQueue_ActionPerformed
                End If
                Return _BroadcastTaskQueue
            End Get
        End Property

        Private Sub BroadcastTaskQueue_ActionPerformed(ByVal sender As Object, ByVal e As GenericEventArgs(Of Integer))
            If Me.BeginReadInternal() Then
                Try
                    Aricie.Services.CacheHelper.SetCacheDependant(Of SerializableDictionary(Of Object, SerializableDictionary(Of String, DateTime))) _
                    (DosDico, PortalKeeperConfig.GetFilePath(True), Constants.Cache.GlobalExpiration)
                Catch ex As Exception
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                Finally
                    Me.EndReadInternal()
                End Try
            End If
        End Sub


        Friend Function MatchDoS(ByVal keeperContext As PortalKeeperContext(Of TEngineEvents), ByVal dosCondition As IDoSEnabledConditionProvider(Of TEngineEvents)) As Boolean
            Dim key As String = Nothing
            Dim clue As Object = Nothing
            If (Not dosCondition.EnableDoSProtection) OrElse (Not keeperContext.CurrentFirewallConfig.DosSettings.Enabled) Then
                Return dosCondition.Match(keeperContext, clue, key)
            Else

                If dosCondition.Match(keeperContext, clue, key) Then
                    Dim currentPerClue As SerializableDictionary(Of String, DateTime) = Nothing
                    Dim found As Boolean
                    If Me.BeginReadInternal() Then
                        Try
                            found = DosDico.TryGetValue(dosCondition, currentPerClue)
                        Catch ex As Exception
                            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                        Finally
                            Me.EndReadInternal()
                        End Try
                        If Not found Then
                            currentPerClue = New SerializableDictionary(Of String, DateTime)
                        End If
                        currentPerClue(key) = Now.Add(dosCondition.DoSProtectionDuration.Value)
                        DosEntries(clue) = currentPerClue
                        Return True
                    End If
                End If
                Return False
            End If
        End Function

        Friend Sub ReleaseDoS(ByVal clue As Object, ByVal key As String)
            Dim currentPerClue As SerializableDictionary(Of String, DateTime) = Nothing
            If DosDico.TryGetValue(clue, currentPerClue) Then
                currentPerClue.Remove(key)
                If currentPerClue.Count = 0 Then
                    DosEntries(clue) = Nothing
                Else
                    DosEntries(clue) = currentPerClue
                End If
            End If
        End Sub

        Public Function GetCurrentBans(ByVal maxLength As Integer) As String
            Dim toReturn As New StringBuilder
            toReturn.Append(" ## ")
            toReturn.Append(Me.Name)
            toReturn.Append(" { ")
            For Each objClueDico As KeyValuePair(Of Object, SerializableDictionary(Of String, DateTime)) In Me.DosDico
                toReturn.Append(objClueDico.Key.ToString)
                toReturn.Append(" [ ")
                For Each objKeys As KeyValuePair(Of String, DateTime) In objClueDico.Value
                    toReturn.Append("("c)
                    toReturn.Append(objKeys.Key.ToString)
                    toReturn.Append(" - ")
                    toReturn.Append(objKeys.Value.ToShortTimeString)
                    toReturn.Append(")"c)
                    If toReturn.Length > maxLength Then
                        toReturn.Append(" (...) ")
                        Exit For
                    End If
                Next
                toReturn.Append(" ] ")
                If toReturn.Length > maxLength Then
                    toReturn.Append(" [...] ")
                    Exit For
                End If
            Next
            toReturn.Append(" } ")
            Return toReturn.ToString
        End Function

    End Class
End Namespace