Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Desktop, IconOptions.Normal)>
    <DisplayName("Run Python")>
    <Description("Runs an Iron Python script")>
    Public Class RunPython(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        <ExtendedCategory("Python")>
        Public Property Python As New SimpleOrExpression(Of IronPython)

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            Dim objPython As IronPython = Python.GetValue(actionContext, actionContext)
            Return objPython.Run(actionContext)
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return GetType(String)
        End Function

    End Class
End NameSpace