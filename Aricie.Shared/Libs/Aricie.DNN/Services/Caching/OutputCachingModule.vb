Imports System
Imports System.Web

Namespace Services.Caching
    Public MustInherit Class OutputCachingModule
        Implements IHttpModule

        Private Shared _Settings As OutputCachingSettings
        ' Fields
        'Private _app As HttpApplication
        Private _trackerCapturingResponse As OutputTrackerBase

        Protected MustOverride Function GetSettings() As OutputCachingSettings

        Public ReadOnly Property Settings() As OutputCachingSettings
            Get
                If _Settings Is Nothing Then
                    _Settings = Me.GetSettings
                End If
                Return _Settings
            End Get
        End Property

        Private Sub Init(ByVal app As HttpApplication) Implements IHttpModule.Init
            'Me._app = app
            OutputCacheManager.EnsureInitialized()
            AddHandler app.ResolveRequestCache, New EventHandler(AddressOf Me.OnResolveRequestCache)
            AddHandler app.UpdateRequestCache, New EventHandler(AddressOf Me.OnUpdateRequestCache)
            AddHandler app.EndRequest, New EventHandler(AddressOf Me.OnEndRequest)
        End Sub

        Private Sub Dispose() Implements IHttpModule.Dispose
        End Sub

     

        ' Methods
        Private Sub OnResolveRequestCache(ByVal sender As Object, ByVal e As EventArgs)
            Dim application As HttpApplication = DirectCast(sender, HttpApplication)
            Me._trackerCapturingResponse = Nothing
            Try
                If Me.IsEnabled(application) Then
                    Dim tracker As OutputTrackerBase = OutputCacheManager.Lookup(application.Context)
                    If (Not tracker Is Nothing) Then

                        If tracker.TrySendResponseOrStartResponseCapture(application.Response) Then
                            application.CompleteRequest()
                        Else
                            Me._trackerCapturingResponse = tracker
                        End If

                    End If
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End Try
        End Sub

        Private Sub OnUpdateRequestCache(ByVal sender As Object, ByVal e As EventArgs)
            Dim application As HttpApplication = DirectCast(sender, HttpApplication)
            Try
                If (Not Me._trackerCapturingResponse Is Nothing) Then
                    Me._trackerCapturingResponse.FinishCaptureAndSaveResponse(application.Response)
                    Me._trackerCapturingResponse = Nothing
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End Try
        End Sub

        Private Sub OnEndRequest(ByVal sender As Object, ByVal e As EventArgs)
            Dim application As HttpApplication = DirectCast(sender, HttpApplication)
            If (Not Me._trackerCapturingResponse Is Nothing) Then
                Try
                    Me._trackerCapturingResponse.CancelCapture(application.Response)
                Catch ex As Exception
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                Finally
                    Me._trackerCapturingResponse = Nothing
                End Try
            End If
        End Sub

        Private Function IsEnabled(ByVal app As HttpApplication) As Boolean
            Return Me.Settings.Enabled _
                   AndAlso app IsNot Nothing _
                   AndAlso app.Context IsNot Nothing _
                   AndAlso app.Context.Request IsNot Nothing _
                   AndAlso Not app.Context.Request.IsAuthenticated _
                   AndAlso app.Context.Request.HttpMethod = "GET"

        End Function

    End Class
End Namespace

