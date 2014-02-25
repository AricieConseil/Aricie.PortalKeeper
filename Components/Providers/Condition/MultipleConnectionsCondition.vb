Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper



    <ActionButton(IconName.Random, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Multiple Connections Condition")> _
    <Description("Matches if the same DNN user connects from different locations")> _
    Public Class MultipleConnectionsCondition
        Inherits ConditionProvider(Of RequestEvent)


        Private _DiscriminationSource As New RequestSource(RequestSourceType.Session)


        Private objLocksLock As New Object
        Private objBackTrackLock As New Object


        <ExtendedCategory("Specifics")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
           <LabelMode(LabelMode.Top)> _
        Public Property DiscriminationSource() As RequestSource
            Get
                Return _DiscriminationSource
            End Get
            Set(ByVal value As RequestSource)
                _DiscriminationSource = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property LockDuration() As STimeSpan = New STimeSpan(TimeSpan.FromMinutes(60))


        <ExtendedCategory("Specifics")> _
        Public Property NewConnectionsDontMatch() As Boolean = False

        Private Shared Property UserLocks() As SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime))
            Get
                Dim toReturn As SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime)) = _
                    DirectCast(GetCache("PKP_MCC_UserLocks"), SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime)))
                If toReturn Is Nothing Then
                    toReturn = New SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime))
                    'Dim onlineUsers As ArrayList = DotNetNuke.Entities.Users.UserController.GetOnlineUsers(NukeHelper.PortalId)
                    'For Each objUser As OnlineUserInfo In onlineUsers
                    '    objUser.Profil()
                    'Next()

                    SetCacheDependant("PKP_MCC_UserLocks", toReturn, Constants.Cache.NoExpiration, PortalKeeperConfig.GetFilePath(True))
                End If
                Return toReturn
            End Get
            Set(ByVal value As SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime)))
                SetCacheDependant("PKP_MCC_UserLocks", value, Constants.Cache.NoExpiration, PortalKeeperConfig.GetFilePath(True))
            End Set
        End Property

        Private Shared Property BacktrackLocks() As SerializableDictionary(Of Integer, List(Of String))
            Get
                Dim toReturn As SerializableDictionary(Of Integer, List(Of String)) = _
                    DirectCast(GetCache("PKP_MCC_BacktrackLocks"), SerializableDictionary(Of Integer, List(Of String)))
                If toReturn Is Nothing Then
                    toReturn = New SerializableDictionary(Of Integer, List(Of String))
                    SetCacheDependant("PKP_MCC_BacktrackLocks", toReturn, Constants.Cache.NoExpiration, PortalKeeperConfig.GetFilePath(True))
                End If
                Return toReturn
            End Get
            Set(ByVal value As SerializableDictionary(Of Integer, List(Of String)))
                SetCacheDependant("PKP_MCC_BacktrackLocks", value, Constants.Cache.NoExpiration, PortalKeeperConfig.GetFilePath(True))
            End Set
        End Property

        Public Overrides Function Match(ByVal context As PortalKeeperContext(Of RequestEvent)) As Boolean

            Dim objUser As UserInfo = context.DnnContext.User
            If objUser IsNot Nothing AndAlso objUser.UserID <> -1 Then
                Dim strCurrentKey As String = Me._DiscriminationSource.GenerateKey(context)
                Dim objUserLock As KeyValuePair(Of String, DateTime) = Nothing
                If UserLocks.TryGetValue(objUser.UserID, objUserLock) Then
                    If objUserLock.Value > Now Then
                        If strCurrentKey <> objUserLock.Key Then
                            context.SetVar("ClientUser", objUser)
                            context.SetVar("LastClientSource", strCurrentKey)
                            context.SetVar("FirstClientSource", objUserLock.Key)
                            ' we are on a different source
                            If Not Me._NewConnectionsDontMatch Then
                                Return True
                            Else
                                Dim histList As List(Of String) = Nothing
                                If BacktrackLocks.TryGetValue(objUser.UserID, histList) AndAlso histList.Contains(strCurrentKey) Then
                                    ' the current source is not new
                                    RemoveBackTracking(objUser.UserID, strCurrentKey)
                                    SetBackTracking(objUser.UserID, objUserLock.Key)
                                    Return CleanLockAndMatch(objUser.UserID)
                                Else
                                    ' the current source is new
                                    SetBackTracking(objUser.UserID, objUserLock.Key)
                                    SetUserLock(objUser.UserID, strCurrentKey)
                                End If
                            End If
                        End If
                    Else
                        SetUserLock(objUser.UserID, strCurrentKey)
                    End If
                Else
                    SetUserLock(objUser.UserID, strCurrentKey)
                End If
            End If
            Return False
        End Function

        ''' <summary>
        ''' Used for maintaining a list of request sources for each users.
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Private Sub SetBackTracking(ByVal userId As Integer, ByVal key As String)
            Dim histList As List(Of String) = Nothing
            SyncLock objBackTrackLock
                Dim backtracks As SerializableDictionary(Of Integer, List(Of String)) = BacktrackLocks

                If Not backtracks.TryGetValue(userId, histList) Then
                    histList = New List(Of String)
                End If
                If Not histList.Contains(key) Then
                    histList.Add(key)
                End If
                backtracks(userId) = histList
                BacktrackLocks = backtracks
            End SyncLock
        End Sub

        ''' <summary>
        ''' Removes the specified request source for the specified user.
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Private Sub RemoveBackTracking(ByVal userId As Integer, ByVal key As String)
            Dim histList As List(Of String) = Nothing
            SyncLock objBackTrackLock
                Dim backtracks As SerializableDictionary(Of Integer, List(Of String)) = BacktrackLocks
                If backtracks.TryGetValue(userId, histList) Then
                    histList.Remove(key)
                    If histList.Count = 0 Then
                        backtracks.Remove(userId)
                    Else
                        backtracks(userId) = histList
                    End If
                End If
                BacktrackLocks = backtracks
            End SyncLock
        End Sub

        ''' <summary>
        ''' Add a lock with the specified source for the specified user.
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Private Sub SetUserLock(ByVal userId As Integer, ByVal key As String)
            SyncLock objLocksLock
                Dim tempUserLocks As SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime)) = UserLocks
                tempUserLocks(userId) = New KeyValuePair(Of String, DateTime)(key, Now.Add(Me._LockDuration.Value))
                UserLocks = tempUserLocks
            End SyncLock
        End Sub

        ''' <summary>
        ''' Remove all locks for the specified user.
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CleanLockAndMatch(ByVal userId As Integer) As Boolean
            SyncLock objLocksLock
                Dim tempUserLocks As SerializableDictionary(Of Integer, KeyValuePair(Of String, DateTime)) = UserLocks
                tempUserLocks.Remove(userId)
                UserLocks = tempUserLocks
            End SyncLock
            Return True
        End Function


    End Class
End Namespace
