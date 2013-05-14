Imports Aricie.DNN.Services.Errors

Namespace Configuration
    <Obsolete("DynamicHandler type is now a property of virtualcustomerrorsinfo")> _
    Public Class CustomErrorDynamicHandlerUpdater
        Inherits VirtualCustomErrorUpdater



        Private _HandlerType As Type


        Public Sub New(ByVal handlerType As Type, ByVal virtualErrors As VirtualCustomErrorsInfo)
            MyBase.New(virtualErrors)
            Me._HandlerType = handlerType
        End Sub



        Protected Overrides Sub UpdateConfig(ByRef objList As System.Collections.Generic.List(Of IConfigElementInfo))
            objList.Add(New HttpHandlerInfo(Me.VirtualCustomErrors.VirtualHandlerName, Me._HandlerType, Me.VirtualCustomErrors.VirtualHandlerPath, "GET,HEAD", "integratedMode"))
        End Sub

    End Class
End Namespace