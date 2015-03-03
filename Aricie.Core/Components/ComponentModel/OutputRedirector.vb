
Imports System.IO
Imports System.Text

Namespace ComponentModel
    Public Class OutputRedirector
        Inherits TextWriter
        Private _output As StringBuilder = New StringBuilder()

        Public Overrides ReadOnly Property Encoding As System.Text.Encoding
            Get
                Return System.Text.Encoding.UTF8
            End Get
        End Property

        Public ReadOnly Property Text As String
            Get
                Return Me._output.ToString()
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub Clear()
            Me._output = New StringBuilder()
        End Sub

        Private Sub OnTextWritten(ByVal txtWritten As String)
            RaiseEvent StringWritten(Me, New OutputEventArgs(txtWritten))
            Me._output.Append(txtWritten)
        End Sub

        Public Overrides Sub Write(ByVal value As String)
            MyBase.Write(value)
            Me.OnTextWritten(value)
        End Sub

        Public Event StringWritten As OutputEventHandler
    End Class
End Namespace