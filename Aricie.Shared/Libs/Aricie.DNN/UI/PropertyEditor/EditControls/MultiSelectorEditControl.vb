Imports Aricie.DNN.UI.Attributes
Imports System.Web.UI
Imports Aricie.Services
Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditControls

    Public Class MultiSelectorEditControl
        Inherits AricieEditControl

#Region "Private members"

        Private _multiSelectorType As String = ""
        Private _textField As String = ""
        Private _valueField As String = ""
        Private WithEvents _multiSelector As MultiSelectorControlBase
        Private _alreadyChanged As Boolean = False
        Private _exclusiveSelector As Boolean
        Private _invertExclusion As Boolean
        Private _exclusiveScopeControlId As String

#End Region

#Region "Public properties"

        Public Property MultiSelectorType() As String
            Get
                Return _multiSelectorType
            End Get
            Set(ByVal value As String)
                _multiSelectorType = value
            End Set
        End Property

        Public Property TextField() As String
            Get
                Return _textField
            End Get
            Set(ByVal value As String)
                _textField = value
            End Set
        End Property

        Public Property ValueField() As String
            Get
                Return _valueField
            End Get
            Set(ByVal value As String)
                _valueField = value
            End Set
        End Property

        Public Property ExclusiveSelector() As Boolean
            Get
                Return _exclusiveSelector
            End Get
            Set(ByVal value As Boolean)
                _exclusiveSelector = value
            End Set
        End Property

        Public Property InvertExclusion() As Boolean
            Get
                Return _invertExclusion
            End Get
            Set(ByVal value As Boolean)
                _invertExclusion = value
            End Set
        End Property

        Public Property ExclusiveScopeControlId() As String
            Get
                Return _exclusiveScopeControlId
            End Get
            Set(ByVal value As String)
                _exclusiveScopeControlId = value
            End Set
        End Property

#End Region

#Region "Events"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            Me.EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()

            Try
                _multiSelector = DirectCast(ReflectionHelper.CreateObject(MultiSelectorType), MultiSelectorControlBase)
                _multiSelector.EnableViewState = True
                _multiSelector.ID = "ckl" & _multiSelector.GetType.Name

                Controls.Add(_multiSelector)

                _multiSelector.DataTextField = TextField
                _multiSelector.DataValueField = ValueField

                If ExclusiveScopeControlId <> String.Empty Then
                    _multiSelector.ExclusiveSelector = ExclusiveSelector
                    _multiSelector.InvertExclusion = InvertExclusion
                    _multiSelector.ExclusiveScopeControlId = ExclusiveScopeControlId
                End If

                _multiSelector.DataBind()

                _multiSelector.SelectedValues = DirectCast(Value, List(Of String))

                AddHandler _multiSelector.SelectedItemsChanged, AddressOf multiSelector_SelectedItemsChanged

                _multiSelector.Enabled = (Me.EditMode = PropertyEditorMode.Edit)

            Finally
                Me.ChildControlsCreated = True
            End Try

        End Sub

        Private Sub multiSelector_SelectedItemsChanged(ByVal sender As Object, ByVal e As EventArgs)
            OnDataChanged(EventArgs.Empty)
        End Sub

        Protected Overrides Sub OnAttributesChanged()
            MyBase.OnAttributesChanged()
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In CustomAttributes
                    If TypeOf attribute Is MultiSelectorTypeAttribute Then
                        Dim multiSelectorTypeAttr As MultiSelectorTypeAttribute = DirectCast(attribute, MultiSelectorTypeAttribute)
                        MultiSelectorType = multiSelectorTypeAttr.MultiSelectorType
                        ExclusiveSelector = multiSelectorTypeAttr.ExclusiveSelector
                        InvertExclusion = multiSelectorTypeAttr.InvertExclusion
                        ExclusiveScopeControlId = multiSelectorTypeAttr.ExclusiveScopeControlId
                    ElseIf TypeOf attribute Is TextFieldAttribute Then
                        TextField = DirectCast(attribute, TextFieldAttribute).Text
                    ElseIf TypeOf attribute Is ValueFieldAttribute Then
                        ValueField = DirectCast(attribute, ValueFieldAttribute).Value
                    ElseIf TypeOf attribute Is WidthAttribute Then
                        Width = DirectCast(attribute, WidthAttribute).Width
                    End If
                Next
            End If
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Me.OldValue = Me.Value
            Me.Value = _multiSelector.SelectedValues

            Dim args As New PropertyEditorEventArgs(Me.Name)
            args.Value = Me.Value
            args.OldValue = Me.OldValue
            args.Changed = (Not args.Value Is args.OldValue)
            args.StringValue = Me.StringValue

            If args.Changed Then
                If Value Is Nothing OrElse String.IsNullOrEmpty(Value.ToString()) Then
                    _multiSelector.SelectedValues = Nothing
                Else
                    _multiSelector.SelectedValues = DirectCast(Value, List(Of String))
                End If
            End If

            MyBase.OnValueChanged(args)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If

        End Sub

        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

#End Region

    End Class

End Namespace