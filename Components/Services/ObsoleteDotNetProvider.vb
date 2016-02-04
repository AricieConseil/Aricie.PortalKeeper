Imports Aricie.DNN.Services
Imports Aricie.Services
Imports Aricie.Web

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class ObsoleteDotNetProvider

        ' singleton reference to the instantiated object 
        Private Shared objProvider As ObsoleteDotNetProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        'Public Shared Function ResolveEmbeddedAssembly(senders As Object, args As ResolveEventArgs) As Assembly
        '    Dim resourceName As [String] = New AssemblyName(args.Name).Name + ".dll"
        '    For Each realResourceName As String In Assembly.GetExecutingAssembly().GetManifestResourceNames()
        '        If realResourceName.Contains(resourceName) Then
        '            Using stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(realResourceName)
        '                Dim assemblyData As [Byte]() = New [Byte](CInt(stream.Length - 1)) {}
        '                stream.Read(assemblyData, 0, assemblyData.Length)
        '                Return Assembly.Load(assemblyData)
        '            End Using
        '        End If
        '    Next
        '    Return Nothing
        'End Function


        ' dynamically create provider
        Private Shared Sub CreateProvider()

            If NukeHelper.DnnVersion.Major < 7 Then
                objProvider = New ObsoleteDotNetProvider()
            Else
                Try

                    objProvider = CType(ReflectionHelper.CreateObject("Aricie.PortalKeeper.DNN7.Dnn7ObsoleteDotNetProvider, Aricie.PortalKeeper.DNN7"), ObsoleteDotNetProvider)


                Catch ex As Exception
                    ExceptionHelper.LogException(ex)
                    objProvider = New ObsoleteDotNetProvider()
                End Try
            End If
        End Sub

        ' return the provider
        Public Shared ReadOnly Property Instance() As ObsoleteDotNetProvider
            Get
                Return objProvider
            End Get
        End Property

        Public Overridable Sub RegisterWebAPI()

        End Sub

        Public Overridable Function GetFormatterNames() As List(Of String)
            Return New List(Of String)
        End Function

        Public Overridable Function GetMediaTypeHeaders() As List(Of String)
            Return New List(Of String)
        End Function

        Public Overridable Function GetWebAPIAttributes() As IEnumerable(Of Type)
            Return New List(Of Type)()
        End Function

        Public Overridable Function GetSampleRoutes(basePath As String, controller As DynamicControllerInfo) As IEnumerable(Of String)
            Return New List(Of String)
        End Function

        Public Overridable Function RunAction(objAction As DynamicAction, controller As DynamicControllerInfo, objService As RestService, _
                                              verb As WebMethod, arguments As IDictionary(Of String, Object)) As Object
            Return Nothing
        End Function

    End Class
End NameSpace