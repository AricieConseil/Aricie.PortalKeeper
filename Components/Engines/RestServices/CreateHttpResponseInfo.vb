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
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class CreateHttpResponseInfo


        Public Property StatusCode As System.Net.HttpStatusCode = Net.HttpStatusCode.OK

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
                    toReturn = BuildCustomObject(objProps)
                Case Else
                    toReturn = Nothing
            End Select
            Return toReturn
        End Function

        Private Shared _DynamicTypes As New Dictionary(Of String, Type)

        Public Function BuildCustomObject(fromProperties As Dictionary(Of String, Object)) As Object
            Dim propDescriptors As New List(Of PropertyDescriptor)
            Dim objNameBuilder As New StringBuilder()
            For Each objPair As KeyValuePair(Of String, Object) In fromProperties
                Dim objDumbProp As DumbPropertyDescriptor
                objNameBuilder.Append(objPair.Key)
                If objPair.Value Is Nothing Then
                    objNameBuilder.Append("Nothing")
                    objDumbProp = New DumbPropertyDescriptor(objPair.Key, GetType(Object))
                Else
                    Dim objType As Type = objPair.Value.GetType()
                    objNameBuilder.Append(objType.AssemblyQualifiedName)
                    objDumbProp = New DumbPropertyDescriptor(objPair.Key, objType)
                End If
                propDescriptors.Add(objDumbProp)
            Next
            Dim objName As String = "CustomType" & objNameBuilder.ToString().GetHashCode()

            Dim dynType As Type = Nothing
            If Not _DynamicTypes.TryGetValue(objName, dynType) Then
                SyncLock _DynamicTypes
                    If Not _DynamicTypes.TryGetValue(objName, dynType) Then
                        dynType = Aricie.Services.TypeMerger.CreateType(objName, propDescriptors.ToArray)
                        _DynamicTypes(objName) = dynType
                    End If
                End SyncLock
            End If
            Dim toReturn As Object = Activator.CreateInstance(dynType, fromProperties.Values.ToArray())
            Return toReturn
        End Function

    End Class
End Namespace