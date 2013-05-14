Imports System.Web.UI.WebControls

Namespace UI.Attributes


    <AttributeUsage(AttributeTargets.Property)> _
    Public Class FieldStyleAttribute
        Inherits Attribute

        Private _width As String
        Private _labelWidth As String
        Private _editControlWidth As String

        Public Sub New(ByVal width As String, ByVal labelWidth As String, ByVal editControlWidth As String)
            _width = width
            _labelWidth = labelWidth
            _editControlWidth = editControlWidth
        End Sub

        Public ReadOnly Property Width() As Unit
            Get
                Return New Unit(_width)
            End Get
        End Property

        Public ReadOnly Property LabelWidth() As Unit
            Get
                Return New Unit(_labelWidth)
            End Get
        End Property

        Public ReadOnly Property EditControlWidth() As Unit
            Get
                Return New Unit(_editControlWidth)
            End Get
        End Property

    End Class

End Namespace
