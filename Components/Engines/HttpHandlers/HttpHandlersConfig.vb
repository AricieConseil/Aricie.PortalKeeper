Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Aricie.DNN.Modules.PortalKeeper

    
    Public Class HttpSubHandlersConfig
        Inherits HttpHandlersConfig

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property DefaultFiddle As SerializableList(Of HttpSubHandlerSettings)
            Get
                Return MyBase.DefaultFiddle
            End Get
            Set(value As SerializableList(Of HttpSubHandlerSettings))
                MyBase.DefaultFiddle = value
            End Set
        End Property

        Public Overrides Function GetNewItem(collectionPropertyName As String) As Object
            Select Case collectionPropertyName
                Case "Handlers"
                    Return New HttpSubHandlerSettings()
            End Select
            Return Nothing
        End Function

    End Class

    
    Public Class HttpHandlersConfig
        Implements IEnabled
        Implements ITypedContainer


        Public Property Enabled As Boolean Implements IEnabled.Enabled

        Public Property Handlers As New SerializableList(Of HttpHandlerSettings)


        Public Overridable Property DefaultFiddle As New SerializableList(Of HttpSubHandlerSettings)

        Public Function MapDynamicHandler(context As HttpContext) As HttpHandlerSettings
            Dim toReturn As HttpHandlerSettings = Nothing
            Dim verb As String = context.Request.RequestType
            Dim path As String = context.Request.Path
            Dim key As String = verb & path
            If Not _CachedMappings.TryGetValue(key, toReturn) Then
                For Each objHandlerSettings In Me.Handlers
                    If (objHandlerSettings.HttpHandlerMode = HttpHandlerMode.DynamicHandler OrElse objHandlerSettings.HttpHandlerMode = HttpHandlerMode.Node) _
                            AndAlso objHandlerSettings.Matches(verb, path) Then
                        toReturn = objHandlerSettings
                        Exit For
                    End If
                Next
                If toReturn IsNot Nothing Then
                    SyncLock _CachedMappings
                        _CachedMappings(key) = toReturn
                    End SyncLock
                End If
            End If
            Return toReturn
        End Function

        Private _CachedMappings As New Dictionary(Of String, HttpHandlerSettings)


        Public Overridable Function GetNewItem(collectionPropertyName As String) As Object Implements ITypedContainer.GetNewItem
            Select Case collectionPropertyName
                Case "Handlers"
                    Return New HttpHandlerSettings
            End Select
            Return Nothing
        End Function
    End Class
End Namespace