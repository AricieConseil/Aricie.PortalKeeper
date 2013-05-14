Imports System.Web.Configuration
Imports Aricie.DNN.Entities
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Configuration
Imports DotNetNuke.UI.WebControls
Imports System.Xml
Imports System.Web
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Errors
    ''' <summary>
    ''' Configuration for custom errors management
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    <XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlInclude(GetType(CustomErrorAddInfo))> _
    Public Class CustomErrorsInfo
        Inherits XmlConfigElementInfo

        Private _RedirectMode As CustomErrorsRedirectMode = CustomErrorsRedirectMode.ResponseRewrite


        Public Sub New()
        End Sub

        ''' <summary>
        ''' Gets or sets custom error mode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("MainSettings")> _
            <MainCategory()> _
        Public Property Mode() As CustomErrorsMode = CustomErrorsMode.On

        ''' <summary>
        ''' Gets or sets the default redirection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("MainSettings")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        Public Property DefaultRedirect() As New ControlUrlInfo

        ''' <summary>
        ''' Gets or sets custom errors
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("CustomErrors")> _
            <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(False, False, True, True, 10)> _
            <LabelMode(LabelMode.Top)> _
        Public Property CustomErrors() As New List(Of CustomErrorInfo)

        ''' <summary>
        ''' Returns whether the custom errors are active for the current request
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CustomErrorsEnabled(ByVal request As HttpRequest) As Boolean
            Select Case Me.Mode
                Case CustomErrorsMode.RemoteOnly
                    Return Not request.IsLocal
                Case CustomErrorsMode.On
                    Return True
                Case CustomErrorsMode.Off
                    Return False
            End Select
            Return False
        End Function

        ''' <summary>
        ''' Returns whether the custom errors are active in the current web.config
        ''' </summary>
        ''' <param name="xmlConfig"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean

            Dim xPath As String = NukeHelper.DefineWebServerElementPath("system.web") & "/customErrors[@mode='" & Me.Mode.ToString & "'"
            'If System.Environment.Version.Major > 2 Then
            xPath &= " and @redirectMode='" & Me.DefaultRedirect.RedirectMode.ToString & "'"
            'End If
            If Not String.IsNullOrEmpty(Me.DefaultRedirect.Url) Then
                xPath &= " and @defaultRedirect='" & Me.DefaultRedirect.UrlPath & "'"
            End If
            xPath &= "]"
            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode(xPath)
            If (Not moduleNode Is Nothing) Then
                Return Me.CustomErrors.TrueForAll(Function(objError As CustomErrorInfo) objError.IsInstalled)
            End If
            Return False
        End Function

        ''' <summary>
        ''' Sets up custom errors in the web.config file
        ''' </summary>
        ''' <param name="targetNodes"></param>
        ''' <param name="actionType"></param>
        ''' <remarks></remarks>
        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install
                    Dim node As New StandardComplexNodeInfo((NukeHelper.DefineWebServerElementPath("system.web")), NodeInfo.NodeAction.update, StandardComplexNodeInfo.NodeCollision.save, NukeHelper.DefineWebServerElementPath("system.web") & "/customErrors")
                    node.Children.Add(New CustomErrorsAddInfo(Me))
                    targetNodes.Nodes.Add(node)
                    If Me.CustomErrors.Count > 0 Then
                        Dim nodeCustom As New StandardComplexNodeInfo((NukeHelper.DefineWebServerElementPath("system.web") & "/customErrors"), NodeInfo.NodeAction.update, "status", StandardComplexNodeInfo.NodeCollision.save)
                        For Each objCustomError As CustomErrorInfo In Me.CustomErrors
                            nodeCustom.Children.Add(New CustomErrorAddInfo(objCustomError))
                        Next
                        targetNodes.Nodes.Add(nodeCustom)
                    End If
                Case ConfigActionType.Uninstall
                    Dim node As New StandardComplexNodeInfo((NukeHelper.DefineWebServerElementPath("system.web")), NodeInfo.NodeAction.update, StandardComplexNodeInfo.NodeCollision.save, NukeHelper.DefineWebServerElementPath("system.web") & "/customErrors")
                    node.Children.Add(New CustomErrorsAddInfo(New CustomErrorsInfo))
                    targetNodes.Nodes.Add(node)
            End Select

        End Sub
    End Class
End Namespace
