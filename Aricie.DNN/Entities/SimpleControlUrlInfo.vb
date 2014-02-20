Imports System.Web.Configuration
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Entities

    ''' <summary>
    ''' Simpler inherited class without Redirection mode display
    ''' </summary>
    ''' <remarks></remarks>
    <ActionButton(IconName.Link, IconOptions.Normal)> _
    <Serializable()> _
    Public Class SimpleControlUrlInfo
        Inherits ControlUrlInfo

        Private _RedirectMode As CustomErrorsRedirectMode = CustomErrorsRedirectMode.ResponseRedirect

        <System.ComponentModel.Browsable(False)> _
        Public Overrides Property RedirectMode() As CustomErrorsRedirectMode
            Get
                Return _RedirectMode
            End Get
            Set(ByVal value As CustomErrorsRedirectMode)
                _RedirectMode = value
            End Set
        End Property

    End Class
End Namespace