Namespace ComponentModel
    
    Public Class RelatedEntityDescriptor

        Private _TargetEntityName As String = ""
        Private _Cardinality As RelationCardinality = RelationCardinality.OneToOne
        Private _Nature As RelationNature
        Private _SourceEntityRole As EntityGenre = EntityGenre.Undefined
        Private _TargetEntityRole As EntityGenre = EntityGenre.Undefined

        Public Sub New()

        End Sub

        Public Sub New(ByVal targetEntityName As String)
            Me.New()
            Me._TargetEntityName = targetEntityName
        End Sub

        Public Sub New(ByVal targetEntityName As String, ByVal cardinality As RelationCardinality, ByVal nature As RelationNature)
            Me.New(targetEntityName)
            Me._Cardinality = cardinality
            Me._Nature = nature

        End Sub

        Public Sub New(ByVal targetEntityName As String, ByVal cardinality As RelationCardinality, ByVal nature As RelationNature, _
                       ByVal sourceEntityRole As EntityGenre, ByVal targetEntityRole As EntityGenre)
            Me.New(targetEntityName, cardinality, nature)
            Me._SourceEntityRole = sourceEntityRole
            Me._TargetEntityRole = targetEntityRole

        End Sub


        Public Property TargetEntityName() As String
            Get
                Return _TargetEntityName
            End Get
            Set(ByVal value As String)
                _TargetEntityName = value
            End Set
        End Property

        Public Property Cardinality() As RelationCardinality
            Get
                Return _Cardinality
            End Get
            Set(ByVal value As RelationCardinality)
                _Cardinality = value
            End Set
        End Property


        Public Property Nature() As RelationNature
            Get
                Return _Nature
            End Get
            Set(ByVal value As RelationNature)
                _Nature = value
            End Set
        End Property

        Public Property SourceEntityRole() As EntityGenre
            Get
                Return _SourceEntityRole
            End Get
            Set(ByVal value As EntityGenre)
                _SourceEntityRole = value
            End Set
        End Property

        Public Property TargetEntityRole() As EntityGenre
            Get
                Return _TargetEntityRole
            End Get
            Set(ByVal value As EntityGenre)
                _TargetEntityRole = value
            End Set
        End Property

    End Class

    Public Class RelatedEntityDescriptor(Of T)
        Inherits RelatedEntityDescriptor

        Public Sub New()
            MyBase.New(GetType(T).Name)
        End Sub

        Public Sub New(ByVal cardinality As RelationCardinality, ByVal nature As RelationNature)
            MyBase.New(GetType(T).Name, cardinality, nature)
        End Sub

        Public Sub New(ByVal cardinality As RelationCardinality, ByVal nature As RelationNature, ByVal sourceEntityRole As EntityGenre, ByVal targetEntityRole As EntityGenre)
            MyBase.New(GetType(T).Name, cardinality, nature, sourceEntityRole, targetEntityRole)
        End Sub

    End Class
End Namespace