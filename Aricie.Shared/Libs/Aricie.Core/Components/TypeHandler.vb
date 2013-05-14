Public Module TypeHandler

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="str"></param>
    ''' <param name="DefaultValue">Valeur par défaut à retourner (0 si non spécifié)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StrToInt(ByVal str As String, Optional ByVal DefaultValue As Integer = 0) As Integer
        Dim toReturn As Integer = DefaultValue
        Integer.TryParse(str, toReturn)
        Return toReturn
    End Function
End Module
