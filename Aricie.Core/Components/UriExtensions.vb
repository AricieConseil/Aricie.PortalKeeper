Imports System.Collections.Specialized
Imports System.Web

    <System.Runtime.CompilerServices.Extension> _
            Public Module UriExtensions



        <System.Runtime.CompilerServices.Extension> _
        Public Function ModifyQueryString(baseUri As Uri, updates As NameValueCollection, removes As IEnumerable(Of String)) As String
            Dim query As NameValueCollection = HttpUtility.ParseQueryString(baseUri.Query)

            Dim url As String = baseUri.AbsolutePath

            updates = If(updates, New NameValueCollection())
            For Each key As String In updates.Keys
                query.[Set](key, updates(key))
            Next

            removes = If(removes, New List(Of String)())
            For Each param As String In removes
                query.Remove(param)
            Next

            If query.HasKeys() Then
                Return String.Format("{0}?{1}", url, query.ToString())
            Else
                Return url
            End If
        End Function



        <System.Runtime.CompilerServices.Extension> _
        Public Function UpdateQueryParam(baseUri As Uri, param As String, value As Object) As String
            Dim collect = New NameValueCollection()
            collect.Add(param, value.ToString())
            Return baseUri.ModifyQueryString(collect, Nothing)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function RemoveQueryParam(baseUri As Uri, param As String) As String
            Dim removes = New List(Of String)
            removes.Add(param)
            Return baseUri.ModifyQueryString(Nothing, removes)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function UpdateQueryParams(baseUri As Uri, updates As NameValueCollection) As String
            Return baseUri.ModifyQueryString(updates, Nothing)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function RemoveQueryParams(baseUri As Uri, removes As List(Of String)) As String
            Return baseUri.ModifyQueryString(Nothing, removes)
        End Function
    End Module
