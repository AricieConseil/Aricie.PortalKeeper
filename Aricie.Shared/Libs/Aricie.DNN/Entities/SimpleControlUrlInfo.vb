Imports System.Web.Configuration
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports System.ComponentModel

Namespace Entities

    ''' <summary>
    ''' Simpler inherited class without Redirection mode display
    ''' </summary>
    ''' <remarks></remarks>
    <ActionButton(IconName.Link, IconOptions.Normal)> _
    Public Class SimpleControlUrlInfo
        Inherits ControlUrlInfo


        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(objMode As UrlControlMode)
            MyBase.New(objMode)
        End Sub

        Public Sub New(ByVal url As String, ByVal track As Boolean)
            MyBase.New(url, track)
        End Sub


        Private _RedirectMode As CustomErrorsRedirectMode = CustomErrorsRedirectMode.ResponseRedirect

        <Browsable(False)> _
        Public Overrides Property RedirectMode() As CustomErrorsRedirectMode
            Get
                Return _RedirectMode
            End Get
            Set(ByVal value As CustomErrorsRedirectMode)
                _RedirectMode = value
            End Set
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property UrlType As DotNetNuke.Entities.Tabs.TabType
            Get
                Return MyBase.UrlType
            End Get
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property UrlPath As String
            Get
                Return MyBase.UrlPath
            End Get
        End Property

        

    End Class
End Namespace