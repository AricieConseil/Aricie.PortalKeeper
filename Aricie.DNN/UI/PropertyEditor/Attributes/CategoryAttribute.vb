Namespace UI.Attributes

    Public Class ExtendedCategoryAttribute
        Inherits Attribute

        Private _SectionName As String
        Private _TabName As String
        Private _Column As Integer

        Public Sub New(ByVal tabName As String)
            Me._TabName = tabName
            Me._SectionName = String.Empty
            Me._Column = 0
        End Sub

        Public Sub New(ByVal sectionName As String, ByVal column As Integer)
            Me._TabName = String.Empty
            Me._SectionName = sectionName
            Me._Column = column
        End Sub

        Public Sub New(ByVal tabName As String, ByVal sectionName As String)
            Me._TabName = tabName
            Me._SectionName = sectionName
            Me._Column = 0
        End Sub
        Public Sub New(ByVal tabName As String, ByVal sectionName As String, ByVal column As Integer)
            Me._TabName = tabName
            Me._SectionName = sectionName
            Me._Column = column
        End Sub

        Public Property TabName As String
            Get
                Return _TabName
            End Get
            Set(ByVal value As String)
                _TabName = value
            End Set
        End Property


        Public Property SectionName As String
            Get
                Return _SectionName
            End Get
            Set(ByVal value As String)
                _SectionName = value
            End Set
        End Property

        Public Property Column As Integer
            Get
                Return _Column
            End Get
            Set(ByVal value As Integer)
                _Column = value
            End Set
        End Property

    End Class

End Namespace
