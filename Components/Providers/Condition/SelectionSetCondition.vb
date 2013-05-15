Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class SelectionSetCondition
        Inherits ConditionProvider(Of RequestEvent)


        Private _Items As New List(Of Integer)

        <ExtendedCategory("Specifics")> _
            <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <InnerEditor(GetType(SelectorEditControl), GetType(ItemsAttributes))> _
            <CollectionEditor(False, False, False, True, 10)> _
        Public Property Items() As List(Of Integer)
            Get
                Return _Items
            End Get
            Set(ByVal value As List(Of Integer))
                _Items = value
            End Set
        End Property


        Private Class ItemsAttributes
            Implements IDynamicAttributesProvider

            Public Function GetAttributes(ByVal valueType As System.Type) As System.Collections.Generic.IEnumerable(Of System.Attribute) Implements IDynamicAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                Dim selectorAttribute As Attribute = DirectCast(Activator.CreateInstance(valueType), SelectionSetCondition).GetSelectorAttribute
                toReturn.Add(selectorAttribute)
                Return toReturn
            End Function
        End Class


        Public Overrides Function Match(ByVal context As PortalKeeperContext(Of RequestEvent)) As Boolean

            Dim currentItem As Integer = Me.GetCurrentValue(context)

            If Me.Items.Contains(currentItem) Then
                Return True
            End If
            Return False
        End Function


        Public MustOverride Function GetCurrentValue(ByVal context As PortalKeeperContext(Of RequestEvent)) As Integer
        Public MustOverride Function GetSelectorAttribute() As Attribute




    End Class
End Namespace