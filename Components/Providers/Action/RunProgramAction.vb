Imports System.ComponentModel
Imports Aricie.DNN.Services.Files
Imports Aricie.DNN.Services
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper


    <ActionButton(IconName.Desktop, IconOptions.Normal)> _
   <Serializable()> _
   <DisplayName("Run Program Action")> _
   <Description("Run an executable program given its path and parameters")> _
    Public Class RunProgramAction(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)


        Public Property ProcessStart As New SimpleProcessStartInfo()

        Public Property UseTimeOut As Boolean

        <ConditionalVisible("UseTimeOut", False, True)> _
        Public Property TimeOut As New STimeSpan(TimeSpan.FromMinutes(1))

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            Dim toReturn As Object = Nothing

            Dim objProcessStart As ProcessStartInfo = Me.ProcessStart.GetProcessStartInfo(actionContext, actionContext)
            Using objProcess As New Process()
                objProcess.StartInfo = objProcessStart

                Dim dataAction As New Action(Of Object, DataReceivedEventArgs)(Sub(sender As Object, e As DataReceivedEventArgs)
                                                                                   If (e.Data IsNot Nothing) Then
                                                                                       toReturn = e.Data
                                                                                   End If
                                                                               End Sub)
                AddHandler objProcess.OutputDataReceived, AddressOf dataAction.Invoke
                AddHandler objProcess.ErrorDataReceived, AddressOf dataAction.Invoke

                Dim startTime As DateTime = Now

                objProcess.Start()
                objProcess.BeginErrorReadLine()
                objProcess.BeginOutputReadLine()

                If UseTimeOut Then
                    If Not objProcess.WaitForExit(CInt(Math.Floor(Me.TimeOut.Value.TotalMilliseconds))) Then
                        objProcess.Kill()
                    End If
                Else
                    objProcess.WaitForExit()
                End If

            End Using

            Return toReturn
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return GetType(String)
        End Function
    End Class
End Namespace