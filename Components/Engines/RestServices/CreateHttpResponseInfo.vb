Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Services
Imports System.ComponentModel
Imports System.Linq
Imports Aricie.DNN.Entities
Imports System.Reflection
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.Services.FileSystem
Imports System.IO
Imports System.Net
Imports Aricie.Services
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class CreateHttpResponseInfo


        Public Property StatusCode As HttpStatusCode = HttpStatusCode.OK

        Public Property ResponseMode As HttpResponseMode = HttpResponseMode.StatusOnly


        <ConditionalVisible("ResponseMode", False, True, HttpResponseMode.TypedReturn)> _
        Public Property TypedReturn As New GeneralVariableInfo() With {.Name = "ContentObject", .DotNetType = New DotNetType(GetType(String))}

        <ConditionalVisible("ResponseMode", False, True, HttpResponseMode.CustomObject)> _
        Public Property CustomObject As New Variables


        Public Property CustomFormatter As New EnabledFeature(Of MediaTypeFormatterInfo)


        Public Property CustomMediaType As New EnabledFeature(Of MediaTypeHeaderInfo)




        Public Function EvaluateToReturn(objContext As IContextLookup) As Object
            Dim toReturn As Object

            Select Case ResponseMode
                Case HttpResponseMode.TypedReturn
                    toReturn = TypedReturn.Evaluate(objContext, objContext)
                Case HttpResponseMode.CustomObject
                    Dim objProps = Me.CustomObject.EvaluateVariables(objContext, objContext)
                    toReturn = objProps.ToCustomObject("")
                Case Else
                    toReturn = Nothing
            End Select
            Return toReturn
        End Function

        
    End Class
End Namespace