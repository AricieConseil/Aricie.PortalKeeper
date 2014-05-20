Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services
Imports Aricie.Web.UI
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Localization


Public Class AricieFieldEditorControl
    Inherits FieldEditorControl





    Public Sub New()
        MyBase.New()
        Me.Validators = New ValidatorCollection
        'Me._ValidationExpression = Null.NullString
        Me._IsValid = True
        Me._Validated = False

    End Sub

    Private _IsHidden As Boolean
    Private _AutoPostBack As Boolean
    Private _PasswordMode As Boolean
    Private _FullWidth As Boolean = False

    Private _IsValid As Boolean
    Private _Validated As Boolean
    Private _Validators As ValidatorCollection
    Private _Editor As EditControl

#Region "Properties"


    Public Property IsHidden() As Boolean
        Get
            Return _IsHidden
        End Get
        Set(ByVal value As Boolean)
            _IsHidden = value
        End Set
    End Property



    Public Property AutoPostBack() As Boolean
        Get
            Return _AutoPostBack
        End Get
        Set(ByVal value As Boolean)
            _AutoPostBack = value
        End Set
    End Property

    Public Property PasswordMode() As Boolean
        Get
            Return _PasswordMode
        End Get
        Set(ByVal value As Boolean)
            _PasswordMode = value
        End Set
    End Property

    'Public Sub OnItemChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
    '    'Me.ListItemChanged(sender, )
    'End Sub

    Public Overloads ReadOnly Property Editor() As EditControl
        Get
            Return _Editor
        End Get
    End Property

    Public Overloads ReadOnly Property IsValid() As Boolean
        Get
            If Not Me._Validated Then
                Me.Validate()
            End If
            Return Me._IsValid

        End Get

    End Property

    Public Property Validators() As ValidatorCollection
        Get
            Return _Validators
        End Get
        Set(ByVal value As ValidatorCollection)
            _Validators = value
        End Set
    End Property


#End Region

#Region "Overrides"

    Protected Overrides Sub CreateEditor()
        If Me.EditorDisplayMode = EditorDisplayMode.Table Then
            MyBase.CreateEditor()

        Else
            Dim editor As EditorInfo = Me.EditorInfoAdapter.CreateEditControl

            If (editor.EditMode = PropertyEditorMode.Edit) Then
                editor.EditMode = Me.EditMode
            End If
            If Not String.IsNullOrEmpty(Me.EditorTypeName) Then
                editor.Editor = Me.EditorTypeName
            End If
            If (Me.LabelMode <> LabelMode.Left) Then
                editor.LabelMode = Me.LabelMode
            End If
            If Me.Required Then
                editor.Required = Me.Required
            End If
            If Not String.IsNullOrEmpty(Me.ValidationExpression) Then
                editor.ValidationExpression = Me.ValidationExpression
            End If
            Me.OnItemCreated(New PropertyEditorItemEventArgs(editor))
            Me.Visible = editor.Visible
            For Each customAttribute As Attribute In editor.Attributes
                If TypeOf customAttribute Is AutoPostBackAttribute Then
                    Me._AutoPostBack = True
                End If
                If TypeOf customAttribute Is PasswordModeAttribute Then
                    Me._PasswordMode = True
                End If
                If TypeOf customAttribute Is CollectionEditorAttribute Or TypeOf customAttribute Is InnerPropertyEditorAttribute Then
                    Me._FullWidth = True
                End If

            Next

            Me.BuildLtDiv(editor)



        End If



    End Sub


    Public Overrides Sub DataBind()

        'Invoke OnDataBinding so DataBinding Event is raised
        MyBase.OnDataBinding(EventArgs.Empty)

        'Clear Existing Controls
        Controls.Clear()

        ''Clear Child View State as controls will be loaded from DataSource
        'ClearChildViewState()

        'Start Tracking ViewState
        TrackViewState()

        'Create the editor
        CreateEditor()

        'Set flag so CreateChildConrols should not be invoked later in control's lifecycle
        ChildControlsCreated = True

    End Sub
#End Region

#Region "Events"

    Protected Overrides Sub ValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
        MyBase.ValueChanged(sender, e)
    End Sub

    Protected Overrides Sub RenderChildren(ByVal writer As HtmlTextWriter)
        Dim newHtmlTextWriter As New FieldEditorHtmlWriter(Me, writer, Me._AutoPostBack, Me.PasswordMode)
        MyBase.RenderChildren(newHtmlTextWriter)
    End Sub


