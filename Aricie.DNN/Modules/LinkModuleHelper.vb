Imports System.Reflection
Imports Aricie.Services

Namespace Modules.Link

    ''' <summary>
    ''' Properties for the Link module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LinkInfoProperty
        ModuleId
    End Enum
    ''' <summary>
    ''' Methods for the Link controller
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LinkControllerMethod
        AddLink
        UpdateLink
        DeleteLink
    End Enum

    ''' <summary>
    ''' Helper to access information about the link module when it is build as code behind
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LinkModuleHelper
        Inherits Modules.Helper.ModuleHelper(Of LinkInfoProperty, LinkControllerMethod)

        Private Shared ReadOnly ControllerName As String = "DotNetNuke.Modules.Links.LinkController"
        Private Shared ReadOnly LinkInfoName As String = "DotNetNuke.Modules.Links.LinkInfo"

        ''' <summary>
        ''' gets Controller type name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property ControllerFullType As String
            Get
                Return ControllerName
            End Get
            
        End Property

        ''' <summary>
        ''' gets entity type name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property ElementInfoFullType As String
            Get
                Return LinkInfoName
            End Get
        End Property

        ''' <summary>
        ''' Set module id property on link entity
        ''' </summary>
        ''' <param name="objLink"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Sub SetProperty(objLink As Object, value As Object)
            GetProperties(LinkInfoProperty.ModuleId).SetValue(objLink, value, Nothing)
        End Sub

        ''' <summary>
        ''' adds link to controller
        ''' </summary>
        ''' <param name="objLink"></param>
        ''' <remarks></remarks>
        Public Sub AddLink(ByVal objLink As Object)
            GetMethods(LinkControllerMethod.AddLink).Invoke(GetController, New Object() {objLink})
        End Sub

        ''' <summary>
        ''' deletes link from controller
        ''' </summary>
        ''' <param name="ItemID"></param>
        ''' <param name="ModuleId"></param>
        ''' <remarks></remarks>
        Public Sub DeleteLink(ByVal ItemID As Integer, ByVal ModuleId As Integer)
            GetMethods(LinkControllerMethod.DeleteLink).Invoke(GetController, New Object() {ItemID, ModuleId})
        End Sub

        ''' <summary>
        ''' updates link on controller
        ''' </summary>
        ''' <param name="objLink"></param>
        ''' <remarks></remarks>
        Public Sub UpdateLink(ByVal objLink As Object)
            GetMethods(LinkControllerMethod.UpdateLink).Invoke(GetController, New Object() {objLink})
        End Sub




    End Class

End Namespace