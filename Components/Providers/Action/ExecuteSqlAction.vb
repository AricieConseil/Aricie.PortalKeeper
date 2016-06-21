Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Data
Imports Aricie.DNN.UI.WebControls
Imports System.Data.Common
Imports System.Linq
Imports Aricie.Collections
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Entities
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Data
Imports Microsoft.Scripting.Ast

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.HddO, IconOptions.Normal)>
    <DisplayName("Execute Sql Action")>
    <Description("Execute a call to a stored procedure, a Sql script or sql query and returns the result")>
    Public Class ExecuteSqlAction(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)



        <ExtendedCategory("SqlSettings")>
        Public Property SqlActionMode As SqlActionMode = SqlActionMode.CallStoredProcedure

        <ExtendedCategory("SqlSettings")>
        Public Property UseTransaction As Boolean

        <ConditionalVisible("SqlActionMode", False, True, SqlActionMode.SqlStatement)>
        <ExtendedCategory("SqlSettings")>
        Public Property SqlStatement() As New SqlStatementInfo()

        <ConditionalVisible("SqlActionMode", False, True, SqlActionMode.CallStoredProcedure)>
        <ExtendedCategory("SqlSettings")>
        Public Property CallStoredProcedure As New CallStoredProcedureInfo()

        <ExtendedCategory("SqlSettings")>
        Public Property HydrateObjects As Boolean

        <ConditionalVisible("HydrateObjects")>
        <ExtendedCategory("SqlSettings")>
        Public Property CustomType() As New EnabledFeature(Of DotNetType)


        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
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
            If HydrateObjects Then
                If TypeOf toReturn Is IDataReader Then
                    dim dr as IDataReader = DirectCast(toReturn, IDataReader)
                    If CustomType.Enabled Then
                        Dim targetType As Type = CustomType.Entity.GetDotNetType()
                        Dim tempList As IList = New SerializableList(Of Object)
                        CBO.FillCollection(dr, targetType, tempList)
                        toReturn = tempList
                    Else
                        Dim tempList As IList = New SerializableList(Of SerializableDictionary(Of String, Object))
                        Try
                            While dr.Read()
                                tempList.Add(New SerializableDictionary(Of String, Object)(Enumerable.Range(0, dr.FieldCount).ToDictionary(Function(i) dr.GetName(i), Function(i) dr.GetValue(i))))
                            End While
                            toReturn = tempList
                        Catch
                            Throw
                        Finally
                            dr.Close()
                        End Try
                    End If
                End If
            End If

            Return toReturn
        End Function

        Protected Overrides Function GetOutputType() As Type
            Dim toReturn As Type = Nothing
            Select Case SqlActionMode
                Case SqlActionMode.CallStoredProcedure
                    toReturn = Me.CallStoredProcedure.GetOutputType()
                Case SqlActionMode.SqlStatement
                    toReturn = Me.SqlStatement.GetOutputType()
            End Select
            If HydrateObjects Then
                If toReturn Is GetType(IDataReader) Then
                    If CustomType.Enabled Then
                        toReturn = GetType(SerializableList(Of Object))
                    Else
                        toReturn = GetType(SerializableList(Of SerializableDictionary(Of String, Object)))
                    End If
                End If
            End If
            Return toReturn
        End Function
    End Class
End Namespace