Imports Aricie.Collections
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.Services.Log.EventLog
Imports System
Imports System.Web


Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class PortalKeeperModuleAfterDNN
        Inherits PortalKeeperModule

        Public Sub New()
            Me.SecondaryModule = True
        End Sub

    End Class
End Namespace


