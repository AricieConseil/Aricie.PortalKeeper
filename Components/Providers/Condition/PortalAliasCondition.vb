Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Globe, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Portal Alias Condition")> _
    <Description("Matches according to the portal alias used in the url")> _
    Public Class PortalAliasCondition
        Inherits SelectionSetCondition


        Public Overrides Function GetCurrentValue(ByVal context As PortalKeeperContext(Of RequestEvent)) As Integer
            Return context.CurrentAliasId
        End Function

        Public Overrides Function GetSelectorAttribute() As Attribute
            Return New SelectorAttribute(GetType(PortalAliasSelector), "HTTPAlias", "PortalAliasID", True, True, "None", "-1", False, False)
        End Function
    End Class


    '<Serializable()> _
    '<System.ComponentModel.DisplayName("Portal Alias Condition")> _
    '<Description("Matches according to the portal alias used in the url")> _
    'Public Class PortalAliasCondition
    '    Inherits ConditionProvider(Of RequestEvent)



    '    Private _PortalAliases As New List(Of Integer)

    '    <ExtendedCategory("Specifics")> _
    '    <Editor(GetType(ListEditControl), GetType(EditControl)), _
    '        InnerEditor(GetType(SelectorEditControl), GetType(PortalAliasesAttributes))> _
    '        <CollectionEditor(False, False, False, True, 10)> _
    '    Public Property PortalAliases() As List(Of Integer)
    '        Get
    '            Return _PortalAliases
    '        End Get
    '        Set(ByVal value As List(Of Integer))
    '            _PortalAliases = value
    '        End Set
    '    End Property

    '    Private Class PortalAliasesAttributes
    '        Implements IAttributesProvider

    '        Public Function GetAttributes() As System.Collections.Generic.IEnumerable(Of System.Attribute) Implements IAttributesProvider.GetAttributes
    '            Dim toReturn As New List(Of Attribute)
    '            toReturn.Add(New SelectorAttribute(GetType(PortalAliasSelector), "HTTPAlias", "PortalAliasID", True, False, "", "", False, False))
    '            Return toReturn
    '        End Function
    '    End Class


    '    Public Overrides Function Match(ByVal context As PortalKeeperContext(Of RequestEvent)) As Boolean

    '        Dim currentAliasId As Integer = context.CurrentAliasId

    '        If Me.PortalAliases.Contains(currentAliasId) Then
    '            Return True
    '        End If
    '        Return False
    '    End Function

    'End Class
End Namespace