#End Region

#Region "Public methods"

    Public Shared Function CreateEditControl(ByVal editorInfo As EditorInfo, ByVal fieldCtl As AricieFieldEditorControl, Optional ByVal container As Control = Nothing) As EditControl

        Dim propEditor As EditControl

        If editorInfo.Editor = "UseSystemType" Then
            Dim objType As Type = ReflectionHelper.CreateType(editorInfo.Type)
            'Use System Type

            Select Case objType.FullName
                Case "System.DateTime"
                    propEditor = New DateTimeEditControl
                Case "System.Boolean"
                    propEditor = New CheckEditControl
                Case "System.Int32", "System.Int16"
                    propEditor = New IntegerEditControl

                Case "System.String"
                    'If editorInfo.Value IsNot Nothing AndAlso editorInfo.Value.ToString.Length > 16 Then
                    '    propEditor = New CustomTextEditControl()
                    'Else
                    '    propEditor = New TextEditControl()
                    'End If
                    propEditor = New CustomTextEditControl()

                Case Else
                    If objType.IsEnum Then
                        propEditor = New EnumEditControl(editorInfo.Type)
                    ElseIf editorInfo.Value IsNot Nothing AndAlso ReflectionHelper.IsTrueReferenceType(objType) OrElse Not objType.Namespace.StartsWith("System") Then
                        editorInfo.LabelMode = LabelMode.Top
                        If objType.GetInterface("ICollection") IsNot Nothing Then
                            If objType.GetInterface("IDictionary") IsNot Nothing Then
                                propEditor = New DictionaryEditControl()
                            Else
                                propEditor = New ListEditControl()
                            End If
                        ElseIf objType.GetInterface("IDictionary") IsNot Nothing Then
                            propEditor = New DictionaryEditControl()
                        Else
                            propEditor = New PropertyEditorEditControl()
                        End If
                    Else
                        propEditor = New TextEditControl(editorInfo.Type)
                    End If
            End Select
        Else
            'Use Editor
            Dim editType As Type = ReflectionHelper.CreateType(editorInfo.Editor)
            propEditor = CType(Activator.CreateInstance(editType), EditControl)
        End If


        'propEditor.ID = editorInfo.Name
        propEditor.ID = editorInfo.Name.Substring(0, 3) & editorInfo.Name.Substring(editorInfo.Name.Length - 2)
        propEditor.Name = editorInfo.Name

        propEditor.EditMode = editorInfo.EditMode
        propEditor.Required = editorInfo.Required

        propEditor.Value = editorInfo.Value
        propEditor.OldValue = editorInfo.Value

        propEditor.CustomAttributes = editorInfo.Attributes
        propEditor.CssClass = "NormalTextBox"

        'AddHandler propEditor.PreRender, AddressOf propEditor_PreRender
        Dim myValidators As IList(Of BaseValidator) = BuildValidators(editorInfo, propEditor.ID, fieldCtl)
        For Each item As BaseValidator In myValidators
            If container Is Nothing Then
                fieldCtl.Validators.Add(item)
            Else
                container.Controls.Add(item)
            End If
        Next
        'For Each item As BaseValidator In myValidators
        '    propEditor.Controls.Add(item)
        'Next


        Return propEditor

    End Function


    Public Overrides Sub Validate()
        Me._IsValid = Me.Editor.IsValid
        If Me._IsValid Then
            'Dim enumerator As IEnumerator = Me.Validators.GetEnumerator
            'For Each myValidator As System.Web.UI.IValidator In Me.Validators
            '    myValidator.Validate()
            '    If Not myValidator.IsValid Then
            '        Me._IsValid = False
            '        Exit Sub
            '    End If
            'Next
            Dim vals As List(Of BaseValidator) = FindControlsRecursive(Of BaseValidator)(Me)
            For Each objVal As BaseValidator In vals
                objVal.Validate()

                If Not objVal.IsValid Then
                    Me._IsValid = False
                    Exit For
                End If

            Next

            Me._Validated = True
        End If
    End Sub


    Public Sub ValidationNeeded()
        _Validated = False
    End Sub

#End Region

