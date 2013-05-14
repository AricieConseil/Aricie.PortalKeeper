Imports System
Imports System.IO

Namespace Services.Caching

    Public Class CaptureStream
        Inherits Stream


        Private _FilterChain As Stream
        Private _CopyStream As Stream


        Public Property FilterChain() As Stream
            Get
                Return _FilterChain
            End Get
            Set(ByVal value As Stream)
                _FilterChain = value
            End Set
        End Property

        Public Property CopyStream() As Stream
            Get
                Return _CopyStream
            End Get
            Set(ByVal value As Stream)
                _CopyStream = value
            End Set
        End Property


        Public Sub New(ByVal first As Stream, ByVal second As Stream)
            Me._FilterChain = first
            Me._CopyStream = second
        End Sub

        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return Me._FilterChain.CanRead
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return Me._FilterChain.CanSeek
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return Me._FilterChain.CanWrite
            End Get
        End Property

        Public Overrides Sub Flush()
            Me._FilterChain.Flush()
            If Me._CopyStream IsNot Nothing Then
                Me._CopyStream.Flush()
            End If

        End Sub

        Public Overrides ReadOnly Property Length() As Long
            Get
                Return Me._FilterChain.Length
            End Get
        End Property

        Public Overrides Property Position() As Long
            Get
                Return Me._FilterChain.Position
            End Get
            Set(ByVal value As Long)
                Me._FilterChain.Position = value
                If Me._CopyStream IsNot Nothing Then
                    Me._CopyStream.Position = value
                End If
            End Set
        End Property



        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Throw New NotSupportedException
        End Function

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Throw New NotSupportedException
        End Function

        Public Overrides Sub SetLength(ByVal value As Long)
            Throw New NotSupportedException
        End Sub

        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            Me._FilterChain.Write(buffer, offset, count)
            If Me._CopyStream IsNot Nothing Then
                Me._CopyStream.Write(buffer, offset, count)
            End If
        End Sub

        Public Overridable Sub StopFiltering(ByVal cancelCapture As Boolean)
            If (Not Me._CopyStream Is Nothing) Then
                Me._CopyStream.Close()
                Me._CopyStream = Nothing
            End If
        End Sub

    End Class

    Public Class FileCaptureFilter
        Inherits CaptureStream

        ' Fields
        Private _captureFilename As String
        'Private _captureFileStream As FileStream
        'Private _chainedStream As Stream

        ' Methods
        Friend Sub New(ByVal filterChain As Stream, ByVal captureFilename As String)
            'Me._chainedStream = filterChain
            MyBase.New(filterChain, New FileStream(captureFilename, FileMode.CreateNew, FileAccess.Write))
            Me._captureFilename = captureFilename
        End Sub

        Public ReadOnly Property CaptureFilename() As String
            Get
                Return Me._captureFilename
            End Get
        End Property



        Public Overrides Sub StopFiltering(ByVal cancelCapture As Boolean)
            MyBase.StopFiltering(cancelCapture)
            If cancelCapture Then
                If File.Exists(Me._captureFilename) Then
                    File.Delete(Me._captureFilename)
                End If
            End If
        End Sub




    End Class
End Namespace

