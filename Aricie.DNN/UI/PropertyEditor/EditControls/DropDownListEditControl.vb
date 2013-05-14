Imports System.Reflection
Imports System.Web.UI
Imports System.Globalization
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditControls
    Public Class DropDownListEditControl
        Inherits AricieEditControl

        Private _Collection As IList
        Private _dataSourceId As String
        Private _dataTextField As String
        Private _dataValueField As String
        Private _valueTypeCode As TypeCode

        ' Methods
        Protected Sub GetItemProperties(ByVal baseItem As Object, ByRef textProp As PropertyInfo, _
                                         ByRef valueProp As PropertyInfo)
            Dim type As Type = baseItem.GetType
            textProp = type.GetProperty(Me.DataTextField)
            valueProp = type.GetProperty(Me.DataValueField)
            If (textProp Is Nothing) Then
                Throw _
                    New ArgumentException( _
                                           String.Format(CultureInfo.InvariantCulture, _
                                                          "L'entité n'a pas de propriété {0}.", Me.DataTextField))
            End If
            If (valueProp Is Nothing) Then
                Throw _
                    New ArgumentException( _
                                           String.Format(CultureInfo.InvariantCulture, _
                                                          "L'entité n'a pas de propriété {0}.", Me.DataValueField))
            End If
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(MyBase.Name)
            args.Value = Convert.ChangeType(MyBase.Value, _valueTypeCode)
            args.OldValue = MyBase.OldValue
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Protected Overrides Sub OnAttributesChanged()
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In CustomAttributes
                    If TypeOf attribute Is DataSourceAttribute Then
                        Dim dsAtt As DataSourceAttribute = CType(attribute, DataSourceAttribute)
                        DataSourceId = dsAtt.DataSource
                    ElseIf TypeOf attribute Is TextFieldAttribute Then
                        Dim txtAtt As TextFieldAttribute = CType(attribute, TextFieldAttribute)
                        DataTextField = txtAtt.Text
                    ElseIf TypeOf attribute Is ValueFieldAttribute Then
                        Dim valAtt As ValueFieldAttribute = CType(attribute, ValueFieldAttribute)
                        DataValueField = valAtt.Value
                        _valueTypeCode = valAtt.TypeCode
                    End If

                Next
            End If
        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            Dim textProp As PropertyInfo = Nothing
            Dim valueProp As PropertyInfo = Nothing
            If (Me.Collection.Count > 0) Then
                Dim baseItem As Object = Me.Collection(0)
                Me.GetItemProperties(baseItem, textProp, valueProp)
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Select)
            If Not MyBase.Required Then
                writer.AddAttribute(HtmlTextWriterAttribute.Value, String.Empty)
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                writer.Write("---")
                writer.RenderEndTag()
            End If
            Dim obj3 As Object
            For Each obj3 In Me.Collection
                Dim str As String = textProp.GetValue(obj3, Nothing).ToString
                Dim str2 As String = valueProp.GetValue(obj3, Nothing).ToString
                writer.AddAttribute(HtmlTextWriterAttribute.Value, str2)
                If ((Not MyBase.Value Is Nothing) AndAlso (str2 = MyBase.Value.ToString)) Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                End If
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                writer.Write(str)
                writer.RenderEndTag()
            Next
            writer.RenderEndTag()
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Dim str As String = "---"
            Dim textProp As PropertyInfo = Nothing
            Dim valueProp As PropertyInfo = Nothing
            If (Me.Collection.Count > 0) Then
                Dim baseItem As Object = Me.Collection(0)
                Me.GetItemProperties(baseItem, textProp, valueProp)
            End If
            Dim obj3 As Object
            For Each obj3 In Me.Collection
                Dim str2 As String = valueProp.GetValue(obj3, Nothing).ToString
                If ((Not MyBase.Value Is Nothing) AndAlso (str2 = MyBase.Value.ToString)) Then
                    str = textProp.GetValue(obj3, Nothing).ToString
                End If
            Next
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "Normal")
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "Normal")
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(str)
            writer.RenderEndTag()
        End Sub

        Private Sub GetCollection()
            Dim ds As IDataSource = CType(FindControlRecursive(Me.Page, DataSourceId), IDataSource)
            If ds IsNot Nothing Then
                Dim viewNamesEnum As IEnumerator = ds.GetViewNames().GetEnumerator()
                If viewNamesEnum.MoveNext Then
                    Dim view As DataSourceView = ds.GetView(DirectCast(viewNamesEnum.Current, String))
                    view.Select(DataSourceSelectArguments.Empty, _
                                 New DataSourceViewSelectCallback(AddressOf OnDataSourceViewSelectCallback))
                End If
            Else
                Throw _
                    New ArgumentException( _
                                           String.Format(CultureInfo.InvariantCulture, _
                                                          "Le controle d'ID {0} n'existe pas.", Me.DataSourceId))
            End If
        End Sub

        Private Function FindControlRecursive(ByVal ctrl As Control, ByVal id As String) As Control
            ' Exit if this is the control we're looking for.
            If ctrl.ID = id Then Return ctrl

            ' Else, look in the hiearchy.
            Dim childCtrl As Control

            For Each childCtrl In ctrl.Controls
                Dim resCtrl As Control = FindControlRecursive(childCtrl, id)
                ' Exit if we've found the result
                If Not resCtrl Is Nothing Then Return resCtrl
            Next

            Return Nothing
        End Function

        Public Sub OnDataSourceViewSelectCallback(ByVal data As IEnumerable)
            Dim dataEnum As IEnumerator = data.GetEnumerator
            _Collection = New ArrayList()
            While dataEnum.MoveNext
                _Collection.Add(dataEnum.Current)
            End While
        End Sub

        Protected ReadOnly Property Collection() As IList
            Get

                If _Collection Is Nothing Then
                    GetCollection()
                End If

                Return _Collection
            End Get
        End Property

        Protected Property DataSourceId() As String
            Get
                Return _dataSourceId
            End Get
            Set(ByVal value As String)
                _dataSourceId = value
            End Set
        End Property

        Protected Property DataTextField() As String
            Get
                Return _dataTextField
            End Get
            Set(ByVal value As String)
                _dataTextField = value
            End Set
        End Property

        Protected Property DataValueField() As String
            Get
                Return _dataValueField
            End Get
            Set(ByVal value As String)
                _dataValueField = value
            End Set
        End Property
    End Class
End Namespace
