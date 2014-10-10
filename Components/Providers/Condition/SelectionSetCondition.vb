Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper






    <Serializable()> _
    Public MustInherit Class SelectionSetCondition(Of TEngineEvents As IConvertible)
        Inherits ConditionProvider(Of TEngineEvents)
        Implements ISelectorAttributeProvider



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





        Public Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Me.DebuggerBreak Then
                CallDebuggerBreak()
            End If
            Dim currentItem As Integer = Me.GetCurrentValue(context)

            If Me.Items.Contains(currentItem) Then
                Return True
            End If
            Return False
        End Function


        Public MustOverride Function GetCurrentValue(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Integer


        Public MustOverride Function GetSelectorAttribute() As Attribute Implements ISelectorAttributeProvider.GetSelectorAttribute
           
    End Class
End Namespace