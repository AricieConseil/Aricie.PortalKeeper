Imports DotNetNuke.UI.WebControls
Imports System.Reflection
Imports System.Globalization
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.UI.WebControls.EditorInfos

Namespace UI.WebControls
    Public Class CollectionEditorInfoAdapter
        Implements IEditorInfoAdapter

        ' Methods
        Public Sub New(ByVal dataSource As Object, ByVal fieldName As String, ByVal field As CollectionField)

            Me._dataSource = dataSource
            Me._fieldName = fieldName
            Me._field = field

        End Sub

        Private Shared Function GetProperty(ByVal dataSource As Object, ByVal fieldName As String) As PropertyInfo
            If Not dataSource Is Nothing Then
                Dim Bindings As BindingFlags = BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.Static
                Dim objProperty As PropertyInfo = dataSource.GetType().GetProperty(fieldName, Bindings)
                Return objProperty
            Else
                Return Nothing
            End If
        End Function

        Public Function CreateEditControl() As EditorInfo Implements IEditorInfoAdapter.CreateEditControl

            Dim editInfo As EditorInfo = Nothing

            Dim objProperty As PropertyInfo = GetProperty(_dataSource, _fieldName)
            If Not objProperty Is Nothing Then
                editInfo = GetEditorInfo(objProperty)
            End If

            Return editInfo
        End Function

        Private Function GetEditorInfo(ByVal objProperty As PropertyInfo) As EditorInfo
            If (Me._editInfo Is Nothing) Then
                Me._editInfo = New AricieEditorInfo
                Me._editInfo.Name = objProperty.Name
                Me._editInfo.Value = objProperty.GetValue(_dataSource, Nothing)
                Me._editInfo.Type = objProperty.PropertyType.AssemblyQualifiedName
                Me._editInfo.EditMode = PropertyEditorMode.Edit
                Me._editInfo.Editor = "Aricie.DNN.UI.WebControls.EditControls.CollectionEditControl, Aricie.DNN"
                Me._editInfo.Attributes = _
                    New Object() {_field.DataSourceId, _field.DataTextField, _field.DataValueField}
                Me._editInfo.LabelMode = LabelMode.Left
                Me._editInfo.Required = _field.Required
                Me._editInfo.ResourceKey = _
                    String.Format(CultureInfo.InvariantCulture, "{0}_{1}", Me._dataSource.GetType.Name, _
                                   objProperty.Name)
                Me._editInfo.Visibility = UserVisibilityMode.AllUsers
                If Not String.IsNullOrEmpty(Me._field.Type) Then
                    Me._editInfo.Editor = Me._field.Type
                End If
                If Not Me._field.Enabled Then
                    Me._editInfo.EditMode = PropertyEditorMode.View
                    Me._editInfo.Required = False
                End If
            End If
            Return Me._editInfo
        End Function

        Public Function UpdateValue(ByVal e As PropertyEditorEventArgs) As Boolean _
            Implements IEditorInfoAdapter.UpdateValue

            Dim changed As Boolean = e.Changed
            Dim oldValue As Object = e.OldValue
            Dim newValue As Object = e.Value
            Dim _IsDirty As Boolean = False

            'Update the DataSource
            If Not _dataSource Is Nothing Then
                Dim objProperty As PropertyInfo = _dataSource.GetType().GetProperty(e.Name)
                If Not objProperty Is Nothing Then
                    If (Not (newValue Is oldValue)) Or changed Then
                        objProperty.SetValue(_dataSource, newValue, Nothing)
                        _IsDirty = True
                    End If
                End If
            End If

            Return _IsDirty

        End Function

        Public Function UpdateVisibility(ByVal e As PropertyEditorEventArgs) As Boolean _
            Implements IEditorInfoAdapter.UpdateVisibility

            Dim _IsDirty As Boolean = False
            Return _IsDirty

        End Function

        ' Fields
        Private _dataSource As Object
        Private _editInfo As EditorInfo
        Private _fieldName As String
        Private _field As CollectionField
    End Class
End Namespace
