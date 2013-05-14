Namespace Diagnostics

    ''' <summary>
    ''' Scope class to ease the use of a live debugger within business code.
    ''' </summary>
    Public Class DebugInfoScope
        Implements IDisposable

        Private Component As String
        Private Process As String
        Private ArgsList As New List(Of KeyValuePair(Of String, String))
        Private EnableLogs As Boolean

        Public Sub New(component As String, process As String, enableLogs As Boolean)
            Me.Component = component
            Me.Process = process
            Me.EnableLogs = enableLogs
        End Sub

        Public Overridable Sub AddInfo(key As String, value As String)
            ArgsList.Add(New KeyValuePair(Of String, String)(key, value))
        End Sub

        Protected Overridable Sub ProcessCollectedData()
            If EnableLogs Then
                Dim DebugInfo As New Aricie.DNN.Diagnostics.DebugInfo(Component, Process, ArgsList.ToArray())
                Aricie.DNN.Diagnostics.SimpleDebugLogger.Instance().AddDebugInfo(DebugInfo)
            End If
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    ProcessCollectedData()
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class


End Namespace