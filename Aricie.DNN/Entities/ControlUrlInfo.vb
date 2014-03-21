Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports System.Web
Imports System.Web.Configuration
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls

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
        Normal = 512 + 256 + 256 + 128 + 64 + 32 + 16 + 8 + 4 + 2 + 1
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
        Public Property FilterMode As UrlControlMode


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
        Private ReadOnly Property ShowTrack As Boolean
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

        <IsReadOnly(True)> _
        Public ReadOnly Property UrlPath() As String
            Get
                If String.IsNullOrEmpty(_UrlPath) Then
                    _UrlPath = Aricie.DNN.Services.NukeHelper.GetPathFromCtrUrl(NukeHelper.PortalId, Me._Url, Me._Track)
                End If
                Return Me._UrlPath
            End Get
            'Set(ByVal value As String)
            '    Me._UrlPath = value
            'End Set
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
            Return Redirect(context, 0, "", "")
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
            Try

                If String.IsNullOrEmpty(Me._Url) Then
                    Return False
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

                    context.Server.Execute(Me.UrlPath)
                    If targetStatus <> 0 Then
                        context.Response.StatusCode = targetStatus
                    End If
                    context.ApplicationInstance.CompleteRequest()
                Else
                    Dim targetUrl As String = Me.UrlPath
                    If Not String.IsNullOrEmpty(queryErrorsParam) Then
                        If (Not context.Request.QueryString.Item(queryErrorsParam) Is Nothing) Then
                            Return False
                        End If
                        If Not String.IsNullOrEmpty(queryErrorsValue) Then
                            Dim sepChar As Char = "?"c
                            If (Me.UrlPath.IndexOf("?"c) > 0) Then
                                sepChar = "&"c
                            End If
                            targetUrl = (targetUrl & sepChar & queryErrorsParam & "=" & queryErrorsValue)
                        End If
                    End If

                    context.Response.Redirect(targetUrl, False)
                End If
            Catch ex As Exception
                Aricie.Services.ExceptionHelper.LogException(ex)
                Return False
            End Try
            Return True
        End Function

    End Class
End Namespace