
Imports Aricie.ComponentModel
Imports Aricie.DNN.Configuration
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Controls
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins.Controls

Namespace Services.Errors

    ''' <summary>
    ''' Type of handling of custom errors
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum CustomErrorsType
        Legacy
        VirtualHandler
    End Enum

    ''' <summary>
    ''' Virtual custom error configuration
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class VirtualCustomErrorsInfo
        Inherits CustomErrorsInfo

        ''' <summary>
        ''' Type of handler for handler
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("MainSettings")> _
            <MainCategory()> _
        Public Property CustomErrorsType() As CustomErrorsType = Errors.CustomErrorsType.VirtualHandler

        ''' <summary>
        ''' Gets or sets whether to use ashx as error handler
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("HandlerSettings")> _
        Public Property UseAshx() As Boolean

        ''' <summary>
        ''' Gets or sets the virtual handler name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("HandlerSettings")> _
        <Required(True)> _
        Public Property VirtualHandlerName() As String = "Aricie.CustomErrors"

        ''' <summary>
        ''' Gets or sets the virtual handler path
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("HandlerSettings")> _
        <Required(True)> _
        Public Property VirtualHandlerPath() As String = "Error.aspx"

        ''' <summary>
        ''' Gets or sets the dynamic handler type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("HandlerSettings")> _
        <ConditionalVisible("UseAshx", True, True)> _
        Public Property DynamicHandlerType() As New DotNetType()

        ''' <summary>
        ''' gets or sets whether to use a random delay in response for security reasons
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("VirtualSettings")> _
        Public Property IncludeRandomDelay() As Boolean = true

        ''' <summary>
        ''' gets or sets whether to use random delays for same duplicate errors
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("VirtualSettings")> _
        Public Property DuplicateStatusMode() As MultipleCustomErrorsMode = MultipleCustomErrorsMode.RandomizedByStatus

        ''' <summary>
        ''' Gets or sets whether the status code must be preserved
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("VirtualSettings")> _
        Public Property PreserveStatusCode() As Boolean = true
            
        ''' <summary>
        ''' Returns legacy information about custom error management from current information
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToLegacy() As CustomErrorsInfo
            Dim toReturn As New CustomErrorsInfo
            toReturn.DefaultRedirect = New ControlUrlInfo(GetVirtualRedirect(), False)
            toReturn.Mode = Me.Mode
            toReturn.DefaultRedirect.RedirectMode = System.Web.Configuration.CustomErrorsRedirectMode.ResponseRewrite
            Return toReturn
        End Function

        ''' <summary>
        ''' returns the Virtual Handler Path
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetVirtualRedirect() As String
            Return Me.VirtualHandlerPath
        End Function

        ''' <summary>
        ''' Applies the currenct configuration to the web.config file
        ''' </summary>
        ''' <param name="pmb"></param>
        ''' <remarks></remarks>
        <ActionButton("~/images/fwd.gif")> _
        Public Sub Apply(pmb As AriciePortalModuleBase)
            Dim customErrorsUpdater As IUpdateProvider = Me.GetUpdateProvider
            Configuration.ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Install, customErrorsUpdater)
            Skin.AddModuleMessage(pmb, Localization.GetString("CustomErrorsSaved.Message", pmb.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        ''' <summary>
        ''' Returns an updater based on current Custom Errors configuration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUpdateProvider() As IUpdateProvider
            Return New VirtualCustomErrorUpdater(Me)
        End Function

    End Class


End Namespace
