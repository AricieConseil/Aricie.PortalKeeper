Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports Aricie.Services
Imports System.Drawing
Imports DotNetNuke.Services.Localization
Imports System.Reflection

Namespace UI.WebControls.EditControls

    Public Class DictionaryEditControl
        Inherits CollectionEditControl



#Region "Private members"

        Protected WithEvents ctlNewItemKeyEditControl As EditControl
        Protected WithEvents ctlNewItemValueEditControl As EditControl
        Private _KeyValueOrientation As Orientation = Orientation.Vertical


#End Region


#Region "Public properties"







        Public Property KeyValueOrientation() As Orientation
            Get
                Return _KeyValueOrientation
            End Get
            Set(ByVal value As Orientation)
                _KeyValueOrientation = value
            End Set
        End Property


        Public Property DictionaryValue() As IDictionary
            Get
                Return DirectCast(Value, IDictionary)
            End Get
            Set(ByVal value As IDictionary)
                Me.Value = value
            End Set
        End Property

        Public ReadOnly Property OldDictionaryValue() As IDictionary
            Get
                Return DirectCast(OldValue, IDictionary)
            End Get
        End Property


#End Region

#Region "Life cycle Overrides"









#End Region

#Region "CollectionEdit overrides"

        Protected Overrides Sub OnAttributesChanged()
            MyBase.OnAttributesChanged()
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In CustomAttributes
                    If TypeOf attribute Is OrientationAttribute Then
                        Me.KeyValueOrientation = DirectCast(attribute, OrientationAttribute).Orientation
                    End If
                Next
            End If
        End Sub


        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Me.Name)
            args.Value = Me.DictionaryValue
            args.OldValue = Me.OldDictionaryValue
            'args.Changed = (Not args.Value Is args.OldValue)
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub





        Protected Overrides Sub CreateRow(ByVal container As Control, ByVal value As Object)


            Dim keyEditorInfo As EditorInfo = Me.BuildEditInfo(value, "Key", PropertyEditorMode.View)
            Dim valueEditorInfo As EditorInfo
            If Not Me.ItemsReadOnly Then
                valueEditorInfo = Me.BuildEditInfo(value, "Value", Me.EditMode)
            Else
                valueEditorInfo = Me.BuildEditInfo(value, "Value", PropertyEditorMode.View)
            End If

            Dim keyEditControl As EditControl = BuildEditor(keyEditorInfo, container)
            
            'keyEditControl.EditMode = PropertyEditorMode.View
            Dim valueEditControl As EditControl = BuildEditor(valueEditorInfo, container)

            AddHandler valueEditControl.ValueChanged, AddressOf Me.ItemValueChanged

            Dim table As HtmlContainerControl = InjectEditors(keyEditControl, valueEditControl)

            container.Controls.Add(table)

        End Sub

        Protected Overrides Sub CreateAddRow(ByVal container As Control)


            Dim newEntry As DictionaryEntry = DirectCast(Me.AddEntry, DictionaryEntry)

            Dim keyEditorInfo As EditorInfo = Me.BuildEditInfo(newEntry, "Key", PropertyEditorMode.Edit)
            Dim valueEditorInfo As EditorInfo = Me.BuildEditInfo(newEntry, "Value", PropertyEditorMode.Edit)
            If valueEditorInfo.Required Then
                valueEditorInfo.Required = False
            End If

            Me.ctlNewItemKeyEditControl = BuildEditor(keyEditorInfo, container)
            If TypeOf Me.ctlNewItemKeyEditControl Is CustomTextEditControl Then
                DirectCast(Me.ctlNewItemKeyEditControl, CustomTextEditControl).Size = 50
            End If

            Dim divKeyControl As New HtmlGenericControl("div")

            Dim label As PropertyLabelControl = Me.ParentAricieField.BuildLtLabel(keyEditorInfo)
            divKeyControl.Controls.Add(label)
            divKeyControl.Controls.Add(Me.ctlNewItemKeyEditControl)

            Me.ctlNewItemValueEditControl = BuildEditor(valueEditorInfo, container)


            AddHandler Me.ctlNewItemKeyEditControl.ValueChanged, AddressOf Me.AddValueChanged
            AddHandler Me.ctlNewItemValueEditControl.ValueChanged, AddressOf Me.AddValueChanged

            Me.ctlNewItemValueEditControl.Visible = False

            'Dim table As HtmlContainerControl = InjectEditors(Me.ctlNewItemKeyEditControl, Me.ctlNewItemValueEditControl)
            Dim table As HtmlContainerControl = InjectEditors(divKeyControl, Me.ctlNewItemValueEditControl)

            container.Controls.Add(table)


        End Sub


        Protected Overrides Sub DeleteItem(ByVal index As Integer)

            Dim en As IDictionaryEnumerator = DictionaryValue.GetEnumerator()
            Dim i As Integer = 0

            en.Reset()
            While i <= index
                en.MoveNext()
                i += 1
            End While

            DictionaryValue.Remove(en.Key)
            Me.PagedCollection.ClearCurrentItems()
        End Sub


        Protected Overrides Function GetNewItem() As Object
            If DictionaryValue Is Nothing Then
                Throw New ApplicationException(String.Format("Null Dictionary bound to {0} Control in PropertyEditor", Me.Name))
            End If
            Dim dictType As Type = DictionaryValue.GetType()

            Dim keyType, valueType As Type

            If dictType.IsGenericType Then
                Dim genArgs As Type() = dictType.GetGenericArguments()
                keyType = genArgs(0)
                valueType = genArgs(1)
            Else
                keyType = ReflectionHelper.GetCollectionElementType(DictionaryValue.Keys)
                valueType = ReflectionHelper.GetCollectionElementType(DictionaryValue.Values)
                'Throw New NotImplementedException("Cet EditControl ne marche qu'avec des collection generiques.")
            End If

            Dim key As Object
            Dim value As Object

            key = ReflectionHelper.CreateObject(keyType.FullName)
            value = ReflectionHelper.CreateObject(valueType.FullName)

            Return New DictionaryEntry(key, value)
        End Function

        Protected Overrides Sub AddItems(items As ICollection)
            If items.Count = 1 Then
                Dim itemToAdd As Object = items(0)
                Dim objAddEntry As Object = Me.AddEntry
                If objAddEntry IsNot Nothing Then
                    Dim entryKeyProp As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(objAddEntry.GetType())("Key")
                    Dim keyAddEntry As Object = entryKeyProp.GetValue(objAddEntry, Nothing)
                    If keyAddEntry IsNot Nothing AndAlso ReflectionHelper.IsSimpleType(keyAddEntry.GetType()) AndAlso Not keyAddEntry.ToString().IsNullOrEmpty() Then
                        Dim itemKeyProp As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(itemToAdd.GetType())("Key")
                        If itemKeyProp.CanWrite Then
                            itemKeyProp.SetValue(itemToAdd, keyAddEntry, Nothing)
                        Else
                            Dim itemValueProp As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(itemToAdd.GetType())("Value")
                            Dim itemValueEntry As Object = itemValueProp.GetValue(itemToAdd, Nothing)
                            itemToAdd = Activator.CreateInstance(itemToAdd.GetType(), keyAddEntry, itemValueEntry)
                        End If
                    End If
                End If
                Me.AddNewItem(itemToAdd)
            Else
                MyBase.AddItems(items)
            End If
        End Sub

        Protected Overrides Sub AddNewItem(ByVal item As Object)
            Dim key As Object
            Dim value As Object
            If TypeOf item Is DictionaryEntry Then
                Dim de As DictionaryEntry = DirectCast(item, DictionaryEntry)
                key = de.Key
                value = de.Value
            Else
                Dim props As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(item.GetType)
                key = props("Key").GetValue(item, Nothing)
                value = props("Value").GetValue(item, Nothing)
            End If
            If Not Me.DictionaryValue.Contains(key) Then
                DictionaryValue.Add(key, value)
                Me.PagedCollection.ClearCurrentItems()
            Else
                Dim errorLabel As New Label
                errorLabel.ControlStyle.ForeColor = Color.Red
                errorLabel.ControlStyle.Font.Italic = True
                Dim errorText As String = Localization.GetString(Me.Name & "_DuplicateKey.Error", Me.LocalResourceFile)
                If String.IsNullOrEmpty(errorText) Then
                    errorText = String.Format("Duplicate Key in {0} dictionary", Me.Name)
                End If
                errorLabel.Text = errorText
                Me.Controls.Add(errorLabel)
            End If

        End Sub

        Protected Overrides Function ExportItem(index As Integer) As System.Collections.ICollection
            Dim dico As IDictionary = DirectCast(ReflectionHelper.CreateObject(Me.DictionaryValue.GetType), IDictionary)
            Dim en As IDictionaryEnumerator = DictionaryValue.GetEnumerator()
            Dim i As Integer = 0

            en.Reset()
            While i <= index
                en.MoveNext()
                i += 1
            End While
            dico.Add(en.Key, en.Value)
            Return dico
        End Function


