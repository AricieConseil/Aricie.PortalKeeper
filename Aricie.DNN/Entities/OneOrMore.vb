Imports System.ComponentModel

Namespace Entities
    <Serializable()> _
    Public Class OneOrMore(Of T As New)

        Public Sub New()
        End Sub

        Public Sub New(initElement As T)
            One = initElement
        End Sub

        Public Property One As New T

        Public Property More As New List(Of T)

        <Browsable(False)> _
        Public ReadOnly Property All As List(Of T)
            Get
                Dim toReturn As New List(Of T)(More.Count + 1)
                toReturn.Add(One)
                toReturn.AddRange(More)
                Return toReturn
            End Get
        End Property

    End Class
End NameSpace