
Imports Aricie.DNN.Services.Errors

Namespace Configuration

    Public Class CustomErrorConfigUpdater
        Implements IUpdateProvider

        Private _CustomErrorsConfig As CustomErrorsInfo


        Public Sub New(ByVal customErrorsConfig As CustomErrorsInfo)
            _CustomErrorsConfig = customErrorsConfig
        End Sub

        Protected ReadOnly Property CustomErrorsConfig() As CustomErrorsInfo
            Get
                Return _CustomErrorsConfig
            End Get
        End Property


        Public Overridable Function GetConfigElements() As System.Collections.Generic.List(Of IConfigElementInfo) Implements IUpdateProvider.GetConfigElements
            Dim toReturn As New List(Of IConfigElementInfo)
            toReturn.Add(Me.CustomErrorsConfig)
            'toReturn.Add(New HttpModuleInfo("Aricie.PortalKeeper", GetType(PortalKeeperModule), "managedHandler"))
            Return toReturn
        End Function


    End Class
End Namespace