#Region "private Methods"

    Private Sub BuildLtDiv(ByVal editInfo As EditorInfo)
        Dim child As HtmlGenericControl = Nothing
        Dim label As PropertyLabelControl = Nothing




        'Dim oppositeSide As String = Me.GetOppositeSide(editInfo.LabelMode)
        'If (oppositeSide.Length > 0) Then
        '    Dim str3 As String = (("float: " & oppositeSide) & "; width: " & Me.EditControlWidth.ToString)
        '    control.Attributes.Add("style", str3)
        'End If

        Dim ctlEditControl As EditControl = Me.BuildEditor(editInfo)

        If editInfo.LabelMode = LabelMode.Top OrElse editInfo.LabelMode = LabelMode.Bottom OrElse editInfo.LabelMode = LabelMode.None Then
            Me._FullWidth = True
        End If

        If (editInfo.LabelMode <> LabelMode.None) Then
            child = New HtmlGenericControl("div")
            child.EnableViewState = False

            Select Case editInfo.LabelMode
                Case LabelMode.Left
                    child.Attributes.Add("class", "ctLeft")
                Case LabelMode.Right
                    child.Attributes.Add("class", "ctRight")
                Case Else
            End Select


            '  Dim str2 As String = ("float: " & editInfo.LabelMode.ToString.ToLower)
            If ((editInfo.LabelMode = LabelMode.Left) Or (editInfo.LabelMode = LabelMode.Right)) AndAlso Me.LabelWidth <> Unit.Empty Then
                'str2 = (str2 & "; width: " & Me.LabelWidth.ToString)
                child.Attributes.Add("style", String.Format("width:{0}", Me.LabelWidth.ToString))
            End If

            label = Me.BuildLtLabel(editInfo)
        End If
        Dim control As New HtmlGenericControl("div")

        Dim ctlVisibility As VisibilityControl = Me.BuildVisibility(editInfo)
        If (ctlVisibility IsNot Nothing) Then
            ctlVisibility.Attributes.Add("class", "ctRight")
            control.Controls.Add(ctlVisibility)
        End If
        control.Controls.Add(ctlEditControl)
        Dim image As Image = Me.BuildRequiredIcon(editInfo)
        If (image IsNot Nothing) Then
            control.Controls.Add(image)
        End If



        If NukeHelper.DnnVersion.Major >= 6 Then

            If label IsNot Nothing Then
                Me.Controls.Add(label)
            End If

            Me.Controls.Add(control)
        Else
            If label IsNot Nothing Then
                child.Controls.Add(label)
            End If

            If ((editInfo.LabelMode = LabelMode.Left) Or (editInfo.LabelMode = LabelMode.Top)) Then
                Me.Controls.Add(child)
                Me.Controls.Add(control)
            Else
                Me.Controls.Add(control)
                If (child IsNot Nothing) Then
                    Me.Controls.Add(child)
                End If
            End If
        End If

        'Me.BuildValidators(editInfo, ctlEditControl.ID)
        If (Me.Validators.Count > 0) Then
            For Each myValidator As BaseValidator In Me.Validators
                'myValidator.Width = Me.Width
                Me.Controls.Add(myValidator)

            Next

        End If
    End Sub

    Protected Function BuildLtLabel(ByVal editInfo As EditorInfo) As PropertyLabelControl
        Dim control2 As New PropertyLabelControl
        control2.ID = (editInfo.Name & "_Label")
        control2.HelpStyle.CopyFrom(Me.HelpStyle)
        control2.LabelStyle.CopyFrom(Me.LabelStyle)
        control2.EnableViewState = False
        Dim str As String = TryCast(editInfo.Value, String)
        Select Case Me.HelpDisplayMode
            Case HelpDisplayMode.Never
                control2.ShowHelp = False
                Exit Select
            Case HelpDisplayMode.EditOnly
                If Not ((editInfo.EditMode = PropertyEditorMode.Edit) Or (editInfo.Required And String.IsNullOrEmpty(str))) Then
                    control2.ShowHelp = False
                    Exit Select
                End If
                control2.ShowHelp = True
                Exit Select
            Case HelpDisplayMode.Always
                control2.ShowHelp = True
                Exit Select
        End Select
        control2.Caption = editInfo.Name
        control2.HelpText = editInfo.Name
        control2.ResourceKey = editInfo.ResourceKey
        If ((editInfo.LabelMode = LabelMode.Left) Or (editInfo.LabelMode = LabelMode.Right)) Then
            control2.Width = Me.LabelWidth
        End If
        If _FullWidth Then
            control2.CssClass &= " aricieFullWidth"
        End If

        Return control2
    End Function

    Protected Function BuildEditor(ByVal editInfo As EditorInfo) As EditControl
        Dim control2 As EditControl = CreateEditControl(editInfo, Me)
        control2.ControlStyle.CopyFrom(Me.EditControlStyle)
        control2.LocalResourceFile = Me.LocalResourceFile
        If (Not editInfo.ControlStyle Is Nothing) Then
            control2.ControlStyle.CopyFrom(editInfo.ControlStyle)
        End If
        AddHandler control2.ValueChanged, New PropertyChangedEventHandler(AddressOf Me.ValueChanged)
        If TypeOf control2 Is DNNListEditControl Then
            Dim control3 As DNNListEditControl = DirectCast(control2, DNNListEditControl)
            AddHandler control3.ItemChanged, New PropertyChangedEventHandler(AddressOf Me.ListItemChanged)
        End If
        Me._Editor = control2
        Return control2
    End Function


    Protected Function BuildRequiredIcon(ByVal editInfo As EditorInfo) As Image
        Dim image2 As Image = Nothing
        Dim str As String = TryCast(editInfo.Value, String)
        If ((Me.ShowRequired AndAlso editInfo.Required) AndAlso ((editInfo.EditMode = PropertyEditorMode.Edit) Or (editInfo.Required And String.IsNullOrEmpty(str)))) Then
            image2 = New Image
            image2.EnableViewState = False
            If (Me.RequiredUrl = Null.NullString) Then
                image2.ImageUrl = "~/images/required.gif"
            Else
                image2.ImageUrl = Me.RequiredUrl
            End If
            image2.Attributes.Add("resourcekey", (editInfo.ResourceKey & ".Required"))
        End If
        Return image2
    End Function

    'Protected Shared Sub BuildValidators(ByVal editInfo As EditorInfo, ByVal targetId As String, ByVal fieldCtl As AricieFieldEditorControl)
    Protected Shared Function BuildValidators(ByVal editInfo As EditorInfo, ByVal targetId As String, ByVal fieldCtl As AricieFieldEditorControl) As IList(Of BaseValidator)
        Dim toReturn As New List(Of BaseValidator)
        If editInfo.Required Then
            Dim validator As New RequiredFieldValidator
            validator.ID = (Guid.NewGuid().ToString().Replace("-"c, "") & "_Req")
            validator.ControlToValidate = targetId
            validator.Display = ValidatorDisplay.Dynamic
            validator.ControlStyle.CopyFrom(fieldCtl.ErrorStyle)
            validator.EnableClientScript = fieldCtl.EnableClientValidation
            validator.Attributes.Add("resourcekey", (editInfo.ResourceKey & ".Required"))
            validator.ErrorMessage = (editInfo.Name & " is Required")
            validator.EnableViewState = False
            'fieldCtl.Validators.Add(validator)
            toReturn.Add(validator)
        End If
        If (editInfo.ValidationExpression <> "") Then
            Dim validator2 As New RegularExpressionValidator
            validator2.ID = (Guid.NewGuid().ToString().Replace("-"c, "") & "_RegEx")
            validator2.ControlToValidate = targetId
            validator2.ValidationExpression = editInfo.ValidationExpression
            validator2.Display = ValidatorDisplay.Dynamic
            validator2.ControlStyle.CopyFrom(fieldCtl.ErrorStyle)
            validator2.EnableClientScript = fieldCtl.EnableClientValidation
            validator2.Attributes.Add("resourcekey", (editInfo.ResourceKey & ".Validation"))
            validator2.ErrorMessage = (editInfo.Name & " is Invalid")
            validator2.EnableViewState = False
            'fieldCtl.Validators.Add(validator2)
            toReturn.Add(validator2)
        End If

        Return toReturn
    End Function

    Protected Function BuildVisibility(ByVal editInfo As EditorInfo) As VisibilityControl
        Dim control2 As VisibilityControl = Nothing
        If Me.ShowVisibility Then
            control2 = New VisibilityControl
            control2.ID = (Me.ID & "_vis")
            control2.Caption = Localization.GetString("Visibility")
            control2.Name = editInfo.Name
            control2.Value = editInfo.Visibility
            control2.ControlStyle.CopyFrom(Me.VisibilityStyle)
            control2.EnableViewState = False
            AddHandler control2.VisibilityChanged, New PropertyChangedEventHandler(AddressOf Me.VisibilityChanged)
        End If
        Return control2
    End Function



#End Region





    'Private Shared Sub propEditor_PreRender(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim myEditCtl As EditControl = DirectCast(sender, EditControl)


    'End Sub













End Class
