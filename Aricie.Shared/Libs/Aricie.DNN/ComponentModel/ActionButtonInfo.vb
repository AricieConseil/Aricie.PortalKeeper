Imports System.Reflection
Imports Aricie.DNN.UI.Attributes

Namespace ComponentModel

    <Serializable> _
    Public Class ActionButtonInfo

        Public Sub New()

        End Sub

        Public Property Method As MethodInfo
        Public Property ExtendedCategory As ExtendedCategory
        Public Property IconPath As String = ""
        Public Property AlertKey As String = ""
        Public Property ConditionalVisibles As List(Of ConditionalVisibleInfo)

        Public Shared Function FromMethod(objMethod As MethodInfo) As ActionButtonInfo
            Dim toReturn As ActionButtonInfo = Nothing
            Dim attrs As Attribute() = DirectCast(objMethod.GetCustomAttributes(GetType(ActionButtonAttribute), True), Attribute())
            If attrs.Length > 0 Then

                toReturn = New ActionButtonInfo
                toReturn.Method = objMethod
                Dim attr As ActionButtonAttribute = DirectCast(attrs(0), ActionButtonAttribute)
                toReturn.IconPath = attr.IconPath
                toReturn.AlertKey = attr.AlertKey
                toReturn.ExtendedCategory = ExtendedCategory.FromMember(objMethod)
                toReturn.ConditionalVisibles = ConditionalVisibleInfo.FromMember(objMethod)
            End If
            Return toReturn
        End Function


    End Class



    Public Class ConditionalVisibleInfo

        ' Fields
        Private _MasterPropertyName As String
        Private _MatchingValues() As Object
        Private _MatchValue As Predicate(Of Object)
        Private _EnforceAutoPostBack As Boolean
        Private _MasterNegate As Boolean

        ' Fields
        Private _SecondaryPropertyName As String = ""
        Private _SecondaryMatchingValue As Object
        Private _SecondaryNegate As Boolean





        Public Sub New(ByVal masterPropertyName As String, ByVal negate As Boolean, ByVal enforcePostBack As Boolean)
            Me._MasterPropertyName = masterPropertyName
            Me._MasterNegate = negate
            Me._MatchValue = AddressOf DefaultPredicate
            _EnforceAutoPostBack = enforcePostBack
        End Sub
        Public Sub New(ByVal masterPropertyName As String, ByVal negate As Boolean, ByVal enforcePostBack As Boolean, ByVal ParamArray objMatchingValues() As Object)
            Me.New(masterPropertyName, negate, enforcePostBack)
            Me._MatchingValues = objMatchingValues
            Me._MatchValue = AddressOf HasMatchingValue
        End Sub

        Public Sub New(ByVal masterPropertyName As String, ByVal negate As Boolean, ByVal enforcePostBack As Boolean, ByVal matchingPredicate As Predicate(Of Object))
            Me.New(masterPropertyName, negate, enforcePostBack)
            Me._MatchValue = matchingPredicate
        End Sub

        Public Sub New(ByVal enforcePostBack As Boolean, ByVal masterPropertyName As String, ByVal masterNegate As Boolean, ByVal masterValue As Object, secondaryPropertyName As String, secondaryNegate As Boolean, secondaryValue As Object)
            Me.New(masterPropertyName, masterNegate, enforcePostBack)
            Me._MatchingValues = New Object(0) {masterValue}
            Me._MatchValue = AddressOf HasMatchingValue
        End Sub



        ' Properties
        Public ReadOnly Property MasterPropertyName() As String
            Get
                Return Me._MasterPropertyName
            End Get
        End Property

        ' Properties
        Public ReadOnly Property SecondaryPropertyName() As String
            Get
                Return Me._SecondaryPropertyName
            End Get
        End Property

        Public ReadOnly Property MatchValue() As Predicate(Of Object)
            Get
                Return _MatchValue
            End Get
        End Property

        Public Function MatchSecondary(secondaryValue As Object) As Boolean
            Dim toReturn As Boolean
            If secondaryValue Is Nothing Then
                toReturn = Me._SecondaryMatchingValue Is Nothing
            Else
                toReturn = secondaryValue.Equals(Me._SecondaryMatchingValue)
            End If
            Return toReturn Xor Me._SecondaryNegate
        End Function

        Public ReadOnly Property EnforceAutoPostBack() As Boolean
            Get
                Return _EnforceAutoPostBack
            End Get
        End Property

        Private Function DefaultPredicate(ByVal value As Object) As Boolean
            Return CType(value, Boolean) Xor Me._MasterNegate
        End Function

        Private Function HasMatchingValue(ByVal value As Object) As Boolean
            For Each objValue As Object In _MatchingValues
                If objValue.Equals(value) Then
                    Return Not Me._MasterNegate
                End If
            Next
            Return Me._MasterNegate
        End Function


        Public Shared Function FromMember(member As MemberInfo) As List(Of ConditionalVisibleInfo)
            Dim toReturn As New List(Of ConditionalVisibleInfo)
            Dim customAttributes As Object() = member.GetCustomAttributes(GetType(ConditionalVisibleAttribute), True)
            If customAttributes.Length > 0 Then
                For Each condAttribute As ConditionalVisibleAttribute In customAttributes
                    toReturn.Add(condAttribute.Value)
                Next
            End If
            Return toReturn
        End Function



    End Class

End Namespace


