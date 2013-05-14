
Imports System.Web.UI.WebControls

Namespace UI.Attributes
    Public Class OrientationAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal objOrientation As Orientation)
            Me._Orientation = objOrientation
        End Sub


        ' Properties
        Public ReadOnly Property Orientation() As Orientation
            Get
                Return Me._Orientation
            End Get
        End Property


        ' Fields
        Private _Orientation As Orientation
    End Class



End Namespace
