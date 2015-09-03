Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading

Namespace IO


    ''' <summary>
    ''' PipeStream is a thread-safe read/write data stream for use between two threads in a 
    ''' single-producer/single-consumer type problem.
    ''' </summary>
    ''' <version>2014/12/15 1.2</version>
    ''' <remarks>2006/10/13 1.0 - initial version.
    ''' Update on 2008/10/9 1.1 - uses Monitor instead of Manual Reset events for more elegant synchronicity.
    ''' Update on 2014/12/15 1.2 - bugfix for read method not using offset - thanks Jörgen Sigvardsson, replace NotImplementedExceptions with NotSupportedException</remarks>
    ''' <license>
    '''	Copyright (c) 2006 James Kolpack (james dot kolpack at google mail)
    '''	
    '''	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
    '''	associated documentation files (the "Software"), to deal in the Software without restriction, 
    '''	including without limitation the rights to use, copy, modify, merge, publish, distribute, 
    '''	sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
    '''	furnished to do so, subject to the following conditions:
    '''	
    '''	The above copyright notice and this permission notice shall be included in all copies or 
    '''	substantial portions of the Software.
    '''	
    '''	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
    '''	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
    '''	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
    '''	LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
    '''	OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
    '''	OTHER DEALINGS IN THE SOFTWARE.
    ''' </license>
    Public Class PipeStream
        Inherits Stream
#Region "Private members"

        ''' <summary>
        ''' Queue of bytes provides the datastructure for transmitting from an
        ''' input stream to an output stream.
        ''' </summary>
        ''' <remarks>Possible more effecient ways to accomplish this.</remarks>
        Private ReadOnly mBuffer As New Queue(Of Byte)()

        ''' <summary>
        ''' Indicates that the input stream has been flushed and that
        ''' all remaining data should be written to the output stream.
        ''' </summary>
        Private mFlushed As Boolean

        ''' <summary>
        ''' Maximum number of bytes to store in the buffer.
        ''' </summary>
        Private mMaxBufferLength As Long = 200 * MB

        ''' <summary>
        ''' Setting this to true will cause Read() to block if it appears
        ''' that it will run out of data.
        ''' </summary>
        Private mBlockLastRead As Boolean

        Private _ReadWriteTimeout As Integer

        Private _TotalWrite As Long = 0

#End Region

#Region "Public Const members"

        ''' <summary>
        ''' Number of bytes in a kilobyte
        ''' </summary>
        Public Const KB As Long = 1024

        ''' <summary>
        ''' Number of bytes in a megabyte
        ''' </summary>
        Public Const MB As Long = KB * 1024

#End Region


        Public Sub New()
            Me._ReadWriteTimeout = 5000
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="PipeStream"/> class.
        ''' </summary>
        Public Sub New(readWriteTimeout As Integer)
            Me._ReadWriteTimeout = readWriteTimeout

        End Sub


