Imports Aricie.Collections
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace ComponentModel
    <Serializable()> _
    Public Class SimpleList(Of TItem)

        Private _Instances As New SerializableList(Of TItem)
        'Private _NewElementProviderName As String = ""

        Private _ShowItems As Boolean

        '<ExtendedCategory("")> _
        '    <MainCategory()> _
        'Public Property ShowItems() As Boolean
        '    Get
        '        Return _ShowItems
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _ShowItems = value
        '    End Set
        'End Property


        '<ConditionalVisible("ShowItems", False, True, True)> _
        <ExtendedCategory("")> _
        <MainCategory()> _
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
        <CollectionEditor(False, False, True, True, 30, CollectionDisplayStyle.Accordion, True)> _
        <LabelMode(LabelMode.Top)> _
        Public Property Instances() As SerializableList(Of TItem)
            Get
                Return _Instances
            End Get
            Set(ByVal value As SerializableList(Of TItem))
                _Instances = value
                Me.OnCollectionChange()
            End Set
        End Property

       


        Protected Overridable Sub OnCollectionChange()

        End Sub

    End Class
End Namespace