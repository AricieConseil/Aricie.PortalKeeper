Imports System.ComponentModel
Imports Aricie.Text

Namespace Aricie.DNN.Modules.PortalKeeper
    <DisplayName("File Read Action")> _
        <Description("This provider allows to read a file to a given String variable, given its path by dynamic expressions")> _
        <Serializable()> _
    Public Class FileReadActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileReadWriteActionProvider(Of TEngineEvents)




        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), isAsync As Boolean) As Object

            Return Me.ReadResult(actionContext, isAsync)

        End Function
    End Class
End Namespace