#End Region

#Region "private methods"


        Private Sub AddValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            Dim objAddEntry As DictionaryEntry = DirectCast(AddEntry, DictionaryEntry)
            Select Case e.Name
                Case "Key"
                    objAddEntry.Key = e.Value
                Case "Value"
                    objAddEntry.Value = e.Value
            End Select
            AddEntry = objAddEntry
        End Sub


        Private Sub ItemValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            Dim item As IDataItemContainer = CType(CType(sender, WebControl).BindingContainer, IDataItemContainer)
            Dim i As Integer = 0

            Dim indexToEdit As Integer = Me.ItemIndex(item.DataItemIndex)
            Dim enTorDico As IDictionaryEnumerator = DictionaryValue.GetEnumerator()

            enTorDico.Reset()
            While i <= indexToEdit
                enTorDico.MoveNext()
                i += 1
            End While

            'todo: should try with ReflectionHelper.CloneObject()
            Dim oldDicoValue As IDictionary = New Hashtable(Me.DictionaryValue)

            DictionaryValue.Item(enTorDico.Key) = e.Value

            Dim newArgs As New PropertyEditorEventArgs(Me.Name, DictionaryValue, oldDicoValue)
            Me.OnValueChanged(newArgs)
        End Sub


        Private Sub DictionaryEditControl_MoveUp(ByVal index As Integer) Handles Me.MoveUp
            Me.MoveItem(index, ListSortDirection.Ascending)


        End Sub


        Private Sub DictionaryEditControl_MoveDown(ByVal index As Integer) Handles Me.MoveDown
            Me.MoveItem(index, ListSortDirection.Descending)
        End Sub

        Private Sub MoveItem(ByVal index As Integer, ByVal direction As ListSortDirection)

            Dim tempDico As IDictionary = Me.DictionaryValue

            Dim offsetIndex As Integer = index - 1

            If direction = ListSortDirection.Descending Then
                offsetIndex += 2
            End If

            Dim count As Integer = tempDico.Count
            Dim keys(count) As Object
            'Dim values(count) As Object

            tempDico.Keys.CopyTo(keys, 0)
            'tempDico.Values.CopyTo(values, 0)
            Dim tempList As New List(Of DictionaryEntry)(count)

            For i = 0 To (Math.Min(index, offsetIndex) - 1)
                tempList.Add(New DictionaryEntry(keys(i), tempDico(keys(i))))
            Next

            If direction = ListSortDirection.Ascending Then
                tempList.Add(New DictionaryEntry(keys(index), tempDico(keys(index))))
                tempList.Add(New DictionaryEntry(keys(offsetIndex), tempDico(keys(offsetIndex))))
            Else
                tempList.Add(New DictionaryEntry(keys(offsetIndex), tempDico(keys(offsetIndex))))
                tempList.Add(New DictionaryEntry(keys(index), tempDico(keys(index))))
            End If

            For i As Integer = (Math.Max(index, offsetIndex) + 1) To count - 1
                tempList.Add(New DictionaryEntry(keys(i), tempDico(keys(i))))
                'tempDico.Remove(keys(i))
            Next
            'tempDico.Remove(keys(index))
            'tempDico.Remove(keys(offsetIndex))

            tempDico.Clear()

            For Each entry As DictionaryEntry In tempList
                tempDico.Add(entry.Key, entry.Value)
            Next
            Me.DictionaryValue = tempDico

        End Sub


        Protected Function InjectEditors(ByVal keyEditControl As Control, ByVal valueEditControl As Control) As HtmlContainerControl

            Dim toReturn As New HtmlGenericControl("div")
            Dim c1 As HtmlContainerControl = Nothing
            Dim c2 As HtmlContainerControl = Nothing

            Select Case Me.KeyValueOrientation

                Case Orientation.Horizontal
                    'toReturn = New HtmlTable
                    'Dim table As HtmlTable = CType(toReturn, HtmlTable)

                    'table.CellPadding = 2
                    'c2 = New HtmlTableCell
                    'c3 = New HtmlTableCell

                    'Dim row As New HtmlTableRow()
                    'table.Rows.Add(row)
                    'row.Cells.Add(CType(c2, HtmlTableCell))
                    'row.Cells.Add(CType(c3, HtmlTableCell))
                    c1 = toReturn
                    c2 = toReturn
                Case Orientation.Vertical
                    'toReturn = New HtmlGenericControl("div")

                    c1 = New HtmlGenericControl("div")
                    c2 = New HtmlGenericControl("div")

                    toReturn.Controls.Add(c1)
                    toReturn.Controls.Add(c2)

            End Select




            c1.Controls.Add(keyEditControl)
            c2.Controls.Add(valueEditControl)

            Return toReturn
        End Function

#End Region


        
    End Class

End Namespace
