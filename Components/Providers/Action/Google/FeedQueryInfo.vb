Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Google.GData.Client

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class FeedQueryInfo

        <ExtendedCategory("General")> _
        Public Overridable Property StartIndex() As New EnabledFeature(Of SimpleOrSimpleExpression(Of Integer))

        <ExtendedCategory("General")> _
        Public Overridable Property NumberToRetrieve() As New EnabledFeature(Of SimpleOrSimpleExpression(Of Integer))

        <ExtendedCategory("General")> _
        Public Property StartDate() As New EnabledFeature(Of SimpleOrSimpleExpression(Of DateTime))

        <ExtendedCategory("General")> _
        Public Property EndDate() As New EnabledFeature(Of SimpleOrSimpleExpression(Of DateTime))

        <ExtendedCategory("General")> _
        Public Property MinPublication() As New EnabledFeature(Of SimpleOrSimpleExpression(Of DateTime))

        <ExtendedCategory("General")> _
        Public Property MaxPublication() As New EnabledFeature(Of SimpleOrSimpleExpression(Of DateTime))

        <ExtendedCategory("General")> _
        Public Property ModifiedSince() As New EnabledFeature(Of SimpleOrSimpleExpression(Of DateTime))

        <ExtendedCategory("General")> _
        Public Property Etag() As New EnabledFeature(Of SimpleOrSimpleExpression(Of String))

        <ExtendedCategory("General")> _
        Public Property Query() As New EnabledFeature(Of SimpleOrSimpleExpression(Of String))

        <ExtendedCategory("General")> _
        Public Property ExtraParameters() As New EnabledFeature(Of SimpleOrSimpleExpression(Of String))

        <ExtendedCategory("General")> _
        Public Property Author() As New EnabledFeature(Of SimpleOrSimpleExpression(Of String))

        <ExtendedCategory("General")> _
        Public Property FeedFormat As New EnabledFeature(Of SimpleOrSimpleExpression(Of AlternativeFormat))(New SimpleOrSimpleExpression(Of AlternativeFormat)(AlternativeFormat.Atom), False)


        Public Overridable Sub SetQuery(ByVal objQuery As FeedQuery, context As IContextLookup)
            If Query.Enabled Then
                objQuery.Query = Query.Entity.GetValue(context)
            End If
            If Etag.Enabled Then
                objQuery.Etag = Etag.Entity.GetValue(context)
            End If
            If ExtraParameters.Enabled Then
                objQuery.ExtraParameters = ExtraParameters.Entity.GetValue(context)
            End If
            If Author.Enabled Then
                objQuery.Author = Author.Entity.GetValue(context)
            End If
            If StartDate.Enabled Then
                objQuery.StartDate = StartDate.Entity.GetValue(context)
            End If
            If EndDate.Enabled Then
                objQuery.EndDate = EndDate.Entity.GetValue(context)
            End If
            If MinPublication.Enabled Then
                objQuery.MinPublication = MinPublication.Entity.GetValue(context)
            End If
            If MaxPublication.Enabled Then
                objQuery.MaxPublication = MaxPublication.Entity.GetValue(context)
            End If
            If ModifiedSince.Enabled Then
                objQuery.ModifiedSince = ModifiedSince.Entity.GetValue(context)
            End If
            If StartIndex.Enabled Then
                objQuery.StartIndex = StartIndex.Entity.GetValue(context)
            End If
            If NumberToRetrieve.Enabled Then
                objQuery.NumberToRetrieve = NumberToRetrieve.Entity.GetValue(context)
            End If
            If FeedFormat.Enabled Then
                objQuery.FeedFormat = FeedFormat.Entity.GetValue(context)
            End If
        End Sub



    End Class
End Namespace