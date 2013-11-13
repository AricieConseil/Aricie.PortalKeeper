Imports Aricie.DNN.Diagnostics
Imports OpenRasta.Diagnostics
Imports OpenRasta.Hosting.AspNet

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class OpenRastaLoggerCookie
        Implements IDisposable

        Public Sub New(objName As String, enabled As Boolean)
            Me.Name = objName
            Me.Enabled = enabled
        End Sub

        Public Property Enabled As Boolean
        Public Property Name As String


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    If Me.Enabled Then
                        Dim objDebug As New DebugInfo("OpenRasta", "End Operation", New KeyValuePair(Of String, String)("name", Me.Name))
                        SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
                    End If
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    Public Class OpenRastaLogger(Of TLogSource As ILogSource)
        Inherits OpenRastaLogger
        Implements ILogger(Of TLogSource)



    End Class

    Public Class OpenRastaLogger
        Implements ILogger


        Public Sub New()
            Dim restSettings As RestServicesSettings = PortalKeeperConfig.Instance.RestServices
            Me._Enabled = restSettings.Enabled AndAlso restSettings.EnableOpenRastaLogger
        End Sub

        Private _Enabled As Boolean

        Public ReadOnly Property Enabled As Boolean
            Get
                Return _Enabled
            End Get
        End Property



        Public Function Operation(source As Object, name As String) As System.IDisposable Implements OpenRasta.Diagnostics.ILogger.Operation
            If _Enabled Then
                Dim objDebug As New DebugInfo("OpenRasta", "Start Operation", New KeyValuePair(Of String, String)("name", name), New KeyValuePair(Of String, String)("sourceType", source.GetType().AssemblyQualifiedName))

                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If

            Return New OpenRastaLoggerCookie(name, Me.Enabled)
        End Function

        Public Sub WriteDebug(message As String, ParamArray format() As Object) Implements OpenRasta.Diagnostics.ILogger.WriteDebug
            If _Enabled Then
                Dim objDebug As New DebugInfo("OpenRasta", "Debug", New KeyValuePair(Of String, String)("message", String.Format(message, format)))
                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If

        End Sub

        Public Sub WriteError(message As String, ParamArray format() As Object) Implements OpenRasta.Diagnostics.ILogger.WriteError
            If _Enabled Then
                Dim objDebug As New DebugInfo("OpenRasta", "Error", New KeyValuePair(Of String, String)("message", String.Format(message, format)))
                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If

        End Sub

        Public Sub WriteException(e As System.Exception) Implements OpenRasta.Diagnostics.ILogger.WriteException
            If _Enabled Then
                DotNetNuke.Services.Exceptions.Exceptions.LogException(e)
            End If

        End Sub

        Public Sub WriteInfo(message As String, ParamArray format() As Object) Implements OpenRasta.Diagnostics.ILogger.WriteInfo
            If _Enabled Then
                Dim objDebug As New DebugInfo("OpenRasta", "Info", New KeyValuePair(Of String, String)("message", String.Format(message, format)))
                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If

        End Sub

        Public Sub WriteWarning(message As String, ParamArray format() As Object) Implements OpenRasta.Diagnostics.ILogger.WriteWarning
            If _Enabled Then
                Dim objDebug As New DebugInfo("OpenRasta", "Warning", New KeyValuePair(Of String, String)("message", String.Format(message, format)))
                SimpleDebugLogger.Instance.AddDebugInfo(objDebug)
            End If

        End Sub
    End Class
End Namespace