Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Entities.Modules
Imports System.Globalization
Imports Aricie.DNN.Settings
Imports Aricie.DNN.Services
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization
Imports System.Web.UI
Imports DotNetNuke.UI.Skins



Namespace Security.Trial
    ''' <summary>
    ''' Trial status information structure
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure TrialStatusInfo

        Public Sub New(ByVal config As TrialConfigInfo, ByVal objStatus As TrialStatus, ByVal nbDaysLeft As Integer)
            ' config?
            Me.Status = objStatus
            Me.NbDaysLeft = nbDaysLeft
        End Sub

        Public Config As TrialConfigInfo
        Public Status As TrialStatus
        Public NbDaysLeft As Integer

        Private Shared _NoTrials As New Dictionary(Of String, TrialStatusInfo)

        ''' <summary>
        ''' Quick property access to a "No trial" information structure for each module
        ''' </summary>
        ''' <param name="config"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property NoTrial(ByVal config As TrialConfigInfo) As TrialStatusInfo
            Get
                Dim toReturn As TrialStatusInfo = Nothing
                If Not _NoTrials.TryGetValue(config.ModuleName, toReturn) Then
                    toReturn = New TrialStatusInfo(config, TrialStatus.Valid, Integer.MaxValue) '???
                    SyncLock _NoTrials
                        _NoTrials(config.ModuleName) = toReturn
                    End SyncLock
                End If
                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' Returns information about trial limitation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsLimited() As Boolean
            Return CInt(Me.Status) < 0
        End Function

    End Structure

    ''' <summary>
    ''' Trial controller
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TrialController


#Region "Private members"


        Private Shared _Controllers As New Dictionary(Of String, TrialController)

        Private _Config As TrialConfigInfo


        Private _PortalSettingsKey As String
        Private _Separator As String = "%"


#End Region

#Region "cTor"

        Private Sub New(ByVal objTrialConfig As TrialConfigInfo)
            Me._Config = objTrialConfig
        End Sub

        ''' <summary>
        ''' Returns the trial controller for the type of the Trial provider
        ''' </summary>
        ''' <param name="provider"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance(ByVal provider As ITrialProvider) As TrialController
            Get
                Dim toReturn As TrialController = Nothing
                Dim providerTypeName As String = provider.GetType.FullName
                If Not _Controllers.TryGetValue(providerTypeName, toReturn) Then
                    SyncLock _Controllers
                        If Not _Controllers.TryGetValue(providerTypeName, toReturn) Then
                            toReturn = New TrialController(provider.GetTrialConfigInfo())
                            _Controllers(providerTypeName) = toReturn
                        End If
                    End SyncLock
                End If
                Return toReturn
            End Get
        End Property

#End Region

