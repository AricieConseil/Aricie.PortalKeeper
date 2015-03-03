

Namespace UI.Attributes
    Public Class LineCountAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal lineCount As Integer)
            Me._lineCount = lineCount
        End Sub

        Public Sub New(ByVal lineCount As Integer, objAutorResize As Boolean)
            Me._lineCount = lineCount
            Me.AutoResize = objAutorResize
        End Sub

        Public Property AutoResize As Boolean = True

        ' Properties
        Public ReadOnly Property LineCount() As Integer
            Get
                Return Me._lineCount
            End Get
        End Property


        ' Fields
        Private _lineCount As Integer
    End Class



End Namespace
