Imports System.ComponentModel
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Framework
Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Web.UI

Namespace Aricie.DNN.Modules.PortalKeeper



    <ActionButton(IconName.Comment, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Display Message Action")> _
        <Description("Displays a DNN message with token replace on the current page. A target module can be optionnally chosen.")> _
    Public Class DisplayMessageAction
        Inherits MessageBasedAction(Of RequestEvent)


        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal enableTokenReplace As Boolean)
            MyBase.New(enableTokenReplace)
        End Sub

        Private _ModuleMessageType As ModuleMessage.ModuleMessageType = ModuleMessage.ModuleMessageType.YellowWarning
        Private _Heading As String = ""
        Private _IsModuleMessage As Boolean
        Private _ModuleId As Integer

        <ExtendedCategory("Specifics")> _
        Public Property ModuleMessageType() As ModuleMessage.ModuleMessageType
            Get
                Return _ModuleMessageType
            End Get
            Set(ByVal value As ModuleMessage.ModuleMessageType)
                _ModuleMessageType = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <Width(400)> _
        Public Property Heading() As String
            Get
                Return _Heading
            End Get
            Set(ByVal value As String)
                _Heading = value
            End Set
        End Property



        <ExtendedCategory("Specifics")> _
        Public Property IsModuleMessage() As Boolean
            Get
                Return _IsModuleMessage
            End Get
            Set(ByVal value As Boolean)
                _IsModuleMessage = value
            End Set
        End Property


        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("IsModuleMessage", False, True)> _
        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal value As Integer)
                _ModuleId = value
            End Set
        End Property



        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of RequestEvent), ByVal aSync As Boolean) As Boolean
            'Dim objLog As New LogInfo()
            If Not aSync Then
                Dim dnnPage As CDefault = actionContext.DnnContext.DnnPage
                If dnnPage IsNot Nothing Then
                    Dim message As String = GetMessage(actionContext)
                    If Me._ModuleId > 0 Then
                        Dim objModules As List(Of PortalModuleBase) = FindControlsRecursive(Of PortalModuleBase)(dnnPage)
                        For Each objModule As PortalModuleBase In objModules
                            If objModule.ModuleId = Me._ModuleId Then
                                Skin.AddModuleMessage(objModule, Me._Heading, message, Me._ModuleMessageType)
                                Return True
                            End If
                        Next
                    Else
                        Dim objSkins As List(Of Skin) = FindControlsRecursive(Of Skin)(dnnPage)
                        If objSkins.Count > 0 Then
                            Dim objSkin As Skin = objSkins(0)
                            Skin.AddPageMessage(objSkin, Me._Heading, message, Me._ModuleMessageType)
                            Return True
                        End If

                    End If
                End If
            End If
            Return False
        End Function
    End Class
End Namespace