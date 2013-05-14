Imports System.ComponentModel
Imports System.Reflection
Imports System.Globalization

Namespace Business.Filters

    Public Class SimpleComparer(Of T)
        Inherits SimpleComparer
        Implements IComparer(Of T)

        Public Sub New(ByVal propName As IConvertible, _
                      ByVal direction As ListSortDirection, Optional ByVal isHybrid As Boolean = False)

            MyBase.New(propName, direction, isHybrid)

        End Sub


        Public Function Compare1(ByVal x As T, ByVal y As T) As Integer Implements System.Collections.Generic.IComparer(Of T).Compare
            Return MyBase.Compare(x, y)
        End Function
    End Class



    Public Class SimpleComparer
        Implements IComparer



        Public Sub New(ByVal propName As IConvertible, _
                       ByVal direction As ListSortDirection, Optional ByVal isHybrid As Boolean = False)

            Me.PropertyName = propName
            Me.SortDirection = direction
            Me._IsHybrid = isHybrid

        End Sub

#Region "private members"

        Private _PropertyName As IConvertible = ""
        Private _SortDirection As ListSortDirection
        Private _PropInfo As PropertyInfo
        Private _IsHybrid As Boolean

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


#Region "IComparer implem"

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim toReturn As Integer = 0
            Dim invertCoef As Integer = CType(IIf(Me.SortDirection = ListSortDirection.Ascending, 1, -1), Integer)
            If Me._IsHybrid OrElse Me._PropInfo Is Nothing Then
                Dim objType As Type = x.GetType
                Dim props As Dictionary(Of String, PropertyInfo) = Aricie.Services.ReflectionHelper.GetPropertiesDictionary(objType)
                props.TryGetValue(Me.PropertyName.ToString(CultureInfo.InvariantCulture), Me._PropInfo)
            End If
            If Me._PropInfo Is Nothing Then
                toReturn = DirectCast(x, IComparable).CompareTo(y)
            Else
                toReturn = DirectCast(Me._PropInfo.GetValue(x, Nothing), IComparable).CompareTo(Me._PropInfo.GetValue(y, Nothing))
            End If


            Return invertCoef * toReturn
        End Function

#End Region


    End Class
End Namespace