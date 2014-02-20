Namespace ComponentModel
    <Serializable()>
    Public Structure EntityKey



        'Sub New(ByVal entity As String)
        '    Me.New(-1, "", "", entity, "")
        'End Sub

        'Sub New(ByVal application As String, ByVal entity As String)
        '    Me.New(-1, application, "", entity, "")
        'End Sub

        'Sub New(ByVal application As String, ByVal user As String, ByVal entity As String)
        '    Me.New(-1, application, user, entity, "")
        'End Sub

        'Sub New(ByVal application As String, ByVal user As String, ByVal entity As String, ByVal field As String)
        '    Me.New(-1, application, user, entity, field)
        'End Sub

        'Sub New(ByVal portalId As Integer, ByVal application As String, ByVal user As String, ByVal entity As String, ByVal field As String)
        '    Me.PortalId = portalId
        '    Me.Application = application
        '    Me.Entity = entity
        '    Me.Field = field
        '    Me.UserName = user
        'End Sub

        Public Property PortalId As Integer

        Public Property Application As String

        Public Property Entity As String

        Public Property UserName As String

        Public Property Field As String





    End Structure
End Namespace