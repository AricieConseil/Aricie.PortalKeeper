Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Cog, IconOptions.Normal)> _
    <DefaultProperty("Title")> _
    <Serializable()> _
    Public Class UserParameterWrapper

        Public Sub New()

        End Sub

        Public Sub New(objTitle As String, objDescription As String, objInstance As Object)
            Me.Title = objTitle
            Me.Description = objDescription
            Me.Instance = objInstance
        End Sub

        <IsReadOnly(True)> _
        Public Property Title As String

        <IsReadOnly(True)> _
        Public Property Description As String


        Public Property Instance As Object

    End Class
End Namespace