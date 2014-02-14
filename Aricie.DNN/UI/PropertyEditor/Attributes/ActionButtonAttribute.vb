Imports Aricie.DNN.UI.WebControls

Namespace UI.Attributes

    Public Enum ActionButtonMode
        CommandButton
        IconButton
    End Enum

    <AttributeUsage(AttributeTargets.Method)> _
    Public Class ActionButtonAttribute
        Inherits Attribute

        Public Sub New()

        End Sub

        Public Sub New(strIconPath As String)
            Me.New(strIconPath, "")
        End Sub

        Public Sub New(strIconPath As String, strAlertKey As String)
            Me.IconPath = strIconPath
            Me.AlertKey = strAlertKey
        End Sub

        Public Sub New(enumIconName As IconName)
            Me.New(enumIconName, "")
        End Sub

        Public Sub New(enumIconName As IconName, strAlertKey As String)
            Me.Mode = ActionButtonMode.IconButton
            Me.IconAction.IconName = enumIconName
            Me.AlertKey = strAlertKey
        End Sub

        Public Sub New(enumIconName As IconName, stackIconName As IconName)
            Me.New(enumIconName, stackIconName, "")
        End Sub

        Public Sub New(enumIconName As IconName, stackIconName As IconName, strAlertKey As String)
            Me.Mode = ActionButtonMode.IconButton
            Me.IconAction.IconName = enumIconName
            Me.IconAction.StackedIconName = stackIconName
            Me.AlertKey = strAlertKey
        End Sub


        Public Property Mode As ActionButtonMode = ActionButtonMode.CommandButton

        Public Property IconAction As New IconActionInfo

        Public Property IconPath As String = ""

        Public Property AlertKey As String = ""

    End Class


End Namespace