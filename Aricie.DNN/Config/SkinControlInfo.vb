Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security

Namespace Configuration

    ''' <summary>
    ''' Configuration element for a Skin Control component
    ''' </summary>
    ''' <remarks>Contains DB properties for DNN declaration</remarks>
    <XmlRoot("moduleControl")> _
    Public Class SkinControlConfigInfo
        Implements IConfigElementInfo


        Private _ControlKey As String = ""
        Private _ControlSrc As String = ""
        Private _SupportsPartialRendering As Boolean


        Public Sub New()

        End Sub


        Public Sub New(ByVal controlKey As String, ByVal controlSrc As String, ByVal supportsPartialRendering As Boolean)
            Me._ControlKey = controlKey
            Me._ControlSrc = controlSrc
            Me._SupportsPartialRendering = supportsPartialRendering
        End Sub



        <XmlElement("controlKey")> _
        Public Property ControlKey() As String
            Get
                Return _ControlKey
            End Get
            Set(ByVal value As String)
                _ControlKey = value
            End Set
        End Property

        <XmlElement("controlSrc")> _
        Public Property ControlSrc() As String
            Get
                Return _ControlSrc
            End Get
            Set(ByVal value As String)
                _ControlSrc = value
            End Set
        End Property

        <XmlElement("supportsPartialRendering")> _
        Public Property SupportsPartialRendering() As Boolean
            Get
                Return _SupportsPartialRendering
            End Get
            Set(ByVal value As Boolean)
                _SupportsPartialRendering = value
            End Set
        End Property



        Public Function IsInstalled() As Boolean Implements IConfigElementInfo.IsInstalled
            Dim moduleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByKeyAndSrc(-1, Me.ControlKey, Me.ControlSrc)
            Return moduleControl IsNot Nothing AndAlso moduleControl.ModuleControlID <> -1
        End Function

        Public Sub ProcessConfig(ByVal actionType As ConfigActionType) Implements IConfigElementInfo.ProcessConfig
            Select Case actionType
                Case ConfigActionType.Install

                    If ModuleControlController.GetModuleControlByKeyAndSrc(-1, Me.ControlKey, Me.ControlSrc) Is Nothing Then
                        Dim moduleControl As New ModuleControlInfo()
                        moduleControl.ControlKey = Me.ControlKey
                        moduleControl.ControlTitle = "SKIN"
                        moduleControl.ControlType = SecurityAccessLevel.SkinObject
                        moduleControl.ControlSrc = Me.ControlSrc
                        moduleControl.SupportsPartialRendering = Me.SupportsPartialRendering
                        moduleControl.ModuleDefID = -1
                        ModuleControlController.AddModuleControl(moduleControl)
                    End If
                Case ConfigActionType.Uninstall
                    Dim moduleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByKeyAndSrc(-1, Me.ControlKey, Me.ControlSrc)
                    If moduleControl IsNot Nothing AndAlso moduleControl.ModuleControlID <> -1 Then
                        ModuleControlController.DeleteModuleControl(moduleControl.ModuleControlID)
                    End If
            End Select
        End Sub


    End Class



End Namespace


