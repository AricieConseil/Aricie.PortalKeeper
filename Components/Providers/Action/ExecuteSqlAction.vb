Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Data
Imports Aricie.DNN.UI.WebControls
Imports System.Data.Common
Imports DotNetNuke.Data

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.HddO, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Execute Sql Action")> _
    <Description("Execute a call to a stored procedure, a Sql script or sql query and returns the result")> _
    Public Class ExecuteSqlAction(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)



        <ExtendedCategory("SqlSettings")> _
        Public Property SqlActionMode As SqlActionMode = SqlActionMode.CallStoredProcedure

        <ExtendedCategory("SqlSettings")> _
        Public Property UseTransaction As Boolean

        <ConditionalVisible("SqlActionMode", False, True, SqlActionMode.SqlStatement)> _
        <ExtendedCategory("SqlSettings")> _
        Public Property SqlStatement() As New SqlStatementInfo()

        <ConditionalVisible("SqlActionMode", False, True, SqlActionMode.CallStoredProcedure)> _
        <ExtendedCategory("SqlSettings")> _
        Public Property CallStoredProcedure As New CallStoredProcedureInfo()


        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            Dim toReturn As Object = Nothing
            If Me.UseTransaction Then
                Using dbt As DbTransaction = DataProvider.Instance.GetTransaction()
                    toReturn = ExecuteWithoutTransaction(actionContext)
                    DataProvider.Instance.CommitTransaction(dbt)
                End Using
            Else
                toReturn = ExecuteWithoutTransaction(actionContext)
            End If
            Return toReturn
        End Function

        Public Function ExecuteWithoutTransaction(actionContext As PortalKeeperContext(Of TEngineEvents)) As Object
            Dim toReturn As Object = Nothing

            Select Case SqlActionMode
                Case SqlActionMode.CallStoredProcedure
                    toReturn = Me.CallStoredProcedure.Execute(actionContext, actionContext)
                Case SqlActionMode.SqlStatement
                    If Me.SqlStatement.EnableTokenReplace Then
                        toReturn = Me.SqlStatement.Execute(Me.GetAdvancedTokenReplace(actionContext))
                    Else
                        toReturn = Me.SqlStatement.Execute()
                    End If
            End Select
            Return toReturn
        End Function

        Protected Overrides Function GetOutputType() As Type
            Select Case SqlActionMode
                Case SqlActionMode.CallStoredProcedure
                    Return Me.CallStoredProcedure.GetOutputType()
                Case SqlActionMode.SqlStatement
                    Return Me.SqlStatement.GetOutputType()
            End Select
            Return Nothing
        End Function
    End Class
End Namespace