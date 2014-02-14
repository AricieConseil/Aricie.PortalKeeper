Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Reflection
Imports Aricie.Services
Imports System.Web.UI

Namespace UI.WebControls.EditControls
    Public Class BreadCrumbsEditControl
        Inherits AricieEditControl

        Private _SelectorInfo As New SelectorInfo() With {.DataTextField = "Key", .DataValueField = "Value"}

        Private _Buttons As New Dictionary(Of String, WebControl)
        Public Property Separator As String = " / "


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
                SetCurrentLevel()
            End If


            MyBase.OnValueChanged(args)
        End Sub

        Private Sub SetCurrentLevel()
            For Each objButton As WebControl In _Buttons.Values
                objButton.Enabled = True
            Next
            If Value IsNot Nothing Then
                Dim objButton As WebControl = Nothing
                If _Buttons.TryGetValue(Value.ToString, objButton) Then
                    objButton.Enabled = False
                End If
            End If
        End Sub


        Protected Overrides Sub CreateChildControls()


            'Dim ids As String() = ParentField.UniqueID.Split("$"c)

            'Me._selector = Me._SelectorInfo.BuildSelector(Me.ParentField)
            'Me.Controls.Add(_selector)
            '_selector.DataBind()



            If Me.EditMode = PropertyEditorMode.Edit Then

                BuildButtons()

                SetCurrentLevel()


            Else
                Dim label As New Label


                Dim objItems As IDictionary(Of String, String) = Me.GetValueKeys

                Dim strText As String = Nothing
                If Not objItems.TryGetValue(Value.ToString, strText) Then
                    strText = Value.ToString()
                End If
                
                label.Text = strText
                label.CssClass = "SubHead"
                label.EnableViewState = False
                Me.Controls.Add(label)

            End If
            ChildControlsCreated = True



            'MyBase.CreateChildControls()
        End Sub


        Private _ValueKeys As Dictionary(Of String, String)


        Private Function GetValueKeys() As Dictionary(Of String, String)
            If _ValueKeys Is Nothing Then
                _ValueKeys = New Dictionary(Of String, String)
                If Me._SelectorInfo.InsertNullItem Then
                    _ValueKeys.Add(_SelectorInfo.NullItemValue, _SelectorInfo.NullItemText)
                End If
                Dim selector As ISelector = DirectCast(ParentField.DataSource, ISelector)
                If selector IsNot Nothing Then

                    Dim rawList As IList = selector.GetSelector(ParentField.DataField)
                    If rawList Is Nothing Then
                        Throw New Exception(String.Format("BreadCrumbs Content not found for property {0}.", ParentField.DataField))
                    End If
                    For Each rawObj As Object In rawList

                        Dim props As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(rawObj.GetType())
                        Dim key As String = rawObj.ToString()
                        Dim objValue As String = rawObj.ToString()
                        Dim prop As PropertyInfo = Nothing
                        If Not String.IsNullOrEmpty(Me._SelectorInfo.DataTextField) Then
                            If props.TryGetValue(_SelectorInfo.DataTextField, prop) Then
                                key = prop.GetValue(rawObj, Nothing).ToString()
                            End If
                        End If
                        If Not String.IsNullOrEmpty(Me._SelectorInfo.DataValueField) Then
                            If props.TryGetValue(_SelectorInfo.DataValueField, prop) Then
                                objValue = prop.GetValue(rawObj, Nothing).ToString()

                            End If
                        End If
                        _ValueKeys(objValue) = key
                    Next


                Else
                    _ValueKeys.Add(Value.ToString(), Value.ToString())
                End If
            End If

            Return _ValueKeys
        End Function


        Private Sub BuildButtons()
            Dim items As Dictionary(Of String, String) = Me.GetValueKeys()
            Dim counter As Integer = 0
            For Each objItem In items
                'Dim objButton As New LinkButton With {.Text = objItem.Value, .CommandArgument = objItem.Key, .CssClass = "dnnTertiaryAction"}
                
                Dim objButton As New IconActionButton With {.Text = objItem.Value, .CommandArgument = objItem.Key}
                If String.IsNullOrEmpty(objItem.Key) Then
                    objButton.ActionItem.IconName = IconName.Sitemap
                End If
                AddHandler objButton.Command, AddressOf ButtonClick
                Me.Controls.Add(objButton)
                Me._Buttons(objItem.Key) = objButton
                counter += 1
                If counter < items.Count Then
                    Dim objLabel As New Label With {.Text = Me.Separator}
                    Me.Controls.Add(objLabel)
                End If
            Next
        End Sub

        Private Sub ButtonClick(sender As Object, e As CommandEventArgs)
            Me.Value = e.CommandArgument
            OnDataChanged(EventArgs.Empty)
        End Sub

        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub
    End Class
End NameSpace