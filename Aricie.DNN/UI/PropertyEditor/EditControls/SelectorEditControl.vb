Imports System.Collections.Specialized
Imports System.Web.UI
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls

Namespace UI.WebControls.EditControls
    Public Class SelectorEditControl
        Inherits AricieEditControl

        'Private _enumerable As IList
        Private _selector As SelectorControl
        'Private _IsIselector As Boolean

        'Private _selectorTypeName As String
        'Private _dataTextField As String
        'Private _dataValueField As String
        'Private _exclusive As Boolean
        'Private _insertNullItem As Boolean
        'Private _nullItemValue As String
        Private _SelectorInfo As New SelectorInfo

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)
            Me.EnsureChildControls()
            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        Protected Overrides Sub OnAttributesChanged()
            MyBase.OnAttributesChanged()
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In CustomAttributes
                    If TypeOf attribute Is SelectorAttribute Then
                        Dim selAtt As SelectorAttribute = CType(attribute, SelectorAttribute)
                        Me._SelectorInfo = selAtt.SelectorInfo
                    End If
                Next
            End If
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Me.Name)
            args.Value = Me.Value
            args.OldValue = Me.OldValue
            args.Changed = (Not args.Value Is args.OldValue)
            args.StringValue = Me.StringValue

            If args.Changed Then
                If Value Is Nothing OrElse String.IsNullOrEmpty(Value.ToString()) Then
                    _selector.SelectedIndex = -1
                Else
                    _selector.SelectedValue = Value.ToString()
                End If

            End If


            MyBase.OnValueChanged(args)
        End Sub


        Protected Overrides Sub CreateChildControls()


            'Dim ids As String() = ParentField.UniqueID.Split("$"c)

            Me._selector = Me._SelectorInfo.BuildSelector(Me.ParentField)
            Me.Controls.Add(_selector)
            _selector.DataBind()



            If Me.EditMode = PropertyEditorMode.Edit Then



                If Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(Value.ToString()) Then
                    Dim objItem As ListItem = _selector.Items.FindByValue(Value.ToString())
                    If objItem IsNot Nothing Then
                        _selector.SelectedIndex = _selector.Items.IndexOf(objItem)
                    End If
                ElseIf _selector.Items.Count > 0 Then
                    'If _selector.InsertNullItem Then

                    'End If
                    _selector.SelectedIndex = 0
                End If
                'If Value IsNot Nothing AndAlso String.IsNullOrEmpty(Value.ToString()) Then
                '    _selector.SelectedValue = Value.ToString()
                'ElseIf _selector.Items.Count > 0 Then
                '    _selector.SelectedIndex = 0
                'End If

                AddHandler _selector.SelectedIndexChanged, AddressOf Me.SelectorSelectedIndexChanged
                _selector.Enabled = True
            Else
                _selector.Visible = False
                Dim label As New Label

                If Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(Value.ToString()) Then
                    Dim objItem As ListItem = _selector.Items.FindByValue(Value.ToString())
                    If objItem IsNot Nothing Then
                        _selector.SelectedIndex = _selector.Items.IndexOf(objItem)
                        label.Text = objItem.Text
                    Else
                        label.Text = Value.ToString()
                    End If
                End If

                label.CssClass = "SubHead"
                label.EnableViewState = False
                'AddHandler label.Unload, AddressOf _selector.OnSelectorUnload
                Me.Controls.Add(label)

            End If




            'MyBase.CreateChildControls()
        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean
            If Me.Value Is Nothing Then
                Throw New ApplicationException(String.Format("value of property {0} can't be null", Me.ParentField.DataField))
            End If
            Dim oldValue As Object = Me.Value
            'Dim toReturn As Boolean = MyBase.LoadPostData(postDataKey, postCollection)
            'If Not String.IsNullOrEmpty(_selector.SelectedValue) Then
            Dim vaueType As Type = Me.Value.GetType()
            If vaueType.IsEnum Then
                Me.Value = [Enum].Parse(vaueType, _selector.SelectedValue)
            Else
                Me.Value = Convert.ChangeType(_selector.SelectedValue, Me.Value.GetType())
            End If
            'ElseIf _selector.InsertNullItem Then
            '    Me.Value = _selector.NullItemValue
            'End If

            Return Not oldValue.Equals(Me.Value)
        End Function

        Private Sub SelectorSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            'MyBase.Value = Convert.ChangeType(_selector.SelectedValue, Me.Value.GetType())
            OnDataChanged(EventArgs.Empty)
        End Sub



        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            'If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
            '    Me.Page.RegisterRequiresPostBack(Me)
            'End If
        End Sub

        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

    End Class

End Namespace
