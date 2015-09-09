Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports Aricie.Collections
Imports Aricie.DNN.Entities
Imports System.IO
Imports WatiN.Core
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Services
Imports System.Net
Imports Aricie.Services
Imports System.Globalization

Namespace Aricie.DNN.Modules.PortalKeeper

    
    Public Enum ClientLocation
        OneTimeLocal
        Variable
        Cache
        Session
    End Enum

    Public Enum WebRequestMode
        DoRequest
        CloseClient
    End Enum


    <Serializable()> _
    Public Class WebRequestInfo(Of TEngineEvents As IConvertible)


        Private _UrlMode As UrlMode
        Private _Url As String = ""
        Private _UrlExpression As New FleeExpressionInfo(Of String)
        Private _Method As WebMethod = WebMethod.Get
        Private _ParametersMode As ParametersMode

        Private _RetryNb As Integer
        Private _UseProxyPool As Boolean
        Private _ProxyIsMandatory As Boolean
        Private _ProxyExpression As New SimpleExpression(Of WebProxyPool)

        Private _STimeout As New STimeSpan(TimeSpan.FromMilliseconds(10000))

        Public Property WebMode As WebRequestMode = WebRequestMode.DoRequest

        '<ExtendedCategory("Url")> _
        <ConditionalVisible("WebMode", False, True, WebRequestMode.DoRequest)> _
        Public Property UrlMode() As UrlMode
            Get
                Return _UrlMode
            End Get
            Set(ByVal value As UrlMode)
                _UrlMode = value
            End Set
        End Property


        '<ExtendedCategory("Url")> _
        <ConditionalVisible("WebMode", False, True, WebRequestMode.DoRequest)> _
        <ConditionalVisible("UrlMode", False, True, UrlMode.String)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
        <Width(500)> _
        <MaxLength(256)> _
        Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal value As String)
                _Url = value
            End Set
        End Property

        '<ExtendedCategory("Url")> _
        <ConditionalVisible("UrlMode", False, True, UrlMode.Expression)> _
        <ConditionalVisible("WebMode", False, True, WebRequestMode.DoRequest)> _
        Public Property UrlExpression() As FleeExpressionInfo(Of String)
            Get
                Return _UrlExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _UrlExpression = value
            End Set
        End Property



        <ExtendedCategory("Client")> _
        Public Property ClientMode As New SimpleOrExpression(Of HttpClientMode)(HttpClientMode.WebClient)

        <ExtendedCategory("Client")> _
        Public Property ClientLocation As ClientLocation = ClientLocation.Variable

        <ExtendedCategory("WebClient")> _
        Public Property WebClientVarName As String = "WebClient"

        <ExtendedCategory("WebBrowser")> _
        Public Property WebBrowserVarName As String = "WebBrowser"

        <ConditionalVisible("WebMode", False, True, WebRequestMode.DoRequest)> _
        <ExtendedCategory("Client")> _
         Public Property CloseClient As Boolean


        <ExtendedCategory("Request")> _
        Public Property ParametersMode() As ParametersMode
            Get
                Return _ParametersMode
            End Get
            Set(ByVal value As ParametersMode)
                _ParametersMode = value
            End Set
        End Property

        <ExtendedCategory("Request")> _
        Public Property TimeOut() As STimeSpan
            Get
                Return _STimeout
            End Get
            Set(ByVal value As STimeSpan)
                _STimeout = value
            End Set
        End Property

       


        <ExtendedCategory("Request")> _
        Public Property RetryNb() As Integer
            Get
                Return _RetryNb
            End Get
            Set(ByVal value As Integer)
                _RetryNb = value
            End Set
        End Property

        

        <ExtendedCategory("Request")> _
        Public Property LogRequest As Boolean

        <ConditionalVisible("LogRequest", False, True)> _
        <ExtendedCategory("Request")> _
        Public Property LogResponse As Boolean


        <ExtendedCategory("WebClient")> _
        Public Property Method() As WebMethod
            Get
                Return _Method
            End Get
            Set(ByVal value As WebMethod)
                _Method = value
            End Set
        End Property

        <ExtendedCategory("WebClient")> _
        Public Property EnableReferer As Boolean
        

        '<ExtendedCategory("Client")> _
        'Public Property ReUseClient As Boolean

        '<ConditionalVisible("ReUseClient", False, True)> _

        <ExtendedCategory("WebClient")> _
        Public Property SwallowWebExceptions As Boolean

        <ExtendedCategory("WebClient")> _
         Public Property MaxConcurrentConnexions As New EnabledFeature(Of Integer)

        

        

        <ExtendedCategory("WebClient")> _
      <MainCategory()> _
        Public Property UseProxyPool() As Boolean
            Get
                Return _UseProxyPool
            End Get
            Set(ByVal value As Boolean)
                _UseProxyPool = value
            End Set
        End Property

        <ConditionalVisible("UseProxyPool", False, True)> _
        <ExtendedCategory("WebClient")> _
        Public Property ProxyExpression() As SimpleExpression(Of WebProxyPool)
            Get
                Return _ProxyExpression
            End Get
            Set(ByVal value As SimpleExpression(Of WebProxyPool))
                _ProxyExpression = value
            End Set
        End Property

        <ExtendedCategory("WebClient")> _
        <ConditionalVisible("UseProxyPool", False, True)> _
        Public Property ProxyIsMandatory() As Boolean
            Get
                Return _ProxyIsMandatory
            End Get
            Set(ByVal value As Boolean)
                _ProxyIsMandatory = value
            End Set
        End Property

        <ExtendedCategory("WebBrowser")> _
        Public Property CaptureFrame As Boolean

        <ExtendedCategory("WebBrowser")> _
        <ConditionalVisible("CaptureFrame", False, True)> _
        Public Property FrameId As New SimpleOrExpression(Of String)("")


       

        Private Function GetParamQueryString(ByVal inputParams As Dictionary(Of String, String)) As String
            Dim toReturn As String = ""
            For Each inputParam As KeyValuePair(Of String, String) In inputParams
                toReturn &= inputParam.Key & "=" & inputParam.Value & "&"c
            Next
            toReturn = toReturn.TrimEnd("&"c)
            Return toReturn
        End Function



        Public Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal inputParams As SerializableDictionary(Of String, String), Optional ByVal headers As SerializableDictionary(Of String, String) = Nothing, _
                            Optional forceCloseClient As Boolean = False) As String

            Dim toReturn As String = ""
           

            'Dim enableStopWatch As Boolean = actionContext.EnableStopWatch

           

            Dim success As Boolean
            Dim objRetryNb As Integer
            Dim objProx As WebProxy = Nothing
            Dim virtualProxy As Boolean
            Dim objTimeout As TimeSpan


            Dim objClient As CompressionEnabledWebClient = Nothing
            Dim browserInstance As IE = Nothing
            Dim existingClient As IDisposable = Nothing

            Dim duration As TimeSpan

            Dim objClientMode As HttpClientMode = Me.ClientMode.GetValue(actionContext, actionContext)
            Dim frameIdValue As String = ""
            If objClientMode = HttpClientMode.Browser AndAlso Me.CaptureFrame Then
                frameIdValue = Me.FrameId.GetValue(actionContext, actionContext)
            End If

            Dim varname As String = WebClientVarName
            If Not Me.ClientLocation = PortalKeeper.ClientLocation.OneTimeLocal Then
                If objClientMode = HttpClientMode.Browser Then
                    varname = WebBrowserVarName
                End If
                Select Case ClientLocation
                    Case PortalKeeper.ClientLocation.Variable
                        existingClient = DirectCast(actionContext.Item(varname), IDisposable)
                    Case PortalKeeper.ClientLocation.Cache
                        If objClientMode = HttpClientMode.Browser Then
                            existingClient = CacheHelper.GetGlobal(Of IE)(varname)
                        Else
                            existingClient = CacheHelper.GetGlobal(Of CompressionEnabledWebClient)(varname)
                        End If
                    Case PortalKeeper.ClientLocation.Session
                        existingClient = DirectCast(actionContext.ActionContext.DnnContext.Session(varname), IDisposable)
                End Select
            End If

            If Me.WebMode = WebRequestMode.CloseClient Then
                If existingClient IsNot Nothing Then
                    existingClient.Dispose()
                End If
                Return ""
            End If

            If existingClient Is Nothing Then
                If objClientMode = HttpClientMode.Browser Then
                    browserInstance = New IE()
                    'Thread.Sleep(2000)
                    'browserInstance.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Hide)
                    browserInstance.Visible = False
                    existingClient = browserInstance
                Else
                    objClient = CompressionEnabledWebClient.GetWebClient(Me._Method.ToString, objTimeout)
                    objClient.Encoding = Encoding.UTF8
                    objClient.EnableReferer = Me.EnableReferer
                    If Me.MaxConcurrentConnexions.Enabled Then
                        objClient.MaxConcurrentConnexions = Me.MaxConcurrentConnexions.Entity
                    End If
                    existingClient = objClient
                End If
                If Not Me.ClientLocation = PortalKeeper.ClientLocation.OneTimeLocal Then
                    Select Case ClientLocation
                        Case PortalKeeper.ClientLocation.Variable
                            actionContext.Item(varname) = existingClient
                        Case PortalKeeper.ClientLocation.Cache
                            If objClientMode = HttpClientMode.Browser Then
                                CacheHelper.SetGlobal(Of IE)(existingClient, varname)
                            Else
                                CacheHelper.SetGlobal(Of CompressionEnabledWebClient)(existingClient, varname)
                            End If
                        Case PortalKeeper.ClientLocation.Session
                            actionContext.ActionContext.DnnContext.Session(varname) = existingClient
                    End Select
                End If
            Else
                If objClientMode = HttpClientMode.Browser Then
                    browserInstance = DirectCast(existingClient, IE)
                Else
                    objClient = DirectCast(existingClient, CompressionEnabledWebClient)
                End If
            End If

            Dim proxyPool As WebProxyPool = Nothing
            If Me._UseProxyPool Then
                proxyPool = Me._ProxyExpression.Evaluate(actionContext, actionContext)
            End If

            Dim targetUrl As String = Me._Url
            If Me._UrlMode = UrlMode.Expression Then
                targetUrl = Me._UrlExpression.Evaluate(actionContext, actionContext)
            End If
            If targetUrl.StartsWith("~") Then
                targetUrl = NukeHelper.BaseUrl & targetUrl.Substring(1)
            ElseIf targetUrl.IndexOf("://", System.StringComparison.Ordinal) = -1 Then
                targetUrl = "http://" & targetUrl
            End If

            If inputParams.Count > 0 Then
                Select Case Me._ParametersMode
                    Case ParametersMode.Query
                        Dim urlHasParams As Boolean = targetUrl.IndexOf("?"c) > 0
                        If Not urlHasParams Then
                            targetUrl &= "?"c
                        Else
                            targetUrl &= "&"c
                        End If
                        targetUrl &= Me.GetParamQueryString(inputParams)

                End Select
            End If

            While (Not success) AndAlso objRetryNb <= Me._RetryNb
                objTimeout = _STimeout.Value
                Dim start As DateTime

                Try
                    objRetryNb += 1

                    If Me._UseProxyPool AndAlso proxyPool IsNot Nothing Then
                        Dim objProxyInfo As WebProxyInfo = proxyPool.GetOne
                        virtualProxy = objProxyInfo.VirtualProxy
                        If objProxyInfo IsNot Nothing Then
                            objProx = objProxyInfo.Proxy
                            If objProx IsNot Nothing Then
                                objTimeout = objTimeout.Add(objProxyInfo.Lag.Value)
                            End If
                        End If
                        If objProx Is Nothing AndAlso Me._ProxyIsMandatory Then
                            toReturn = String.Empty
                            Exit While
                        End If
                    End If

                    If objClientMode = HttpClientMode.WebClient Then
                        If objProx IsNot Nothing Then
                            objClient.Proxy = objProx
                            objClient.VirtualProxy = virtualProxy
                        End If
                        If headers IsNot Nothing AndAlso headers.Count > 0 Then
                            For Each headerPair As KeyValuePair(Of String, String) In headers
                                objClient.Headers.Add(headerPair.Key, headerPair.Value)
                            Next
                        End If
                    End If

                    If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                        Dim targetKey As New KeyValuePair(Of String, String)("Target Url", targetUrl)
                        Dim paramsKey As New KeyValuePair(Of String, String)("Params", ReflectionHelper.Serialize(inputParams).InnerXml)
                        Dim objStep As New StepInfo(Debug.PKPDebugType, "Web Request - Start : " & targetUrl, WorkingPhase.EndOverhead, False, False, -1, _
                                                    actionContext.FlowId, targetKey, paramsKey)
                        PerformanceLogger.Instance.AddDebugInfo(objStep)
                    End If

                    start = Now

                    If objClientMode = HttpClientMode.Browser Then
                        'browserInstance.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Hide)
                        browserInstance.GoTo(targetUrl)
                        browserInstance.WaitForComplete(CInt(objTimeout.Ticks \ TimeSpan.TicksPerSecond))
                        If frameIdValue.IsNullOrEmpty() Then
                            toReturn = browserInstance.Html
                        Else
                            Dim objFrame As Frame = browserInstance.Frames.First(Function(objTempFrame) objTempFrame.Id = frameIdValue)
                            If objFrame IsNot Nothing Then
                                toReturn = objFrame.Html
                            End If
                        End If
                    Else
                        Select Case Me._Method
                            Case WebMethod.Get
                                toReturn = objClient.DownloadString(targetUrl)

                            Case WebMethod.Post, WebMethod.Put, WebMethod.Delete
                                Dim postData As String = Me.GetPostData(inputParams)
                                objClient.Headers("Content-Type") = "application/x-www-form-urlencoded"
                                toReturn = objClient.UploadString(targetUrl, Me._Method.ToString().ToUpperInvariant(), postData)

                        End Select
                    End If
                    duration = Now.Subtract(start)

                    success = True
                Catch ex As WebException
                    Try
                        Dim objRep As WebResponse = ex.Response
                        If objRep IsNot Nothing Then
                           
                            Dim objStrem As Stream = objRep.GetResponseStream()
                            If objStrem IsNot Nothing Then
                                Dim objEncoding As Encoding = Nothing
                                If Not objRep.ContentType.IsNullOrEmpty() Then
                                    objEncoding = Me.GetEncodingFromContentType(objRep.ContentType)
                                End If
                                If objEncoding IsNot Nothing Then
                                    objEncoding = objClient.Encoding
                                End If
                                Using sr As New StreamReader(objStrem, objEncoding)
                                    toReturn = sr.ReadToEnd()
                                    If Me.SwallowWebExceptions Then
                                        Exit While
                                    End If
                                End Using
                            End If
                        End If
                    Catch
                        'todo: do nothing?
                    End Try
                    If objRetryNb >= Me._RetryNb Then
                        Dim obfuscatedParams As New Dictionary(Of String, String)(inputParams)
                        For Each objKey As String In inputParams.Keys
                            If objKey.ToLower.Contains("pass") Then
                                obfuscatedParams(objKey) = New String("x"c, inputParams(objKey).Length)
                            End If
                        Next
                        Dim message As String = String.Format("WebAction Exception: Duration:{0}, Target Url: {1}, Params: {2}, TimeOut: {3}", _
                                                              Now.Subtract(start).FormatTimeSpan(), targetUrl, Me.GetParamQueryString(obfuscatedParams), objTimeout.FormatTimeSpan())
                        Dim newEx As New ApplicationException(message, ex)
                        Throw newEx
                    End If
                Catch ex As Exception
                    If objRetryNb >= Me._RetryNb Then
                        Dim obfuscatedParams As New Dictionary(Of String, String)(inputParams)
                        For Each objKey As String In inputParams.Keys
                            If objKey.ToLower.Contains("pass") Then
                                obfuscatedParams(objKey) = New String("x"c, inputParams(objKey).Length)
                            End If
                        Next
                        Dim message As String = String.Format("WebAction Exception: Duration:{0}, Target Url: {1}, Params: {2}, TimeOut: {3}", _
                                                              Now.Subtract(start).FormatTimeSpan(), targetUrl, Me.GetParamQueryString(obfuscatedParams), objTimeout.FormatTimeSpan())
                        Dim newEx As New ApplicationException(message, ex)
                        Throw newEx
                    End If
                Finally
                    If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                        Dim responseLengthPair As New KeyValuePair(Of String, String)("Response Length", toReturn.Length.ToString(CultureInfo.InvariantCulture))
                        Dim responseBeginning As New KeyValuePair(Of String, String)("Response First 10000 chars", toReturn.ToString(CultureInfo.InvariantCulture).Substring(0, Math.Min(toReturn.Length, 10000)))
                        Dim objStep As New StepInfo(Debug.PKPDebugType, "Web Request - End: " & targetUrl, WorkingPhase.InProgress, False, False, -1, actionContext.FlowId, _
                                                    responseLengthPair, responseBeginning)
                        PerformanceLogger.Instance.AddDebugInfo(objStep)
                    End If
                    If existingClient IsNot Nothing AndAlso ((Me.ClientLocation = PortalKeeper.ClientLocation.OneTimeLocal) OrElse Me.CloseClient OrElse forceCloseClient) Then
                        existingClient.Dispose()
                    End If
                End Try
            End While


            If LogRequest Then
                Dim objUrl As New KeyValuePair(Of String, String)("Url", targetUrl)
                Dim durationPair As New KeyValuePair(Of String, String)("Duration", duration.ToString())
                Dim objParams As New KeyValuePair(Of String, String)("Params", ReflectionHelper.Serialize(inputParams).Beautify())
                Dim objDebug As DebugInfo
                If LogResponse Then
                    Dim objResponse As New KeyValuePair(Of String, String)("Response", toReturn)
                    objDebug = New DebugInfo(Debug.PKPDebugType, "Web Request: " & targetUrl, objUrl, durationPair, objParams, objResponse)
                Else
                    objDebug = New DebugInfo(Debug.PKPDebugType, "Web Request: " & targetUrl, objUrl, durationPair, objParams)
                End If
                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If

            Return toReturn

        End Function



        Private Function GetEncodingFromContentType(contentType As String) As Encoding
                contentType = contentType.ToLower(CultureInfo.InvariantCulture)
                Dim parsedList As String() = contentType.Split(New Char() {";"c, "="c, " "c})
                Dim nextItem As Boolean = False
                For Each item As String In parsedList
                    If item = "charset" Then
                        nextItem = True
                    ElseIf nextItem Then
                        Return Encoding.GetEncoding(item)
                    End If
                Next
            Return Nothing
        End Function



        Public Function GetPostData(params As IDictionary(Of String, String)) As String
            Dim toReturn As New StringBuilder
            Dim delimiter As String = String.Empty
            For Each objPair As KeyValuePair(Of String, String) In params
                toReturn.Append(delimiter)
                toReturn.Append(HttpUtility.UrlEncode(objPair.Key))
                toReturn.Append("=")
                toReturn.Append(HttpUtility.UrlEncode(objPair.Value))
                delimiter = "&"
            Next
            Return toReturn.ToString
        End Function

    End Class



End Namespace