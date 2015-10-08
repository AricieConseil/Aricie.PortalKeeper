Imports IronPython.Hosting
Imports Microsoft.Scripting
Imports Microsoft.Scripting.Hosting
Imports System
Imports System.IO
Imports System.Text

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class IronPythonScriptHost
        Implements IDisposable


        Private m_engine As ScriptEngine

        Private m_scope As ScriptScope

        Private m_outputStream As MemoryStream

        Private m_outputStreamWriter As StreamWriter

        Public Sub New()
            MyBase.New()
            Me.m_engine = Python.CreateEngine()
            Me.m_scope = Me.m_engine.CreateScope()
            Me.CreateOutputBuffer()
        End Sub

        Public Sub ClearOutput()
            Me.DisposeOutputBuffer()
            Me.CreateOutputBuffer()
        End Sub

        Private Sub CreateOutputBuffer()
            Me.m_outputStream = New MemoryStream()
            Me.m_outputStreamWriter = New StreamWriter(Me.m_outputStream)
            Me.m_engine.Runtime.IO.SetOutput(Me.m_outputStream, Me.m_outputStreamWriter)
        End Sub



        Private Sub DisposeOutputBuffer()
            If (Me.m_outputStreamWriter IsNot Nothing) Then
                Me.m_outputStreamWriter.Dispose()
                Me.m_outputStreamWriter = Nothing
                Me.m_outputStream = Nothing
            End If
        End Sub

        Public Function Execute(ByVal expression As String) As Object
            Dim obj As Object
            Try
                Dim source As ScriptSource = Me.m_engine.CreateScriptSourceFromString(expression, SourceCodeKind.Statements)
                obj = source.Compile().Execute(Me.m_scope)
            Catch exception As System.Exception
                Me.m_outputStreamWriter.Write(Environment.NewLine)
                Me.m_outputStreamWriter.Write(exception.ToString())
                Me.m_outputStreamWriter.Write(Environment.NewLine)
                Me.m_outputStreamWriter.Flush()
                obj = Nothing
            End Try
            Return obj
        End Function

        Public Function GetOutput() As String
            Return IronPythonScriptHost.ReadFromStream(Me.m_outputStream)
        End Function

        Public Sub LoadAssembly(ByVal [assembly] As System.Reflection.[Assembly])
            Me.m_engine.Runtime.LoadAssembly([assembly])
        End Sub

        Private Shared Function ReadFromStream(ByVal ms As MemoryStream) As String
            Dim bytes(CInt(ms.Length)) As Byte
            ms.Seek(CLng(0), SeekOrigin.Begin)
            ms.Read(bytes, 0, CInt(ms.Length))
            Return Encoding.UTF8.GetString(bytes, 0, CInt(ms.Length))
        End Function

        Public Sub RegisterVariable(ByVal name As String, ByVal value As Object)
            Me.m_scope.SetVariable(name, value)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Me.DisposeOutputBuffer()
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace