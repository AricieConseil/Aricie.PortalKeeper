Namespace Providers
    ''' <summary>
    ''' System provider that provides infrastructure services
    ''' </summary>
    Public MustInherit Class SystemServiceProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As SystemServiceProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()

            Dim applicationName As String = ""
            '= System.Reflection.Assembly.GetEntryAssembly().FullName 'System.AppDomain.CurrentDomain.ApplicationIdentity.FullName
            Dim appDirectory As String = AppDomain.CurrentDomain.BaseDirectory
            If System.IO.Directory.Exists(System.IO.Path.Combine(appDirectory, "DesktopModules\")) Then
                applicationName = "DotNetNuke"
            End If



            Dim typeName As String = ""
            Select Case applicationName
                Case "DotNetNuke"
                    typeName = "Aricie.DNN.Providers.DotNetNukeServiceProvider, Aricie.DNN"
                Case Else
                    typeName = GetType(DefaultServiceProvider).AssemblyQualifiedName
            End Select
            Dim providerType As Type = Type.GetType(typeName)
            objProvider = DirectCast(Activator.CreateInstance(providerType), SystemServiceProvider)

        End Sub

        ' return the provider
        Public Shared Function Instance() As SystemServiceProvider
            Return objProvider
        End Function

#End Region

#Region "Members"

        Private _exceptionSink As Action(Of Exception)

        Public WriteOnly Property ExceptionSink As Action(Of Exception)
            Set(value As Action(Of Exception))
                _exceptionSink = value
            End Set
        End Property

#End Region


#Region "Abstract Methods"

        Public MustOverride Function GetCache(ByVal key As String) As Object

        Public MustOverride Sub SetCache(ByVal key As String, ByVal value As Object)

        Public MustOverride Sub RemoveCache(ByVal key As String)

        Public MustOverride Sub SetCacheDependant(ByVal key As String, ByVal value As Object, _
                                                   ByVal absoluteExpiration As TimeSpan, ByVal ParamArray dependencies() As String)

        Public MustOverride Sub ClearCache()

        'Public MustOverride Function CreateObject(ByVal typeName As String) As Object

        Public Overridable Sub LogException(ByVal ex As Exception)
            If _exceptionSink IsNot Nothing Then
                _exceptionSink.Invoke(ex)
            End If
        End Sub


#End Region
    End Class
End Namespace


