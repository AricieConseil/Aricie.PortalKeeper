Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Google.GData.Client
Imports Google.GData.Spreadsheets

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class ListQueryInfo
        Inherits FeedQueryInfo

        <ExtendedCategory("Specifics")> _
        <SortOrder(0)> _
        Public Property SpreadsheetQuery() As New EnabledFeature(Of SimpleOrSimpleExpression(Of String))

        <ExtendedCategory("Specifics")> _
        Public Property OrderByPosition() As New EnabledFeature(Of SimpleOrSimpleExpression(Of Boolean))

        <ExtendedCategory("Specifics")> _
        Public Property OrderByColumn() As New EnabledFeature(Of SimpleOrSimpleExpression(Of String))

        <ExtendedCategory("Specifics")> _
        Public Property Reverse() As New EnabledFeature(Of SimpleOrSimpleExpression(Of Boolean))

        Public Overrides Sub SetQuery(objQuery As FeedQuery, context As IContextLookup)
            MyBase.SetQuery(objQuery, context)
            Dim objListQuery As ListQuery = TryCast(objQuery, ListQuery)
            If objListQuery IsNot Nothing Then
                If SpreadsheetQuery.Enabled Then
                    objListQuery.SpreadsheetQuery = SpreadsheetQuery.Entity.GetValue(context)
                End If
                If OrderByColumn.Enabled Then
                    objListQuery.OrderByColumn = OrderByColumn.Entity.GetValue(context)
                End If
                If OrderByPosition.Enabled Then
                    objListQuery.OrderByPosition = OrderByPosition.Entity.GetValue(context)
                End If
                If Reverse.Enabled Then
                    objListQuery.Reverse = Reverse.Entity.GetValue(context)
                End If
            End If
        End Sub



    End Class
End NameSpace