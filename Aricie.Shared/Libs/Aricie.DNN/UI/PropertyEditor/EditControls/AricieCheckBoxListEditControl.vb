Imports System.Web.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Lists
Imports System.Web.UI
Imports DotNetNuke.UI.WebControls
Imports System.Globalization
Imports DotNetNuke.Services.Localization

Namespace UI.WebControls.EditControls

    Public Enum CheckBoxListMode
        DNNList
        FLagsEnum
    End Enum

    Public Class AricieCheckBoxListEditControl
        Inherits AricieEditControl

        Protected WithEvents _ckList As CheckBoxList
        Private _ListName As String = Null.NullString
        Private _List As ListEntryInfoCollection
        Private _ParentKey As String = Null.NullString
        Private _alreadyChanged As Boolean = False

        Private ReadOnly Property Mode As CheckBoxListMode
            Get
                If Me.Value.GetType().IsEnum Then
                    Return CheckBoxListMode.FLagsEnum
                End If
                Return CheckBoxListMode.DNNList
            End Get
        End Property
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim olvValue As Object = Value
            Dim selectedValues As New List(Of String)
            For Each i As ListItem In Me._ckList.Items
                If i.Selected Then
                    selectedValues.Add(i.Value)
                End If
            Next
            Select Case Mode
                Case CheckBoxListMode.DNNList
                    Value = String.Join(";"c, selectedValues.ToArray())
                Case CheckBoxListMode.FLagsEnum
                    Dim compound As Integer = 0
                    For Each strSelected As String In selectedValues
                        compound = compound Or Integer.Parse(strSelected, CultureInfo.InvariantCulture)
                    Next
                    Value = [Enum].Parse(Me.Value.GetType(), compound.ToString(CultureInfo.InvariantCulture))
            End Select
            


            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = Value
            args.OldValue = olvValue
            args.StringValue = Value.ToString()

            MyBase.OnValueChanged(args)

        End Sub


        Protected ReadOnly Property List() As ListEntryInfoCollection
            Get
                If _List Is Nothing Then
                    Dim objListController As New ListController
                    _List = objListController.GetListEntryInfoCollection(ListName, ParentKey)
                End If
                Return _List
            End Get
        End Property

        Protected Overridable Property ListName() As String
            Get
                If _ListName = Null.NullString Then
                    _ListName = Me.Name
                End If
                Return _ListName
            End Get
            Set(ByVal Value As String)
                _ListName = Value
            End Set
        End Property

        Protected Overridable Property ParentKey() As String
            Get
                Return _ParentKey
            End Get
            Set(ByVal Value As String)
                _ParentKey = Value
            End Set
        End Property

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            Me._ckList.RenderControl(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Me._ckList.Enabled = False
            Me._ckList.RenderControl(writer)
        End Sub


        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            Me._ckList = New CheckBoxList
            Me.Controls.Add(Me._ckList)
            Me._ckList.RepeatLayout = RepeatLayout.Table
            Me._ckList.RepeatDirection = RepeatDirection.Horizontal
            Me._ckList.RepeatColumns = 3
            Me._ckList.CssClass = "enumItem"


            Dim selectedValues As List(Of String) = Nothing

            Select Case Mode
                Case CheckBoxListMode.DNNList
                    Me._ckList.DataSource = Me.List
                    Me._ckList.DataValueField = "Value"
                    Me._ckList.DataTextField = "Text"
                    Dim strValue As String = Me.StringValue
                    selectedValues = strValue.Split(";"c).ToList()
                Case CheckBoxListMode.FLagsEnum
                    Dim objEnumValues = [Enum].GetValues(Me.Value.GetType)
                    Dim intValue As Integer = CInt(Me.Value)
                    Me._ckList.DataSource = (From objEnum As Object In objEnumValues _
                                            Where CInt(objEnum) <> 0 _
                                            Select New ListItem(Localization.GetString(objEnum.ToString(), LocalResourceFile), CInt(objEnum).ToString(CultureInfo.InvariantCulture))).ToList()
                    Me._ckList.DataValueField = "Value"
                    Me._ckList.DataTextField = "Text"
                    selectedValues = (From objEnum As Object In objEnumValues _
                                      Select intVal = CInt(objEnum) _
                                      Where intVal <> 0 AndAlso (intVal And intValue) = intVal _
                                      Select CStr(intVal)).ToList()
            End Select
            Me._ckList.DataBind()

            For Each s As String In selectedValues
                Try
                    Me._ckList.Items.FindByValue(s).Selected = True
                Catch ex As Exception
                End Try
            Next

            AddHandler _ckList.SelectedIndexChanged, AddressOf Me._ckList_SelectedIndexChanged

        End Sub

        Private Sub AricieCheckBoxListEditControl_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            Me.EnsureChildControls()
        End Sub

        Private Sub _ckList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _ckList.SelectedIndexChanged
            If Not _alreadyChanged Then
                OnDataChanged(e)
                _alreadyChanged = True
            End If
        End Sub
    End Class
End Namespace
