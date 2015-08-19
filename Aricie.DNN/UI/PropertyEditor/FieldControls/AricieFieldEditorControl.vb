Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditorInfos
Imports Microsoft.VisualBasic.CompilerServices
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
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Exceptions


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
    Private _ParentEditor As PropertyEditorControl
    Private _ParentModule As PortalModuleBase

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
    Public Property EditorInfo As EditorInfo

    Public ReadOnly Property AricieEditorInfo As AricieEditorInfo
        Get
            Return DirectCast(Me.EditorInfo, AricieEditorInfo)
        End Get
    End Property


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

    Public ReadOnly Property ParentModule() As PortalModuleBase
        Get
            If _ParentModule Is Nothing Then
                _ParentModule = Aricie.Web.UI.ControlHelper.FindParentControlRecursive(Of PortalModuleBase)(Me)
            End If
            Return _ParentModule
        End Get
    End Property


    Public ReadOnly Property ParentEditor() As PropertyEditorControl
        Get
            If Me._ParentEditor Is Nothing Then
                Dim parentControl As PropertyEditorControl = ControlHelper.FindParentControlRecursive(Of PropertyEditorControl)(Me)
                If parentControl IsNot Nothing Then
                    Me._ParentEditor = parentControl
                End If
            End If
            Return Me._ParentEditor
        End Get
    End Property

    Public ReadOnly Property ParentAricieEditor() As AriciePropertyEditorControl
        Get
            If TypeOf Me.ParentEditor Is AriciePropertyEditorControl Then
                Return DirectCast(ParentEditor, AriciePropertyEditorControl)
            End If
            Return Nothing
        End Get
    End Property

#End Region

#Region "Overrides"

    Protected Overrides Sub CreateEditor()
        If Me.EditorDisplayMode = EditorDisplayMode.Table Then
            MyBase.CreateEditor()

        Else
            Dim objEditor As EditorInfo = Me.EditorInfoAdapter.CreateEditControl()

            If (objEditor.EditMode = PropertyEditorMode.Edit) Then
                objEditor.EditMode = Me.EditMode
            End If
            If Not String.IsNullOrEmpty(Me.EditorTypeName) Then
                objEditor.Editor = Me.EditorTypeName
            End If
            If (Me.LabelMode <> LabelMode.Left) Then
                objEditor.LabelMode = Me.LabelMode
            End If
            If Me.Required Then
                objEditor.Required = Me.Required
            End If
            If Not String.IsNullOrEmpty(Me.ValidationExpression) Then
                objEditor.ValidationExpression = Me.ValidationExpression
            End If
            Me.OnItemCreated(New PropertyEditorItemEventArgs(objEditor))
            Me.Visible = objEditor.Visible
            For Each customAttribute As Attribute In objEditor.Attributes
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
            Me.EditorInfo = objEditor
            Me.BuildLtDiv()

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


        'todo
        CreateEditor()
    
        'Set flag so CreateChildConrols should not be invoked later in control's lifecycle
        ChildControlsCreated = True

    End Sub
#End Region

#Region "Events"

    Protected Overrides Sub ValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
        Try
            MyBase.ValueChanged(sender, e)
            If Me.IsDirty Then
                If Me.ParentAricieEditor IsNot Nothing Then
                    'If e.Changed Then
                    'Me.ParentAricieEditor.ItemChanged = True
                    Me.ParentAricieEditor.RootEditor.ItemChanged = True
                    'End If
                End If
            End If
        Catch ex As Exception
            If Me.ParentAricieEditor IsNot Nothing Then
                Me.ParentAricieEditor.ProcessException(ex)
            Else
                Exceptions.ProcessModuleLoadException(Me.ParentModule, ex)
            End If
        End Try
    End Sub

    Protected Overrides Sub RenderChildren(ByVal writer As HtmlTextWriter)
        Dim newHtmlTextWriter As New FieldEditorHtmlWriter(Me, writer, Me._AutoPostBack, Me.PasswordMode)
        MyBase.RenderChildren(newHtmlTextWriter)
    End Sub


