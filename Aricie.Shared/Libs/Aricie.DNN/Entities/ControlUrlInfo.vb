Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports System.Web
Imports System.Web.Configuration
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Collections.Specialized
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Tabs
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Entities.Portals

Namespace Entities
    <Flags()> _
    Public Enum UrlControlMode
        Empty = 0
        None = 1
        Url = 2
        Tab = 4
        File = 8
        Secure = 16
        Database = 32
        Upload = 64
        Member = 128
        Track = 256
        Log = 512 + 256
        NewWindow = 1024
        Normal = 1024 + 512 + 256 + 256 + 128 + 64 + 32 + 16 + 8 + 4 + 2 + 1
    End Enum


    ''' <summary>
    ''' Entity class for a Link
    ''' </summary>
    <Serializable()> _
    Public Class ControlUrlInfo

        Private _Url As String = ""

        Private _Track As Boolean

        Private _UrlPath As String = ""

        Private _RedirectMode As CustomErrorsRedirectMode = CustomErrorsRedirectMode.ResponseRedirect

        Private _PortalAlias As String = ""

        Public Sub New()

        End Sub

        Public Sub New(objMode As UrlControlMode)
            Me.FilterMode = objMode
        End Sub


        Public Sub New(ByVal url As String, ByVal track As Boolean)
            Me._Url = url
            Me.Track = track
            Me._RedirectMode = CustomErrorsRedirectMode.ResponseRedirect
        End Sub


        Public Overridable ReadOnly Property UrlType() As DotNetNuke.Entities.Tabs.TabType
            Get
                Return DotNetNuke.Common.Globals.GetURLType(Me._Url)
            End Get
        End Property

        <Browsable(False)> _
        Public Overridable Property FilterMode As UrlControlMode = UrlControlMode.Normal


        <Editor(GetType(AricieUrlEditControl), GetType(EditControl))> _
        Public Overridable Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal value As String)
                If _Url <> value Then
                    _UrlPath = ""
                End If
                _Url = value
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property ShowTrack As Boolean
            Get
                Return ((Me.FilterMode And UrlControlMode.Track) = UrlControlMode.Track)
            End Get
        End Property




        <ConditionalVisible("ShowTrack", False, True)> _
        Public Overridable Property Track() As Boolean
            Get
                Return _Track
            End Get
            Set(ByVal value As Boolean)
                If _Track <> value Then
                    _UrlPath = ""
                End If
                _Track = value
            End Set
        End Property

        <ConditionalVisible("UrlType", False, True, TabType.Tab)> _
        Public Property DefinePortalAlias As Boolean

        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector(GetType(PortalAliasSelector), "HTTPAlias", "HTTPAlias", False, False, "", "", False, False)> _
        <ConditionalVisible("UrlType", False, True, TabType.Tab)> _
        <ConditionalVisible("DefinePortalAlias", False, True)>
        Public Property PortalAlias As String
            Get
                Return _PortalAlias
            End Get
            Set(value As String)
                If _PortalAlias <> value Then
                    _UrlPath = ""
                    _PortalAlias = value
                End If
            End Set
        End Property

        <IsReadOnly(True)> _
        Public Overridable ReadOnly Property UrlPath() As String
            Get
                If String.IsNullOrEmpty(_UrlPath) Then
                    _UrlPath = Aricie.DNN.Services.NukeHelper.GetPathFromCtrUrl(NukeHelper.PortalId, Me._Url, Me._Track)
                    If DefinePortalAlias AndAlso Not PortalAlias.IsNullOrEmpty() AndAlso Not _UrlPath.IsNullOrEmpty() Then
                        Dim targetPA As Uri = New Uri(DotNetNuke.Common.Globals.AddHTTP(PortalAlias))
                        _UrlPath = New Uri(targetPA, _UrlPath).ToString()
                    End If
                End If
                Return Me._UrlPath
            End Get
            'Set(ByVal value As String)
            '    Me._UrlPath = value
            'End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property FileInfo As DotNetNuke.Services.FileSystem.FileInfo
            Get
                Dim toReturn As FileInfo = Nothing
                If Me.UrlType = DotNetNuke.Entities.Tabs.TabType.File Then
                    toReturn = NukeHelper.GetFileInfoFromCtrUrl(NukeHelper.PortalId, Me._Url)
                End If
                Return toReturn
            End Get
        End Property



        Public Overridable Property RedirectMode() As CustomErrorsRedirectMode
            Get
                Return _RedirectMode
            End Get
            Set(ByVal value As CustomErrorsRedirectMode)
                _RedirectMode = value
            End Set
        End Property

        ''' <summary>
        '''  Redirect to Url
        ''' </summary>
        ''' <param name="context">HTTP Context</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Redirect(ByVal context As HttpContext) As Boolean
            Return Redirect(context, Nothing, Nothing)
        End Function

        ''' <summary>
        '''  Redirect to Url
        ''' </summary>
        ''' <param name="context">HTTP Context</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Redirect(ByVal context As HttpContext, queryUpdates As NameValueCollection, queryRemoves As IEnumerable(Of String)) As Boolean
            Return Redirect(context, 0, "", "", queryUpdates, queryRemoves)
        End Function

        ''' <summary>
        ''' Redirect to Url when there is an error
        ''' </summary>
        ''' <param name="context">HTTP Context</param>
        ''' <param name="targetStatus">Page status</param>
        ''' <param name="queryErrorsParam">Name of the error params to avoid infinite loop</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Redirect(ByVal context As HttpContext, ByVal targetStatus As Integer, ByVal queryErrorsParam As String, ByVal queryErrorsValue As String, _
                                 queryUpdates As NameValueCollection, queryRemoves As IEnumerable(Of String)) As Boolean
            Try

                If String.IsNullOrEmpty(Me._Url) Then
                    Return False
                End If
                Dim target As String = Me.UrlPath
                If (queryUpdates IsNot Nothing AndAlso queryUpdates.Count > 0) OrElse (queryRemoves IsNot Nothing AndAlso queryRemoves.Count > 0) Then
                    Dim targetUri As New Uri(target)
                    target = targetUri.ModifyQueryString(queryUpdates, queryRemoves)
                End If
                If (RedirectMode = CustomErrorsRedirectMode.ResponseRewrite) Then
                    context.Response.Clear()
                    If Me.UrlType = DotNetNuke.Entities.Tabs.TabType.Tab Then
                        Dim activeTab As DotNetNuke.Entities.Tabs.TabInfo = NukeHelper.PortalSettings.ActiveTab
                        If activeTab IsNot Nothing Then
                            Dim targetTabId As Integer = Integer.Parse(Me.Url)
                            If targetTabId <> activeTab.TabID Then
                                NukeHelper.RenewPortalSettings(targetTabId, context)
                            End If
                        End If
                    End If

                    context.Server.Execute(target)
                    If targetStatus <> 0 Then
                        context.Response.StatusCode = targetStatus
                    End If
                    context.ApplicationInstance.CompleteRequest()
                Else
                    'Dim targetUrl As String = Me.UrlPath
                    If Not String.IsNullOrEmpty(queryErrorsParam) Then
                        If (context.Request.QueryString.Item(queryErrorsParam) IsNot Nothing) Then
                            Return False
                        End If
                        If Not String.IsNullOrEmpty(queryErrorsValue) Then
                            Dim sepChar As Char = "?"c
                            If (Me.UrlPath.IndexOf("?"c) > 0) Then
                                sepChar = "&"c
                            End If
                            target = (target & sepChar & queryErrorsParam & "=" & queryErrorsValue)
                        End If
                    End If

                    context.Response.Redirect(target, False)
                End If
            Catch ex As Exception
                Aricie.Services.ExceptionHelper.LogException(ex)
                Return False
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Redirect to Url when there is an error
        ''' </summary>
        ''' <param name="context">HTTP Context</param>
        ''' <param name="targetStatus">Page status</param>
        ''' <param name="queryErrorsParam">Name of the error params to avoid infinite loop</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Redirect(ByVal context As HttpContext, ByVal targetStatus As Integer, ByVal queryErrorsParam As String, ByVal queryErrorsValue As String) As Boolean
            Return Redirect(context, targetStatus, queryErrorsParam, queryErrorsValue, Nothing, Nothing)
        End Function

    End Class
End Namespace