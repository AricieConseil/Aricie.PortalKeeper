Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Web
Imports Aricie.Services

Namespace UI.WebControls.EditControls
    Public Class SelectorInfo

        Private _IsIselector As Boolean
        Private _SelectorTypeName As String
        Private _DataTextField As String
        Private _DataValueField As String
        Private _IsExclusive As Boolean
        Private _InsertNullItem As Boolean
        Private _NullItemValue As String

        Private _LocalizeItems As Boolean
        Private _LocalizeNull As Boolean


        Public Property IsIselector() As Boolean
            Get
                Return _IsIselector
            End Get
            Set(ByVal value As Boolean)
                _IsIselector = value
            End Set
        End Property

        Public Property SelectorTypeName() As String
            Get
                Return _SelectorTypeName
            End Get
            Set(ByVal value As String)
                _SelectorTypeName = value
            End Set
        End Property


        Public Property DataTextField() As String
            Get
                Return _DataTextField
            End Get
            Set(ByVal value As String)
                _DataTextField = value
            End Set
        End Property

        Public Property DataValueField() As String
            Get
                Return _DataValueField
            End Get
            Set(ByVal value As String)
                _DataValueField = value
            End Set
        End Property

        Public Property IsExclusive() As Boolean
            Get
                Return _IsExclusive
            End Get
            Set(ByVal value As Boolean)
                _IsExclusive = value
            End Set
        End Property

        Public Property InsertNullItem() As Boolean
            Get
                Return _InsertNullItem
            End Get
            Set(ByVal value As Boolean)
                _InsertNullItem = value
            End Set
        End Property

        Public Property NullItemText As String = "---"

        Public Property NullItemValue() As String
            Get
                Return _NullItemValue
            End Get
            Set(ByVal value As String)
                _NullItemValue = value
            End Set
        End Property

        Public Property LocalizeItems() As Boolean
            Get
                Return _LocalizeItems
            End Get
            Set(ByVal value As Boolean)
                _LocalizeItems = value
            End Set
        End Property

        Public Property LocalizeNull() As Boolean
            Get
                Return _LocalizeNull
            End Get
            Set(ByVal value As Boolean)
                _LocalizeNull = value
            End Set
        End Property


        Public Function BuildSelector(ByVal parentField As FieldEditorControl) As SelectorControl
            Dim toReturn As SelectorControl = Nothing
            If Not String.IsNullOrEmpty(Me._SelectorTypeName) Then
                toReturn = DirectCast(ReflectionHelper.CreateObject(Me._SelectorTypeName), SelectorControl)
            ElseIf Me._IsIselector Then
                Dim enumerable As IList = DirectCast(parentField.DataSource, ISelector).GetSelector(parentField.DataField)
                If Not enumerable Is Nothing Then
                    toReturn = New AutoSelectorControl(enumerable)
                Else
                    Throw New Exception(String.Format("Selector Content not found for property {0}.", parentField.DataField))
                End If
            End If

            If toReturn Is Nothing Then
                Throw New HttpException("selector not correctly configured for field " & parentField.Editor.Name)
            End If


            toReturn.ID = "ddl" & toReturn.GetType.Name
            '_selector.Enabled = (Me.EditMode = PropertyEditorMode.Edit)




            toReturn.DataTextField = Me._DataTextField
            toReturn.DataValueField = Me._DataValueField
            toReturn.ExclusiveSelector = Me._IsExclusive
            toReturn.ExclusiveScopeControl = parentField.NamingContainer
            If Me._InsertNullItem Then
                toReturn.InsertNullItem = True
                toReturn.NullItemText = Me.NullItemText
                toReturn.NullItemValue = Me._NullItemValue
            Else
                toReturn.InsertNullItem = False
            End If
            Return toReturn
        End Function


    End Class
End Namespace