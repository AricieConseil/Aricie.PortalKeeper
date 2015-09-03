Imports System.IO

Namespace Web.Proxy
    Public Module Log

        Private logStream As StreamWriter
        Private lockObject As New Object()
        Public Sub WriteLine(ByVal msg As String)
#If DEBUG Then
            If logStream Is Nothing Then
                SyncLock lockObject
                    If logStream Is Nothing Then
                        logStream = File.AppendText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StreamingProxyLog.txt"))
                    End If
                End SyncLock
            End If
            logStream.WriteLine(msg)
#End If

        End Sub
    End Module
End NameSpace