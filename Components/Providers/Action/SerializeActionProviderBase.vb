Imports System.IO
Imports System.Xml
Imports System.Globalization
Imports Newtonsoft.Json
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class SerializeActionProviderBase(Of TEngineEvents As IConvertible)
        Inherits SerializationBaseActionProvider(Of TEngineEvents)


        Public MustOverride Function GetContent(actionContext As PortalKeeperContext(Of TEngineEvents)) As Object

        Public Function Serialize(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As String
            Dim toReturn As String

            'Using sw As New System.IO.StringWriter(toReturn, CultureInfo.InvariantCulture)
            Using memStream As New MemoryStream
                Dim value As Object = GetContent(actionContext)  '= _inputExpression.Evaluate(actionContext, actionContext)
                If Not value Is Nothing Then
                    Select Case Me.SerializationType
                        Case PortalKeeper.SerializationType.Xml
                            Using xw As New System.Xml.XmlTextWriter(memStream, Encoding.UTF8)
                                xw.Formatting = System.Xml.Formatting.Indented
                                Aricie.Services.ReflectionHelper.Serialize(value, DirectCast(xw, XmlWriter))
                            End Using
                        Case PortalKeeper.SerializationType.Binary
                            ReflectionHelper.Instance.BinaryFormatter.Serialize(memStream, value)
                        Case PortalKeeper.SerializationType.IConvertible
                            Using sw As New StreamWriter(memStream, Encoding.UTF8)
                                sw.Write(DirectCast(value, IConvertible).ToString(CultureInfo.InvariantCulture))
                            End Using
                        Case PortalKeeper.SerializationType.Json
                            Using sw As New StreamWriter(memStream, Encoding.UTF8)
                                Dim jsonSettings As New JsonSerializerSettings()
                                Using jw As New JsonTextWriter(sw)
                                    Dim serializer As JsonSerializer = JsonSerializer.CreateDefault(jsonSettings)
                                    serializer.Serialize(jw, value)
                                End Using
                            End Using
                        Case PortalKeeper.SerializationType.FileHelpers
                            Dim inputObjects As IEnumerable = DirectCast(value, IEnumerable)
                            If inputObjects IsNot Nothing Then
                                Using sw As New StreamWriter(memStream, Encoding.UTF8)
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

            Return toReturn
        End Function


        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            Return Serialize(actionContext)
        End Function
    End Class
End NameSpace