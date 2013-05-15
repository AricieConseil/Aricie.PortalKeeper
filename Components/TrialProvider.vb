Imports Aricie.DNN.Security.Trial


Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class TrialProvider
        Implements ITrialProvider


#If TRIAL Then
        Private Const IS_TRIAL As Boolean = True
#Else
        Private Const IS_TRIAL As Boolean = False
#End If

        Private Const ENCRYPTION_KEY As String = "DNNCerberus"
        Private Const DURATION As Integer = 30


        Public Function GetTrialConfigInfo() As TrialConfigInfo Implements ITrialProvider.GetTrialConfigInfo
            Return New TrialConfigInfo(IS_TRIAL, BusinessController.MODULE_NAME, TrialLimitation.Limitation _
                                                                                    Or TrialLimitation.ExplainView, ENCRYPTION_KEY, DURATION)

        End Function
    End Class
End Namespace
