Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class DynamicRestMethod
        Inherits SimpleRuleEngine

        Public Overrides Property Mode As RuleEngineMode = RuleEngineMode.Actions

        <CollectionEditor(False, True, False, False, 0, CollectionDisplayStyle.List, False, 5, "", True)> _
        Public Property HttpVerbs() As New List(Of WebMethod)


    End Class
End NameSpace