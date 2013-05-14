Imports DotNetNuke.Services.Log.EventLog

Namespace UI.WebControls
    Public Class LogTypeSelector
        Inherits SelectorControl(Of LogTypeInfo)


        Public Overrides Function GetEntitiesG() As IList(Of LogTypeInfo)
            Dim objController As New LogController
            Dim logTypes As ArrayList = objController.GetLogTypeInfo
            Dim toReturn As New List(Of LogTypeInfo)
            For Each objLogType As LogTypeInfo In logTypes
                'objLogType.LogTypeKey
                'objLogType.LogTypeFriendlyName
                toReturn.Add(objLogType)
            Next
            Return toReturn
        End Function
    End Class
End Namespace