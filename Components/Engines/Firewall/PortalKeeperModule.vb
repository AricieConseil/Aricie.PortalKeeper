Imports Aricie.Collections
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.Services.Log.EventLog
Imports System
Imports System.Web
Imports System.Reflection
Imports Aricie.Web
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions


Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class Debug

        'Public Const DebugType As String = "PKP"
        Public Const PKPDebugType As String = "PKP"

        'Public Class TimingSteps

        '    Public Const StartPKPFlow As String = "Start PKP FLow"
        '    Public Const EndDoS As String = "End DoS Evaluation"

        'End Class

    End Class

    Public Class PortalKeeperModule
        Implements IHttpModule

        Private Const InstanceNbKey As String = "PKPApplicationInstanceNb"
        Private Shared _InitLock As New Object
        Private Shared _ApplicationState As HttpApplicationState

        Protected SecondaryModule As Boolean



        Public Overridable Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init



            SyncLock _InitLock


                If _ApplicationState Is Nothing Then
                    _ApplicationState = application.Application
                    Me.ProcessApplicationStep(ApplicationEvent.ApplicationStart, False)
                    If PortalKeeperConfig.Instance.ApplicationSettings.Enabled AndAlso PortalKeeperConfig.Instance.ApplicationSettings.EnableCriticalChangesHandler Then
                        HttpInternals.Instance.CombineCriticalChangeCallBack(New EventHandler(AddressOf OnCriticaldirChange))
                    End If

                End If
                Dim instanceNb As Integer = CInt(_ApplicationState.Get(InstanceNbKey))
                instanceNb += 1
               
                _ApplicationState.Set(InstanceNbKey, instanceNb)
            End SyncLock

            AddHandler application.BeginRequest, AddressOf Me.OnBeginRequest

            If Not Me.SecondaryModule Then

                AddHandler application.AuthenticateRequest, AddressOf Me.OnAuthenticateRequest
                AddHandler application.PostMapRequestHandler, AddressOf Me.PostMapRequestHandler

                AddHandler application.PreRequestHandlerExecute, AddressOf Me.PreRequestHandlerExecute

                AddHandler application.PostRequestHandlerExecute, AddressOf Me.PostRequestHandlerExecute

                AddHandler application.PreSendRequestHeaders, AddressOf Me.PreSendRequestHeaders

                AddHandler application.ReleaseRequestState, AddressOf Me.ReleaseRequestState

                AddHandler application.EndRequest, AddressOf Me.OnEndRequest

            End If

        End Sub

      


        Private Sub OnBeginRequest(ByVal sender As Object, ByVal e As EventArgs)
            'Try
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context

            Dim keeperContext As PortalKeeperContext(Of RequestEvent) = PortalKeeperContext(Of RequestEvent).Instance(context)
            keeperContext.SetEngine(keeperContext.CurrentFirewallConfig)
            'Dim requestOutInScope As Boolean = keeperContext.RequestOutOfScope
            If keeperContext.CurrentFirewallConfig.Enabled AndAlso Not keeperContext.RequestOutOfScope Then
                If Not Me.SecondaryModule Then

                    If keeperContext.LoggingLevel > LoggingLevel.None Then
                        Dim objPair As New KeyValuePair(Of String, String)("Input Uri", keeperContext.DnnContext.AbsoluteUri)
                        keeperContext.LogStartEngine(objPair)
                    End If
                    keeperContext.Init(keeperContext.CurrentFirewallConfig)
                    Me.ProcessRecovery(keeperContext)
                    Dim dosMatched As Boolean = False
                    If keeperContext.CurrentFirewallConfig.DosSettings.Enabled Then
                        keeperContext.LogStart("DoS Evaluation", LoggingLevel.Steps, False)
                        dosMatched = Me.ProcessDenialOfService(keeperContext)
                        If keeperContext.LoggingLevel >= LoggingLevel.Steps Then
                            Dim objPair As New KeyValuePair(Of String, String)("Dos Matched", dosMatched.ToString)
                            'Dim objStep As New StepInfo(Debug.PKPDebugType, Debug.TimingSteps.EndDoS, WorkingPhase.InProgress, False, False, -1, keeperContext.FlowId)
                            'PerformanceLogger.Instance.AddDebugInfo(objStep)
                            keeperContext.LogEnd("Dos Evaluation", False, False, LoggingLevel.Steps, Nothing, objPair)
                        End If
                    End If
                    If Not dosMatched Then
                        Me.ProcessStep(context, RequestEvent.BeginRequest, False)
                    End If
                Else
                    Me.ProcessRecovery(keeperContext)
                    Me.ProcessStep(context, RequestEvent.BeginRequestAfterDNN, False)
                End If
            End If
            keeperContext.beginRequestPassed = True
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub


        Private Sub OnAuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
            'Try
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.AuthenticateRequest, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try


        End Sub

        Private Sub PostMapRequestHandler(ByVal sender As Object, ByVal e As EventArgs)
            'Try
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.PostMapRequestHandler, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try


        End Sub


        Private Shared _AdaptersRegistered As New Dictionary(Of String, Integer)
        Private Shared _RestServicesRegistered As Boolean = False

        Public Shared Sub RegisterAdapters()
            SyncLock _AdaptersRegistered
                _AdaptersRegistered = New Dictionary(Of String, Integer)
            End SyncLock
        End Sub

        Public Shared Sub RegisterServices()
            _RestServicesRegistered = False
        End Sub

        Private Sub PreRequestHandlerExecute(ByVal s As Object, ByVal e As EventArgs)

            'Try
            Dim context As HttpContext = DirectCast(s, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.PreRequestHandlerExecute, False)
            'Me.ProcessStep(context, RequestEvent.Default, False)

            Dim keeperContext As PortalKeeperContext(Of RequestEvent) = PortalKeeperContext(Of RequestEvent).Instance(context)
            Dim keeperConfig As FirewallConfig = keeperContext.CurrentFirewallConfig

            If context.CurrentHandler IsNot Nothing AndAlso TypeOf context.CurrentHandler Is Page Then
                Dim objPage As Page = DirectCast(context.CurrentHandler, Page)
                If keeperConfig.Enabled Then
                    AddHandler objPage.PreInit, AddressOf Me.Page_PreInit
                    AddHandler objPage.Init, AddressOf Me.Page_Init
                    AddHandler objPage.Load, AddressOf Me.Page_Load
                    AddHandler objPage.PreRender, AddressOf Me.Page_PreRender
                    AddHandler objPage.PreRenderComplete, AddressOf Me.Page_PreRenderComplete
                End If
                If keeperContext.GlobalConfig.ControlAdapters.Enabled Then
                    Dim nbRegistered As Integer
                    If Not _AdaptersRegistered.TryGetValue(context.Request.Browser.Id, nbRegistered) OrElse context.Request.Browser.Adapters.Count < nbRegistered Then
                        AddHandler objPage.PreInit, AddressOf Me.Page_PreInitAdapters
                    End If
                End If
                
            End If


            If keeperContext.GlobalConfig.RestServices.Enabled Then
                If Not _RestServicesRegistered Then
                    SyncLock keeperContext.GlobalConfig.RestServices
                        If Not _RestServicesRegistered Then
                            keeperContext.GlobalConfig.RestServices.RegisterRestServices()
                            _RestServicesRegistered = True
                        End If
                    End SyncLock
                End If
            End If
            
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try


        End Sub

        Private Sub Page_PreInitAdapters(ByVal sender As Object, ByVal args As EventArgs)
            Dim id As String = HttpContext.Current.Request.Browser.Id
            Dim nbRegistered As Integer = PortalKeeperConfig.Instance.ControlAdapters.RegisterAdapters()
            SyncLock _AdaptersRegistered
                _AdaptersRegistered(id) = nbRegistered
            End SyncLock
        End Sub





        Private Sub Page_PreInit(ByVal sender As Object, ByVal args As EventArgs)

            'Try
            Dim context As HttpContext = HttpContext.Current
            Me.ProcessStep(context, RequestEvent.PagePreInit, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal args As EventArgs)

            'Try
            Dim context As HttpContext = HttpContext.Current
            Me.ProcessStep(context, RequestEvent.PageInit, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal args As EventArgs)

            'Try
            Dim context As HttpContext = HttpContext.Current
            Me.ProcessStep(context, RequestEvent.PageLoad, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal args As EventArgs)

            'Try
            Dim context As HttpContext = HttpContext.Current
            Me.ProcessStep(context, RequestEvent.PagePreRender, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal args As EventArgs)

            'Try
            Dim context As HttpContext = HttpContext.Current
            Me.ProcessStep(context, RequestEvent.PagePreRenderComplete, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub

        Private Sub PostRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)

            'Try
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.PostRequestHandlerExecute, False)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub

         Private Sub PreSendRequestHeaders(sender As Object, e As EventArgs)
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.PreSendRequestHeaders, False)
        End Sub


        Public Const ResponseLengthMessageKey As String = "ResponseLength"
        Private Sub ReleaseRequestState(sender As Object, e As EventArgs)
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.ReleaseRequestState, False)
            'If context.Response IsNot Nothing AndAlso context.Response.Filter IsNot Nothing Then
            '    Dim responseSize As Long = context.Response.Filter.Length
            '    Dim keeperContext As PortalKeeperContext(Of RequestEvent) = PortalKeeperContext(Of RequestEvent).Instance(context)
            '    keeperContext.OnSendMessage(Of Long)(ResponseLengthMessageKey, responseSize)
            'End If
        End Sub

        Private Sub OnEndRequest(ByVal sender As Object, ByVal e As EventArgs)

            'Try
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.EndRequest, True)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub


        Private Sub OnCriticaldirChange(sender As Object, e As EventArgs)
            PortalKeeperContext(Of ApplicationEvent).GlobalInstance.SetVar("Eargs", e)
            Me.ProcessApplicationStep(ApplicationEvent.CriticalChange, False)
        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
            Dim instanceNb As Integer
            SyncLock _InitLock
                instanceNb = CInt(_ApplicationState.Get(InstanceNbKey))
                instanceNb -= 1

                _ApplicationState.Set(InstanceNbKey, instanceNb)
            End SyncLock
            If instanceNb <= 0 Then
                Me.ProcessApplicationStep(ApplicationEvent.ApplicationEnd, False)
            End If
        End Sub



