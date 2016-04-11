Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class WaitSynchronizationInfo

        Public  Property SynchronizationKey() As New SimpleOrExpression(Of String)("Synchro")

       
        <DefaultValue(False)> _
        Public Property WaitAfterRun As Boolean

    End Class
End NameSpace