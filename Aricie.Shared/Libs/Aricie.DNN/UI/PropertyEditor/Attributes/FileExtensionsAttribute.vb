

Namespace UI.Attributes
    Public Class FileExtensionsAttribute
        Inherits Attribute

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="extensions">les extentions autorisées séparées par des virgules</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal extensions As String)
            Me._extensions = extensions
        End Sub


        ' Properties
        Public ReadOnly Property Extensions() As String
            Get
                Return Me._extensions
            End Get
        End Property


        ' Fields
        Private _extensions As String
    End Class
End Namespace
