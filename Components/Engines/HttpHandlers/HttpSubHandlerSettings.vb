Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class HttpSubHandlerSettings
        Inherits HttpHandlerSettings


        <Browsable(False)> _
        Public Overrides ReadOnly Property FriendlyName As String
            Get
                Return MyBase.FriendlyName.Replace("unregistered", "").Replace("registered", "")
            End Get
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property Installed As Boolean
            Get
                Return True
            End Get
        End Property

        Public Property RunMainHandler As Boolean

        <ConditionalVisible("RunMainHandler", True, True)> _
        Public Property InitParamsToSubHandler As Boolean = True

        <Browsable(False)> _
        Public Overrides Sub AddTestFiddle(pe As AriciePropertyEditorControl)
            MyBase.AddTestFiddle(pe)
        End Sub

        <Browsable(False)> _
        Public Overrides Sub Install(pe As AriciePropertyEditorControl)
            MyBase.Install(pe)
        End Sub

        <Browsable(False)> _
        Public Overrides Sub Uninstall(pe As AriciePropertyEditorControl)
            MyBase.Uninstall(pe)
        End Sub

        <Browsable(False)> _
        Public Overrides Sub Update(pe As AriciePropertyEditorControl)
            MyBase.Update(pe)
        End Sub

    End Class
End NameSpace