

Namespace UI.Attributes

    Public Class CollectionEditorAttribute
        Inherits Attribute

        ''' <summary>
        ''' Constructeur simplifié pour ne pas se prendre la tête avec les paramètres dans le désordre
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub


        ' Methods
        <Obsolete("Use the other constructor")> _
        Public Sub New(ByVal showAddItem As Boolean, ByVal ordered As Boolean)
            Me._ShowAddItem = showAddItem
            Me._Ordered = ordered
        End Sub

        Public Sub New(ByVal noAddition As Boolean, ByVal showAddItem As Boolean, ByVal ordered As Boolean, ByVal paged As Boolean, ByVal pageSize As Integer)
            Me._NoAdd = noAddition
            Me._ShowAddItem = showAddItem
            Me._Ordered = ordered
            Me._Paged = paged
            Me._PageSize = pageSize
        End Sub

        Public Sub New(ByVal noAddition As Boolean, ByVal showAddItem As Boolean, ByVal ordered As Boolean, ByVal paged As Boolean, ByVal pageSize As Integer, ByVal displayStyle As CollectionDisplayStyle)
            Me.New(noAddition, showAddItem, ordered, paged, pageSize)
            Me._DisplayStyle = displayStyle
        End Sub

        Public Sub New(ByVal noAddition As Boolean, ByVal showAddItem As Boolean, ByVal ordered As Boolean, ByVal paged As Boolean, ByVal pageSize As Integer, ByVal displayStyle As CollectionDisplayStyle, ByVal enableExport As Boolean)
            Me.New(noAddition, showAddItem, ordered, paged, pageSize, displayStyle)
            Me._EnableExport = enableExport
        End Sub

        Public Sub New(ByVal noAddition As Boolean, ByVal showAddItem As Boolean, ByVal ordered As Boolean, ByVal paged As Boolean, ByVal pageSize As Integer, ByVal displayStyle As CollectionDisplayStyle, ByVal enableExport As Boolean, maxItemNb As Integer)
            Me.New(noAddition, showAddItem, ordered, paged, pageSize, displayStyle, enableExport)
            Me._MaxItemNb = maxItemNb
        End Sub

        Public Sub New(ByVal noAddition As Boolean, ByVal showAddItem As Boolean, ByVal ordered As Boolean, ByVal paged As Boolean, ByVal pageSize As Integer, ByVal displayStyle As CollectionDisplayStyle, ByVal enableExport As Boolean, maxItemNb As Integer, pagerDisplayFieldName As String)
            Me.New(noAddition, showAddItem, ordered, paged, pageSize, displayStyle, enableExport, maxItemNb)
            Me._PagerDisplayFieldName = pagerDisplayFieldName
        End Sub

        Public Sub New(ByVal noAddition As Boolean, ByVal showAddItem As Boolean, ByVal ordered As Boolean, ByVal paged As Boolean, ByVal pageSize As Integer, ByVal displayStyle As CollectionDisplayStyle, ByVal enableExport As Boolean, ByVal maxItemNb As Integer, ByVal pagerDisplayFieldName As String, ByVal itemsReadOnly As Boolean)
            Me.New(noAddition, showAddItem, ordered, paged, pageSize, displayStyle, enableExport, maxItemNb, pagerDisplayFieldName)
            Me._ItemsReadOnly = itemsReadOnly
        End Sub

        Public Property NoAdd As Boolean
        Public Property ShowAddItem As Boolean
        Public Property Ordered As Boolean = True
        Public Property Paged As Boolean = True
        Public Property PageSize As Integer = 30
        Public Property DisplayStyle As CollectionDisplayStyle = CollectionDisplayStyle.Accordion
        Public Property EnableExport As Boolean = False
        Public Property MaxItemNb As Integer
        Public Property PagerDisplayFieldName As String = String.Empty
        Public Property ItemsReadOnly As Boolean = False

    End Class

    Public Enum CollectionDisplayStyle
        List
        Accordion
    End Enum

End Namespace
