Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Web
Imports System.Text
Imports Aricie.Services
Imports System.Xml.Serialization
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee

Namespace Services.Caching
    
    Public Class OutputCachingStrategy
        Implements IEnabled

        Private _Enabled As Boolean = True
        Private _Mode As OutputCacheMode = OutputCacheMode.Cache
        Private _Duration As New STimeSpan(TimeSpan.FromMinutes(30))
        Private _EmptyPathInfoOnly As Boolean
        Private _EmptyQueryStringOnly As Boolean
        Private _VaryByMode As VaryByMode = Caching.VaryByMode.AllButList
        Private _VaryByList As List(Of String)
        Private _VaryBy As String = ""
        Private _VaryByHeadersList As New List(Of String)
        Private _VaryByHeaders As String = ""

        Private _Verbs As String = "get"
        Private _VerbsList As List(Of String)
        Private _VaryByContentEncodings As String = "gzip;deflate"


        Public Property Enabled() As Boolean Implements IEnabled.Enabled
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property Mode() As OutputCacheMode
            Get
                Return _Mode
            End Get
            Set(ByVal value As OutputCacheMode)
                _Mode = value
            End Set
        End Property



        <ExtendedCategory("Policy")> _
        Public Property Cacheability As HttpCacheability = HttpCacheability.Public

        <ExtendedCategory("Policy")> _
        Public Property SetExpire As Boolean = True

        <ExtendedCategory("Policy")> _
        Public Property SetValidUntilExpires As Boolean = True

        <ConditionalVisible("SetExpire")> _
        <ExtendedCategory("Policy")> _
        Public Property Duration() As STimeSpan
            Get
                Return _Duration
            End Get
            Set(ByVal value As STimeSpan)
                _Duration = value
            End Set
        End Property


        <ExtendedCategory("Policy")> _
        Public Property SetLastModified As Boolean = True

        <ExtendedCategory("Policy")> _
        Public Property SetEtag As Boolean

        <ExtendedCategory("Policy")> _
        <ConditionalVisible("SetEtag")> _
        Public Property EtagExpression As New SimpleExpression(Of String)()

        <ExtendedCategory("Policy")> _
        Public Property VaryByStar As Boolean = True

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property VaryByList() As List(Of String)
            Get
                If _VaryByList Is Nothing Then
                    SyncLock _VaryBy
                        If _VaryByList Is Nothing Then
                            _VaryByList = Common.ParseStringList(_VaryBy)
                        End If
                    End SyncLock
                End If
                Return _VaryByList
            End Get
        End Property

        <ExtendedCategory("Policy")> _
        <ConditionalVisible("VaryByStar", True)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(3), Width(400), LabelMode(LabelMode.Top)> _
        Public Property VaryBy As String
            Get
                Return _VaryBy
            End Get
            Set(ByVal value As String)
                _VaryBy = value
                _VaryByList = Nothing
            End Set
        End Property

        <ExtendedCategory("Policy")> _
        Public Property VaryByBrowser As Boolean = True

        <ExtendedCategory("Policy")> _
        Public Property VaryByUserLanguage As Boolean

        <ExtendedCategory("Policy")> _
        Public Property VaryByUserAgent As Boolean

        <ExtendedCategory("Policy")> _
        Public Property VaryByUserCharSet As Boolean

        <ExtendedCategory("Policy")> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(3), Width(400)> _
        Public Property VaryByHeaders As String
            Get
                Return _VaryByHeaders
            End Get
            Set(value As String)
                _VaryByHeaders = value
                _VaryByHeadersList = Nothing
            End Set
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property VaryByHeadersList() As List(Of String)
            Get
                If _VaryByHeadersList Is Nothing Then
                    SyncLock _VaryByHeaders
                        If _VaryByHeadersList Is Nothing Then
                            _VaryByHeadersList = Common.ParseStringList(_VaryByHeaders)
                        End If
                    End SyncLock
                End If
                Return _VaryByHeadersList
            End Get
        End Property

        <ExtendedCategory("Policy")> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <LineCount(2), Width(400)>
        Public Property VaryByContentEncodings As String
            Get
                Return _VaryByContentEncodings
            End Get
            Set(value As String)
                _VaryByContentEncodings = value
            End Set
        End Property

        <ExtendedCategory("Policy")> _
        Public Property ClearCookies As Boolean = True

        <ExtendedCategory("Policy")> _
        Public Property AddArrOptOutHeaders As Boolean

        <ExtendedCategory("Policy")> _
        Public Property ToggleDynamicCompression As New EnabledFeature(Of Boolean)

        <ExtendedCategory("Policy")> _
        Public Property ToggleBandwidthThrottling As New EnabledFeature(Of ResponseThrottlerInfo)


        Private _VaryByContentEncodingsList As List(Of String)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property VaryByContentEncodingsList() As List(Of String)
            Get
                If _VaryByContentEncodingsList Is Nothing Then
                    SyncLock _VaryByContentEncodings
                        If _VaryByContentEncodingsList Is Nothing Then
                            _VaryByContentEncodingsList = _VaryByContentEncodings.ParseStringList(";"c)
                        End If
                    End SyncLock
                End If
                Return _VaryByContentEncodingsList
            End Get
        End Property


        <Browsable(False)> _
        Public ReadOnly Property VerbsList() As List(Of String)
            Get
                If _VerbsList Is Nothing Then
                    SyncLock _Verbs
                        If _VerbsList Is Nothing Then
                            _VerbsList = _Verbs.ParseStringList()
                        End If
                    End SyncLock
                End If
                Return _VerbsList
            End Get
        End Property

        <ExtendedCategory("Scope")> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(2), Width(400), LabelMode(LabelMode.Top)> _
        Public Property Verbs() As String
            Get
                Return _Verbs
            End Get
            Set(ByVal value As String)
                _Verbs = value
                _VerbsList = Nothing
            End Set
        End Property

        <ExtendedCategory("Scope")> _
        Public Property EmptyPathInfoOnly() As Boolean
            Get
                Return _EmptyPathInfoOnly
            End Get
            Set(ByVal value As Boolean)
                _EmptyPathInfoOnly = value
            End Set
        End Property

        <ExtendedCategory("Scope")> _
        Public Property EmptyQueryStringOnly() As Boolean
            Get
                Return _EmptyQueryStringOnly
            End Get
            Set(ByVal value As Boolean)
                _EmptyQueryStringOnly = value
            End Set
        End Property


        Public Function CalculateVaryByKey(ByVal request As HttpRequest) As String
            If Me.VaryByList.Count = 0 Then
                Return String.Empty
            End If
            If ((Me.VaryByList.Count = 1) AndAlso (Me.VaryByList(0) = "*")) Then
                Return request.QueryString.ToString
            End If
            Dim builder As New StringBuilder
            Dim i As Integer
            For i = 0 To Me.VaryByList.Count - 1
                builder.Append(request.QueryString.Item(Me.VaryByList(i)))
                builder.Append("-"c)
            Next i
            Return HttpUtility.UrlEncodeUnicode(builder.ToString)
        End Function


        Public Function MatchRequest(objContext As HttpContext) As Boolean
            Dim currentVerb As String = objContext.Request.HttpMethod.ToUpperInvariant()
            If VerbsList.Any(Function(objVerb) objVerb.ToUpperInvariant = currentVerb) Then
                If (Not EmptyQueryStringOnly) OrElse objContext.Request.Url.Query.IsNullOrEmpty() Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Sub SetCache(objContext As HttpContext)
            SetCache(objContext, Nothing)


        End Sub

        Public Sub SetCache(ByVal objContext As HttpContext, ByVal callBackInfo As ValidationCallBackInfo)
            If Me.Enabled Then
                Dim objResponse As HttpResponse = objContext.Response
                If Me.ClearCookies Then
                    objResponse.Cookies.Clear()
                End If
                objResponse.Cache.SetCacheability(Me.Cacheability)
                Dim timeStamp As DateTime = Now
                Dim expiration As DateTime = DateTime.MaxValue
                If Me.SetExpire Then
                    expiration = timeStamp.Add(Me.Duration)
                    objResponse.Cache.SetExpires(expiration)
                    If SetValidUntilExpires Then
                        objResponse.Cache.SetValidUntilExpires(True)
                    End If
                End If
                If Me.SetLastModified Then
                    objResponse.Cache.SetLastModified(timeStamp)
                End If
                If Me.SetEtag Then
                    objResponse.Cache.SetETag(Me.EtagExpression.Evaluate(DnnContext.Current(objContext)))
                End If

                If Me.VaryByStar Then
                    objResponse.Cache.VaryByParams("*") = True
                Else
                    For Each objVaryBy As String In Me.VaryByList
                        objResponse.Cache.VaryByParams(objVaryBy) = True
                    Next
                End If
                If VaryByUserLanguage Then
                    objResponse.Cache.VaryByHeaders.UserLanguage = True
                End If
                If VaryByUserAgent Then
                    objResponse.Cache.VaryByHeaders.UserAgent = True
                End If
                If VaryByUserCharSet Then
                    objResponse.Cache.VaryByHeaders.UserCharSet = True
                End If
                For Each objHeader As String In VaryByHeadersList
                    objResponse.Cache.VaryByHeaders(objHeader) = True
                Next

                If Me.VaryByBrowser Then
                    objResponse.Cache.SetVaryByCustom("browser")
                End If
                For Each strEncoding As String In VaryByContentEncodingsList
                    objResponse.Cache.VaryByContentEncodings.Item(strEncoding) = True
                Next
                If AddArrOptOutHeaders Then
                    objResponse.Headers.Add("Arr-Disable-Session-Affinity", "True")
                End If

                If callBackInfo Is Nothing Then
                    callBackInfo = New ValidationCallBackInfo(timeStamp, expiration <> DateTime.MaxValue, expiration)
                Else
                    callBackInfo.Timestamp = timeStamp
                    callBackInfo.IsExpiresSet = True
                    callBackInfo.Expiration = expiration

                End If
                callBackInfo.CachingStrategy = Me

                objResponse.Cache.AddValidationCallback(New HttpCacheValidateHandler(AddressOf ValidateCache), callBackInfo)

                If Me.ToggleDynamicCompression.Enabled Then
                    If Me.ToggleDynamicCompression.Entity Then
                        objContext.Request.ServerVariables("IIS_EnableDynamicCompression") = "1"
                    Else
                        objContext.Request.ServerVariables("IIS_EnableDynamicCompression") = "0"
                    End If
                End If

                If Me.ToggleBandwidthThrottling.Enabled Then
                    Me.ToggleBandwidthThrottling.Entity.ApplyBandwidthThrottling(objContext)
                End If
            End If


        End Sub



        Public Shared Sub ValidateCache(ByVal context As HttpContext, ByVal data As Object, ByRef status As HttpValidationStatus)
            Try

                If context.Request.IsAuthenticated Then
                    status = HttpValidationStatus.IgnoreThisRequest
                Else
                    Dim callBackInfo As ValidationCallBackInfo = DirectCast(data, ValidationCallBackInfo)
                    status = callBackInfo.ValidateCacheCallback(context)
                End If
            Catch ex As Exception
                status = HttpValidationStatus.IgnoreThisRequest
                ExceptionHelper.LogException(ex)
            End Try

        End Sub


        Private Class VaryByListAttributes
            Implements IAttributesProvider

            Public Function GetAttributes() As System.Collections.Generic.IEnumerable(Of System.Attribute) Implements IAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                toReturn.Add(New WidthAttribute(300))
                Return toReturn
            End Function
        End Class

    End Class
End Namespace