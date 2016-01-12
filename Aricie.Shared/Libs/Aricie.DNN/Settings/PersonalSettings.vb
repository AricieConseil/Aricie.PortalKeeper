Imports Aricie.Collections


Namespace Settings
    ''' <summary>
    ''' Base settings class
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Class PersonalSettings
        Inherits SerializableDictionary(Of String, Object)

        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Returns the setting value for key, cast as T
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="settingName"></param>
        ''' <param name="defaultValue">Default returned value in case the setting doesn't exist</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Get](Of T)(ByVal settingName As String, ByVal defaultValue As T) As T
            Dim toReturn As Object = Nothing
            If Me.TryGetValue(settingName, toReturn) Then
                Return DirectCast(toReturn, T)
            Else
                Return defaultValue
            End If
        End Function

        ''' <summary>
        ''' Sets the setting value for settingName of type T
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="settingName"></param>
        ''' <param name="settingValue"></param>
        ''' <remarks></remarks>
        Public Sub Put(Of T)(ByVal settingName As String, ByVal settingValue As T)
            Me(settingName) = settingValue
        End Sub

        Public Class Properties
            Public Const DefaultDrillDown As String = "DefaultDrillDown"
        End Class

    End Class
End Namespace

