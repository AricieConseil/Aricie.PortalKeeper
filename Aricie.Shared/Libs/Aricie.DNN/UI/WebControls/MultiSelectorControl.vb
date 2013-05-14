Imports System.Web.UI.WebControls
Imports Aricie.ComponentModel
Imports System.Web.UI
Imports Aricie.Web.UI
Imports Aricie.Collections
Imports System.Reflection
Imports Aricie.Services

Namespace UI.WebControls

    Public MustInherit Class MultiSelectorControl(Of T As Class)
        Inherits MultiSelectorControlBase

        Public Property SelectedObjectsG() As T
            Get
                Return DirectCast(Me.SelectedObjects, T)
            End Get
            Set(ByVal value As T)
                Me.SelectedObjects = DirectCast(value, IList)
            End Set
        End Property

        Public ReadOnly Property AllItemsG() As IList(Of T)
            Get
                Return DirectCast(Me.AllItems, IList(Of T))
            End Get
        End Property

        Public Overrides Function GetEntities() As IEnumerable
            Return Me.GetEntitiesG
        End Function

        Public Overrides Function GetValue(ByVal fromObject As Object) As String
            Return Me.GetValueG(DirectCast(fromObject, T))
        End Function

        Public Overrides Function GetObject(ByVal value As String) As Object
            Return Me.GetObjectG(value)
        End Function

        Public Overrides Function GetObjects(ByVal values As IList(Of String)) As IEnumerable
            Return Me.GetObjectsG(values)
        End Function

        Private Function GetValueG(ByVal fromObject As T) As String
            Dim propInfo As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(Of T)()(Me.DataValueField)
            Return propInfo.GetValue(fromObject, Nothing).ToString
        End Function

        Private Function GetObjectG(ByVal value As String) As T
            Dim propInfo As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(Of T)()(Me.DataValueField)
            For Each obj As T In Me.AllItems
                If propInfo.GetValue(obj, Nothing).ToString = value Then
                    Return obj
                End If
            Next
            Return Nothing
        End Function

        Private Function GetObjectsG(ByVal values As IList(Of String)) As IList(Of T)
            Dim propInfo As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(Of T)()(Me.DataValueField)
            Dim toReturn As New List(Of T)
            For Each obj As T In Me.AllItems
                If values.Contains(propInfo.GetValue(obj, Nothing).ToString) Then
                    toReturn.Add(obj)
                End If
            Next
            Return toReturn
        End Function

        Public MustOverride Function GetEntitiesG() As IList(Of T)

    End Class

End Namespace
