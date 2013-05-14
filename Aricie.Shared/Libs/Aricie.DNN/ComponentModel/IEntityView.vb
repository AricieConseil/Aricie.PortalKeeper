Namespace ComponentModel
    Public Interface IEntityView

        Function GetSupportedViews() As List(Of EntityViewGenre)

        Function GetControlKey(ByVal viewGenre As EntityViewGenre) As String

        Function GetQueryStringParam(ByVal viewGenre As EntityViewGenre) As String


    End Interface
End Namespace