#Region "Private Methods"

        Private Sub ProcessRecovery(ByVal keeperContext As PortalKeeperContext(Of RequestEvent))
            Dim keeperConfig As FirewallConfig = keeperContext.CurrentFirewallConfig
            If keeperConfig.EnableRecoveryParams Then
                For Each recoveryParam In keeperConfig.RecoveryParam.Split(","c)
                    If (Not String.IsNullOrEmpty(recoveryParam.Trim)) AndAlso keeperContext.DnnContext.HttpContext.Request.RawUrl.Contains(recoveryParam) Then
                        keeperConfig.Enabled = False
                    End If
                Next
            End If
            If keeperConfig.EnableRecoveryParams AndAlso keeperContext.DnnContext.HttpContext.Request.RawUrl.Contains(keeperConfig.RestartParam) Then

                Dim config As Type = GetType(Config)
                config.InvokeMember("Touch", BindingFlags.Public Or BindingFlags.InvokeMethod, Nothing, Nothing, Nothing)

                'Jesse: pb with suposedly different signatures between dnn versions
                'todo: Not sure what's wrong with the following statement
                'DotNetNuke.Common.Utilities.Config.Touch()
            End If
        End Sub

        Protected Sub ProcessStep(ByVal context As HttpContext, ByVal newStep As RequestEvent, ByVal endSequence As Boolean)
            Dim keeperContext As PortalKeeperContext(Of RequestEvent) = PortalKeeperContext(Of RequestEvent).Instance(context)
            If (Not keeperContext.Disabled AndAlso keeperContext.CurrentFirewallConfig.Enabled) AndAlso Not keeperContext.RequestOutOfScope Then
                keeperContext.CurrentFirewallConfig.ProcessRules(keeperContext, newStep, False, True)
                If endSequence AndAlso keeperContext.LoggingLevel > LoggingLevel.None Then
                    Dim objPair As New KeyValuePair(Of String, String)("Input Uri", keeperContext.DnnContext.AbsoluteUri)
                    Dim objPair2 As New KeyValuePair(Of String, String)("Verb", context.Request.HttpMethod)
                    keeperContext.LogEndEngine(objPair, objPair2)
                End If
            End If
        End Sub

        Protected Sub ProcessApplicationStep(ByVal newStep As ApplicationEvent, ByVal endSequence As Boolean)
            Dim keeperContext As PortalKeeperContext(Of ApplicationEvent) = PortalKeeperContext(Of ApplicationEvent).GlobalInstance
            If Not keeperContext.Disabled AndAlso keeperContext.GlobalConfig.ApplicationSettings.Enabled Then
                keeperContext.GlobalConfig.ApplicationSettings.ProcessRules(keeperContext, newStep, False, True)
            End If
        End Sub


        Private Function ProcessDenialOfService(ByVal keeperContext As PortalKeeperContext(Of RequestEvent)) As Boolean
            Dim currentKey As String = Nothing
            Dim currentExpiration As DateTime
            Dim toRelease As New List(Of KeyValuePair(Of Object, String))
            DosEnabledConditionProvider(Of RequestEvent).BeginReadProviders()
            Dim tempProvs As New List(Of DosEnabledConditionProvider(Of RequestEvent))(DosEnabledConditionProvider(Of RequestEvent).DosProviders)
            DosEnabledConditionProvider(Of RequestEvent).EndReadProviders()
            For Each dosConditionSettings As DosEnabledConditionProvider(Of RequestEvent) In tempProvs
                toRelease.Clear()
                For Each clueDico As KeyValuePair(Of Object, SerializableDictionary(Of String, DateTime)) In dosConditionSettings.DosDico
                    Try
                        currentKey = dosConditionSettings.FastGetKey(keeperContext, clueDico.Key)
                    Catch ex As Exception
                        Exceptions.LogException(ex)
                        currentKey = ""
                    End Try
                    If clueDico.Value.TryGetValue(currentKey, currentExpiration) Then
                        If PerformanceLogger.Now < currentExpiration Then
                            'match -> run DoS action
                            keeperContext.CurrentFirewallConfig.DosSettings.DoSAction.Run(keeperContext)
                            Return True
                        Else
                            'expired -> reset
                            toRelease.Add(New KeyValuePair(Of Object, String)(clueDico.Key, currentKey))
                        End If
                    End If
                Next
                For Each objPair As KeyValuePair(Of Object, String) In toRelease
                    dosConditionSettings.ReleaseDoS(objPair.Key, objPair.Value)
                Next
            Next

            Return False
        End Function



#End Region

    End Class
End Namespace


