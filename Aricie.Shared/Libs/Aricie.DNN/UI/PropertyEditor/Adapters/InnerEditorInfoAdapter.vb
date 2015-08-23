Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.UI.WebControls.EditorInfos

Namespace UI.WebControls

    Public Class InnerEditorInfoAdapter
        Implements IEditorInfoAdapter


        Private _Name As String

        Private _Value As Object


        'Private _EditorType As String



        Private _EditMode As PropertyEditorMode



        Private _InnerAttributes As Attribute()



        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property




        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal value As Object)
                _Value = value
            End Set
        End Property

        'Public Property EditorType() As String
        '    Get
        '        Return _EditorType
        '    End Get
        '    Set(ByVal value As String)
        '        _EditorType = value
        '    End Set
        'End Property

        Public Property EditMode() As PropertyEditorMode
            Get
                Return _EditMode
            End Get
            Set(ByVal value As PropertyEditorMode)
                _EditMode = value
            End Set
        End Property



        Public Property InnerAttributes() As Attribute()
            Get
                Return _InnerAttributes
            End Get
            Set(ByVal value As Attribute())
                _InnerAttributes = value
            End Set
        End Property


        Public Sub New(ByVal name As String, ByVal objValue As Object, _
                         ByVal editMode As PropertyEditorMode, ByVal innerAttributes As Attribute())
            Me._Name = name
            Me._Value = objValue
            Me._EditMode = editMode
            Me._InnerAttributes = innerAttributes
        End Sub


        Private Function GetEditorInfo() As EditorInfo
            Dim editInfo As New AricieEditorInfo

            'Add the Name 
            editInfo.Name = _Name

            'Add the value object
            editInfo.Value = Me._Value

            'Add the type of the value object
            If Me._Value IsNot Nothing Then
                editInfo.Type = Me._Value.GetType().AssemblyQualifiedName
            End If


            'Add the Custom Attributes 
            editInfo.Attributes = _InnerAttributes

            editInfo.Category = String.Empty
            editInfo.Required = False
            editInfo.ControlStyle = New Style
            editInfo.LabelMode = LabelMode.Left
            If Me._Value IsNot Nothing Then
                editInfo.ResourceKey = String.Format("{0}", Me._Value.GetType.Name)
            End If

            editInfo.ValidationExpression = String.Empty
            editInfo.Visibility = UserVisibilityMode.AllUsers

            editInfo.EditMode = Me._EditMode

            For Each attr As Attribute In InnerAttributes
                If TypeOf attr Is CategoryAttribute Then
                    editInfo.Category = DirectCast(attr, CategoryAttribute).Category
                ElseIf TypeOf attr Is ReadOnlyAttribute Then
                    If DirectCast(attr, ReadOnlyAttribute).IsReadOnly Then
                        editInfo.EditMode = PropertyEditorMode.View
                    End If
                ElseIf TypeOf attr Is EditorAttribute Then
                    editInfo.Editor = DirectCast(attr, EditorAttribute).EditorTypeName
                ElseIf TypeOf attr Is RequiredAttribute Then
                    editInfo.Required = DirectCast(attr, RequiredAttribute).Required
                ElseIf TypeOf attr Is ControlStyleAttribute Then
                    Dim ctrStyleAttr As ControlStyleAttribute = DirectCast(attr, ControlStyleAttribute)
                    editInfo.ControlStyle.CssClass = ctrStyleAttr.CssClass
                    editInfo.ControlStyle.Height = ctrStyleAttr.Height
                    editInfo.ControlStyle.Width = ctrStyleAttr.Width
                ElseIf TypeOf attr Is LabelModeAttribute Then
                    editInfo.LabelMode = DirectCast(attr, LabelModeAttribute).Mode
                ElseIf TypeOf attr Is RegularExpressionValidatorAttribute Then
                    editInfo.ValidationExpression = DirectCast(attr, RegularExpressionValidatorAttribute).Expression
                End If
            Next
            If editInfo.Editor = "" Then
                'If ReflectionHelper.IsTrueReferenceType(Me.Value.GetType) Then
                '    editInfo.Editor = ReflectionHelper.GetSafeTypeName(GetType(PropertyEditorEditControl)) '"Aricie.DNN.UI.WebControls.EditControls.PropertyEditorEditControl, Aricie.DNN"
                'Else
                '    editInfo.Editor = "UseSystemType"
                'End If
                editInfo.Editor = "UseSystemType"
            End If
            'If Not String.IsNullOrEmpty(EditorType) Then
            '    editInfo.Editor = EditorType
            'End If


            Return editInfo
        End Function



        Public Function CreateEditControl() As EditorInfo Implements IEditorInfoAdapter.CreateEditControl
            Return Me.GetEditorInfo()
        End Function

        Public Function UpdateValue(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateValue
            If e.Changed Then 'OrElse Not e.OldValue Is e.Value
                Me._Value = e.Value
                Return True
            End If
            Return False
        End Function

        Public Function UpdateVisibility(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateVisibility
            Return e.Changed
        End Function
    End Class


End Namespace
