Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls

Namespace Entities
    <ActionButton(IconName.Search, IconOptions.Normal)> _
    Public Class HtmlPageScrapsInfo(Of TScrap As HtmlPageScrapInfo)

        <ExtendedCategory("PageScraps")> _
        Public Property PageScraps As New SerializableList(Of TScrap)

        <ExtendedCategory("Global")> _
        Public Property MaxNbPages As Integer

        <ExtendedCategory("Global")> _
        Public Property MaxResultsPerPage As Integer

        <ExtendedCategory("Global")> _
        Public Property PrimaryKeys As New List(Of String)

        <ExtendedCategory("Advanced")> _
        Public Property Custom As New SerializableDictionary(Of String, String)

        <ExtendedCategory("Advanced")> _
        Public Property AdditionalColumns As New SerializableDictionary(Of String, FilteredString)

        Public Function GetPrimaryKey(input As Dictionary(Of String, String)) As String
            Dim tempKey As String = Nothing
            Return (From objPrimaryKey In PrimaryKeys Where input.TryGetValue(objPrimaryKey, tempKey)).Aggregate("", Function(current, objPrimaryKey) current & tempKey)
        End Function

    End Class
End NameSpace