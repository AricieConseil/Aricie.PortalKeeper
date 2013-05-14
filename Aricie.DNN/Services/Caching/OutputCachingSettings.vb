Imports System.Web
Imports Aricie.Collections
Imports Aricie.DNN.UI.Attributes
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Caching
    ''' <summary>
    ''' Settings for cache output
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OutputCachingSettings

        ''' <summary>
        ''' Gets or sets whether the output caching is enabled
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        <MainCategory()> _
        Public Property Enabled() As Boolean

        ''' <summary>
        '''  Gets or sets the output caching strategy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        Public Property DefaultStrategy() As New OutputCachingStrategy

        Public Property VaryByLimitPerStrategy() As Integer = 20

        <ExtendedCategory("TabStrategies")> _
        <Editor(GetType(DictionaryEditControl), GetType(EditControl))> _
        <KeyEditor(GetType(SelectorEditControl), GetType(TabSpecificStrategiesKeyAttributes))> _
        <ValueEditor(GetType(PropertyEditorEditControl))> _
        <LabelMode(LabelMode.Top), Orientation(Orientation.Vertical)> _
        <CollectionEditor(False, True, True, True, 10)> _
        Public Property TabSpecificStrategies() As New SerializableDictionary(Of Integer, OutputCachingStrategy)

        Private Class TabSpecificStrategiesKeyAttributes
            Implements IAttributesProvider


            Public Function GetAttributes() As System.Collections.Generic.IEnumerable(Of System.Attribute) Implements UI.Attributes.IAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                toReturn.Add(New SelectorAttribute("Aricie.DNN.UI.WebControls.TabSelector, Aricie.DNN", "TabPath", "TabID", True, False))
                Return toReturn
            End Function
        End Class
    End Class

End Namespace