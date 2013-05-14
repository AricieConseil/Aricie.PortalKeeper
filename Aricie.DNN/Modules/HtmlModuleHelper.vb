Imports System.Reflection
Imports Aricie.Services

Namespace Modules.Html

    ''' <summary>
    ''' Html entities properties we're interested in 
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum HtmlTextInfoProperty
        ModuleId
        DeskTopHTML
        DesktopSummary
        CreatedByUser
        CreatedDate
        ItemID
    End Enum

    ''' <summary>
    ''' Controller methods we're interested in
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum HtmlTextControllerMethod
        AddHtmlText
        GetHtmlText
        UpdateHtmlText
        GetSearchItems
        ExportModule
        ImportModule
        GetAllHtmlText
        DeleteHtmlText
    End Enum

    ''' <summary>
    ''' A helper class to access the Html module when it is declared as code behind in some DNN versions
    ''' </summary>
    ''' <remarks></remarks>
    Public Module HtmlModuleHelper

        Public Const glbHtmlInfo As String = "HtmlTextInfo"
        Public Const glbFullTypeHtmlInfo As String = "DotNetNuke.Modules.HTML.HtmlTextInfo"
        Public Const glbFullTypeHtmlController As String = "DotNetNuke.Modules.HTML.HtmlTextController"

        Private _HtmlTextInfoType As Type
        Private _HtmlTextInfoProperties As Dictionary(Of HtmlTextInfoProperty, PropertyInfo)
        Private _HtmlTextControllerMethods As Dictionary(Of HtmlTextControllerMethod, MethodInfo)
        Private _HtmlTextControllerType As Type
        Private _HtmlTextController As Object
        Private _HtmlTextAssemblyVersion As Version

        Private _Lock As New Object
        ''' <summary>
        ''' Returns new HtmlModule entity
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNewHtmlTextInfo() As Object
            Return ReflectionHelper.CreateObject(glbFullTypeHtmlInfo)
        End Function

        ''' <summary>
        ''' gets HtmlModule entity type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextInfoType() As Type
            Get
                If _HtmlTextInfoType Is Nothing Then
                    SyncLock _Lock
                        If _HtmlTextInfoType Is Nothing Then
                            _HtmlTextInfoType = ReflectionHelper.CreateType(glbFullTypeHtmlInfo)
                        End If

                    End SyncLock

                End If
                Return _HtmlTextInfoType
            End Get
        End Property

        ''' <summary>
        ''' returns type of the htmlmodule controller
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextControllerType() As Type
            Get
                If _HtmlTextControllerType Is Nothing Then
                    SyncLock _Lock
                        If _HtmlTextControllerType Is Nothing Then
                            _HtmlTextControllerType = ReflectionHelper.CreateType(glbFullTypeHtmlController)
                        End If

                    End SyncLock

                End If
                Return _HtmlTextControllerType
            End Get
        End Property

        ''' <summary>
        ''' returns new instance of the HtmlModule controllers
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextController() As Object
            Get
                If _HtmlTextController Is Nothing Then
                    SyncLock _Lock
                        If _HtmlTextController Is Nothing Then
                            _HtmlTextController = ReflectionHelper.CreateObject(glbFullTypeHtmlController)
                        End If

                    End SyncLock

                End If
                Return _HtmlTextController
            End Get
        End Property

        ''' <summary>
        ''' Returns properties of the HtmlText information entity
        ''' </summary>
        ''' <param name="infoProperty"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextInfoProperties(ByVal infoProperty As HtmlTextInfoProperty) As PropertyInfo
            Get
                If _HtmlTextInfoProperties Is Nothing Then
                    SyncLock _Lock
                        If _HtmlTextInfoProperties Is Nothing Then
                            _HtmlTextInfoProperties = New Dictionary(Of HtmlTextInfoProperty, PropertyInfo)
                            Dim tempDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(HtmlTextInfoType)
                            For Each objEnum As HtmlTextInfoProperty In GetEnumMembers(Of HtmlTextInfoProperty)()
                                If tempDico.ContainsKey(objEnum.ToString) Then
                                    _HtmlTextInfoProperties(objEnum) = tempDico(objEnum.ToString)
                                End If
                            Next
                        End If

                    End SyncLock

                End If

                Return _HtmlTextInfoProperties(infoProperty)
            End Get
        End Property

        ''' <summary>
        ''' Calls a method on the HtmlModule controller
        ''' </summary>
        ''' <param name="infoMehod"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextControllerMethods(ByVal infoMehod As HtmlTextControllerMethod) As MethodInfo
            Get
                If _HtmlTextControllerMethods Is Nothing Then
                    SyncLock _Lock
                        If _HtmlTextControllerMethods Is Nothing Then
                            _HtmlTextControllerMethods = New Dictionary(Of HtmlTextControllerMethod, MethodInfo)
                            Dim tempDico As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(HtmlTextControllerType)
                            For Each objEnum As HtmlTextControllerMethod In GetEnumMembers(Of HtmlTextControllerMethod)()
                                If tempDico.ContainsKey(objEnum.ToString) Then
                                    _HtmlTextControllerMethods(objEnum) = DirectCast(tempDico(objEnum.ToString), MethodInfo)
                                End If
                            Next
                        End If

                    End SyncLock

                End If
                Return _HtmlTextControllerMethods(infoMehod)
            End Get
        End Property

        ''' <summary>
        ''' Returns the version of the assembly that contains the HtmlModule controller
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextAssemblyVersion() As Version
            Get
                If _HtmlTextAssemblyVersion Is Nothing AndAlso HtmlTextControllerType IsNot Nothing Then
                    _HtmlTextAssemblyVersion = HtmlTextControllerType.Assembly.GetName.Version
                End If
                Return _HtmlTextAssemblyVersion
            End Get
        End Property

    End Module

End Namespace


