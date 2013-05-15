Imports System.ComponentModel
Imports Aricie.Text

Namespace Aricie.DNN.Modules.PortalKeeper
    <DisplayName("File Read Action")> _
        <Description("This provider allows to read a file to a given String variable, given its path by dynamic expressions")> _
        <Serializable()> _
    Public Class FileReadActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileReadWriteActionProvider(Of TEngineEvents)




        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object

            Dim toReturn As String
            Dim mapPath As String = Me.GetFileMapPath(actionContext)
            'Select Case Me.AccessMode
            '    Case FileAccessMode.StringReadWrite
            Try
                RWLock.AcquireReaderLock(LockTimeSpan)
                If System.IO.File.Exists(mapPath) Then
                    toReturn = System.IO.File.ReadAllText(mapPath, EncodingHelper.GetEncoding(Me.Encoding))
                Else
                    toReturn = ""
                End If
            Catch ex As Exception
                Throw ex
            Finally
                RWLock.ReleaseReaderLock()
            End Try
            If Me.UseCompression AndAlso Not String.IsNullOrEmpty(toReturn) Then
                toReturn = Common.DoDeCompress(toReturn, CompressionMethod.Deflate)
            End If
            'End Select
            Return toReturn

        End Function
    End Class
End Namespace