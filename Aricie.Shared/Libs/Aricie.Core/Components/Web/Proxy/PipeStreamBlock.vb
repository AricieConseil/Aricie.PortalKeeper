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

        Public Sub New(ByVal readWriteTimeout As Integer)
            MyBase.New(readWriteTimeout)
        End Sub

        Protected Overloads Overrides Sub WriteToBuffer(ByVal buffer__1 As Byte(), ByVal offset As Integer, ByVal count As Integer)
            Dim bufferCopy As Byte() = New Byte(count - 1) {}
            Buffer.BlockCopy(buffer__1, offset, bufferCopy, 0, count)
            Me._Buffer.Enqueue(bufferCopy)

            Me._Length += count
        End Sub

        Protected Overloads Overrides Function ReadToBuffer(ByVal buffer__1 As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            If 0 = Me._Buffer.Count Then
                Return 0
            End If

            Dim chunk As Byte() = Me._Buffer.Dequeue()
            ' It's possible the chunk has smaller number of bytes than buffer capacity
            Buffer.BlockCopy(chunk, 0, buffer__1, offset, chunk.Length)

            Me._Length -= chunk.Length
            Return chunk.Length
        End Function

        Public Overloads Overrides ReadOnly Property Length() As Long
            Get
                Return Me._Length
            End Get
        End Property

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)

            Me._Length = 0
            _Buffer.Clear()
        End Sub
    End Class
End Namespace
