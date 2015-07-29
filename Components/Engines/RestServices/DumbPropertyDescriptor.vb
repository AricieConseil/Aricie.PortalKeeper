Imports System.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DumbPropertyDescriptor
        Inherits PropertyDescriptor

        Private _PropertyType As Type


        Public Sub New(name As String, propType As Type)
            Me.New(name, propType, New List(Of Attribute)().ToArray())
        End Sub

        Public Sub New(name As String, propType As Type, attrs As Attribute())
            MyBase.New(name, attrs)
            _PropertyType = propType
        End Sub


        Public Overrides Function CanResetValue(component As Object) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property ComponentType As Type
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides Function GetValue(component As Object) As Object
            Return Nothing
        End Function

        Public Overrides ReadOnly Property IsReadOnly As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property PropertyType As Type
            Get
                Return _PropertyType
            End Get
        End Property

        Public Overrides Sub ResetValue(component As Object)

        End Sub

        Public Overrides Sub SetValue(component As Object, value As Object)

        End Sub

        Public Overrides Function ShouldSerializeValue(component As Object) As Boolean
            Return True
        End Function
    End Class
End NameSpace