#End Region

#Region "Public methods"

    Public Shared Function CreateEditControl(ByVal editorInfo As EditorInfo, ByVal fieldCtl As AricieFieldEditorControl, Optional ByVal container As Control = Nothing) As EditControl

        Dim objEditControl As EditControl

        If editorInfo.Editor = "UseSystemType" Then
            Dim objType As Type
            If editorInfo.Value IsNot Nothing Then 'AndAlso editorInfo.Value.GetType() IsNot objType AndAlso Not ReflectionHelper.CanConvert(objType, editorInfo.Value.GetType()) Then
                objType = editorInfo.Value.GetType()
            Else
                objType = ReflectionHelper.CreateType(editorInfo.Type)
            End If

            Select Case objType.FullName
                Case "System.DateTime"
                    objEditControl = New DateTimeEditControl
                Case "System.Boolean"
                    'If NukeHelper.DnnVersion.Major < 6 OrElse DnnVersion.Major = 7 AndAlso DnnVersion.Minor < 1 Then
                    '    objEditControl = New TrueFalseEditControl()
                    'Else
                    objEditControl = New CheckEditControl
                    'End If
                Case "System.Int32", "System.Int16"
                    objEditControl = New IntegerEditControl

                Case "System.String"
                    'If editorInfo.Value IsNot Nothing AndAlso editorInfo.Value.ToString.Length > 16 Then
                    '    propEditor = New CustomTextEditControl()
                    'Else
                    '    propEditor = New TextEditControl()
                    'End If
                    objEditControl = New CustomTextEditControl()
                Case "System.Version"
                    If NukeHelper.DnnVersion.Major > 5 Then
                        objEditControl = DirectCast(ReflectionHelper.CreateObject("DotNetNuke.UI.WebControls.VersionEditControl, DotNetNuke"), EditControl)
                    Else
                        objEditControl = New TextEditControl(editorInfo.Type)
                    End If
                Case Else

                    If objType.IsEnum Then
                        If ReflectionHelper.GetCustomAttributes(objType).Where(Function(objAttr) TypeOf objAttr Is FlagsAttribute).Any() Then
                            objEditControl = New AricieCheckBoxListEditControl()
                        Else
                            'objEditControl = New EnumEditControl(objType.AssemblyQualifiedName)
                            objEditControl = New AricieEnumEditControl(objType)
                        End If

                    Else

                        'If editorInfo.Value IsNot Nothing AndAlso ReflectionHelper.IsTrueReferenceType(objType) OrElse Not objType.Namespace.StartsWith("System") Then
                        If ReflectionHelper.IsTrueReferenceType(objType) OrElse Not objType.Namespace.StartsWith("System") Then
                            If objType.GetInterface("ICollection") IsNot Nothing Then
                                editorInfo.LabelMode = LabelMode.Top
                                If objType.GetInterface("IDictionary") IsNot Nothing Then
                                    objEditControl = New DictionaryEditControl()
                                Else
                                    objEditControl = New ListEditControl()
                                End If
                            ElseIf objType.GetInterface("IDictionary") IsNot Nothing Then
                                editorInfo.LabelMode = LabelMode.Top
                                objEditControl = New DictionaryEditControl()
                            ElseIf objType Is GetType(CData) Then
                                Dim ctc As New CustomTextEditControl()
                                ctc.Width = 600
                                Dim strValue = Conversions.ToString(editorInfo.Value)
                                ctc.LineCount = RestrictedLineCount(strValue, 3, 200)
                                objEditControl = ctc
                            Else

                                editorInfo.LabelMode = LabelMode.Top
                                objEditControl = New PropertyEditorEditControl()
                            End If
                        Else
                            objEditControl = New TextEditControl(editorInfo.Type)
                        End If
                    End If
            End Select
        Else
            'Use Editor
            Dim editType As Type = ReflectionHelper.CreateType(editorInfo.Editor, False)

            If editType IsNot Nothing Then
                objEditControl = CType(Activator.CreateInstance(editType), EditControl)
            Else
                objEditControl = New TextEditControl(editorInfo.Type)
            End If

        End If


        If editorInfo.Name.Length > 5 Then
            objEditControl.ID = editorInfo.Name.Substring(0, 3) & editorInfo.Name.Substring(editorInfo.Name.Length - 2)
        Else
            objEditControl.ID = editorInfo.Name
        End If


        objEditControl.Name = editorInfo.Name

        objEditControl.EditMode = editorInfo.EditMode
        objEditControl.Required = editorInfo.Required

        objEditControl.Value = editorInfo.Value
        objEditControl.OldValue = editorInfo.Value

        objEditControl.CustomAttributes = editorInfo.Attributes
        If editorInfo.EditMode = PropertyEditorMode.Edit Then
            objEditControl.CssClass = "NormalTextBox"
        Else
            objEditControl.CssClass = "Normal"
        End If


        'AddHandler propEditor.PreRender, AddressOf propEditor_PreRender
        Dim myValidators As IList(Of BaseValidator) = BuildValidators(editorInfo, objEditControl.ID, fieldCtl)
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


        Return objEditControl

    End Function


    Public Overrides Sub Validate()
        If Me.Editor IsNot Nothing Then
            Me._IsValid = Me.Editor.IsValid
        End If
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

    Private Sub BuildLtDiv()

        Dim label As PropertyLabelControl = Nothing

        'Dim oppositeSide As String = Me.GetOppositeSide(editInfo.LabelMode)
        'If (oppositeSide.Length > 0) Then
        '    Dim str3 As String = (("float: " & oppositeSide) & "; width: " & Me.EditControlWidth.ToString)
        '    control.Attributes.Add("style", str3)
        'End If

        Dim ctlEditControl As EditControl = Me.BuildEditor(Me.EditorInfo, Nothing)
        'Me._Editor = ctlEditControl


        If Me.EditorInfo.LabelMode = LabelMode.Top OrElse Me.EditorInfo.LabelMode = LabelMode.Bottom OrElse Me.EditorInfo.LabelMode = LabelMode.None Then
            Me._FullWidth = True
        End If

        If (Me.EditorInfo.LabelMode <> LabelMode.None) Then
            label = Me.BuildLtLabel(Me.EditorInfo)
            label.EditControl = ctlEditControl
        End If
        Dim divEditControl As New HtmlGenericControl("div")

        Dim ctlVisibility As VisibilityControl = Me.BuildVisibility(Me.EditorInfo)
        If (ctlVisibility IsNot Nothing) Then
            ctlVisibility.Attributes.Add("class", "ctRight")
            divEditControl.Controls.Add(ctlVisibility)
        End If
        divEditControl.Controls.Add(ctlEditControl)
        Dim image As Image = Me.BuildRequiredIcon(Me.EditorInfo)
        If (image IsNot Nothing) Then
            divEditControl.Controls.Add(image)
        End If



        If NukeHelper.DnnVersion.Major >= 6 Then

            If label IsNot Nothing Then
                Me.Controls.Add(label)
            End If

            Me.Controls.Add(divEditControl)
        Else
            Dim divLabel As HtmlGenericControl = Nothing


            If label IsNot Nothing Then
                If (Me.EditorInfo.LabelMode <> LabelMode.None) Then
                    divLabel = New HtmlGenericControl("div")
                    divLabel.EnableViewState = False

                    Select Case Me.EditorInfo.LabelMode
                        Case LabelMode.Left
                            divLabel.Attributes.Add("class", "ctLeft")
                        Case LabelMode.Right
                            divLabel.Attributes.Add("class", "ctRight")
                        Case Else
                    End Select


                    '  Dim str2 As String = ("float: " & editInfo.LabelMode.ToString.ToLower)
                    If ((Me.EditorInfo.LabelMode = LabelMode.Left) Or (Me.EditorInfo.LabelMode = LabelMode.Right)) AndAlso Me.LabelWidth <> Unit.Empty Then
                        'str2 = (str2 & "; width: " & Me.LabelWidth.ToString)
                        divLabel.Attributes.Add("style", String.Format("width:{0}", Me.LabelWidth.ToString))
                    End If

                    label = Me.BuildLtLabel(Me.EditorInfo)
                End If


                divLabel.Controls.Add(label)
            End If

            If ((Me.EditorInfo.LabelMode = LabelMode.Left) Or (Me.EditorInfo.LabelMode = LabelMode.Top)) Then
                Me.Controls.Add(divLabel)
                Me.Controls.Add(divEditControl)
            Else
                Me.Controls.Add(divEditControl)
                If (divLabel IsNot Nothing) Then
                    Me.Controls.Add(divLabel)
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

    Public Function BuildLtLabel(ByVal editInfo As EditorInfo) As PropertyLabelControl
        Dim toReturn As New PropertyLabelControl
        toReturn.ID = (editInfo.Name & "_Label")
        toReturn.HelpStyle.CopyFrom(Me.HelpStyle)
        toReturn.LabelStyle.CopyFrom(Me.LabelStyle)
        toReturn.EnableViewState = False
        Dim str As String = TryCast(editInfo.Value, String)
        Select Case Me.HelpDisplayMode
            Case HelpDisplayMode.Never
                toReturn.ShowHelp = False
                Exit Select
            Case HelpDisplayMode.EditOnly
                If Not ((editInfo.EditMode = PropertyEditorMode.Edit) OrElse (editInfo.Required AndAlso String.IsNullOrEmpty(str))) Then
                    toReturn.ShowHelp = False
                    Exit Select
                End If
                toReturn.ShowHelp = True
                Exit Select
            Case HelpDisplayMode.Always
                toReturn.ShowHelp = True
                Exit Select
        End Select
        toReturn.Caption = editInfo.Name
        toReturn.HelpText = editInfo.Name
        toReturn.ResourceKey = editInfo.ResourceKey
        If ((editInfo.LabelMode = LabelMode.Left) Or (editInfo.LabelMode = LabelMode.Right)) Then
            toReturn.Width = Me.LabelWidth
        End If
        If _FullWidth Then
            toReturn.CssClass = (toReturn.CssClass & " aricieFullWidth").Trim()
        End If

        Return toReturn
    End Function

    Public Function BuildEditor(ByVal editInfo As EditorInfo, Optional ByVal container As Control = Nothing) As EditControl
        Dim toReturn As EditControl = CreateEditControl(editInfo, Me, container)
        toReturn.ControlStyle.CopyFrom(Me.EditControlStyle)
        toReturn.LocalResourceFile = Me.LocalResourceFile
        If (editInfo.ControlStyle IsNot Nothing) Then
            toReturn.ControlStyle.CopyFrom(editInfo.ControlStyle)
        End If

        If container Is Nothing Then
            AddHandler toReturn.ValueChanged, New PropertyChangedEventHandler(AddressOf Me.ValueChanged)
            Dim control3 As DNNListEditControl = TryCast(toReturn, DNNListEditControl)
            If (control3 IsNot Nothing) Then
                AddHandler control3.ItemChanged, New PropertyChangedEventHandler(AddressOf Me.ListItemChanged)
            End If
            Me._Editor = toReturn
        End If

        Return toReturn
    End Function

    Private Sub SubValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)

        Dim pe As New PropertyEditorEventArgs(Me.Editor.Name, Me.Editor.Value, Me.Editor.OldValue)
        Me.ValueChanged(sender, pe)
    End Sub


    Protected Function BuildRequiredIcon(ByVal editInfo As EditorInfo) As Image
        Dim image2 As Image = Nothing
        Dim str As String = TryCast(editInfo.Value, String)
        If ((Me.ShowRequired AndAlso editInfo.Required) AndAlso ((editInfo.EditMode = PropertyEditorMode.Edit) OrElse (editInfo.Required AndAlso String.IsNullOrEmpty(str)))) Then
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
