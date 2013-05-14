Imports System.Web.UI
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports Aricie.UI.WebControls.EditControls
Imports Aricie.Services

Namespace UI.WebControls.EditControls

    Public Class ListEditControl
        Inherits CollectionEditControl



#Region "Public properties"

        Public ReadOnly Property ListValue() As IList
            Get
                Return DirectCast(Value, IList)
            End Get
        End Property

        Public ReadOnly Property OldListValue() As IList
            Get
                Return DirectCast(OldValue, IList)
            End Get
        End Property

        Public ReadOnly Property PagedList() As PagedList
            Get
                Return DirectCast(Me.PagedCollection, PagedList)
            End Get
        End Property

#End Region

#Region "CollectionEdit overrides"

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Me.Name)
            args.Value = Me.ListValue
            args.OldValue = Me.OldListValue
            'args.Changed = (Not args.Value Is args.OldValue)
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Protected Overrides Sub CreateRow(ByVal container As Control, ByVal value As Object)

            Dim itemEditorInfo As EditorInfo = Me.BuildEditInfo(value, Me.EditMode)
            If Me.ItemsReadOnly Then
                itemEditorInfo.EditMode = PropertyEditorMode.View
            End If
            Dim itemEditControl As EditControl = BuildEditor(itemEditorInfo, container)
            AddHandler itemEditControl.ValueChanged, AddressOf Me.ItemValueChanged
            container.Controls.Add(itemEditControl)
        End Sub

        Protected Overrides Sub CreateAddRow(ByVal container As Control)
            Dim itemEditorInfo As EditorInfo = Me.BuildEditInfo(AddEntry, PropertyEditorMode.Edit)
            Dim itemEditControl As EditControl = BuildEditor(itemEditorInfo, container)


            AddHandler itemEditControl.ValueChanged, AddressOf Me.AddValueChanged
            container.Controls.Add(itemEditControl)
        End Sub

        Protected Overrides Sub DeleteItem(ByVal index As Integer)
            Me.ListValue.RemoveAt(index)
            Me.PagedList.ClearCurrentItems()

            'PagedList.RemoveAt(index)
        End Sub

        Protected Overrides Function ExportItem(index As Integer) As System.Collections.ICollection
            Dim toReturn As IList = DirectCast(ReflectionHelper.CreateObject(Me.ListValue.GetType), IList)
            
            toReturn.Add(Me.ListValue(index))
            Return toReturn
        End Function

        Protected Overrides Sub AddNewItem(ByVal obj As Object)

            'ListValue.Add(obj)
            PagedList.Add(obj)

        End Sub

        Protected Overrides Function GetPagedCollection() As PagedCollection
            If TypeOf Me.CollectionValue Is IList Then
                Return New PagedList(Me.ListValue, Me.PageSize, Me.PageIndex)
            Else
                Return New PagedCollection(Me.CollectionValue, Me.PageSize, Me.PageIndex)
            End If
        End Function

#Region "private methods"


        Private Sub AddValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            AddEntry = e.Value
        End Sub


        Private Sub ItemValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            Dim item As IDataItemContainer = CType(CType(sender, WebControl).BindingContainer, IDataItemContainer)
            Dim i As Integer = 0
            Dim indexToEdit As Integer = Me.ItemIndex(item.DataItemIndex)

            'todo: should try with ReflectionHelper.CloneObject()
            Dim oldListValue As IList = New ArrayList(Me.ListValue)

            Me.ListValue.Item(indexToEdit) = e.Value

            Dim newArgs As New PropertyEditorEventArgs(Me.Name, Me.ListValue, oldListValue)
            Me.OnValueChanged(newArgs)
        End Sub

#End Region

#End Region



        Private Sub ListEditControl_MoveUp(ByVal index As Integer) Handles Me.MoveUp
            Dim item As Object = ListValue(index)

            Me.ListValue.Remove(item)
            ListValue.Insert(index - 1, item)
            Me.PagedList.ClearCurrentItems()
        End Sub

        Private Sub ListEditControl_MoveDown(ByVal index As Integer) Handles Me.MoveDown
            Dim item As Object = ListValue(index)

            ListValue.Remove(item)
            ListValue.Insert(index + 1, item)
            Me.PagedList.ClearCurrentItems()

        End Sub

       
    End Class

End Namespace
