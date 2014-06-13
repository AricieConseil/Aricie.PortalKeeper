Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Services
Imports System.Net
Imports Aricie.Services
Imports System.Globalization
Imports System.Reflection

Namespace Aricie.DNN.Modules.PortalKeeper
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


        <ExtendedCategory("Url")> _
        <MainCategory()> _
        Public Property UrlMode() As UrlMode
            Get
                Return _UrlMode
            End Get
            Set(ByVal value As UrlMode)
                _UrlMode = value
            End Set
        End Property


        <ExtendedCategory("Url")> _
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

        <ExtendedCategory("Url")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <ConditionalVisible("UrlMode", False, True, UrlMode.Expression)> _
            <LabelMode(LabelMode.Top)> _
        Public Property UrlExpression() As FleeExpressionInfo(Of String)
            Get
                Return _UrlExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _UrlExpression = value
            End Set
        End Property

        <ExtendedCategory("Request")> _
        Public Property Method() As WebMethod
            Get
                Return _Method
            End Get
            Set(ByVal value As WebMethod)
                _Method = value
            End Set
        End Property



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
        <ExtendedCategory("Request")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property ProxyExpression() As SimpleExpression(Of WebProxyPool)
            Get
                Return _ProxyExpression
            End Get
            Set(ByVal value As SimpleExpression(Of WebProxyPool))
                _ProxyExpression = value
            End Set
        End Property

        <ExtendedCategory("Request")> _
        <ConditionalVisible("UseProxyPool", False, True)> _
        Public Property ProxyIsMandatory() As Boolean
            Get
                Return _ProxyIsMandatory
            End Get
            Set(ByVal value As Boolean)
                _ProxyIsMandatory = value
            End Set
        End Property

        <ExtendedCategory("Request")> _
        Public Property LogRequest() As Boolean

        Private Function GetParamQueryString(ByVal inputParams As Dictionary(Of String, String)) As String
            Dim toReturn As String = ""
            For Each inputParam As KeyValuePair(Of String, String) In inputParams
                toReturn &= inputParam.Key & "=" & inputParam.Value & "&"c
            Next
            toReturn = toReturn.TrimEnd("&"c)
            Return toReturn
        End Function



        Public Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal inputParams As SerializableDictionary(Of String, String), Optional ByVal headers As SerializableDictionary(Of String, String) = Nothing) As String

            Dim toReturn As String = ""
            Dim targetUrl As String = Me._Url
            If Me._UrlMode = UrlMode.Expression Then
                targetUrl = Me._UrlExpression.Evaluate(actionContext, actionContext)
            End If
            If targetUrl.StartsWith("~") Then
                targetUrl = "http://" & (PortalAliasesByPortalId(PortalIds(0))(0).HTTPAlias) & targetUrl.Substring(1)
            ElseIf targetUrl.IndexOf("://") = -1 Then
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





            Dim enableStopWatch As Boolean = actionContext.EnableStopWatch

            Dim proxyPool As WebProxyPool = Nothing
            If Me._UseProxyPool Then
                proxyPool = Me._ProxyExpression.Evaluate(actionContext, actionContext)
            End If

            Dim success As Boolean
            Dim objRetryNb As Integer
            Dim objProx As WebProxy = Nothing
            Dim virtualProxy As Boolean
            Dim objTimeout As TimeSpan
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

                    Using objClient As CompressionEnabledWebClient = CompressionEnabledWebClient.GetWebClient(Me._Method.ToString, objTimeout)
                        If objProx IsNot Nothing Then
                            objClient.Proxy = objProx
                            objClient.VirtualProxy = virtualProxy
                        End If
                        If headers IsNot Nothing AndAlso headers.Count > 0 Then
                            For Each headerPair As KeyValuePair(Of String, String) In headers
                                objClient.Headers.Add(headerPair.Key, headerPair.Value)
                            Next
                        End If
                        If enableStopWatch Then
                            Dim targetKey As New KeyValuePair(Of String, String)("Target Url", targetUrl)
                            Dim paramsKey As New KeyValuePair(Of String, String)("Params", ReflectionHelper.Serialize(inputParams).InnerXml)
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Web Request - Start : " & targetUrl, WorkingPhase.EndOverhead, False, False, -1, _
                                                        actionContext.FlowId, targetKey, paramsKey)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If

                        start = Now
                        Select Case Me._Method
                            Case WebMethod.Get
                                toReturn = objClient.DownloadString(targetUrl)

                            Case WebMethod.Post, WebMethod.Put, WebMethod.Delete
                                Dim postData As String = Me.GetPostData(inputParams)
                                objClient.Headers("Content-Type") = "application/x-www-form-urlencoded"
                                toReturn = objClient.UploadString(targetUrl, Me._Method.ToString().ToUpperInvariant(), postData)

                        End Select
                        success = True
                    End Using
                Catch ex As WebException
                    If objRetryNb >= Me._RetryNb Then
                        Dim obfuscatedParams As New Dictionary(Of String, String)(inputParams)
                        For Each objKey As String In inputParams.Keys
                            If objKey.ToLower.Contains("pass") Then
                                obfuscatedParams(objKey) = New String("x"c, inputParams(objKey).Length)
                            End If
                        Next
                        Dim message As String = String.Format("WebAction Exception: Duration:{0}, Target Url: {1}, Params: {2}, TimeOut: {3}", FormatTimeSpan(Now.Subtract(start)), targetUrl, Me.GetParamQueryString(obfuscatedParams), FormatTimeSpan(objTimeout))
                        Dim newEx As New ApplicationException(message, ex)
                        Throw newEx
                    End If
                End Try
            End While



            If enableStopWatch Then
                Dim responseLengthPair As New KeyValuePair(Of String, String)("Response Length", toReturn.Length.ToString(CultureInfo.InvariantCulture))
                Dim responseBeginning As New KeyValuePair(Of String, String)("Response First 10000 chars", toReturn.ToString(CultureInfo.InvariantCulture).Substring(0, Math.Min(toReturn.Length, 10000)))
                Dim objStep As New StepInfo(Debug.PKPDebugType, "Web Request - End: " & targetUrl, WorkingPhase.InProgress, False, False, -1, actionContext.FlowId, responseLengthPair, responseBeginning)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If

            If LogRequest Then
                Dim objUrl As New KeyValuePair(Of String, String)("Url", targetUrl)
                Dim objResponse As New KeyValuePair(Of String, String)("Response", toReturn)
                Dim objDebug As New DebugInfo(Debug.PKPDebugType, "Web Request: " & targetUrl, objUrl, objResponse)
                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If


            Return toReturn

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