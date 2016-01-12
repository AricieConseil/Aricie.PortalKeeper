Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class ConditionProvider(Of TEngineEvents As IConvertible)
        Inherits ConditionProviderSettings(Of TEngineEvents)
        Implements IConditionProvider(Of TEngineEvents)


        Public Overridable Function MatchWithPreConditions(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean Implements IConditionProvider(Of TEngineEvents).Match
            Dim stopEvaluate As Boolean
            If Me.AddPreConditionActions Then
                Dim result As Boolean = Me.PreConditionActions.Run(context)
                If Me.StopActions AndAlso Not result Then
                    stopEvaluate = True
                End If
            End If
            If Not stopEvaluate Then
                Return Match(context)
            Else
                Return StopMatches
            End If
        End Function

        Public Overridable Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return False

        End Function
    End Class
End Namespace