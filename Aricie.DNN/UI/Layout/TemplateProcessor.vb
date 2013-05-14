Imports Aricie.Services
Imports DotNetNuke.Entities.Modules
Imports System.Web
Imports System.IO
Imports System.Xml


Namespace Layout

    Public Interface ITemplateProcessor


        Function ProcessTemplate(ByVal input As String, ByVal mid As Integer) As String

    End Interface

    Public Class ProviderController

        Private _ModuleId As Integer

        Public Sub New(ByVal mid As Integer)
            Me._ModuleId = mid
        End Sub

        Private ReadOnly Property Providers() As List(Of ITemplateProcessor)
            Get
                Dim cacheKey As String = "Aricie-" & _ModuleId & "Templates"
                Dim toReturn As List(Of ITemplateProcessor) = GetGlobal(Of List(Of ITemplateProcessor))(Me._ModuleId.ToString)
                Dim myController As New DesktopModuleController

                If toReturn Is Nothing Then
                    toReturn = New List(Of ITemplateProcessor)
                    Dim fileName As String = ""
                    fileName = Path.Combine(HttpContext.Current.Server.MapPath("~/DesktopModules\Aricie.ModuleWorkflows"), "TemplateProcessor.config")
                    Dim configDoc As New XmlDocument
                    configDoc.Load(fileName)
                    Dim providersNode As XmlNode = configDoc.SelectSingleNode("Aricie/TemplateProcessors")
                    If Not providersNode Is Nothing Then
                        For Each providerNode As XmlNode In providersNode.SelectNodes("provider")
                            Dim providerType As String = providerNode.Attributes.GetNamedItem("type").Value.ToString()
                            Dim providerName As String = providerNode.Attributes.GetNamedItem("name").Value.ToString()
                            Dim provider As ITemplateProcessor = ReflectionHelper.CreateObject(Of ITemplateProcessor)(providerType)
                            toReturn.Add(provider)
                        Next
                    End If
                    'Dim fileDep As New System.Web.Caching.CacheDependency(fileName)
                    SetCacheDependant(Of List(Of ITemplateProcessor))(toReturn, fileName, TimeSpan.FromMinutes(30), Me._ModuleId.ToString)
                End If
                Return toReturn
            End Get
        End Property


        Public Function ProcessTemplate(ByVal input As String) As String

            Dim toReturn As String = input
            For Each provider As ITemplateProcessor In Me.Providers

                toReturn = provider.ProcessTemplate(toReturn, _ModuleId)

            Next

            Return toReturn
        End Function

    End Class


End Namespace



