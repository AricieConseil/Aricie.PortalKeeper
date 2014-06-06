
Imports System.Web.Configuration
Imports Aricie.DNN.Entities
Imports System.Web
Imports Aricie.Security.Cryptography
Imports DotNetNuke.Entities.Portals

Namespace Services.Errors

    ''' <summary>
    ''' Error manager as a page
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class CustomErrorsHandler
        Inherits System.Web.UI.Page

        Protected Random As New Random

        ''' <summary>
        ''' This page can be reused for further error management
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads ReadOnly Property IsReusable() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Returns custom errors information
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetCustomErrors() As VirtualCustomErrorsInfo

        ''' <summary>
        ''' Handles request
        ''' </summary>
        ''' <param name="context"></param>
        ''' <remarks></remarks>
        Public Overrides Sub ProcessRequest(ByVal context As System.Web.HttpContext)

            Try
                Dim objCustomErrors As VirtualCustomErrorsInfo = GetCustomErrors()
                Me.ProcessRequest(context, objCustomErrors)

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End Try

        End Sub

        ''' <summary>
        ''' Process the error handling request
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="objCustomErrors"></param>
        ''' <remarks></remarks>
        Protected Overridable Overloads Sub ProcessRequest(ByVal context As HttpContext, ByVal objCustomErrors As VirtualCustomErrorsInfo)
            If objCustomErrors.IncludeRandomDelay Then
                CryptoHelper.AddRandomDelay()
            End If
            Dim targetStatus As Integer = 0
            If context.Error IsNot Nothing AndAlso TypeOf context.Error Is HttpException Then
                targetStatus = DirectCast(context.Error, HttpException).GetHttpCode
            End If
            Dim target As ControlUrlInfo = GetTargetUrl(context, targetStatus, objCustomErrors)
            If Not objCustomErrors.PreserveStatusCode Then
                targetStatus = 0
            End If
            If Not target.Redirect(context, targetStatus, "aspxerrorpath", HttpUtility.UrlEncode(context.Request.Path)) Then
                context.Response.Redirect(DotNetNuke.Common.Globals.ApplicationURL)
            End If
        End Sub

        ''' <summary>
        ''' Gets redirection url matching the error code
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="targetStatus"></param>
        ''' <param name="objCustomErrors"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function GetTargetUrl(ByVal context As HttpContext, ByVal targetStatus As Integer, ByVal objCustomErrors As VirtualCustomErrorsInfo) As ControlUrlInfo
            Dim toReturn As ControlUrlInfo = Nothing
            Select Case objCustomErrors.DuplicateStatusMode
                Case MultipleCustomErrorsMode.DefaultOnly
                    toReturn = objCustomErrors.DefaultRedirect
                Case MultipleCustomErrorsMode.AllRandomized, MultipleCustomErrorsMode.RandomizedByStatus
                    Dim all As Boolean = objCustomErrors.DuplicateStatusMode = MultipleCustomErrorsMode.AllRandomized
                    If targetStatus <> 0 Then
                        Dim targets As New List(Of ControlUrlInfo)
                        For Each objCustomError As CustomErrorInfo In objCustomErrors.CustomErrors
                            If all OrElse objCustomError.StatusCode = targetStatus Then
                                targets.Add(objCustomError.Redirect)
                            End If
                        Next
                        If all OrElse targets.Count = 0 Then
                            targets.Add(objCustomErrors.DefaultRedirect)
                        End If
                        toReturn = targets(Random.Next(0, targets.Count))
                    Else
                        toReturn = objCustomErrors.DefaultRedirect
                    End If
            End Select
            Return toReturn
        End Function


    End Class
End Namespace


