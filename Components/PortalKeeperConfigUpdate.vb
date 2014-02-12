Imports Aricie.DNN.Configuration
Imports OpenRasta.Hosting.AspNet

Namespace Aricie.DNN.Modules.PortalKeeper


    Public Class PortalKeeperConfigUpdate
        Implements IUpdateProvider


        Public Function GetConfigElements() As System.Collections.Generic.List(Of IConfigElementInfo) Implements IUpdateProvider.GetConfigElements
            Dim toReturn As New List(Of IConfigElementInfo)
            toReturn.Add(New SchedulerTaskElementInfo("PortalKeeper Bot Farm", GetType(PortalKeeperSchedule), TimeSpan.FromMinutes(1)))
            Dim toAdd As WebServerElementInfo = New HttpModuleInfo("Aricie.PortalKeeper", GetType(PortalKeeperModule), "managedHandler")
            toAdd.InsertBeforeKey = "UrlRewrite"
            toReturn.Add(toAdd)

            toAdd = New HttpModuleInfo("Aricie.PortalKeeperAfterDNN", GetType(PortalKeeperModuleAfterDNN), "managedHandler")
            toAdd.InsertAfterKey = "UrlRewrite"
            toReturn.Add(toAdd)

            ''Jesse:      OpenRasta()
            'todo: gérer les services web
            'toAdd = New HttpModuleInfo("OpenRastaModule", GetType(OpenRastaModule), "managedHandler")
            'toReturn.Add(toAdd)
            'toAdd = New HttpHandlerInfo("OpenRastaHandler", GetType(OpenRastaHandler), "*.rastahook", "*", "integratedMode")
            'toReturn.Add(toAdd)

            toReturn.Add(New TrustInfo("Full"))

          
            Return toReturn
        End Function


    End Class

    'Public Class PortalKeeperRastaConfigUpdate
    '    Implements IUpdateProvider



    '    Public Function GetConfigElements() As System.Collections.Generic.List(Of IConfigElementInfo) Implements IUpdateProvider.GetConfigElements
    '        Dim toReturn As New List(Of IConfigElementInfo)

    '        Dim toAdd As WebServerElementInfo 
    '        'Jesse:      OpenRasta()
    '        toAdd = New HttpModuleInfo("OpenRastaModule", GetType(OpenRastaModule), "managedHandler")
    '        toReturn.Add(toAdd)
    '        toAdd = New HttpHandlerInfo("OpenRastaHandler", GetType(OpenRastaHandler), "*.rastahook", "*", "integratedMode")
    '        toReturn.Add(toAdd)

    '        Return toReturn
    '    End Function


    'End Class


End Namespace
