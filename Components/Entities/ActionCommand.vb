Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ActionCommand
        Inherits RuleEngineSettings(Of ScheduleEvent)

        <ExtendedCategory("")> _
        Public Property IconPath() As String = "~/images/fwd.gif"

        <ExtendedCategory("")> _
        Public Property ResxKey() As String = ""


        <LineCount(10)> _
        <Width(500)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <ExtendedCategory("")> _
        Public Property ComputedVars() As String = String.Empty

    End Class
End NameSpace