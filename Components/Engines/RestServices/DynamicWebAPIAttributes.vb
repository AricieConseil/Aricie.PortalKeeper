Imports Aricie.DNN.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DynamicWebAPIAttributes
        Inherits DynamicAttributes


        Public Overrides Function GetCandidateAttributes() As IEnumerable(Of Type)
            Return ObsoleteDotNetProvider.Instance.GetWebAPIAttributes()
        End Function

    End Class
End NameSpace