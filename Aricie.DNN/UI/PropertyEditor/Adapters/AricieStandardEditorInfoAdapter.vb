Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Reflection
Imports Aricie.Services
Imports System.Web.UI.WebControls
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.UI.WebControls.EditorInfos

Namespace UI.WebControls

    Public Class AricieStandardEditorInfoAdapter
        Implements IEditorInfoAdapter

        'Private _StandardAdapter As StandardEditorInfoAdapter


        Protected DataSource As Object
        Protected FieldName As String
        Private _CurrentProperty As PropertyInfo
        Private _AdditionalAttributes As IEnumerable(Of Object)

        Private Shared _EditorClones As New Dictionary(Of String, EditorInfo)

        Public Sub New(objProp As PropertyInfo)
            Me._CurrentProperty = objProp
        End Sub

        Public Sub New(ByVal dataSource As Object, ByVal fieldName As String)
            Me.New(dataSource, fieldName, Nothing)
        End Sub

        Public Sub New(ByVal dataSource As Object, ByVal fieldName As String, ByVal additionalAttributes As IEnumerable(Of Object))
            If dataSource Is Nothing Then
                Throw New ArgumentException("dataSource cannot be null", "dataSource")
            End If
            Me.DataSource = dataSource
            Me.FieldName = fieldName
            If Not ReflectionHelper.GetPropertiesDictionary(Me.DataSource.GetType).TryGetValue(Me.FieldName, _CurrentProperty) Then
                Throw New ArgumentException("fieldName must be a property of type " & Me.DataSource.GetType.Name, "fieldName")
            ElseIf _CurrentProperty.GetIndexParameters().Length > 0 Then
                Throw New ArgumentException("Displayed Property " & _CurrentProperty.Name & " from type " & _CurrentProperty.DeclaringType.Name & " can't have indexing parameters", "fieldName")
            End If
            Me._AdditionalAttributes = additionalAttributes
        End Sub


        

        'Private ReadOnly Property StandardAdapter() As StandardEditorInfoAdapter
        '    Get
        '        If _StandardAdapter Is Nothing Then
        '            Me._StandardAdapter = New StandardEditorInfoAdapter(dataSource, fieldName)
        '        End If
        '        Return _StandardAdapter
        '    End Get
        'End Property





        Public Function CreateEditControl() As EditorInfo Implements IEditorInfoAdapter.CreateEditControl
            Dim currentType As Type
            If Me.DataSource IsNot Nothing Then
                currentType = Me.DataSource.GetType
            Else
                currentType = _CurrentProperty.DeclaringType
            End If

            Dim toReturn As EditorInfo = Nothing
            'Dim cachedEditor As EditorInfo = Nothing '= CacheHelper.GetGlobal(Of EditorInfo)(currentType.FullName, FieldName)
            Dim key As String = currentType.Name & "."c & Me._CurrentProperty.Name
            Dim value As Object = Nothing
            If Me.DataSource IsNot Nothing OrElse _CurrentProperty.GetGetMethod.IsStatic Then
                Try
                    value = Me._CurrentProperty.GetValue(DataSource, Nothing)
                Catch ex As Exception
                    ExceptionHelper.LogException(ex)
                    value = Nothing
                End Try

            End If
            If value IsNot Nothing Then
                key &= "."c & value.GetType.Name
            End If

            If Me._AdditionalAttributes IsNot Nothing Then
                key = Me._AdditionalAttributes.Cast(Of Attribute)().Aggregate(key, Function(current, attr) current & attr.ToString())
            End If

            If Not _EditorClones.TryGetValue(key, toReturn) Then
                toReturn = GetEditorInfo(value)
                SyncLock _EditorClones
                    _EditorClones(key) = toReturn
                End SyncLock
                'CacheHelper.SetGlobal(Of EditorInfo)(toReturn, currentType.FullName, FieldName)
            End If
            toReturn = GetCloneEditor(toReturn, value)
            Return toReturn
        End Function

        Public Function UpdateValue(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateValue
            If _CurrentProperty.CanWrite Then
                If e.Value IsNot Nothing Then
                    Dim eType As Type = e.Value.GetType
                    If e.Changed OrElse ReflectionHelper.AreEqual(e.Value, e.OldValue) Then
                        'If (Not (e.Value Is e.OldValue)) OrElse e.Changed Then
                        Try

                            Dim propType As Type = _CurrentProperty.PropertyType 'ReflectionHelper.GetPropertiesDictionary(Me.DataSource.GetType)(e.Name).PropertyType
                            If Not (eType Is propType) Then
                                Dim objConverter As TypeConverter = TypeDescriptor.GetConverter(propType)
                                If objConverter.CanConvertFrom(eType) Then
                                    e.Value = objConverter.ConvertFrom(e.Value)
                                End If
                            End If



                            _CurrentProperty.SetValue(DataSource, e.Value, Nothing)
                            Return True
                        Catch ex As Exception
                            Throw New ApplicationException(String.Format("Could not assignate value {0} to property {1} in datasource {2}", e.Value.ToString(), _
                                                                        _CurrentProperty.DeclaringType.Name & "."c & _CurrentProperty.Name, ReflectionHelper.GetFriendlyName(DataSource)), ex)
                        End Try
                    End If
                End If
            End If
            
            Return False
        End Function


        Public Function UpdateVisibility(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateVisibility
            Return e.Changed
        End Function

        Public Function GetCloneEditor(ByVal previousEditor As EditorInfo, ByVal objValue As Object) As EditorInfo
            Dim toReturn As New AricieEditorInfo()
            toReturn.Name = previousEditor.Name
            toReturn.Type = previousEditor.Type
            toReturn.Attributes = previousEditor.Attributes
            toReturn.Category = previousEditor.Category

            'If Not _CurrentProperty.CanWrite Then
            '    toReturn.EditMode = PropertyEditorMode.View
            'Else
            '    Dim readOnlyAttributes As Object() = _CurrentProperty.GetCustomAttributes(GetType(IsReadOnlyAttribute), True)
            '    If (readOnlyAttributes.Length > 0) Then
            '        Dim readOnlyMode As IsReadOnlyAttribute = CType(readOnlyAttributes(0), IsReadOnlyAttribute)
            '        If readOnlyMode.IsReadOnly Then
            '            toReturn.EditMode = PropertyEditorMode.View
            '        End If
            '    End If
            'End If
            toReturn.EditMode = previousEditor.EditMode

            'toReturn.EditMode = previousEditor.EditMode
            toReturn.Editor = previousEditor.Editor
            toReturn.Required = previousEditor.Required
            toReturn.ControlStyle = previousEditor.ControlStyle
            toReturn.LabelMode = previousEditor.LabelMode
            toReturn.ResourceKey = previousEditor.ResourceKey
            toReturn.ValidationExpression = previousEditor.ValidationExpression
            toReturn.Visibility = previousEditor.Visibility

            toReturn.Value = objValue

            Return toReturn
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetEditorInfo builds an EditorInfo object for a propoerty
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Private Function GetEditorInfo(value As Object) As EditorInfo

            

            Dim editInfo As New AricieEditorInfo()

            'Get the Name of the property
            editInfo.Name = _CurrentProperty.Name()

         
            editInfo.Value = value


            'Get the type of the property
            editInfo.PropertyType = _CurrentProperty.PropertyType

            'Get the Custom Attributes for the property
            Dim attrList As New List(Of Object)(ReflectionHelper.GetCustomAttributes(_CurrentProperty))

            If Me._AdditionalAttributes IsNot Nothing Then
                attrList.AddRange(Me._AdditionalAttributes)
            End If
            'todo: is this ok?
            attrList.AddRange(ReflectionHelper.GetCustomAttributes(_CurrentProperty.PropertyType))

            editInfo.Attributes = attrList.ToArray()


            editInfo.Category = String.Empty

            If Not _CurrentProperty.CanWrite Then
                editInfo.EditMode = PropertyEditorMode.View
            End If
            editInfo.Editor = "UseSystemType"
            editInfo.Required = False
            editInfo.ControlStyle = New Style
            editInfo.LabelMode = LabelMode.Left

            Dim declaringType As Type = _CurrentProperty.GetAccessors().First.GetBaseDefinition.DeclaringType

            editInfo.ResourceKey = String.Format("{0}_{1}", declaringType.Name, _CurrentProperty.Name)
            editInfo.ValidationExpression = String.Empty

            'Set Visibility
            editInfo.Visibility = UserVisibilityMode.AllUsers

            Me.ParseAttributes(editInfo)

            Return editInfo

        End Function


        Private Sub ParseAttributes(editInfo As EditorInfo)
            For Each objAttribute As Object In editInfo.Attributes
                If TypeOf objAttribute Is CategoryAttribute Then
                    'Get Category Field
                    Dim category As CategoryAttribute = DirectCast(objAttribute, CategoryAttribute)
                    editInfo.Category = category.Category
                ElseIf TypeOf objAttribute Is IsReadOnlyAttribute Then
                    Dim readOnlyMode As IsReadOnlyAttribute = DirectCast(objAttribute, IsReadOnlyAttribute)
                    If readOnlyMode.IsReadOnly Then
                        editInfo.EditMode = PropertyEditorMode.View
                    Else
                        editInfo.EditMode = PropertyEditorMode.Edit
                    End If
                ElseIf TypeOf objAttribute Is EditorAttribute Then
                    Dim editorAttribute As EditorAttribute = DirectCast(objAttribute, EditorAttribute)
                    If editorAttribute.EditorBaseTypeName.Contains("DotNetNuke.UI.WebControls.EditControl") Then
                        editInfo.Editor = editorAttribute.EditorTypeName
                    End If
                ElseIf TypeOf objAttribute Is RequiredAttribute Then
                    Dim required As RequiredAttribute = DirectCast(objAttribute, RequiredAttribute)
                    If required.Required Then
                        editInfo.Required = True
                    End If
                ElseIf TypeOf objAttribute Is ControlStyleAttribute Then
                    Dim attribute As ControlStyleAttribute = DirectCast(objAttribute, ControlStyleAttribute)
                    editInfo.ControlStyle.CssClass = attribute.CssClass
                    editInfo.ControlStyle.Height = attribute.Height
                    editInfo.ControlStyle.Width = attribute.Width
                ElseIf TypeOf objAttribute Is LabelModeAttribute Then
                    Dim mode As LabelModeAttribute = DirectCast(objAttribute, LabelModeAttribute)
                    editInfo.LabelMode = mode.Mode
                ElseIf TypeOf objAttribute Is RegularExpressionValidatorAttribute Then
                    Dim regExAttribute As RegularExpressionValidatorAttribute = DirectCast(objAttribute, RegularExpressionValidatorAttribute)
                    editInfo.ValidationExpression = regExAttribute.Expression
                End If
            Next
        End Sub



    End Class


End Namespace
