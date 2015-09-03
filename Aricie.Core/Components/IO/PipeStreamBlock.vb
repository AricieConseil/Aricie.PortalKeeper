Imports System
Imports System.Collections.Generic


Namespace IO

    ''' <summary>
    ''' Contains a block of queued data
    ''' </summary>
    Public Class PipeStreamBlock
        Inherits PipeStream
        Private _Length As Integer = 0
        Private _Buffer As New Queue(Of Byte())(1000)

        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(readWriteTimeout As Integer)
            MyBase.New(readWriteTimeout)
        End Sub

        Protected Overrides Sub WriteToBuffer(objBuffer As Byte(), offset As Integer, count As Integer)
            Dim bufferCopy As Byte() = New Byte(count - 1) {}
            Buffer.BlockCopy(objBuffer, offset, bufferCopy, 0, count)
            Me._Buffer.Enqueue(bufferCopy)

            Me._Length += count
        End Sub

        Protected Overrides Function ReadToBuffer(objBuffer As Byte(), offset As Integer, count As Integer) As Integer
            If 0 = Me._Buffer.Count Then
                Return 0
            End If

            Dim chunk As Byte() = Me._Buffer.Dequeue()
            ' It's possible the chunk has smaller number of bytes than buffer capacity
            Buffer.BlockCopy(chunk, 0, objBuffer, offset, chunk.Length)

            Me._Length -= chunk.Length
            Return chunk.Length
        End Function

        Public Overrides ReadOnly Property Length() As Long
            Get
                Return Me._Length
            End Get
        End Property

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)

            Me._Length = 0
            _Buffer.Clear()
        End Sub
    End Class
End Namespace
