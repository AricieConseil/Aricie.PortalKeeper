Imports System.IO
Imports System.Threading

Namespace IO
    ''' <summary>
    ''' Class for streaming data with throttling support.
    ''' </summary>
    ''' <remarks>Improved from http://www.codeproject.com/Articles/18243/Bandwidth-throttling</remarks>
    Public Class ThrottledStream
        Inherits Stream
        ''' <summary>
        ''' A constant used to specify an infinite number of bytes that can be transferred per second.
        ''' </summary>
        Public Const Infinite As Long = 0

#Region "Private members"
        ''' <summary>
        ''' The base stream.
        ''' </summary>
        Private _baseStream As Stream

        ''' <summary>
        ''' The maximum bytes per second that can be transferred through the base stream.
        ''' </summary>
        Private _maximumBytesPerSecond As Long

        ''' <summary>
        ''' The number of bytes that has been transferred since the last throttle.
        ''' </summary>
        Private _byteCount As Long

        Private _LocalStopWatch As Stopwatch

        Private ReadOnly Property LocalStopWatch As Stopwatch
            Get
                If _LocalStopWatch Is Nothing Then
                    _LocalStopWatch = Stopwatch.StartNew()
                End If
                Return _LocalStopWatch
            End Get
        End Property

        ''' <summary>
        ''' The start time in milliseconds of the last throttle.
        ''' </summary>

        Private _RemainingUnthrottledBytes As Long


#End Region

#Region "Properties"


        ''' <summary>
        ''' Gets or sets the maximum bytes per second that can be transferred through the base stream.
        ''' </summary>
        ''' <value>The maximum bytes per second.</value>
        ''' <exception cref="T:System.ArgumentOutOfRangeException">Value is negative. </exception>
        Public Property MaximumBytesPerSecond() As Long
            Get
                Return _maximumBytesPerSecond
            End Get
            Set(value As Long)
                If value < 0 Then
                    Throw New ArgumentOutOfRangeException("MaximumBytesPerSecond", value, "The maximum number of bytes per second can't be negative.")
                End If
                If _maximumBytesPerSecond <> value Then
                    _maximumBytesPerSecond = value
                    Reset()
                End If
            End Set
        End Property

        Public Property BlockSize() As Integer
           

        ''' <summary>
        ''' Gets a value indicating whether the current stream supports reading.
        ''' </summary>
        ''' <returns>true if the stream supports reading; otherwise, false.</returns>
        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return _baseStream.CanRead
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the current stream supports seeking.
        ''' </summary>
        ''' <value></value>
        ''' <returns>true if the stream supports seeking; otherwise, false.</returns>
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return _baseStream.CanSeek
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the current stream supports writing.
        ''' </summary>
        ''' <value></value>
        ''' <returns>true if the stream supports writing; otherwise, false.</returns>
        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return _baseStream.CanWrite
            End Get
        End Property

        ''' <summary>
        ''' Gets the length in bytes of the stream.
        ''' </summary>
        ''' <value></value>
        ''' <returns>A long value representing the length of the stream in bytes.</returns>
        ''' <exception cref="T:System.NotSupportedException">The base stream does not support seeking. </exception>
        ''' <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        Public Overrides ReadOnly Property Length() As Long
            Get
                Return _baseStream.Length
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the position within the current stream.
        ''' </summary>
        ''' <value></value>
        ''' <returns>The current position within the stream.</returns>
        ''' <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        ''' <exception cref="T:System.NotSupportedException">The base stream does not support seeking. </exception>
        ''' <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        Public Overrides Property Position() As Long
            Get
                Return _baseStream.Position
            End Get
            Set(value As Long)
                _baseStream.Position = value
            End Set
        End Property
#End Region

#Region "Ctor"
        ''' <summary>
        ''' Initializes a new instance of the ThrottledStream class with an
        ''' infinite amount of bytes that can be processed.
        ''' </summary>
        ''' <param name="baseStream">The base stream.</param>
        Public Sub New(baseStream As Stream)
            Me.New(baseStream, ThrottledStream.Infinite)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the ThrottledStream class.
        ''' </summary>
        ''' <param name="baseStream">The base stream.</param>
        ''' <param name="maximumBytesPerSecond">The maximum bytes per second that can be transferred through the base stream.</param>
        ''' <exception cref="ArgumentNullException">Thrown when baseStream is a null reference.</exception>
        ''' <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="maximumBytesPerSecond"/> is a negative value.</exception>
        Public Sub New(baseStream As Stream, maximumBytesPerSecond As Long)
            Me.New(baseStream, maximumBytesPerSecond, 0)
        End Sub

        Public Sub New(baseStream As Stream, maximumBytesPerSecond As Long, initialSendBytes As Integer)
            If baseStream Is Nothing Then
                Throw New ArgumentNullException("baseStream")
            End If

            If maximumBytesPerSecond < 0 Then
                Throw New ArgumentOutOfRangeException("maximumBytesPerSecond", maximumBytesPerSecond, "The maximum number of bytes per second can't be negative.")
            End If

            _baseStream = baseStream
            _maximumBytesPerSecond = maximumBytesPerSecond
            BlockSize = 512
            _byteCount = 0
            _RemainingUnthrottledBytes = initialSendBytes
        End Sub

#End Region

#Region "Public methods"
        ''' <summary>
        ''' Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        ''' </summary>
        ''' <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        Public Overrides Sub Flush()
            _baseStream.Flush()
        End Sub

        ''' <summary>
        ''' Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        ''' </summary>
        ''' <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        ''' <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        ''' <param name="count">The maximum number of bytes to be read from the current stream.</param>
        ''' <returns>
        ''' The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        ''' </returns>
        ''' <exception cref="T:System.ArgumentException">The sum of offset and count is larger than the buffer length. </exception>
        ''' <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        ''' <exception cref="T:System.NotSupportedException">The base stream does not support reading. </exception>
        ''' <exception cref="T:System.ArgumentNullException">buffer is null. </exception>
        ''' <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        ''' <exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
        Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
            Dim total As Integer = 0
            While count > BlockSize
                Throttle(CLng(BlockSize))
                Dim rb As Integer = _baseStream.Read(buffer, offset, BlockSize)
                total += rb
                If rb < BlockSize Then
                    Return total
                End If
                offset += BlockSize
                count -= BlockSize
            End While
            Throttle(count)
            Return total + _baseStream.Read(buffer, offset, count)
        End Function

        ''' <summary>
        ''' Sets the position within the current stream.
        ''' </summary>
        ''' <param name="offset">A byte offset relative to the origin parameter.</param>
        ''' <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"></see> indicating the reference point used to obtain the new position.</param>
        ''' <returns>
        ''' The new position within the current stream.
        ''' </returns>
        ''' <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        ''' <exception cref="T:System.NotSupportedException">The base stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
        ''' <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
            Return _baseStream.Seek(offset, origin)
        End Function

        ''' <summary>
        ''' Sets the length of the current stream.
        ''' </summary>
        ''' <param name="value">The desired length of the current stream in bytes.</param>
        ''' <exception cref="T:System.NotSupportedException">The base stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
        ''' <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        ''' <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        Public Overrides Sub SetLength(value As Long)
            _baseStream.SetLength(value)
        End Sub

        ''' <summary>
        ''' Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        ''' </summary>
        ''' <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        ''' <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        ''' <param name="count">The number of bytes to be written to the current stream.</param>
        ''' <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        ''' <exception cref="T:System.NotSupportedException">The base stream does not support writing. </exception>
        ''' <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        ''' <exception cref="T:System.ArgumentNullException">buffer is null. </exception>
        ''' <exception cref="T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
        ''' <exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
        Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
            While count > BlockSize
                Throttle(BlockSize)
                _baseStream.Write(buffer, offset, BlockSize)
                offset += BlockSize
                count -= BlockSize
            End While
            Throttle(count)

            _baseStream.Write(buffer, offset, count)
        End Sub

        Public Overrides Function ReadByte() As Integer
            Throttle(1)
            Return _baseStream.ReadByte()
        End Function

        Public Overrides Sub WriteByte(value As Byte)
            Throttle(1)
            _baseStream.WriteByte(value)
        End Sub

        ''' <summary>
        ''' Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ''' </returns>
        Public Overrides Function ToString() As String
            Return _baseStream.ToString()
        End Function
#End Region

#Region "Protected methods"
        ''' <summary>
        ''' Throttles for the specified buffer size in bytes.
        ''' </summary>
        ''' <param name="bufferSizeInBytes">The buffer size in bytes.</param>
        Protected Overridable Sub Throttle(bufferSizeInBytes As Long)

            ' Make sure the buffer isn't empty.
            If _maximumBytesPerSecond <= 0 OrElse bufferSizeInBytes <= 0 Then
                Exit Sub
            End If

            If _RemainingUnthrottledBytes > bufferSizeInBytes Then
                _RemainingUnthrottledBytes -= bufferSizeInBytes
                Exit Sub
            Else
                bufferSizeInBytes -= _RemainingUnthrottledBytes
                _RemainingUnthrottledBytes = 0
            End If



            _byteCount += bufferSizeInBytes
            If _byteCount < 16 Then
                Exit Sub
            End If

            Dim wakeElapsed As Long = _byteCount * 1000L \ _maximumBytesPerSecond

            Dim elapsedMilliseconds As Long = LocalStopWatch.ElapsedMilliseconds

            ' Calculate the time to sleep.

            Dim toSleep As Integer = CInt(wakeElapsed - elapsedMilliseconds)

            If toSleep > 1 Then
                Try
                    ' The time to sleep is more then a millisecond, so sleep.
                    Thread.Sleep(toSleep)
                    ' Eatup ThreadAbortException.
                Catch generatedExceptionName As ThreadAbortException
                End Try

                ' A sleep has been done, reset.
                Reset()
            End If
        End Sub

        ''' <summary>
        ''' Will reset the bytecount to 0 and reset the start time to the current time.
        ''' </summary>
        Protected Sub Reset()
            Dim difference As Long = LocalStopWatch.ElapsedMilliseconds

            ' Only reset counters when a known history is available of more then 1 second.
            If difference > 1000 Then
                _byteCount = 0
                LocalStopWatch.Reset()
                LocalStopWatch.Start()
            End If
        End Sub
#End Region
    End Class
End NameSpace