Imports Aricie.Services
Imports System.Reflection
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Common.Utilities
Imports System.IO

Namespace Services
    Public Class ObsoleteDNNProvider

        ' singleton reference to the instantiated object 
        Private Shared objProvider As ObsoleteDNNProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        Public Shared Function ResolveEmbeddedAssembly(senders As Object, args As ResolveEventArgs) As Assembly
            Dim resourceName As [String] = New AssemblyName(args.Name).Name + ".dll"
            For Each realResourceName As String In Assembly.GetExecutingAssembly().GetManifestResourceNames()
                If realResourceName.Contains(resourceName) Then
                    Using stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                        Dim assemblyData As [Byte]() = New [Byte](CInt(stream.Length - 1)) {}
                        stream.Read(assemblyData, 0, assemblyData.Length)
                        Return Assembly.Load(assemblyData)
                    End Using
                End If
            Next
            Return Nothing
        End Function


        ' dynamically create provider
        Private Shared Sub CreateProvider()

            If NukeHelper.DnnVersion.Major < 7 Then
                objProvider = New ObsoleteDNNProvider()
            Else
                Try

                    AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveEmbeddedAssembly

                    'Function(sender, args)
                    'Dim resourceName As [String] = New AssemblyName(args.Name).Name + ".dll"
                    'For Each realResourceName As String In Assembly.GetExecutingAssembly().GetManifestResourceNames()
                    '    If realResourceName.Contains(resourceName) Then
                    '        Using stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                    '            Dim assemblyData As [Byte]() = New [Byte](CInt(stream.Length - 1)) {}
                    '            stream.Read(assemblyData, 0, assemblyData.Length)
                    '            Return Assembly.Load(assemblyData)
                    '        End Using
                    '    End If
                    'Next
                    'Return Nothing
                    '                                                    End Function


                    'Dim objAssembly As Assembly = Assembly.LoadFile("Aricie.DNN7.dll")
                    '(NukeHelper.GetModuleDirectoryMapPath("Aricie-Shared").TrimEnd("\"c) & "\DNN7\Aricie.DNN7.dll")
                    objProvider = CType(ReflectionHelper.CreateObject("Aricie.DNN.DNN7ObsoleteDNNProvider, Aricie.DNN7"), ObsoleteDNNProvider)

                    RemoveHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveEmbeddedAssembly

                Catch ex As Exception
                    ExceptionHelper.LogException(ex)
                    objProvider = New ObsoleteDNNProvider()
                End Try
            End If
        End Sub

        ' return the provider
        Public Shared ReadOnly Property Instance() As ObsoleteDNNProvider
            Get
                Return objProvider
            End Get
        End Property

        Public Overridable Sub AddFolder(objFolderInfo As FolderInfo)
            NukeHelper.FolderController.AddFolder(objFolderInfo.PortalID, objFolderInfo.FolderPath, objFolderInfo.StorageLocation, objFolderInfo.IsProtected, objFolderInfo.IsCached)
        End Sub

        Public Overridable Function GetFolderFromPath(portalId As Integer, path As String) As FolderInfo
            Return NukeHelper.FolderController.GetFolder(portalId, path, False)
        End Function

        Public Overridable Function GetFileContent(objFileInfo As DotNetNuke.Services.FileSystem.FileInfo) As Byte()

            Return NukeHelper.FileController.GetFileContent(objFileInfo.FileId, objFileInfo.PortalId)

        End Function

        Public Overridable Sub UpdateFileContent(objFileInfo As DotNetNuke.Services.FileSystem.FileInfo, content As Byte())

            NukeHelper.FileController.UpdateFileContent(objFileInfo.FileId, content)

        End Sub

        Public Overridable Sub ClearFileContent(objFileInfo As DotNetNuke.Services.FileSystem.FileInfo)

            NukeHelper.FileController.ClearFileContent(objFileInfo.FileId)

        End Sub


        Public Overridable Function GetFiles(folderInfo As FolderInfo) As IEnumerable(Of DotNetNuke.Services.FileSystem.FileInfo)
            Return CBO.FillCollection(Of DotNetNuke.Services.FileSystem.FileInfo)(NukeHelper.FileController.GetFiles(folderInfo.PortalID, folderInfo.FolderID))
        End Function


    End Class
End Namespace