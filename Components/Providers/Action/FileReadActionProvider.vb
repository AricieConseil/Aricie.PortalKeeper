Imports System.ComponentModel
Imports Aricie.Text
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.FileTextO, IconOptions.Normal)> _
   <DisplayName("File Read Action")> _
       <Description("This provider allows to read a file to a given String variable, given its path by dynamic expressions")> _
       <Serializable()> _
    Public Class FileReadActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileReadWriteActionProvider(Of TEngineEvents)





        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), isAsync As Boolean) As Object

            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If

            Return Me.ReadResult(actionContext, isAsync)

        End Function

        Protected Overrides Function GetOutputType() As Type
            Return GetType(String)
        End Function
    End Class


End Namespace