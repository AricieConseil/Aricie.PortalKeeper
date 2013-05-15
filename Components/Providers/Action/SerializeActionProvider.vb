Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
Imports Jayrock.Json.Conversion
Imports DotNetNuke.UI.WebControls
Imports System.IO
Imports System.Xml
Imports System.Globalization
Imports Aricie.Services
Imports Jayrock.Json
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
        <DisplayName("Serialize Action Provider")> _
        <Description("This provider allows to serialize a given entity, result of dynamic expression, into a string")> _
    Public Class SerializeActionProvider(Of TEngineEvents As IConvertible)
        Inherits SerializationBaseActionProvider(Of TEngineEvents)


        Private _inputExpression As New FleeExpressionInfo(Of Object)

        <ExtendedCategory("Serialization")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property InputExpression() As FleeExpressionInfo(Of Object)
            Get
                Return _InputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of Object))
                _InputExpression = value
            End Set
        End Property





        Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
            Dim toReturn As Object

            'Using sw As New System.IO.StringWriter(toReturn, CultureInfo.InvariantCulture)
            Using memStream As New MemoryStream
                Dim value As Object = _InputExpression.Evaluate(actionContext, actionContext)
                If Not value Is Nothing Then
                    Select Case Me.SerializationType
                        Case PortalKeeper.SerializationType.Xml
                            Using xw As New System.Xml.XmlTextWriter(memStream, Encoding.UTF8)
                                xw.Formatting = Formatting.Indented
                                Aricie.Services.ReflectionHelper.Serialize(value, DirectCast(xw, XmlWriter))
                            End Using
                        Case PortalKeeper.SerializationType.Binary
                            ReflectionHelper.Instance.BinaryFormatter.Serialize(memStream, value)
                        Case PortalKeeper.SerializationType.IConvertible
                            Using sw As New StreamWriter(memStream)
                                sw.Write(DirectCast(value, IConvertible).ToString(CultureInfo.InvariantCulture))
                            End Using
                        Case PortalKeeper.SerializationType.Json
                            Using sw As New StreamWriter(memStream)
                                Using jw As New JsonTextWriter(sw)
                                    jw.PrettyPrint = True
                                    JsonConvert.Export(value, jw)
                                End Using
                            End Using
                        Case PortalKeeper.SerializationType.FileHelpers
                            Dim inputObjects As IEnumerable = DirectCast(value, IEnumerable)
                            If inputObjects IsNot Nothing Then
                                Using sw As New StreamWriter(memStream)
                                    Me.FileHelpersSettings.Serialize(inputObjects, sw)
                                End Using
                            End If
                    End Select
                    Dim bytes As Byte() = memStream.ToArray
                    If Me.UseCompression Then
                        bytes = Common.DoCompress(bytes, CompressionMethod.Deflate)
                    End If
                    toReturn = Encoding.UTF8.GetString(memStream.ToArray)
                Else
                    toReturn = ""
                End If

            End Using

            Return toReturn.ToString
        End Function

    End Class
End Namespace