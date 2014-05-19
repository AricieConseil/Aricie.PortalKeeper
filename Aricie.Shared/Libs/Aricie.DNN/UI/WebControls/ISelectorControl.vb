Imports System.Web.UI
Imports Aricie.DNN.UI.Attributes


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

    Public Interface ISelectorAttributeProvider

        Function GetSelectorAttribute() As Attribute

    End Interface

    Public Class ItemsAttributes
        Implements IDynamicAttributesProvider

        Public Function GetAttributes(ByVal valueType As Type) As IEnumerable(Of Attribute) Implements IDynamicAttributesProvider.GetAttributes
            Dim toReturn As New List(Of Attribute)
            Dim selectorAttribute As Attribute = DirectCast(Activator.CreateInstance(valueType), ISelectorAttributeProvider).GetSelectorAttribute
            toReturn.Add(selectorAttribute)
            Return toReturn
        End Function
    End Class


End Namespace
