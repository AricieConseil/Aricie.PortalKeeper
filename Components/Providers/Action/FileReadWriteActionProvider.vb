Imports Aricie.Text
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class FileReadWriteActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileAccessActionProvider(Of TEngineEvents)

        'Private _AccessMode As FileAccessMode

        'Private _EntityType As New DotNetType
        Private _Encoding As SimpleEncoding

        Private _UseCompression As Boolean

        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
        <ExtendedCategory("File")> _
        Public Property Encoding() As SimpleEncoding
            Get
                Return _Encoding
            End Get
            Set(ByVal value As SimpleEncoding)
                _Encoding = value
            End Set
        End Property

        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
        <ExtendedCategory("File")> _
        Public Property UseCompression() As Boolean
            Get
                Return _UseCompression
            End Get
            Set(ByVal value As Boolean)
                _UseCompression = value
            End Set
        End Property

        '<ExtendedCategory("File")> _
        'Public Property AccessMode() As FileAccessMode
        '    Get
        '        Return _AccessMode
        '    End Get
        '    Set(ByVal value As FileAccessMode)
        '        _AccessMode = value
        '    End Set
        'End Property




        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        'Public Property EntityType() As DotNetType
        '    Get
        '        Return _EntityType
        '    End Get
        '    Set(ByVal value As DotNetType)
        '        _EntityType = value
        '    End Set
        'End Property


        Public Function ReadResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object

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
End NameSpace