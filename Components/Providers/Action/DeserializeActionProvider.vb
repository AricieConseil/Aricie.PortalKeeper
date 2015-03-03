Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
'Imports Jayrock.Json.Conversion
'Imports Jayrock.Json
Imports DotNetNuke.UI.WebControls
Imports System.IO
Imports System.Xml
Imports Aricie.Services
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Eye, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Deserialize Action Provider")> _
        <Description("This provider allows to deserialize a given string, result of dynamic expression, into a typed object")> _
    Public Class DeserializeActionProvider(Of TEngineEvents As IConvertible)
        Inherits SerializationBaseActionProvider(Of TEngineEvents)



        Private _InputExpression As New FleeExpressionInfo(Of String)

        Private _OutputType As New DotNetType(GetType(Object))

        Private _AdditionalTypes As New List(Of DotNetType)

        <ExtendedCategory("Serialization")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property InputExpression() As FleeExpressionInfo(Of String)
            Get
                Return _InputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _InputExpression = value
            End Set
        End Property

        <ExtendedCategory("Serialization")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property OutputType() As DotNetType
            Get
                Return _OutputType
            End Get
            Set(ByVal value As DotNetType)
                _OutputType = value
            End Set
        End Property





        <ExtendedCategory("Serialization")> _
           <Editor(GetType(ListEditControl), GetType(EditControl))> _
           <CollectionEditor(False, False, True, True, 5, CollectionDisplayStyle.List)> _
           <LabelMode(LabelMode.Top)> _
        Public Property AdditionalTypes() As List(Of DotNetType)
            Get
                Return _AdditionalTypes
            End Get
            Set(ByVal value As List(Of DotNetType))
                _AdditionalTypes = value
            End Set
        End Property

        <ExtendedCategory("Serialization")> _
        Public Property DefaultToNewEntity As Boolean

        Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim toReturn As Object = Nothing
            Dim serializedValue As String = Me._InputExpression.Evaluate(actionContext, actionContext)
            If String.IsNullOrEmpty(serializedValue) AndAlso Me.DefaultToNewEntity Then
                Return ReflectionHelper.CreateObject(Me._OutputType.GetDotNetType())
            End If
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(serializedValue)
            If Me.UseCompression Then
                bytes = Decompress(bytes, CompressionMethod.Deflate)
            End If
            Dim additionalTypes As New List(Of Type)
            For Each addDNT As DotNetType In Me._AdditionalTypes
                additionalTypes.Add(addDNT.GetDotNetType)
            Next
            'Using sw As New System.IO.StringWriter(toReturn, CultureInfo.InvariantCulture)
            Using memStream As New MemoryStream(bytes)
                Select Case Me.SerializationType
                    Case SerializationType.Xml
                        Using reader As New StreamReader(memStream, Encoding.UTF8)
                            Using xmlReader As New XmlTextReader(memStream)
                                Dim serializer As XmlSerializer = ReflectionHelper.GetSerializer(Me._OutputType.GetDotNetType, additionalTypes.ToArray, True)
                                toReturn = serializer.Deserialize(xmlReader)
                            End Using
                        End Using
                    Case SerializationType.Binary
                        toReturn = ReflectionHelper.Instance.BinaryFormatter.Deserialize(memStream)
                    Case SerializationType.Json
                        Dim jsonSettings As New JsonSerializerSettings()
                        'For Each addType As Type In additionalTypes
                        '    jsonSettings.ContractResolver.ResolveContract(addType)
                        'Next
                        Using reader As New StreamReader(memStream, Encoding.UTF8)
                            Using jsonR As New JsonTextReader(reader)
                                Dim serializer As JsonSerializer = JsonSerializer.Create(jsonSettings)
                                toReturn = serializer.Deserialize(jsonR, Me._OutputType.GetDotNetType)
                            End Using
                        End Using

                    Case SerializationType.IConvertible
                        toReturn = TypeDescriptor.GetConverter(Me._OutputType.GetDotNetType).ConvertFromInvariantString(serializedValue)
                    Case SerializationType.FileHelpers
                        Dim sR As New StreamReader(memStream)
                        Dim inputString As String = sR.ReadToEnd()
                        toReturn = Me.FileHelpersSettings.DeSerialize(inputString)
                End Select
            End Using
            Return toReturn
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return Me.OutputType.GetDotNetType()
        End Function
    End Class
End Namespace