#Region "Public properties"

        ''' <summary>
        ''' Gets or sets the maximum number of bytes to store in the buffer.
        ''' </summary>
        ''' <value>The length of the max buffer.</value>
        Public Property MaxBufferLength() As Long
            Get
                Return mMaxBufferLength
            End Get
            Set(value As Long)
                mMaxBufferLength = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to block last read method before the buffer is empty.
        ''' When true, Read() will block until it can fill the passed in buffer and count.
        ''' When false, Read() will not block, returning all the available buffer data.
        ''' </summary>
        ''' <remarks>
        ''' Setting to true will remove the possibility of ending a stream reader prematurely.
        ''' </remarks>
        ''' <value>
        ''' 	<c>true</c> if block last read method before the buffer is empty; otherwise, <c>false</c>.
        ''' </value>
        Public Property BlockLastReadBuffer() As Boolean
            Get
                Return mBlockLastRead
            End Get
            Set(value As Boolean)
                mBlockLastRead = value

                ' when turning off the block last read, signal Read() that it may now read the rest of the buffer.
                If Not mBlockLastRead Then
                    SyncLock mBuffer
                        Monitor.Pulse(mBuffer)
                    End SyncLock
                End If
            End Set
        End Property

        Public Property TotalWrite() As Long
            Get
                Return _TotalWrite
            End Get
            Set(value As Long)
                _TotalWrite = value
            End Set
        End Property

#End Region

#Region "Stream overide methods"

        '''<summary>
        '''Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        '''</summary>
        '''<filterpriority>2</filterpriority>
        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)

            mBuffer.Clear()
            _TotalWrite = 0

        End Sub

        '''<summary>
        '''When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        '''</summary>
        '''
        '''<exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>2</filterpriority>
        Public Overrides Sub Flush()
            mFlushed = True
            SyncLock mBuffer
                Monitor.Pulse(mBuffer)
            End SyncLock
        End Sub

        '''<summary>
        '''When overridden in a derived class, sets the position within the current stream.
        '''</summary>
        '''<returns>
        '''The new position within the current stream.
        '''</returns>
        '''<param name="offset">A byte offset relative to the origin parameter. </param>
        '''<param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"></see> indicating the reference point used to obtain the new position. </param>
        '''<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        '''<exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
        '''<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function

        '''<summary>
        '''When overridden in a derived class, sets the length of the current stream.
        '''</summary>
        '''<param name="value">The desired length of the current stream in bytes. </param>
        '''<exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
        '''<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        '''<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>2</filterpriority>
        Public Overrides Sub SetLength(value As Long)
            Throw New NotSupportedException()
        End Sub


        '''<summary>
        '''When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        '''</summary>
        '''<returns>
        '''The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        '''</returns>
        '''<param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream. </param>
        '''<param name="count">The maximum number of bytes to be read from the current stream. </param>
        '''<param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source. </param>
        '''<exception cref="T:System.ArgumentException">The sum of offset and count is larger than the buffer length. </exception>
        '''<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        '''<exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
        '''<exception cref="T:System.ArgumentNullException">buffer is null. </exception>
        '''<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        '''<exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception><filterpriority>1</filterpriority>
        Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
            If offset <> 0 Then
                Throw New NotSupportedException("Offsets with value of non-zero are not supported")
            End If
            If buffer Is Nothing Then
                Throw New ArgumentException("Buffer is null")
            End If
            If offset + count > buffer.Length Then
                Throw New ArgumentException("The sum of offset and count is greater than the buffer length. ")
            End If
            If offset < 0 OrElse count < 0 Then
                Throw New ArgumentOutOfRangeException("offset", "offset or count is negative.")
            End If
            If BlockLastReadBuffer AndAlso count >= mMaxBufferLength Then
                Throw New ArgumentException([String].Format("count({0}) > mMaxBufferLength({1})", count, mMaxBufferLength))
            End If

            If count = 0 Then
                Return 0
            End If

            Dim readLength As Integer = 0

            SyncLock mBuffer
                While Not ReadAvailable(count)
                    Monitor.Wait(mBuffer)
                End While

                ' fill the read buffer
                readLength = ReadToBuffer(buffer, offset, count)


                Monitor.Pulse(mBuffer)
            End SyncLock
            Return readLength
        End Function

        ''' <summary>
        ''' Returns true if there are 
        ''' </summary>
        ''' <param name="count"></param>
        ''' <returns></returns>
        Private Function ReadAvailable(count As Integer) As Boolean
            Return (Length >= count OrElse mFlushed) AndAlso (Length >= (count + 1) OrElse Not BlockLastReadBuffer)
        End Function

        Protected Overridable Function ReadToBuffer(buffer As Byte(), offset As Integer, count As Integer) As Integer
            Dim readLength As Integer = 0

            While readLength < count AndAlso Length > 0
                buffer(offset + readLength) = mBuffer.Dequeue()
                readLength += 1
            End While

            Return readLength
        End Function


        '''<summary>
        '''When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        '''</summary>
        '''<param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream. </param>
        '''<param name="count">The number of bytes to be written to the current stream. </param>
        '''<param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream. </param>
        '''<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        '''<exception cref="T:System.NotSupportedException">The stream does not support writing. </exception>
        '''<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        '''<exception cref="T:System.ArgumentNullException">buffer is null. </exception>
        '''<exception cref="T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
        '''<exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception><filterpriority>1</filterpriority>
        Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
            If buffer Is Nothing Then
                Throw New ArgumentException("Buffer is null")
            End If
            If offset + count > buffer.Length Then
                Throw New ArgumentException("The sum of offset and count is greater than the buffer length. ")
            End If
            If offset < 0 OrElse count < 0 Then
                Throw New ArgumentOutOfRangeException("offset", "offset or count is negative.")
            End If
            If count = 0 Then
                Return
            End If

            SyncLock mBuffer
                ' wait until the buffer isn't full
                While Length >= mMaxBufferLength
                    Monitor.Wait(mBuffer)
                End While

                mFlushed = False
                ' if it were flushed before, it soon will not be.
                ' queue up the buffer data
                WriteToBuffer(buffer, offset, count)

                Me._TotalWrite += count

                ' signal that write has occured
                Monitor.Pulse(mBuffer)
            End SyncLock
        End Sub

        Protected Overridable Sub WriteToBuffer(buffer As Byte(), offset As Integer, count As Integer)
            ' queue up the buffer data
            For i As Integer = offset To count - 1
                mBuffer.Enqueue(buffer(i))
            Next
        End Sub

        '''<summary>
        '''When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        '''</summary>
        '''<returns>
        '''true if the stream supports reading; otherwise, false.
        '''</returns>
        '''<filterpriority>1</filterpriority>
        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return True
            End Get
        End Property

        '''<summary>
        '''When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        '''</summary>
        '''<returns>
        '''true if the stream supports seeking; otherwise, false.
        '''</returns>
        '''<filterpriority>1</filterpriority>
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return False
            End Get
        End Property

        '''<summary>
        '''When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        '''</summary>
        '''<returns>
        '''true if the stream supports writing; otherwise, false.
        '''</returns>
        '''<filterpriority>1</filterpriority>
        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return True
            End Get
        End Property

        '''<summary>
        '''When overridden in a derived class, gets the length in bytes of the stream.
        '''</summary>
        '''<returns>
        '''A long value representing the length of the stream in bytes.
        '''</returns>
        '''
        '''<exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
        '''<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        Public Overrides ReadOnly Property Length() As Long
            Get
                Return mBuffer.Count
            End Get
        End Property

        '''<summary>
        '''When overridden in a derived class, gets or sets the position within the current stream.
        '''</summary>
        '''<returns>
        '''The current position within the stream.
        '''</returns>
        '''<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        '''<exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        '''<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        Public Overrides Property Position() As Long
            Get
                Return 0
            End Get
            Set(value As Long)
                Throw New NotSupportedException()
            End Set
        End Property

#End Region
    End Class
End Namespace
