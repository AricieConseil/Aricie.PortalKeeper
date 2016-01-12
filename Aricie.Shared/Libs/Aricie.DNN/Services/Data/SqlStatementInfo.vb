Imports Aricie.DNN.Services.Filtering

Namespace Services.Data
    
    Public Class SqlStatementInfo
        Inherits TokenizedTextInfo

        Public Property StatementType As SqlStatementType

       

        Public Function Execute() As Object
            Return Execute(Nothing)
        End Function

        Public Function Execute(atr As AdvancedTokenReplace) As Object
            Dim toReturn As Object = Nothing
            Dim sqlText As String = Me.GetText(atr)
            Select Case StatementType
                Case SqlStatementType.Script
                    toReturn = DotNetNuke.Data.DataProvider.Instance().ExecuteScript(sqlText)
                Case SqlStatementType.IDataReader
                    toReturn = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sqlText)
            End Select
            Return toReturn
        End Function


        Public Function GetOutputType() As Type
            Select Case Me.StatementType
                Case SqlStatementType.Script
                    Return GetType(String)
                Case SqlStatementType.IDataReader
                    Return GetType(IDataReader)
            End Select
            Return Nothing
        End Function


    End Class
End NameSpace