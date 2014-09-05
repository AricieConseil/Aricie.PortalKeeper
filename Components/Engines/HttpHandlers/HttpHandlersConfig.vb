Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class HttpHandlersConfig
        Implements IEnabled

        Public Property Enabled As Boolean Implements IEnabled.Enabled

        Public Property Handlers As New SerializableList(Of HttpHandlerSettings)


        Public Function MapDynamicHandler(context As HttpContext) As HttpHandlerSettings
            Dim toReturn As HttpHandlerSettings = Nothing
            Dim verb As String = context.Request.RequestType
            Dim path As String = context.Request.Path
            Dim key As String = verb & path
            If Not _CachedMappings.TryGetValue(key, toReturn) Then
                For Each objHandlerSettings In Me.Handlers
                    If objHandlerSettings.HttpHandlerMode = HttpHandlerMode.DynamicHandler AndAlso objHandlerSettings.Matches(verb, path) Then
                        toReturn = objHandlerSettings
                        Exit For
                    End If
                Next
                If toReturn IsNot Nothing Then
                    _CachedMappings(key) = toReturn
                End If
            End If
            Return toReturn
        End Function

        Private _CachedMappings As New Dictionary(Of String, HttpHandlerSettings)


    End Class
End Namespace