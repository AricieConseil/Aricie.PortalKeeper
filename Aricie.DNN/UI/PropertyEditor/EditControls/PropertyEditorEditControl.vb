Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Globalization
Imports System.ComponentModel
Imports Aricie.DNN.Security.Trial

Namespace UI.WebControls.EditControls
    Public Class PropertyEditorEditControl
        Inherits AricieEditControl
        Implements INamingContainer

        Private _InnerEditor As PropertyEditorControl
        Private _width As Unit
        Private _labelWidth As Unit
        Private _editControlWidth As Unit
        Private _EnableExports As Nullable(Of Boolean)

        Private _ActionButton As ActionButtonInfo

        Public ReadOnly Property InnerEditor() As PropertyEditorControl
            Get
                Return _InnerEditor
            End Get
        End Property


        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            Me.EnsureChildControls()
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            'Me.EnsureChildControls()
            Dim args As New PropertyEditorEventArgs(Me.Name)
            args.Value = Me._InnerEditor.DataSource
            args.OldValue = Me.OldValue
            'args.Changed = (Not args.Value Is args.OldValue)
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Protected Overrides Sub OnAttributesChanged()
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In CustomAttributes
                    If TypeOf attribute Is FieldStyleAttribute Then
                        _width = DirectCast(attribute, FieldStyleAttribute).Width
                        _labelWidth = DirectCast(attribute, FieldStyleAttribute).LabelWidth
                        _editControlWidth = DirectCast(attribute, FieldStyleAttribute).EditControlWidth
                    ElseIf TypeOf attribute Is EnableExportsAttribute Then
                        Me._EnableExports = DirectCast(attribute, EnableExportsAttribute).Enabled
                    ElseIf TypeOf attribute Is ActionButtonAttribute Then
                        Me._ActionButton = ActionButtonInfo.FromAttribute(DirectCast(attribute, ActionButtonAttribute))
                    End If
                Next
            End If
        End Sub

        Protected Overridable Function GetNewEditor() As PropertyEditorControl
            Return New AriciePropertyEditorControl()
        End Function

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            If Me.Value IsNot Nothing Then
                Me._InnerEditor = GetNewEditor()
                If TypeOf Me._InnerEditor Is AriciePropertyEditorControl Then
                    Dim aEditor As AriciePropertyEditorControl = DirectCast(Me._InnerEditor, AriciePropertyEditorControl)
                    If Me.ParentAricieField IsNot Nothing Then
                        If Me.ParentAricieField.IsHidden Then
                            aEditor.IsHidden = True
                        End If
                    End If
                    If Me.ParentAricieEditor IsNot Nothing Then
                        aEditor.DisableExports = ParentAricieEditor.DisableExports
                        aEditor.PropertyDepth = Me.ParentAricieEditor.PropertyDepth + 1
                        aEditor.EnabledOnDemandSections = ParentAricieEditor.EnabledOnDemandSections AndAlso Me.OndemandEnabled
                        aEditor.TrialStatus = ParentAricieEditor.TrialStatus
                    Else
                        aEditor.PropertyDepth = 0
                    End If
                    If Me._EnableExports.HasValue Then
                        aEditor.DisableExports = Not Me._EnableExports.Value
                    End If
                End If

                Me._InnerEditor.ID = "pe"
                If Me.ParentAricieField IsNot Nothing _
                        AndAlso TypeOf (Me.ParentAricieField.Editor) Is CollectionEditControl _
                        AndAlso CType(Me.ParentAricieField.Editor, CollectionEditControl).DisplayStyle = CollectionDisplayStyle.Accordion Then
                    Me.Controls.Add(Me._InnerEditor)
                Else
                    Dim myPeIco As IconActionControl
                    Dim strCssClass As String = "odd"
                    If _ActionButton IsNot Nothing Then
                        myPeIco = New IconActionControl()
                        myPeIco.ActionItem = _ActionButton.IconAction
                        myPeIco.Text = Me.Value.GetType.Name
                        myPeIco.ResourceKey = Me.Value.GetType.Name & "_Title"
                        Dim ariciePropCt As AriciePropertyEditorControl = TryCast(_InnerEditor, AriciePropertyEditorControl)
                        If ariciePropCt IsNot Nothing Then
                            If (Not ariciePropCt Is Nothing) Then
                                If (ariciePropCt.PropertyDepth Mod 2 = 0) Then
                                    strCssClass = "even"
                                End If
                            End If
                        End If
                    End If

                    Dim subPE As Control = AddSubDiv(Me._InnerEditor, strCssClass)
                    If myPeIco IsNot Nothing Then
                        subPE.Controls.AddAt(0, myPeIco)
                    End If

                    Me.Controls.Add(subPE)
                End If


                If Not _width = Unit.Empty Then
                    Me._InnerEditor.Width = _width
                Else
                    'quel est l'interet de ce code ? un div prend automatiquement 100% de son conteneur.
                    'Dim newValue As Double = Math.Round(Me.ParentEditor.Width.Value - 20)
                    'Select Case Me.ParentEditor.Width.Type
                    '    Case UnitType.Percentage
                    '        Me._InnerEditor.Width = Unit.Percentage(newValue)
                    '    Case UnitType.Point
                    '        Me._InnerEditor.Width = Unit.Point(Convert.ToInt32(newValue))
                    '    Case UnitType.Pixel
                    '        Me._InnerEditor.Width = Unit.Pixel(Convert.ToInt32(newValue))
                    '    Case Else
                    '        Me._InnerEditor.Width = Unit.Parse(Me.ParentEditor.Width.ToString.Replace( _
                    '                                           Me.ParentEditor.Width.Value.ToString( _
                    '                                           CultureInfo.InvariantCulture), _
                    '                                           Convert.ToInt32(newValue).ToString( _
                    '                                        CultureInfo.InvariantCulture)))
                    'End Select

                End If
                'If Me.ParentEditor.CssClass = "ItemEven" Then
                '    Me._InnerEditor.CssClass = "ItemOdd"
                'Else
                '    Me._InnerEditor.CssClass = "ItemEven"
                'End If

                'Me._InnerEditor.CssClass = 

                Me._InnerEditor.LabelWidth = CType(IIf(_labelWidth = Unit.Empty, Me.ParentEditor.LabelWidth, _labelWidth), Unit)
                Me._InnerEditor.EditControlWidth = CType(IIf(_editControlWidth = Unit.Empty, Me.ParentEditor.EditControlWidth, _editControlWidth), Unit)
                Me._InnerEditor.EnableClientValidation = Me.ParentEditor.EnableClientValidation
                Me._InnerEditor.ErrorStyle.CssClass = Me.ParentEditor.ErrorStyle.CssClass
                Me._InnerEditor.GroupHeaderStyle.CssClass = Me.ParentEditor.GroupHeaderStyle.CssClass
                Me._InnerEditor.GroupHeaderIncludeRule = Me.ParentEditor.GroupHeaderIncludeRule
                Me._InnerEditor.HelpStyle.CssClass = Me.ParentEditor.HelpStyle.CssClass
                Me._InnerEditor.LabelStyle.CssClass = Me.ParentEditor.LabelStyle.CssClass
                Me._InnerEditor.VisibilityStyle.CssClass = Me.ParentEditor.VisibilityStyle.CssClass
                Me._InnerEditor.GroupByMode = Me.ParentEditor.GroupByMode
                Me._InnerEditor.DisplayMode = Me.ParentEditor.DisplayMode
                Me._InnerEditor.EditMode = Me.EditMode
                Me._InnerEditor.LocalResourceFile = Me.LocalResourceFile
                Me._InnerEditor.HelpDisplayMode = Me.ParentEditor.HelpDisplayMode
                Me._InnerEditor.ShowRequired = Me.ParentEditor.ShowRequired
                Me._InnerEditor.ShowVisibility = Me.ParentEditor.ShowVisibility
                Me._InnerEditor.SortMode = Me.ParentEditor.SortMode



                AddHandler Me._InnerEditor.ItemCreated, AddressOf Me.InnerEditor_ItemCreatedEventHandler



                'AddHandler _datalist.ItemDataBound, AddressOf DatalistItemDataBound
                'AddHandler _datalist.ItemCommand, AddressOf DatalistItemCommand

                'Me._InnerEditor.DataSource = Value
                'Me._InnerEditor.DataBind()

                Me.DataBind()

                'FormHelper.AddSection(Me, Me._InnerEditor, Me.Name)
            End If


        End Sub

        Private Sub InnerEditor_ItemCreatedEventHandler(ByVal sender As Object, ByVal e As PropertyEditorItemEventArgs)

            'Me.ParentField.
            Me.OnItemChanged(sender, New PropertyEditorEventArgs(e.Editor.Name))

        End Sub

        Public Overrides Sub DataBind()

            If Me.Value IsNot Nothing Then
                Me._InnerEditor.DataSource = Me.Value
                Me._InnerEditor.DataBind()
                'AddHandler _InnerEditor.ItemCreated, New EditorCreatedEventHandler(AddressOf Me.EditorItemCreated)
                'MyBase.DataBind()
            End If

        End Sub


        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
            Return False
        End Function


        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub
        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

        Public Overrides Sub EnforceTrialMode(ByVal mode As TrialPropertyMode)
            MyBase.EnforceTrialMode(mode)
            For Each subField As AricieFieldEditorControl In Me.InnerEditor.Fields
                If TypeOf subField.Editor Is Aricie.DNN.UI.WebControls.EditControls.CollectionEditControl Then
                    DirectCast(subField.Editor, CollectionEditControl).EnforceTrialMode(mode)
                End If
            Next
        End Sub
    End Class

End Namespace
