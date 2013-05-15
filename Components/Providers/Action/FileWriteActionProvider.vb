Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.Text
Imports System.IO
Imports FileHelpers
Imports FileHelpers.Dynamic
Imports Aricie.Collections

Namespace Aricie.DNN.Modules.PortalKeeper





    <DisplayName("File Write Action")> _
        <Description("This provider allows to write a content to a file, given its path and the content to write by dynamic expressions")> _
        <Serializable()> _
    Public Class FileWriteActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileReadWriteActionProvider(Of TEngineEvents)


        Private _InputExpression As New FleeExpressionInfo(Of String)

        Private _InputEntityExpression As New FleeExpressionInfo(Of IEnumerable)

        Private _AppendContent As Boolean





        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
        <ExtendedCategory("File")> _
        Public Property InputExpression() As FleeExpressionInfo(Of String)
            Get
                Return _InputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _InputExpression = value
            End Set
        End Property

        <ExtendedCategory("File")> _
        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
        Public Property AppendContent() As Boolean
            Get
                Return _AppendContent
            End Get
            Set(ByVal value As Boolean)
                _AppendContent = value
            End Set
        End Property



        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        'Public Property InputEntityExpression() As FleeExpressionInfo(Of IEnumerable)
        '    Get
        '        Return _InputEntityExpression
        '    End Get
        '    Set(ByVal value As FleeExpressionInfo(Of IEnumerable))
        '        _InputEntityExpression = value
        '    End Set
        'End Property

        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        'Public Property InputEntityVarName As String = "CurrentInput"

        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        '<Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
        '  LineCount(4), Width(500)> _
        '  <SortOrder(1000)> _
        'Public Property FieldVars() As String = String.Empty







        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object

            Dim mapPath As String = Me.GetFileMapPath(actionContext)
            Try
                RWLock.AcquireWriterLock(LockTimeSpan)
                Dim dirName As String = Path.GetDirectoryName(mapPath)
                If Not System.IO.Directory.Exists(dirName) Then
                    Directory.CreateDirectory(dirName)
                End If

                'Select Case Me.AccessMode
                '    Case FileAccessMode.StringReadWrite
                Dim contents As String = Me._InputExpression.Evaluate(actionContext, actionContext)
                Dim objEncoding As Encoding = EncodingHelper.GetEncoding(Me.Encoding)
                If Me.UseCompression Then
                    contents = Common.DoCompress(contents, CompressionMethod.Deflate)
                End If

                If _AppendContent Then
                    System.IO.File.AppendAllText(mapPath, contents, objEncoding)
                Else
                    System.IO.File.WriteAllText(mapPath, contents, objEncoding)
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