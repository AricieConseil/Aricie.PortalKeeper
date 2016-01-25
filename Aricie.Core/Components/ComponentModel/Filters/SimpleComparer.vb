Imports System.ComponentModel
Imports System.Reflection
Imports System.Globalization

Namespace Business.Filters
    Public Class SimpleComparer(Of T)
        Inherits SimpleComparer
        Implements IComparer(Of T)
        Implements IEqualityComparer(Of T)


        Private _CompareDelegate As Func(Of T, T, Integer)
        Private _HashCodeDelegate As Func(Of T, Integer)

        Public Sub New(ByVal propName As IConvertible, _
                      ByVal direction As ListSortDirection, Optional ByVal isHybrid As Boolean = False)

            MyBase.New(propName, direction, isHybrid)

        End Sub


        Public Sub New(objCompareFunction As Func(Of T, T, Integer), objHashCodeFunction As Func(Of T, Integer))
            _CompareDelegate = objCompareFunction
            _HashCodeDelegate = objHashCodeFunction
        End Sub


        Public Function Compare1(ByVal x As T, ByVal y As T) As Integer Implements System.Collections.Generic.IComparer(Of T).Compare
            Return Me.Compare(x, y)
        End Function

        Public Function Equals1(x As T, y As T) As Boolean Implements IEqualityComparer(Of T).Equals
            Return Me.Compare1(x, y) = 0
        End Function

        Public Overrides Function Compare(x As Object, y As Object) As Integer
            If _CompareDelegate IsNot Nothing Then
                Return _CompareDelegate.Invoke(DirectCast(x, T), DirectCast(y, T))
            End If
            Return MyBase.Compare(x, y)
        End Function

        Public Function GetHashCode1(obj As T) As Integer Implements IEqualityComparer(Of T).GetHashCode
            If _HashCodeDelegate IsNot Nothing Then
                Return _HashCodeDelegate.Invoke(obj)
            End If
            Me.SetUp(obj)
            If Me.PropInfo Is Nothing Then
                Return DirectCast(obj, IConvertible).GetHashCode()
            Else
                Return DirectCast(PropInfo.GetValue(obj, Nothing), IConvertible).GetHashCode()
            End If
        End Function
    End Class



    Public Class SimpleComparer
        Implements IComparer


        Public Sub New()

        End Sub

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

        Public Property PropInfo As PropertyInfo
            Get
                Return _PropInfo
            End Get
            Set(value As PropertyInfo)
                _PropInfo = value
            End Set
        End Property


#End Region

        Protected Sub SetUp(x As Object)
            If Me._IsHybrid OrElse Me.PropInfo Is Nothing Then
                Dim objType As Type = x.GetType()
                Dim props As Dictionary(Of String, PropertyInfo) = Aricie.Services.ReflectionHelper.GetPropertiesDictionary(objType)
                props.TryGetValue(Me.PropertyName.ToString(CultureInfo.InvariantCulture), Me.PropInfo)
            End If
        End Sub


#Region "IComparer implem"



        Public Overridable Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim toReturn As Integer = 0
            Dim invertCoef As Integer = CType(IIf(Me.SortDirection = ListSortDirection.Ascending, 1, -1), Integer)
            If x IsNot Nothing Then
                SetUp(x)
            ElseIf y IsNot Nothing Then
                SetUp(y)
            Else
                Return 0
            End If

            If Me.PropInfo Is Nothing Then
                toReturn = DirectCast(x, IComparable).CompareTo(y)
            Else
                toReturn = DirectCast(Me.PropInfo.GetValue(x, Nothing), IComparable).CompareTo(Me.PropInfo.GetValue(y, Nothing))
            End If
            Return invertCoef * toReturn
        End Function

#End Region


    End Class
End Namespace