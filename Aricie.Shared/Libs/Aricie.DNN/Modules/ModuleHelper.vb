Imports System.Reflection
Imports Aricie.Services

Namespace Modules.Helper

    ''' <summary>
    ''' Class to try and replace behaviors on modules that have been set as code behind by DNN in some versions
    ''' </summary>
    ''' <typeparam name="Properties"></typeparam>
    ''' <typeparam name="Methods"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class ModuleHelper(Of Properties, Methods)

        Public MustOverride ReadOnly Property ElementInfoFullType As String
        Public MustOverride ReadOnly Property ControllerFullType As String

        Private _InfoType As Type
        Private _Properties As Dictionary(Of Properties, PropertyInfo)
        Private _ControllerMethods As Dictionary(Of Methods, MethodInfo)
        Private _ControllerType As Type
        Private _Controller As Object
        Private _AssemblyVersion As Version

        Private _Lock As New Object

        ''' <summary>
        ''' Returns an instantiated entity of type ElementInfoFullType
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InstantiateElementInfo() As Object
            Return ReflectionHelper.CreateObject(ElementInfoFullType)
        End Function

        ''' <summary>
        ''' Returns type defined by ElementInfoFullType
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetElementInfoType() As Type
            Get
                If _InfoType Is Nothing Then
                    SyncLock _Lock
                        If _InfoType Is Nothing Then
                            _InfoType = ReflectionHelper.CreateType(ElementInfoFullType)
                        End If

                    End SyncLock

                End If
                Return _InfoType
            End Get
        End Property

        ''' <summary>
        ''' Returns type defined by ControllerFullType
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetControllerType() As Type
            Get
                If _ControllerType Is Nothing Then
                    SyncLock _Lock
                        If _ControllerType Is Nothing Then
                            _ControllerType = ReflectionHelper.CreateType(ControllerFullType)
                        End If

                    End SyncLock

                End If
                Return _ControllerType
            End Get
        End Property
        ''' <summary>
        ''' return ControllerFullType instance
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetController() As Object
            Get
                If _Controller Is Nothing Then
                    SyncLock _Lock
                        If _Controller Is Nothing Then
                            _Controller = ReflectionHelper.CreateObject(ControllerFullType)
                        End If

                    End SyncLock

                End If
                Return _Controller
            End Get
        End Property

        ''' <summary>
        ''' Returns module property 
        ''' </summary>
        ''' <param name="infoProperty"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected ReadOnly Property GetProperties(ByVal infoProperty As Properties) As PropertyInfo
            Get
                If _Properties Is Nothing Then
                    SyncLock _Lock
                        If _Properties Is Nothing Then
                            _Properties = New Dictionary(Of Properties, PropertyInfo)
                            Dim tempDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(GetElementInfoType())
                            For Each objEnum As Properties In GetEnumMembers(Of Properties)()
                                If tempDico.ContainsKey(objEnum.ToString) Then
                                    _Properties(objEnum) = tempDico(objEnum.ToString)
                                End If
                            Next
                        End If

                    End SyncLock

                End If

                Return _Properties(infoProperty)
            End Get
        End Property
        ''' <summary>
        ''' returns controller MethodInfo
        ''' </summary>
        ''' <param name="infoMehod"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected ReadOnly Property GetMethods(ByVal infoMehod As Methods) As MethodInfo
            Get
                If _ControllerMethods Is Nothing Then
                    SyncLock _Lock
                        If _ControllerMethods Is Nothing Then
                            _ControllerMethods = New Dictionary(Of Methods, MethodInfo)
                            Dim tempDico As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(GetControllerType)
                            For Each objEnum As Methods In GetEnumMembers(Of Methods)()
                                If tempDico.ContainsKey(objEnum.ToString) Then
                                    _ControllerMethods(objEnum) = DirectCast(tempDico(objEnum.ToString), MethodInfo)
                                End If
                            Next
                        End If

                    End SyncLock

                End If
                Return _ControllerMethods(infoMehod)
            End Get
        End Property

        ''' <summary>
        ''' Returns version number of the assembly for the controller
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HtmlTextAssemblyVersion() As Version
            Get
                If _AssemblyVersion Is Nothing AndAlso GetControllerType() IsNot Nothing Then
                    _AssemblyVersion = GetControllerType().Assembly.GetName.Version
                End If
                Return _AssemblyVersion
            End Get
        End Property
    End Class


End Namespace