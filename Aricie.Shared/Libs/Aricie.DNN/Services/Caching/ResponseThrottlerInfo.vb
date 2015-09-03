Imports System.Globalization
Imports System.Web
Imports Aricie.DNN.UI.Attributes
Imports Aricie.IO

Namespace Services.Caching

    Public Enum ThrottlingMode
        ManagedStream
        BitRateModule
    End Enum
    Public Class ResponseThrottlerInfo

        Public Property Mode As ThrottlingMode = ThrottlingMode.ManagedStream

        Public Property Disable As Boolean


        <ConditionalVisible("Disable", True)> _
        <ConditionalVisible("Mode", False, True, ThrottlingMode.ManagedStream)> _
        Public Property BlockSize As Integer = 4096

        <ConditionalVisible("Disable", True)> _
        Public Property InitialSendSizeKB As Integer = 50

        <ConditionalVisible("Disable", True)> _
        Public Property RateKBs As Integer = 50


        Public Sub ApplyBandwidthThrottling(ByVal objContext As HttpContext)

            Select Case Mode
                Case ThrottlingMode.ManagedStream
                    If Not Disable Then
                        If objContext.Response IsNot Nothing AndAlso objContext.Response.Filter IsNot Nothing Then
                            Dim objFilter As New ThrottledStream(objContext.Response.Filter, RateKBs * 1000, InitialSendSizeKB * 1000)
                            objFilter.BlockSize = Me.BlockSize
                            objContext.Response.Filter = objFilter
                        End If
                    End If
                Case ThrottlingMode.BitRateModule
                    If Me.Disable Then
                        objContext.Request.ServerVariables("ResponseThrottler-Enabled") = "0"
                    Else

                        objContext.Request.ServerVariables("ResponseThrottler-Enabled") = "1"
                        objContext.Request.ServerVariables("ResponseThrottler-InitialSendSize") = InitialSendSizeKB.ToString(CultureInfo.InvariantCulture)
                        objContext.Request.ServerVariables("ResponseThrottler-Rate") = (RateKBs * 8).ToString(CultureInfo.InvariantCulture)
                    End If
            End Select


        End Sub


    End Class
End Namespace