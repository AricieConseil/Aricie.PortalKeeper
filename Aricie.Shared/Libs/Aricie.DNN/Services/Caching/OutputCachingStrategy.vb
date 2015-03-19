Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Web
Imports System.Text
Imports System.Globalization
Imports Aricie.Services

Namespace Services.Caching
    <Serializable()> _
    Public Class OutputCachingStrategy

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


        Public Property Enabled() As Boolean
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

        <Browsable(False)> _
        Public ReadOnly Property VerbsList() As List(Of String)
            Get
                If _VerbsList Is Nothing Then
                    SyncLock _Verbs
                        If _VerbsList Is Nothing Then
                            _VerbsList = Common.ParseStringList(_Verbs)
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

        <ExtendedCategory("Policy")> _
        Public Property Cacheability As HttpCacheability = HttpCacheability.Public


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
        Public Property VaryByStar As Boolean


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
        <ConditionalVisible("VaryByStar", True, True)> _
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
            <LineCount(3), Width(400), LabelMode(LabelMode.Top)> _
        Public Property VaryByHeaders As String
            Get
                Return _VaryByHeaders
            End Get
            Set(value As String)
                _VaryByHeaders = value
                _VaryByHeadersList = Nothing
            End Set
        End Property


        <Browsable(False)> _
        Public ReadOnly Property VaryByHeadersList() As List(Of String)
            Get
                If _VaryByList Is Nothing Then
                    SyncLock _VaryBy
                        If _VaryByHeadersList Is Nothing Then
                            _VaryByHeadersList = Common.ParseStringList(_VaryByHeaders)
                        End If
                    End SyncLock
                End If
                Return _VaryByHeadersList
            End Get
        End Property

        <ExtendedCategory("Policy")> _
        Public Property ClearCookies As Boolean = True

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
        Public Property AddArrOptOutHeaders As Boolean


        Private _VaryByContentEncodingsList As List(Of String)

        <Browsable(False)> _
        Public ReadOnly Property VaryByContentEncodingsList() As List(Of String)
            Get
                If _VaryByContentEncodingsList Is Nothing Then
                    SyncLock _VaryBy
                        If _VaryByContentEncodingsList Is Nothing Then
                            _VaryByContentEncodingsList = Common.ParseStringList(_VaryByContentEncodings, ";"c)
                        End If
                    End SyncLock
                End If
                Return _VaryByContentEncodingsList
            End Get
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
            Dim objResponse As HttpResponse = objContext.Response
            If Me.ClearCookies Then
                objResponse.Cookies.Clear()
            End If
            objResponse.Cache.SetCacheability(Me.Cacheability)
            Dim timeStamp As DateTime = Now
            Dim expiration As DateTime = timeStamp.Add(Me.Duration)
            objResponse.Cache.SetExpires(expiration)

            objResponse.Cache.SetValidUntilExpires(True)
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


            Dim callBackInfo As New ValidationCallBackInfo(timeStamp, True, expiration)

            objResponse.Cache.AddValidationCallback(New HttpCacheValidateHandler(AddressOf ValidateCache), callBackInfo)


        End Sub


        Public Shared Sub ValidateCache(ByVal context As HttpContext, ByVal data As Object, ByRef status As HttpValidationStatus)
            Try
              
                If context.Request.IsAuthenticated Then
                    status = HttpValidationStatus.IgnoreThisRequest
                Else
                    Dim callBackInfo As ValidationCallBackInfo = DirectCast(data, ValidationCallBackInfo)
                    If callBackInfo Is Nothing OrElse callBackInfo.ValidateCacheCallback(context) Then
                        status = HttpValidationStatus.Valid
                    Else
                        status = HttpValidationStatus.IgnoreThisRequest
                    End If
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


    Public Class ValidationCallBackInfo


        Private Shared _HeaderSeparators() As Char = New Char() {","c, " "c}

        Public Sub New(ByVal objTimestamp As DateTime, ByVal setExpiration As Boolean, ByVal objExpires As DateTime)
            Me.Timestamp = objTimestamp
            Me.Expiration = objExpires
            Me.IsExpiresSet = setExpiration
        End Sub

        Public Property Timestamp As DateTime
        Public Property IsExpiresSet As Boolean
        Public Property Expiration As DateTime
        'Public FriendlyUrl As String = ""



        Public Function ValidateCacheCallback(ByVal context As HttpContext) As Boolean
            Dim headerValues As String() = Nothing
            Dim cacheControlHeader As String = context.Request.Headers.Item("Cache-Control")
            If (cacheControlHeader IsNot Nothing) Then
                headerValues = cacheControlHeader.Split(_HeaderSeparators)
                Dim headerValue As String
                For num As Integer = 0 To headerValues.Length - 1
                    headerValue = headerValues(num)
                    If headerValue = "no-cache" OrElse headerValue = "no-store" Then
                        Return False
                    End If
                    If headerValue.StartsWith("max-age=") Then
                        Dim maxAgeSeconds As Integer
                        Try
                            maxAgeSeconds = Convert.ToInt32(headerValue.Substring(8), CultureInfo.InvariantCulture)
                        Catch
                            maxAgeSeconds = -1
                        End Try
                        If (maxAgeSeconds >= 0) Then
                            Dim currentAgeSeconds As Integer = CInt(((context.Timestamp.Ticks - Me.Timestamp.Ticks) \ 10000000))
                            If (currentAgeSeconds >= maxAgeSeconds) Then
                                Return False
                            End If
                        End If
                    ElseIf headerValue.StartsWith("min-fresh=") Then
                        Dim num5 As Integer
                        Try
                            num5 = Convert.ToInt32(headerValue.Substring(10), CultureInfo.InvariantCulture)
                        Catch
                            num5 = -1
                        End Try
                        If (num5 >= 0 AndAlso Me.IsExpiresSet) Then
                            Dim num7 As Integer = CInt((Me.Expiration.Ticks - context.Timestamp.Ticks) \ 10000000)
                            If (num7 < num5) Then
                                Return False
                            End If
                        End If
                    End If
                Next
            End If
            Dim pragmaHeader As String = context.Request.Headers.Item("Pragma")
            If (pragmaHeader IsNot Nothing) Then
                headerValues = pragmaHeader.Split(_HeaderSeparators)
                For num As Integer = 0 To headerValues.Length - 1
                    If (headerValues(num) = "no-cache") Then
                        Return False
                    End If
                Next
            End If
            Return True

        End Function


    End Class

End Namespace