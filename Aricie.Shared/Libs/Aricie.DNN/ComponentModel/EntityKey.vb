Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee

Namespace ComponentModel


    Public Class EntityKeyInfo

        Public Property PortalId As New EnabledFeature(Of SimpleOrExpression(Of Integer))

        Public Property Application As New EnabledFeature(Of SimpleOrExpression(Of String))

        Public Property Entity As New EnabledFeature(Of SimpleOrExpression(Of String))

        Public Property UserName As New EnabledFeature(Of SimpleOrExpression(Of String))

        Public Property Field As New EnabledFeature(Of SimpleOrExpression(Of String))

        Public Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup) As EntityKey
            Dim toReturn As New EntityKey()
            If PortalId.Enabled
                toReturn.PortalId = PortalId.Entity.GetValue(owner, globalVars)
            End If
            If Application.Enabled
                toReturn.Application = Application.Entity.GetValue(owner, globalVars)
            End If
            If UserName.Enabled
                toReturn.UserName = UserName.Entity.GetValue(owner, globalVars)
            End If
            If Entity.Enabled
                toReturn.Entity = Entity.Entity.GetValue(owner, globalVars)
            End If
            If Field.Enabled
                toReturn.Field = Field.Entity.GetValue(owner, globalVars)
            End If
                
            Return toReturn
        End Function

    End Class

    <Serializable()>
    Public Class EntityKey




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


        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1} - {2} - {3} - {4}", PortalId, Application, Entity, UserName, Field)
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is EntityKey
                Dim target As EntityKey = DirectCast(obj, EntityKey)
                Return Me.ToString() = target.ToString()
            End If
            Return False
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return ToString().GetHashCode()
        End Function

    End Class
End Namespace