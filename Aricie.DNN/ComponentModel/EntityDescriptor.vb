Imports System.Xml.Serialization
Imports Aricie.Services

Namespace ComponentModel
    Public Class EntityDescriptor

        Private _EntityTypeName As String = ""
        Private _Services As EntityServices = EntityServices.None
        Private _Genre As EntityGenre = EntityGenre.Undefined
        Private _CollectionGenre As CollectionGenre = CollectionGenre.List
        Private _DisplayProperties As New List(Of String)
        Private _RelatedEntities As New List(Of RelatedEntityDescriptor)
        Private _Requirements As EntityRequirements


        Public Sub New()
        End Sub

        Public Sub New(ByVal entityTypeName As String)
            Me.New()
            Me._EntityTypeName = entityTypeName
        End Sub

        Public Sub New(ByVal entityType As Type)
            Me.New()
            Me.EntityType = entityType
        End Sub

        Public Sub New(ByVal entityType As Type, ByVal genre As EntityGenre, ByVal services As EntityServices, ByVal collectionGenre As CollectionGenre)
            Me.New(entityType)
            Me._Genre = genre
            Me._Services = services
            Me._CollectionGenre = collectionGenre
        End Sub

        Public Sub New(ByVal entityTypeName As String, ByVal genre As EntityGenre, ByVal services As EntityServices, ByVal collectionGenre As CollectionGenre)
            Me.New(entityTypeName)
            Me._Genre = genre
            Me._Services = services
            Me._CollectionGenre = collectionGenre
        End Sub


        Public ReadOnly Property Name() As String
            Get
                Return Me.EntityType.Name
            End Get
        End Property

        <XmlIgnore()> _
        Public Property EntityType() As Type
            Get
                Return ReflectionHelper.CreateType(_EntityTypeName)
            End Get
            Set(ByVal value As Type)
                _EntityTypeName = ReflectionHelper.GetSafeTypeName(value)
            End Set
        End Property

        Public Property EntityTypeName() As String
            Get
                Return _EntityTypeName
            End Get
            Set(ByVal value As String)
                _EntityTypeName = value
            End Set
        End Property

        Public Property Services() As EntityServices
            Get
                Return _Services
            End Get
            Set(ByVal value As EntityServices)
                _Services = value
            End Set
        End Property

        Public Property Genre() As EntityGenre
            Get
                Return _Genre
            End Get
            Set(ByVal value As EntityGenre)
                _Genre = value
            End Set
        End Property

        Public Property CollectionGenre() As CollectionGenre
            Get
                Return _CollectionGenre
            End Get
            Set(ByVal value As CollectionGenre)
                _CollectionGenre = value
            End Set
        End Property



        Public Property DisplayProperties() As List(Of String)
            Get
                Return _DisplayProperties
            End Get
            Set(ByVal value As List(Of String))
                _DisplayProperties = value
            End Set
        End Property


        Public Property RelatedEntities() As List(Of RelatedEntityDescriptor)
            Get
                Return _RelatedEntities
            End Get
            Set(ByVal value As List(Of RelatedEntityDescriptor))
                _RelatedEntities = value
            End Set
        End Property

        Public Property EntityRequirements() As EntityRequirements
            Get
                Return Me._Requirements
            End Get
            Set(ByVal value As EntityRequirements)
                Me._Requirements = value
            End Set
        End Property

    End Class

    Public Class EntityDescriptor(Of T)
        Inherits EntityDescriptor

        Public Sub New()
            MyBase.New(GetType(T))
        End Sub

        Public Sub New(ByVal genre As EntityGenre, ByVal services As EntityServices, ByVal collectionGenre As CollectionGenre)
            MyBase.New(GetType(T), genre, services, collectionGenre)
        End Sub

    End Class
End Namespace