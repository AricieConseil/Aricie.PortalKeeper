Imports System.ComponentModel

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    Public Class SimpleSorter(Of T As {IComparable})
        Implements IComparer(Of T)


        Public Sub New(ByVal propName As IConvertible, _
                        ByVal direction As ListSortDirection)

            Me.PropertyName = propName
            Me.SortDirection = direction

        End Sub

#Region "private members"

        Private _PropertyName As IConvertible = ""
        Private _SortDirection As ListSortDirection
        Private _Value As T = Nothing


#End Region

#Region "Public Properties"

        Public Property PropertyName() As IConvertible
            Get
                Return Me._PropertyName
            End Get
            Set(ByVal value As IConvertible)
                Me._PropertyName = value
            End Set
        End Property

        Public Property SortDirection() As ListSortDirection
            Get
                Return Me._SortDirection
            End Get
            Set(ByVal value As ListSortDirection)
                Me._SortDirection = value
            End Set
        End Property


#End Region


#Region "IComparer(T) implem"

        Public Function Compare(ByVal x As T, ByVal y As T) As Integer Implements IComparer(Of T).Compare
            Dim invertCoef As Integer = DirectCast(IIf(Me.SortDirection = ListSortDirection.Ascending, 1, -1), Integer)
            Return invertCoef * x.CompareTo(y)
        End Function

#End Region
    End Class
End Namespace
