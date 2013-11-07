Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.ComponentModel
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports System.IO
Imports FileHelpers
Imports FileHelpers.Dynamic
Imports System.Reflection
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class FileHelpersSettings

        Private mRecordType As Type

        Public Property FileHelpersMode() As FileHelpersMode = FileHelpersMode.Delimiter

        Public Property RecordsMode() As RecordsMode = PortalKeeper.RecordsMode.StaticType

        <ConditionalVisible("FileHelpersMode", False, True, FileHelpersMode.Delimiter)> _
        Public Property Delimiter As Char = ","c

        <ConditionalVisible("FileHelpersMode", False, True, FileHelpersMode.Delimiter)> _
        Public Property IncludeHeaders As Boolean = True

        <ConditionalVisible("RecordsMode", False, True, RecordsMode.StaticType)> _
        Public Property RecordType() As New DotNetType()

        <ConditionalVisible("RecordsMode", False, True, RecordsMode.DynamicExpressions)> _
        Public Property InputEntityVarName As String = "CurrentInput"

        <ConditionalVisible("RecordsMode", False, True, RecordsMode.DynamicExpressions)> _
        <LineCount(10)> _
        <Width(500)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property FieldVars() As String = String.Empty




        Public Sub Serialize(inputObjects As IEnumerable, tw As TextWriter)
            Dim target As New List(Of Object)()
            Dim objRecordType As Type
            If Me.RecordsMode = PortalKeeper.RecordsMode.StaticType Then
                objRecordType = Me.RecordType.GetDotNetType
                For Each item As Object In inputObjects
                    target.Add(item)
                Next
            Else
                Dim dynamicExps As List(Of SerializableDictionary(Of String, Object)) = Me.ComputeDynamicExpressions(inputObjects)
                Dim classId As String = ""
                Select Case Me.FileHelpersMode
                    Case FileHelpersMode.Delimiter
                        Dim cb As New DelimitedClassBuilder("DelimClass", Me.Delimiter)
                        If dynamicExps.Count > 0 Then
                            Dim sampleData As SerializableDictionary(Of String, Object) = dynamicExps(0)
                            For Each objPair As KeyValuePair(Of String, Object) In sampleData
                                cb.AddField(objPair.Key, objPair.Value.GetType)
                                classId &= objPair.Key
                            Next
                            cb.ClassName &= Math.Abs(classId.GetHashCode).ToString()
                        End If
                        objRecordType = cb.CreateRecordClass
                    Case Else
                        Dim cb As New FixedLengthClassBuilder("FixedClass", FixedMode.AllowVariableLength)
                        If dynamicExps.Count > 0 Then
                            Dim sampleData As SerializableDictionary(Of String, Object) = dynamicExps(0)
                            For Each objPair As KeyValuePair(Of String, Object) In sampleData
                                cb.AddField(objPair.Key, 10, objPair.Value.GetType)
                                classId &= objPair.Key
                            Next
                            cb.ClassName &= Math.Abs(classId.GetHashCode).ToString()
                        End If
                        objRecordType = cb.CreateRecordClass
                End Select
                For Each item As SerializableDictionary(Of String, Object) In dynamicExps
                    Dim newRecord As Object = ReflectionHelper.CreateObject(objRecordType)
                    For Each objField As FieldInfo In newRecord.GetType.GetFields()
                        If Not item(objField.Name).GetType() Is objField.FieldType Then
                            objField.SetValue(newRecord, Convert.ChangeType(item(objField.Name), objField.FieldType))
                        Else
                            objField.SetValue(newRecord, item(objField.Name))
                        End If
                    Next
                    target.Add(newRecord)
                Next
            End If
            Dim engine As FileHelperEngine = Nothing
            Select Case Me.FileHelpersMode
                Case FileHelpersMode.Delimiter
                    Dim delimiterEngine As New DelimitedFileEngine(objRecordType)
                    delimiterEngine.Options.Delimiter = Me.Delimiter
                    If Me.IncludeHeaders Then
                        'Dim fields As FieldInfo() = objRecordType.GetFields((BindingFlags.NonPublic Or (BindingFlags.Public Or (BindingFlags.Instance Or BindingFlags.DeclaredOnly))))
                        'Dim fieldNames As New List(Of String)
                        'For Each objField As FieldInfo In fields
                        '    fieldNames.Add(objField.Name)
                        'Next
                        Dim header As String = String.Join(Me.Delimiter, delimiterEngine.Options.FieldsNames)
                        delimiterEngine.HeaderText = header
                    End If
                    engine = delimiterEngine
                Case Else
                    Dim fixedEngine As New FixedFileEngine(objRecordType)
                    engine = fixedEngine
            End Select
            engine.WriteStream(tw, target)
        End Sub

        Public Function DeSerialize(input As String) As IEnumerable

            Dim objRecordType As Type
            If Me.RecordsMode = PortalKeeper.RecordsMode.StaticType Then
                objRecordType = Me.RecordType.GetDotNetType
            Else
                'todo, gérer le cas dynamique
            End If

            Dim engine As FileHelperEngine = Nothing
            Select Case Me.FileHelpersMode
                Case FileHelpersMode.Delimiter
                    'todo, cf plus haut: gérer le cas dynamique
                    Dim delimiterEngine As New DelimitedFileEngine(objRecordType)
                    delimiterEngine.Options.Delimiter = Me.Delimiter
                    If Me.IncludeHeaders Then
                        'Dim fields As FieldInfo() = objRecordType.GetFields((BindingFlags.NonPublic Or (BindingFlags.Public Or (BindingFlags.Instance Or BindingFlags.DeclaredOnly))))
                        'Dim fieldNames As New List(Of String)
                        'For Each objField As FieldInfo In fields
                        '    fieldNames.Add(objField.Name)
                        'Next
                        Dim header As String = String.Join(Me.Delimiter, delimiterEngine.Options.FieldsNames)
                        delimiterEngine.HeaderText = header
                    End If
                    engine = delimiterEngine
                Case Else
                    Dim fixedEngine As New FixedFileEngine(objRecordType)
                    engine = fixedEngine
            End Select
            Dim listToReturn() As Object = engine.ReadString(input)
            Dim arrayToReturn As Array = Array.CreateInstance(objRecordType, listToReturn.Length)
            listToReturn.CopyTo(arrayToReturn, 0)
            Return arrayToReturn

        End Function

        Private Function ComputeDynamicExpressions(inputObjects As IEnumerable) As List(Of SerializableDictionary(Of String, Object))
           
            Dim toReturn As New List(Of SerializableDictionary(Of String, Object))
            Dim fieldNames As Dictionary(Of String, String) = Common.ParsePairs(Me.FieldVars, True)
            For Each inputObject As Object In inputObjects
                Dim context As New PortalKeeperContext(Of String)
                context.SetVar(Me.InputEntityVarName, inputObject)
                Dim dump As SerializableDictionary(Of String, Object) = context.GetDump(False, fieldNames.Keys, "")
                Dim fieldVarsDico As New SerializableDictionary(Of String, Object)
                Dim fieldName As String = Nothing
                For Each dumpVar As KeyValuePair(Of String, Object) In dump
                    If fieldNames.TryGetValue(dumpVar.Key, fieldName) Then
                        fieldVarsDico(fieldName) = dumpVar.Value
                    End If
                Next
                toReturn.Add(fieldVarsDico)
            Next
            Return toReturn
        End Function

        
    End Class
End NameSpace