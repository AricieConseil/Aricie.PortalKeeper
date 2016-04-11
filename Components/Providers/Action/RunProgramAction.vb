Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Desktop, IconOptions.Normal)>
    <DisplayName("Run Program Action")>
    <Description("Run an executable program given its path and parameters")>
    Public Class RunProgramAction(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        <ExtendedCategory("Program")>
        Public Property ProcessStart As New SimpleProcessStartInfo()

        <ExtendedCategory("Program")>
        Public Property UseTimeOut As Boolean

        <ExtendedCategory("Program")> _
        <ConditionalVisible("UseTimeOut", False, True)>
        Public Property TimeOut As New STimeSpan(TimeSpan.FromMinutes(1))

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim toReturn As Object = Nothing

            Dim objProcessStart As ProcessStartInfo = Me.ProcessStart.GetProcessStartInfo(actionContext, actionContext)
            Using objProcess As New Process()
                objProcess.StartInfo = objProcessStart

                objProcess.StartInfo.CreateNoWindow = True
                objProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                objProcess.StartInfo.UseShellExecute = False
                objProcess.StartInfo.RedirectStandardError = True
                objProcess.StartInfo.RedirectStandardOutput = True

                'Dim dataAction As New Action(Of Object, DataReceivedEventArgs)(Sub(sender As Object, e As DataReceivedEventArgs)
                '                                                                   If (e.Data IsNot Nothing) Then
                '                                                                       toReturn = e.Data
                '                                                                   End If
                '                                                               End Sub)
                'AddHandler objProcess.OutputDataReceived, AddressOf dataAction.Invoke
                'AddHandler objProcess.ErrorDataReceived, AddressOf dataAction.Invoke

                'Dim startTime As DateTime = Now

                'objProcess.Start()
                'objProcess.BeginErrorReadLine()
                'objProcess.BeginOutputReadLine()

                'If UseTimeOut Then
                '    If Not objProcess.WaitForExit(CInt(Math.Floor(Me.TimeOut.Value.TotalMilliseconds))) Then
                '        objProcess.Kill()
                '    End If
                'Else
                '    objProcess.WaitForExit()
                'End If


                Dim stdOutput = New StringBuilder()
                AddHandler objProcess.OutputDataReceived, Function(sender, args) stdOutput.Append(args.Data)

                Dim stdError As String = Nothing
                Try
                    objProcess.Start()
                    objProcess.BeginOutputReadLine()
                    stdError = objProcess.StandardError.ReadToEnd()
                    If UseTimeOut Then
                        If Not objProcess.WaitForExit(CInt(Math.Floor(Me.TimeOut.Value.TotalMilliseconds))) Then
                            objProcess.Kill()
                        End If
                    Else
                        objProcess.WaitForExit()
                    End If
                Catch e As Exception
                    Throw New ApplicationException("OS error while executing " & ": " + e.Message, e)
                End Try

                If objProcess.ExitCode = 0 Then
                    toReturn = stdOutput.ToString()
                Else
                    Dim message = New StringBuilder()

                    If Not String.IsNullOrEmpty(stdError) Then
                        message.AppendLine(stdError)
                    End If

                    If stdOutput.Length <> 0 Then
                        message.AppendLine("Std output:")
                        message.AppendLine(stdOutput.ToString())
                    End If

                    Throw New ApplicationException("program finished with exit code = " & objProcess.ExitCode.ToString() & ": " & message.ToString())
                End If

            End Using

            Return toReturn
        End Function


        'Public Function RunExternalExe(filename As String, Optional arguments As String = Nothing) As String
        '    Dim process = New Process()

        '    process.StartInfo.FileName = filename
        '    If Not String.IsNullOrEmpty(arguments) Then
        '        process.StartInfo.Arguments = arguments
        '    End If

        '    process.StartInfo.CreateNoWindow = True
        '    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        '    process.StartInfo.UseShellExecute = False

        '    process.StartInfo.RedirectStandardError = True
        '    process.StartInfo.RedirectStandardOutput = True
        '    Dim stdOutput = New StringBuilder()
        '    AddHandler process.OutputDataReceived, Function(sender, args) stdOutput.Append(args.Data)

        '    Dim stdError As String = Nothing
        '    Try
        '        process.Start()
        '        process.BeginOutputReadLine()
        '        stdError = process.StandardError.ReadToEnd()
        '        process.WaitForExit()
        '    Catch e As Exception
        '        'Throw New ApplicationException((Convert.ToString("OS error while executing ") & Format(filename, arguments)) + ": " + e.Message, e)
        '    End Try

        '    If process.ExitCode = 0 Then
        '        Return stdOutput.ToString()
        '    Else
        '        Dim message = New StringBuilder()

        '        If Not String.IsNullOrEmpty(stdError) Then
        '            message.AppendLine(stdError)
        '        End If

        '        If stdOutput.Length <> 0 Then
        '            message.AppendLine("Std output:")
        '            message.AppendLine(stdOutput.ToString())
        '        End If

        '        Throw New ApplicationException(" finished with exit code = " & process.ExitCode.ToString() & ": " & message.ToString())
        '    End If
        'End Function







        Protected Overrides Function GetOutputType() As Type
            Return GetType(String)
        End Function
    End Class
End Namespace