Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services

Namespace Entities
    <Serializable()> _
    Public Class DnnPortalAlias

        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector(GetType(PortalAliasSelector), "HTTPAlias", "PortalAliasID", False, False, "None", "-1", False, False)> _
        Public Property PortalAliasId As Integer = -1

        <Browsable(False)> _
        Public ReadOnly Property PortalAlias As String
            Get
                If PortalAliasId <> -1 Then
                    Return NukeHelper.PortalAliasByPortalAliasId(PortalAliasId).HTTPAlias
                End If
                Return ""
            End Get
        End Property

        Public Function GetUrl() As String
            If Not PortalAlias.IsNullOrEmpty() Then
                Return DotNetNuke.Common.Globals.AddHTTP(PortalAlias)
            End If
            Return ""
        End Function

    End Class
End NameSpace