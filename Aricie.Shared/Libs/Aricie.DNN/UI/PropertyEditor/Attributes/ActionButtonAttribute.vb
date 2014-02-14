Imports Aricie.DNN.UI.WebControls

Namespace UI.Attributes

    Public Enum ActionButtonMode
        CommandButton
        IconButton
    End Enum

    <AttributeUsage(AttributeTargets.Method)> _
    Public Class ActionButtonAttribute
        Inherits Attribute

        Public Property Mode As ActionButtonMode = ActionButtonMode.CommandButton

        Public Property IconAction As New IconActionInfo

        Public Property IconPath As String = ""

        Public Property AlertKey As String = ""

        Public Sub New()

        End Sub

        Public Sub New(strIconPath As String)
            Me.New(strIconPath, "")
        End Sub

        Public Sub New(strIconPath As String, strAlertKey As String)
            Me.IconPath = strIconPath
            Me.AlertKey = strAlertKey
        End Sub

        Public Sub New(mainIconName As IconName, mainIconOptions As IconOptions)
            Me.New(mainIconName, mainIconOptions, IconName.None, IconOptions.Normal, IconOptions.Normal, "")
        End Sub

        Public Sub New(mainIconName As IconName, mainIconOptions As IconOptions, strAlertKey As String)
            Me.New(mainIconName, mainIconOptions, IconName.None, IconOptions.Normal, IconOptions.Normal, strAlertKey)
        End Sub

        Public Sub New(mainIconName As IconName, mainIconOptions As IconOptions, stackedIconName As IconName, stackedIconOptions As IconOptions, stackContainerOptions As IconOptions)
            Me.New(mainIconName, mainIconOptions, stackedIconName, stackedIconOptions, stackContainerOptions, "")
        End Sub

        Public Sub New(mainIconName As IconName, mainIconOptions As IconOptions, stackedIconName As IconName, stackedIconOptions As IconOptions, stackContainerOptions As IconOptions, strAlertKey As String)
            Me.Mode = ActionButtonMode.IconButton
            Me.IconAction.IconName = mainIconName
            Me.IconAction.IconOptions = mainIconOptions
            Me.IconAction.StackedIconName = stackedIconName
            Me.IconAction.StackedIconOptions = stackedIconOptions
            Me.IconAction.StackContainerOptions = stackContainerOptions
            Me.AlertKey = strAlertKey
        End Sub


    End Class


End Namespace