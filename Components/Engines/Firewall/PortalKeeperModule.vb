Imports Aricie.Collections
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.Services.Log.EventLog
Imports System
Imports System.Web
Imports System.Reflection
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

        Private Shared LogController As New EventLogController

        Protected SecondaryModule As Boolean


        Public Overridable Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init

            AddHandler application.BeginRequest, AddressOf Me.OnBeginRequest

            If Not Me.SecondaryModule Then

                AddHandler application.AuthenticateRequest, AddressOf Me.OnAuthenticateRequest
                AddHandler application.PostMapRequestHandler, AddressOf Me.PostMapRequestHandler

                AddHandler application.PreRequestHandlerExecute, AddressOf Me.PreRequestHandlerExecute

                AddHandler application.PostRequestHandlerExecute, AddressOf Me.PostRequestHandlerExecute

                AddHandler application.EndRequest, AddressOf Me.OnEndRequest
            Else

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
                        Dim objPair As New KeyValuePair(Of String, String)("Input Uri", context.Request.Url.AbsoluteUri)
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

        Private Sub PreRequestHandlerExecute(ByVal s As Object, ByVal e As EventArgs)

            'Try
            Dim context As HttpContext = DirectCast(s, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.PreRequestHandlerExecute, False)
            'Me.ProcessStep(context, RequestEvent.Default, False)

            Dim keeperContext As PortalKeeperContext(Of RequestEvent) = PortalKeeperContext(Of RequestEvent).Instance(context)
            Dim keeperConfig As FirewallConfig = keeperContext.CurrentFirewallConfig
            If keeperConfig.Enabled Then
                If context.CurrentHandler IsNot Nothing AndAlso TypeOf context.CurrentHandler Is Page Then
                    Dim objPage As Page = DirectCast(context.CurrentHandler, Page)
                    AddHandler objPage.PreInit, AddressOf Me.Page_PreInit
                    AddHandler objPage.Init, AddressOf Me.Page_Init
                    AddHandler objPage.Load, AddressOf Me.Page_Load
                    AddHandler objPage.PreRender, AddressOf Me.Page_PreRender
                    AddHandler objPage.PreRenderComplete, AddressOf Me.Page_PreRenderComplete
                End If
            End If
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try


        End Sub

        Private Sub Page_PreInit(ByVal sender As Object, ByVal args As EventArgs)

            'Try
            Dim context As HttpContext = HttpContext.Current
            PortalKeeperConfig.Instance.ControlAdapters.RegisterAdapters()
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

        Private Sub OnEndRequest(ByVal sender As Object, ByVal e As EventArgs)

            'Try
            Dim context As HttpContext = DirectCast(sender, HttpApplication).Context
            Me.ProcessStep(context, RequestEvent.EndRequest, True)
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try

        End Sub




        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub



#Region "Private Methods"

        Private Sub ProcessRecovery(ByVal keeperContext As PortalKeeperContext(Of RequestEvent))
            Dim keeperConfig As FirewallConfig = keeperContext.CurrentFirewallConfig
            For Each recoveryParam In keeperConfig.RecoveryParam.Split(","c)
                If (Not String.IsNullOrEmpty(recoveryParam.Trim)) AndAlso keeperContext.DnnContext.HttpContext.Request.RawUrl.Contains(recoveryParam) Then
                    keeperConfig.Enabled = False
                End If
            Next
            If keeperContext.DnnContext.HttpContext.Request.RawUrl.Contains(keeperConfig.RestartParam) Then

                Dim config As Type = GetType(Config)
                config.InvokeMember("Touch", BindingFlags.Public, Nothing, Nothing, Nothing)

                'Jesse: pb with suposedly different signatures between dnn versions
                'todo: Not sure what's wrong with the following statement
                'DotNetNuke.Common.Utilities.Config.Touch()
            End If
        End Sub



        Protected Sub ProcessStep(ByVal context As HttpContext, ByVal newStep As RequestEvent, ByVal endSequence As Boolean)
            Dim keeperContext As PortalKeeperContext(Of RequestEvent) = PortalKeeperContext(Of RequestEvent).Instance(context)
            If (Not keeperContext.Disabled AndAlso keeperContext.CurrentFirewallConfig.Enabled) AndAlso Not keeperContext.RequestOutOfScope Then
                keeperContext.CurrentFirewallConfig.ProcessRules(keeperContext, newStep, False, True)
                If endSequence Then
                    Dim objPair As New KeyValuePair(Of String, String)("Input Uri", context.Request.Url.AbsoluteUri)
                    Dim objPair2 As New KeyValuePair(Of String, String)("Verb", context.Request.HttpMethod)
                    keeperContext.LogEndEngine(objPair, objPair2)
                End If
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


