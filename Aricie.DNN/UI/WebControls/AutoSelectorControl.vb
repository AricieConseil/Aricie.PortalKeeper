Namespace UI.WebControls
    Public Class AutoSelectorControl
        Inherits SelectorControl

        Private _Enumerable As IList

        Public Sub New(ByVal enumarable As IList)
            Me._Enumerable = enumarable
            Me.ExclusiveSelector = False
        End Sub

        Public Overrides ReadOnly Property AllItems() As System.Collections.IList
            Get
                Return Me._Enumerable
            End Get
        End Property

        Public Overrides Function GetEntities() As IEnumerable
            Return Me._Enumerable
        End Function

    End Class

    Public Class AutoSelectorControl(Of T As Class)
        Inherits SelectorControl(Of T)

        Private _Enumerable As IEnumerable(Of T)

        Public Sub New(ByVal enumarable As IList(Of T))
            Me._Enumerable = enumarable
        End Sub

        Public Overrides Function GetEntitiesG() As System.Collections.Generic.IList(Of T)
            Return New List(Of T)(Me._Enumerable)
        End Function

    End Class
End Namespace