Imports Aricie.DNN.Services.Errors

Namespace Configuration


    Public Class VirtualCustomErrorUpdater
        Inherits CustomErrorConfigUpdater

        Protected ReadOnly Property VirtualCustomErrors() As VirtualCustomErrorsInfo
            Get
                Return DirectCast(Me.CustomErrorsConfig, VirtualCustomErrorsInfo)
            End Get
        End Property


        Public Sub New(ByVal virtualErrors As VirtualCustomErrorsInfo)
            MyBase.New(virtualErrors)
        End Sub

        Public Overrides Function GetConfigElements() As System.Collections.Generic.List(Of IConfigElementInfo)
            If Me.VirtualCustomErrors.CustomErrorsType = CustomErrorsType.Legacy Then
                Return MyBase.GetConfigElements
            Else
                Dim toReturn As New List(Of IConfigElementInfo)
                toReturn.Add(Me.VirtualCustomErrors.ToLegacy())
                Me.UpdateConfig(toReturn)
                Return toReturn
            End If

        End Function


        Protected Overridable Sub UpdateConfig(ByRef objList As List(Of IConfigElementInfo))
            If Not Me.VirtualCustomErrors.UseAshx Then
                objList.Add(New HttpHandlerInfo(Me.VirtualCustomErrors.VirtualHandlerName, Me.VirtualCustomErrors.DynamicHandlerType.GetDotNetType(), Me.VirtualCustomErrors.VirtualHandlerPath, "GET,HEAD", "integratedMode"))
            End If
        End Sub


    End Class
End Namespace