#Region "Public methods"

        ''' <summary>
        ''' States if the dll is compiled trial mode
        ''' </summary>
        Public Function IsTrial() As Boolean
            Return Me._Config.IsTrial
        End Function

#Region "Obsoletes"

        <Obsolete("Use GetCurrentStatusInfo or LimitModule")> _
      Public Function CheckTrialPeriod() As Integer
            Return CInt(GetCurrentStatusInfo.Status)
        End Function

        <Obsolete("Use GetCurrentStatusInfo or LimitModule")> _
      Public Function CheckTrialStatus() As Integer
            Return CInt(GetCurrentStatusInfo.Status)
        End Function

        <Obsolete("Use GetCurrentStatusInfo or LimitModule")> _
       Public Function GetModuleLeftDays(Optional ByVal moduleID As Integer = -1) As Integer
            Return Me.GetCurrentStatusInfo(moduleID).NbDaysLeft
        End Function

        <Obsolete("Use GetCurrentStatusInfo or LimitModule")> _
        Public Function CheckTrialStatusEnum() As TrialStatus
            Return Me.GetCurrentStatusInfo.Status
        End Function

        <Obsolete("use override with 2 parameters")> _
        Public Function LimitModule(ByRef moduleControl As PortalModuleBase) As Boolean
            Return CInt(Me.GetCurrentStatusInfo(moduleControl.ModuleId).Status) < 0
        End Function

        <Obsolete("use override with 2 parameters")> _
        Public Function LimitModule(ByVal moduleId As Integer) As Boolean
            Return CInt(Me.GetCurrentStatusInfo(moduleId).Status) < 0
        End Function

        <Obsolete("Use LimitModule")> _
        Public Sub LimitView(ByRef moduleControl As PortalModuleBase)
            Me.LimitModule(moduleControl, True)
        End Sub

        <Obsolete("Use LimitModule")> _
        Public Sub LimitEdit(ByRef moduleControl As PortalModuleBase)
            Me.LimitModule(moduleControl, False)
        End Sub

        <Obsolete("Use LimitModule")> _
        Public Sub ShowLimitMessage(ByRef moduleControl As PortalModuleBase)
            LimitModule(moduleControl, False)
        End Sub

        <Obsolete("Use LimitModule")> _
       Public Function GetMessage(ByVal nbOfDaysLeft As Integer, ByVal messageType As TrialMessage, _
                                   ByVal moduleControl As PortalModuleBase) As String

            Me.LimitModule(moduleControl, False)

            Return ""
        End Function



#End Region

        ''' <summary>
        ''' Returns the current trial status of the module, together with the nb of days left
        ''' </summary>
        Public Function GetCurrentStatusInfo(Optional ByVal moduleID As Integer = -1) As TrialStatusInfo


            If Me._Config.IsTrial Then
                Dim toReturn As TrialStatusInfo = New TrialStatusInfo(Me._Config, TrialStatus.Valid, Integer.MaxValue)
                If ((Me._Config.Limitation And TrialLimitation.Expiration) = TrialLimitation.Expiration) Then
                    Dim currentStatus As TrialInfo = Me.GetCurrentStatus
                    If moduleID <> -1 AndAlso (Me._Config.Limitation And TrialLimitation.FlagModules) = TrialLimitation.FlagModules Then
                        Dim moduleEvidence As TrialInfo = Me.GetCurrentStatus(moduleID)
                        If moduleEvidence.ExpirationDate <> Date.MinValue Then
                            If currentStatus.ExpirationDate > moduleEvidence.ExpirationDate Then
                                'portal setting was deleted after the module was instanciated 
                                toReturn.Status = TrialStatus.InvalidModule
                                toReturn.NbDaysLeft = Integer.MinValue
                            End If
                        Else
                            Me.SetModuleTimeStamp(moduleID)
                        End If
                    End If
                    If currentStatus.ExpirationDate = DateTime.MinValue Then
                        toReturn.Status = TrialStatus.Install
                        toReturn.NbDaysLeft = Me._Config.Duration
                        Me.SetStatus()
                    Else
                        toReturn.NbDaysLeft = (currentStatus.ExpirationDate - Now).Days
                        If toReturn.NbDaysLeft >= 0 Then
                            If toReturn.NbDaysLeft > Me._Config.Duration Then
                                'string was modified, it's not a valid date anymore 
                                toReturn.Status = TrialStatus.Invalid
                                toReturn.NbDaysLeft = Me._Config.Duration
                                Me.SetStatus()
                            Else
                                toReturn.Status = TrialStatus.Valid
                            End If
                        Else
                            toReturn.Status = TrialStatus.Expired
                        End If
                    End If
                ElseIf ((Me._Config.Limitation And TrialLimitation.Limitation) = TrialLimitation.Limitation) Then
                    toReturn.Status = TrialStatus.Expired
                End If
                Return toReturn
            End If
            Return TrialStatusInfo.NoTrial(Me._Config)

        End Function


        ''' <summary>
        ''' Returns the current trial status of the module, together with the nb of days left, and deals with displaing corresponding messages
        ''' </summary>
        Public Function LimitModule(ByRef moduleControl As PortalModuleBase, ByVal IsViewControl As Boolean) As TrialStatusInfo

            Dim status As TrialStatusInfo = Me.GetCurrentStatusInfo(moduleControl.ModuleId)
            If moduleControl IsNot Nothing AndAlso Not moduleControl.IsPostBack AndAlso Me._Config.IsTrial Then
                Me.DisplayMessage(status, moduleControl, IsViewControl)
            End If
            Return status

        End Function


#End Region

#Region "Private methods"

        ''' <summary>
        ''' Starts the trial period and stores the corresponding encrypted token in the portal settings
        ''' </summary>
        Private Sub SetStatus()
            Dim newStatus As TrialInfo = Me.GetCurrentStatus
            newStatus.ExpirationDate = Now + TimeSpan.FromDays(Me._Config.Duration)
            Me.SetStatus(newStatus)
        End Sub

        ''' <summary>
        ''' Stores the current trial status in the portal settings
        ''' </summary>
        Private Sub SetStatus(ByVal objTrialInfo As TrialInfo)
            objTrialInfo.Encrypt(Me._Config.EncryptionKey)
            SetModuleSettings(Of TrialInfo)(SettingsScope.PortalSettings, NukeHelper.PortalId, objTrialInfo, True, Me.GetSettingsKey)
        End Sub

        ''' <summary>
        ''' Stores the current trial status in a module's settings
        ''' Prevents pondering with removing the global portal status
        ''' </summary>
        Private Sub SetModuleTimeStamp(ByVal moduleID As Integer)
            Me.GetCurrentStatus.Encrypt(Me.GetSettingsKey)
            SetModuleSettings(Of TrialInfo)(SettingsScope.ModuleSettings, moduleID, Me.GetCurrentStatus)
        End Sub


        ''' <summary>
        ''' Gets the specific key used to store for portal token trial setting (to be combined with other parts in the setmodulesettings method)
        ''' </summary>
        Private Function GetSettingsKey() As String
            If String.IsNullOrEmpty(_PortalSettingsKey) Then
                _PortalSettingsKey = Constants.Business.ConstLimitingKey & _
                                     Me._Config.GetHashCode().ToString(CultureInfo.InvariantCulture)
            End If
            Return _PortalSettingsKey
        End Function

        ''' <summary>
        ''' Gets the current global (portal wide) trial status as stored in the portal settings 
        ''' </summary>
        Private Function GetCurrentStatus() As TrialInfo

            Return GetCurrentStatus(-1)

        End Function

        ''' <summary>
        ''' Gets the current module specific trial status as stored in the module settings 
        ''' </summary>
        Private Function GetCurrentStatus(ByVal moduleId As Integer) As TrialInfo
            Dim objCurrentStatus As TrialInfo = Nothing
            If moduleId <> -1 AndAlso (Me._Config.Limitation And TrialLimitation.FlagModules) = TrialLimitation.FlagModules Then
                objCurrentStatus = GetModuleSettings(Of TrialInfo)(SettingsScope.ModuleSettings, moduleId)
            Else
                objCurrentStatus = GetModuleSettings(Of TrialInfo)(SettingsScope.PortalSettings, PortalId, Nothing, True, Me.GetSettingsKey)
            End If
            If Not objCurrentStatus.IsDecrypted Then
                objCurrentStatus.Decrypt(Me._Config.EncryptionKey)
            End If
            Return objCurrentStatus
        End Function


        ''' <summary>
        ''' Displays a module message according to the trial status and configuration
        ''' </summary>
        Private Sub DisplayMessage(ByVal objTrialStatusInfo As TrialStatusInfo, ByVal moduleControl As PortalModuleBase, ByVal isView As Boolean)

            Dim color As ModuleMessage.ModuleMessageType
            Select Case objTrialStatusInfo.Status
                Case TrialStatus.Valid
                    If isView AndAlso (Me._Config.Limitation And TrialLimitation.ExplainView) <> TrialLimitation.ExplainView Then
                        Exit Sub
                    End If
                    color = ModuleMessage.ModuleMessageType.GreenSuccess
                Case TrialStatus.Expired
                    If isView _
                        AndAlso (Me._Config.Limitation And TrialLimitation.ExpireView) <> TrialLimitation.ExpireView _
                        AndAlso Not PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Admin, PortalSettings, _
                                                                   moduleControl.ModuleConfiguration) Then
                        Exit Sub
                    End If
                    color = ModuleMessage.ModuleMessageType.YellowWarning
                Case TrialStatus.Invalid, TrialStatus.InvalidModule
                    color = ModuleMessage.ModuleMessageType.RedError
                Case TrialStatus.Install
                    color = ModuleMessage.ModuleMessageType.GreenSuccess
            End Select

            Dim message As String = objTrialStatusInfo.Status.ToString

            Dim key As String = "Trial." & objTrialStatusInfo.Status.ToString
            message = Localization.GetString(key, NukeHelper.GetModuleSharedResourceFile(moduleControl.ModuleConfiguration.ModuleDefID))

            message = message.Replace("[Trial:NbDaysLeft]", (objTrialStatusInfo.NbDaysLeft).ToString)
            message = message.Replace("[Trial:NbDays]", (Me._Config.Duration).ToString)
            message = message.Replace("[Trial:ModuleName]", (Me._Config.ModuleName).ToString)
            If TypeOf moduleControl Is ModuleSettingsBase Then
                Dim messageControl As Control = Skin.GetModuleMessageControl("", message, color)
                moduleControl.Controls.AddAt(0, messageControl)
                'AddAt(moduleControl.Parent.Controls.IndexOf(moduleControl), messageControl)
            Else

                Skin.AddModuleMessage(moduleControl, message, color)
            End If

        End Sub


#End Region

    End Class
End Namespace
