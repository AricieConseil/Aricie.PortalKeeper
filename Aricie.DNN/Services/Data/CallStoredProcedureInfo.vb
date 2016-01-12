Imports Aricie.DNN.Services.Flee

Namespace Services.Data
    
    Public Class CallStoredProcedureInfo

        Public Property StoredProcedureType As StoredProcedureType = StoredProcedureType.NonQuery

        Public Property StoredProcedureName() As String = ""

        Public Property Parameters As New Variables()

        Public Function Execute(owner As Object, lookup As IContextLookup) As Object
            Dim toReturn As Object = Nothing
            Dim params As Dictionary(Of String, Object) = Me.Parameters.EvaluateVariables(owner, lookup)
            Dim objArray As Object() = New List(Of Object)(params.Values).ToArray()
            Select Case Me.StoredProcedureType
                Case StoredProcedureType.NonQuery
                    DotNetNuke.Data.DataProvider.Instance().ExecuteNonQuery(Me.StoredProcedureName, objArray)
                Case StoredProcedureType.DataSet
                    toReturn = DotNetNuke.Data.DataProvider.Instance().ExecuteDataSet(Me.StoredProcedureName, objArray)
                Case StoredProcedureType.IDataReader
                    toReturn = DotNetNuke.Data.DataProvider.Instance().ExecuteReader(Me.StoredProcedureName, objArray)
                Case StoredProcedureType.Scalar
                    toReturn = DotNetNuke.Data.DataProvider.Instance().ExecuteScalar(Me.StoredProcedureName, objArray)
            End Select
            Return toReturn
        End Function

        Public Function GetOutputType() As Type
            Select Case Me.StoredProcedureType
                Case StoredProcedureType.NonQuery
                    Return Nothing
                Case StoredProcedureType.DataSet
                    Return GetType(DataSet)
                Case StoredProcedureType.IDataReader
                    Return GetType(IDataReader)
                Case StoredProcedureType.Scalar
                    Return GetType(Object)
            End Select
            Return Nothing
        End Function

    End Class
End NameSpace