Imports Aricie.DNN.UI.Attributes
Imports System.IO
Imports Aricie.Text
Imports System.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class FileWriteActionProviderBase(Of TEngineEvents As IConvertible)
        Inherits FileReadWriteActionProvider(Of TEngineEvents)

        <Browsable(False)> _
        Public Overrides ReadOnly Property ShowOutput As Boolean
            Get
                Return False
            End Get
        End Property

        <ExtendedCategory("File")> _
        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
        Public Property AppendContent() As Boolean

        Public MustOverride Function GetContent(actionContext As PortalKeeperContext(Of TEngineEvents)) As String




        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object

            Dim mapPath As String = Me.GetFileMapPath(actionContext)
            Try
                RWLock.AcquireWriterLock(LockTimeSpan)
                Dim dirName As String = Path.GetDirectoryName(mapPath)
                If Not Directory.Exists(dirName) Then
                    Directory.CreateDirectory(dirName)
                End If

                'Select Case Me.AccessMode
                '    Case FileAccessMode.StringReadWrite

                Dim contents As String = Me.GetContent(actionContext)
                Dim objEncoding As Encoding = GetEncoding(Me.Encoding)
                If Me.UseCompression Then
                    contents = Compress(contents, CompressionMethod.Deflate)
                End If

                If AppendContent Then
                    File.AppendAllText(mapPath, contents, objEncoding)
                Else
                    File.WriteAllText(mapPath, contents, objEncoding)
                End If

                'Case FileAccessMode.FileHelperCSV
                'Dim inputObjects As IEnumerable = Me._InputEntityExpression.Evaluate(actionContext, actionContext)
                'If inputObjects IsNot Nothing Then
                '    Select Case Me.FileHelpersMode
                '        Case PortalKeeper.FileHelpersMode.TypeSerialization
                '            Dim engine As New DelimitedFileEngine(Me.EntityType.GetDotNetType())
                '            engine.Encoding = EncodingHelper.GetEncoding(Me.Encoding)
                '            engine.Options.Delimiter = Me.Delimiter
                '            Dim target As New List(Of Object)()
                '            For Each item As Object In inputObjects
                '                target.Add(item)
                '            Next
                '            engine.WriteFile(mapPath, target)
                '        Case PortalKeeper.FileHelpersMode.CustomExpressions
                '            Dim dumpVars As List(Of String) = Common.ParseStringList(Me.FieldVars)
                '            Dim fieldNames As New Dictionary(Of String, String)
                '            For Each dumpVar As String In dumpVars
                '                Dim splitVar() As String = dumpVar.Split("="c)
                '                If splitVar.Length = 2 Then
                '                    fieldNames(splitVar(1)) = splitVar(0)
                '                End If
                '            Next
                '            'Dim delimBuilder As New DelimitedClassBuilder(Me.Name & "DelimClass")
                '            'delimBuilder.AddField("", GetType(String))
                '            Dim dt As New DataTable
                '            Dim firstPass As Boolean = True
                '            For Each inputObject As Object In inputObjects
                '                actionContext.SetVar(Me.InputEntityVarName, inputObject)
                '                Dim dump As SerializableDictionary(Of String, Object) = actionContext.GetDump(False, fieldNames.Keys)
                '                Dim fieldVars As New SerializableDictionary(Of String, Object)
                '                Dim fieldName As String = Nothing
                '                For Each dumpVar As KeyValuePair(Of String, Object) In dump
                '                    If fieldNames.TryGetValue(dumpVar.Key, fieldName) Then
                '                        fieldVars(fieldName) = dumpVar.Value
                '                    End If
                '                Next
                '                If firstPass Then
                '                    For Each fieldVar As KeyValuePair(Of String, Object) In fieldVars
                '                        Dim targetType As Type
                '                        If fieldVar.Value Is Nothing Then
                '                            targetType = GetType(Object)
                '                        Else
                '                            targetType = fieldVar.Value.GetType
                '                        End If
                '                        dt.Columns.Add(fieldVar.Key, targetType)
                '                    Next
                '                End If
                '                Dim objRow As DataRow = dt.NewRow
                '                For Each fieldVar As KeyValuePair(Of String, Object) In fieldVars
                '                    objRow(fieldVar.Key) = fieldVar.Value
                '                Next
                '                firstPass = False
                '                dt.Rows.Add(objRow)
                '            Next
                '            FileHelpers.CommonEngine.DataTableToCsv(dt, mapPath, Me.Delimiter)
                '    End Select
                'End If

                'End Select

            Catch ex As Exception
                Throw ex
            Finally
                RWLock.ReleaseWriterLock()
            End Try

            Return True

        End Function


    End Class
End Namespace