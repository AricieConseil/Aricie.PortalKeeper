Imports System.Web.UI


Namespace UI.WebControls

    Public Interface ISelectorControl

        Property ExclusiveSelector() As Boolean
        Property ExclusiveScopeControl() As Control
        Property ExclusiveScopeControlId() As String
        Property InsertNullItem() As Boolean
        Property NullItemText() As String
        Property NullItemValue() As String
        Property LocalizeItems() As Boolean
        Property LocalizeNull() As Boolean

        Function GetEntities() As IList

    End Interface

    Public Interface ISelectorControl(Of T)

        Function GetEntitiesG() As IList(Of T)
    End Interface
End Namespace
