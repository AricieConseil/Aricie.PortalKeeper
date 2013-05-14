
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
Imports Aricie.Providers
Imports System.Text
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Globalization
Imports System.Web.Compilation
Imports System.Runtime.Serialization
Imports System.Security
Imports System.Web
Imports System.Security.Permissions
Imports Aricie.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Web.Caching
Imports System.Runtime.CompilerServices
Imports System.ComponentModel

Namespace Services

    ''' <summary>
    ''' Global Helper with many Reflection related methods.
    ''' </summary>
    Public Class ReflectionHelper
        Implements System.Web.Hosting.IRegisteredObject
        'Implements IDisposable

        Public Delegate Function CreateInstanceCallBack(ByVal objType As Type) As Object


        Public Enum ReflectionMembers
            Type
            EntityProperties
            EntityCollectionProperties
            Properties
            Methods
            Interfaces
            Serialization
        End Enum

#Region "Singleton Logic"

        Private Shared _Instance As ReflectionHelper
        Private Shared _SingletonLock As New Object
        Private Shared _HasUnrestrictedSecurityPermission As Nullable(Of Boolean)

        Public Shared Function HasUnrestrictedSecurityPermission() As Boolean
            If Not _HasUnrestrictedSecurityPermission.HasValue Then
                _HasUnrestrictedSecurityPermission = SecurityManager.IsGranted(New SecurityPermission(PermissionState.Unrestricted))
            End If
            Return _HasUnrestrictedSecurityPermission.Value
        End Function

        Public Shared Function Instance() As ReflectionHelper
            If _Instance Is Nothing Then
                SyncLock _SingletonLock
                    If _Instance Is Nothing Then
                        Dim tempInstance As New ReflectionHelper
                        If HasUnrestrictedSecurityPermission() Then
                            System.Web.Hosting.HostingEnvironment.RegisterObject(tempInstance)
                        End If
                        _Instance = tempInstance
                    End If
                End SyncLock
            End If
            Return _Instance
        End Function

        Public Sub DisposeAll()
            For Each objSingleton As Object In Me._Singletons.Values
                If objSingleton IsNot Nothing AndAlso TypeOf objSingleton Is IDisposable Then
                    DirectCast(objSingleton, IDisposable).Dispose()
                End If
            Next
            Me._Singletons.Clear()
        End Sub

        Public Sub [Stop](ByVal immediate As Boolean) Implements System.Web.Hosting.IRegisteredObject.Stop
            Me.DisposeAll()
            System.Web.Hosting.HostingEnvironment.UnregisterObject(Me)
        End Sub

#End Region

        Private Const glbDependency As String = Aricie.Constants.Cache.Dependency & "Reflection"

        Private _Singletons As New Dictionary(Of Type, Object)
        Private _XmlSerializers As New Dictionary(Of Type, Dictionary(Of String, XmlSerializer))
        Private Shared _XmlSerializerFactory As New XmlSerializerFactory
        Private _TypesByTypeName As New Dictionary(Of String, Type)
        Private _PropertiesByType As New Dictionary(Of Type, Dictionary(Of String, PropertyInfo))



#Region "type methods"

        'Public Shared Function GetCachingKey(ByVal typeName As String, ByVal mode As ReflectionMembers) As String
        '    Return Constants.GetKey(Of Type)(typeName, mode.ToString)

        'End Function




        Public Shared Function CreateType(ByVal typeName As String) As Type
            Return CreateType(typeName, True)

        End Function

        Public Shared Function CreateType(ByVal typeName As String, ByVal throwOnError As Boolean) As Type
            Dim toReturn As Type = Nothing

            If Not ReflectionHelper.Instance._TypesByTypeName.TryGetValue(typeName, toReturn) Then
                ' use reflection to get the type of the class
                toReturn = BuildManager.GetType(typeName, throwOnError, True)

                ' insert the type into the cache
                SyncLock ReflectionHelper.Instance._TypesByTypeName
                    ReflectionHelper.Instance._TypesByTypeName(typeName) = toReturn
                End SyncLock

                'CacheHelper.SetGlobal(Of Type)(toReturn, typeName)

            End If
            Return toReturn

        End Function


        Private Shared _RegexSafeTypeName As New Regex("([^\[\]]+)(\,\sVersion\=[^\[\]]+)", RegexOptions.Compiled)
        Private Shared _SafeTypeNames As New Dictionary(Of String, String)



        Public Shared Function GetSafeTypeName(ByVal objType As Type) As String
            Return GetSafeTypeName(objType.AssemblyQualifiedName)
        End Function

        Public Shared Function GetSafeTypeName(ByVal objAssemblyQualifiedName As String) As String
            Dim toReturn As String = Nothing
            If Not _SafeTypeNames.TryGetValue(objAssemblyQualifiedName, toReturn) Then
                toReturn = _RegexSafeTypeName.Replace(objAssemblyQualifiedName, "$1")
                Try
                    ReflectionHelper.CreateType(toReturn)
                Catch ex As Exception
                    toReturn = objAssemblyQualifiedName
                End Try
                SyncLock _SafeTypeNames
                    _SafeTypeNames(objAssemblyQualifiedName) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function


        Public Shared Function GetSimpleTypeName(objType As Type) As String
            If objType IsNot Nothing Then
                If objType.IsGenericType Then
                    Dim genTypes As Type() = objType.GetGenericArguments
                    Dim toReturn As String = objType.Name & "["
                    For Each genType As Type In genTypes
                        toReturn &= "["c & GetSimpleTypeName(genType) & "],"
                    Next
                    toReturn = toReturn.TrimEnd(","c) & "]"c
                    Return toReturn
                Else
                    Return objType.Name
                End If
            End If
            Return String.Empty
        End Function

        Public Shared Function GetCollectionElementType(ByVal collection As ICollection) As Type

            Dim objetType As Type

            If collection IsNot Nothing AndAlso collection.Count > 0 Then
                Dim enumTor As IEnumerator = collection.GetEnumerator
                enumTor.MoveNext()
                objetType = enumTor.Current.GetType()
            Else
                Dim collectionType As Type = collection.GetType


                If collectionType.IsGenericType Then
                    Dim args As Type() = collectionType.GetGenericArguments()
                    objetType = args(args.Length - 1)
                Else
                    objetType = collectionType.GetElementType
                End If
                If objetType Is Nothing Then
                    Throw New ArgumentException("type is not an array or a generic list ", "collectionType")
                End If
            End If


            Return objetType

        End Function

        Private Shared _TrueReferenceTypes As New Dictionary(Of Type, Boolean)


        Public Shared Function IsTrueReferenceType(ByVal objType As Type) As Boolean
            Dim toReturn As Boolean
            If Not _TrueReferenceTypes.TryGetValue(objType, toReturn) Then
                toReturn = (Not objType.IsValueType) AndAlso Type.GetTypeCode(objType) = TypeCode.Object
                SyncLock _TrueReferenceTypes
                    _TrueReferenceTypes(objType) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function

        Public Shared Function HasDefaultConstructor(ByVal obType As Type) As Boolean
            If obType.IsValueType Then
                Return True
            End If
            Dim cTor As ConstructorInfo = obType.GetConstructor(Type.EmptyTypes)
            If cTor Is Nothing Then
                Return False
            End If
            Return True
        End Function


#End Region

#Region "Singleton methods"

        Public Shared Function GetSingleton(Of T)() As T
            Return GetSingleton(Of T)(GetType(T), Nothing)
        End Function

        Public Shared Function GetSingleton(Of T)(ByVal objCreateInstanceCallBack As CreateInstanceCallBack) As T
            Return GetSingleton(Of T)(GetType(T), objCreateInstanceCallBack)
        End Function

        Public Shared Function GetSingleton(Of T)(ByVal objType As Type) As T
            Return GetSingleton(Of T)(objType, Nothing)
        End Function


        Public Shared Function GetSingleton(Of T)(ByVal objType As Type, ByVal objCreateInstanceCallBack As CreateInstanceCallBack) As T
            Return DirectCast(GetSingleton(objType, objCreateInstanceCallBack), T)
        End Function

        Public Shared Function GetSingleton(ByVal objType As Type, ByVal objCreateInstanceCallBack As CreateInstanceCallBack) As Object
            'Return GetSingleton(GetSafeTypeName(objType))
            Dim toReturn As Object = Nothing
            If Not ReflectionHelper.Instance._Singletons.TryGetValue(objType, toReturn) Then
                SyncLock _SingletonLock
                    If Not ReflectionHelper.Instance._Singletons.TryGetValue(objType, toReturn) Then
                        If objCreateInstanceCallBack Is Nothing Then
                            toReturn = ReflectionHelper.CreateObject(objType)

                        Else
                            toReturn = objCreateInstanceCallBack.Invoke(objType)
                        End If
                        ReflectionHelper.Instance._Singletons(objType) = toReturn
                    End If
                End SyncLock
            End If
            Return toReturn
        End Function


        Public Shared Function GetSingleton(ByVal objType As Type) As Object
            Return GetSingleton(objType, Nothing)
        End Function


        Public Shared Function GetSingleton(ByVal typeName As String) As Object
            If typeName <> "" Then
                Dim objType As Type = ReflectionHelper.CreateType(typeName)
                Return ReflectionHelper.GetSingleton(objType)
            End If
            Return Nothing
        End Function



#End Region


#Region "Properties methods"

        Public Shared Function GetPropertiesDictionary(Of T)() As Dictionary(Of String, PropertyInfo)
            Return GetPropertiesDictionary(GetType(T))
        End Function

        Public Shared Function GetMembersDictionary(Of T)() As Dictionary(Of String, MemberInfo)
            Return GetMembersDictionary(GetType(T))
        End Function

        Public Shared Function GetMembersDictionary(ByVal objType As Type) As Dictionary(Of String, MemberInfo)


            ' Use the cache because the reflection used later is expensive
            Dim objMembers As Dictionary(Of String, MemberInfo) = GetGlobal(Of Dictionary(Of String, MemberInfo))(objType.FullName)

            If objMembers Is Nothing Then
                objMembers = New Dictionary(Of String, MemberInfo)(StringComparer.OrdinalIgnoreCase)
                For Each objMember As MemberInfo In objType.GetMembers()
                    If Not objMembers.ContainsKey(objMember.Name) Then
                        objMembers(objMember.Name) = objMember
                    End If

                Next
                SetCacheDependant(Of Dictionary(Of String, MemberInfo))(objMembers, glbDependency, Cache.NoSlidingExpiration, objType.FullName)
            End If

            Return objMembers

        End Function

        Public Shared Function GetFullMembersDictionary(ByVal objType As Type) As Dictionary(Of String, List(Of MemberInfo))


            ' Use the cache because the reflection used later is expensive
            Dim objMembers As Dictionary(Of String, List(Of MemberInfo)) = GetGlobal(Of Dictionary(Of String, List(Of MemberInfo)))(objType.FullName)

            If objMembers Is Nothing Then
                objMembers = New Dictionary(Of String, List(Of MemberInfo))(StringComparer.OrdinalIgnoreCase)
                For Each objMember As MemberInfo In objType.GetMembers()
                    Dim memberList As List(Of MemberInfo) = Nothing
                    If Not objMembers.TryGetValue(objMember.Name, memberList) Then
                        memberList = New List(Of MemberInfo)
                    End If
                    memberList.Add(objMember)
                    objMembers(objMember.Name) = memberList
                Next
                SetCacheDependant(Of Dictionary(Of String, List(Of MemberInfo)))(objMembers, glbDependency, Cache.NoSlidingExpiration, objType.FullName)
            End If

            Return objMembers

        End Function




        Public Shared Function GetMember(ByVal objtype As Type, ByVal memberName As String) As MemberInfo
            Dim toReturn As MemberInfo = Nothing
            If objtype IsNot Nothing Then
                If memberName.IndexOf("-"c) > 0 Then
                    Dim memberSplit As String() = memberName.Split("-"c)
                    If memberSplit.Length <> 2 OrElse Not IsNumber(memberSplit(1)) Then
                        Throw New ArgumentException(String.Format("memberName {0} wrongly Formatted", memberName), "memberName")
                    End If
                    Dim objMembers As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(objtype)
                    Dim objList As List(Of MemberInfo) = Nothing
                    If objMembers.TryGetValue(memberSplit(0), objList) Then
                        Dim idx As Integer = Integer.Parse(memberSplit(1))
                        If idx > objList.Count - 1 Then
                            Throw New ArgumentException(String.Format("{0} has an invalid member index", memberName), "memberName")
                        End If
                        toReturn = objList(idx)
                    End If
                Else
                    Dim objMembers As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(objtype)
                    objMembers.TryGetValue(memberName, toReturn)

                End If
            End If
            Return toReturn
        End Function



        Public Shared Function GetPropertiesDictionary(ByVal objType As Type) As Dictionary(Of String, PropertyInfo)


            ' Use the cache because the reflection used later is expensive
            Dim objProperties As Dictionary(Of String, PropertyInfo) = Nothing

            'objProperties = GetGlobal(Of Dictionary(Of String, PropertyInfo))(objType.FullName)
            If Not ReflectionHelper.Instance._PropertiesByType.TryGetValue(objType, objProperties) Then
                objProperties = New Dictionary(Of String, PropertyInfo)(StringComparer.OrdinalIgnoreCase)
                Dim objProperty As PropertyInfo
                For Each objProperty In objType.GetProperties()
                    If Not objProperties.ContainsKey(objProperty.Name) Then
                        objProperties(objProperty.Name) = objProperty
                    End If

                Next
                ReflectionHelper.Instance._PropertiesByType(objType) = objProperties
                'SetCacheDependant(Of Dictionary(Of String, PropertyInfo))(objProperties, glbDependency, TimeSpan.FromMinutes(20), _
                'objType.FullName)
            End If


            Return objProperties

        End Function


        Public Shared Function GetEntityPropertiesDictionary(ByVal objType As Type) _
            As Dictionary(Of String, PropertyInfo)

            ' Use the cache because the reflection used later is expensive
            Dim objProperties As Dictionary(Of String, PropertyInfo) = CacheHelper.GetGlobal(Of Dictionary(Of String, PropertyInfo))(objType.FullName, ReflectionMembers.EntityProperties.ToString())

            If objProperties Is Nothing Then
                objProperties = New Dictionary(Of String, PropertyInfo)
                Dim tempProperties As Dictionary(Of String, PropertyInfo) = GetPropertiesDictionary(objType)
                For Each pkey As String In tempProperties.Keys
                    If Not tempProperties(pkey).PropertyType.GetInterface("IEntity2") Is Nothing Then
                        objProperties(pkey) = tempProperties(pkey)
                    End If
                Next
                CacheHelper.SetGlobal(Of Dictionary(Of String, PropertyInfo))(objProperties, objType.FullName, ReflectionMembers.EntityProperties.ToString())
            End If

            Return objProperties

        End Function

        Public Shared Function GetEntityCollectionPropertiesDictionary(ByVal objType As Type) _
            As Dictionary(Of String, PropertyInfo)


            ' Use the cache because the reflection used later is expensive
            Dim objProperties As Dictionary(Of String, PropertyInfo) = CacheHelper.GetGlobal(Of Dictionary(Of String, PropertyInfo))(objType.FullName, ReflectionMembers.EntityCollectionProperties.ToString())

            If objProperties Is Nothing Then
                objProperties = New Dictionary(Of String, PropertyInfo)
                Dim tempProperties As Dictionary(Of String, PropertyInfo) = GetPropertiesDictionary(objType)
                For Each pkey As String In tempProperties.Keys
                    If Not tempProperties(pkey).PropertyType.GetInterface("IEntityCollection2") Is Nothing Then
                        objProperties.Add(pkey, tempProperties(pkey))
                    End If
                Next
                CacheHelper.SetGlobal(Of Dictionary(Of String, PropertyInfo))(objProperties, objType.FullName, ReflectionMembers.EntityCollectionProperties.ToString())
            End If

            Return objProperties

        End Function

        ''' <summary>
        ''' Renvoie la propriété d'un objet
        ''' </summary>
        ''' <param name="t">Le type de l'objet sur lequel on travaille</param>
        ''' <param name="propertyName">Nom de la propriété</param>
        ''' <param name="target">Cible de l'accès à la propriété</param>
        ''' <returns>L'objet retourné par l'accès à la propriété de l'objet cible</returns>
        ''' <remarks>Straight rip de DNN pour besoin d'abstraction Jean-Sylvain</remarks>
        Public Shared Function GetProperty(ByVal t As Type, ByVal propertyName As String, ByVal target As Object) As Object
            If (t IsNot Nothing) Then
                Return t.InvokeMember(propertyName, BindingFlags.GetProperty, Nothing, RuntimeHelpers.GetObjectValue(target), Nothing)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Renvoie la propriété d'un objet sans donner son type 
        ''' </summary>
        ''' <param name="target">Objet dont on veut récupérer la propriété</param>
        ''' <param name="propertyName">Nom de la propriété</param>
        ''' <returns>Valeur de la propriété pour l'objet cible</returns>
        ''' <remarks></remarks>
        Public Shared Function GetProperty(target As Object, propertyName As String) As Object
            Return GetProperty(target.GetType(), propertyName, target)
        End Function

        ''' <summary>
        '''  Renvoie la propriété typée d'un objet sans donner son type 
        ''' </summary>
        ''' <param name="target">Objet dont on veut récupérer la propriété</param>
        ''' <param name="propertyName">Nom de la propriété</param>
        ''' <returns>Valeur de la propriété castée pour l'objet cible</returns>
        ''' <remarks>Une exception est lancée s'il y la propriété n'est pas du bon type</remarks>
        Public Shared Function GetProperty(Of T As Class)(target As Object, propertyName As String) As T
            Dim result As T = TryCast(GetProperty(target, propertyName), T)
            If result Is Nothing Then
                Throw New InvalidOperationException(String.Format("La propriété {0} de l'objet de type {1} n'est pas de type {2}", propertyName, target.GetType().Name, GetType(T).Name))
            End If
            Return result
        End Function

#End Region

#Region "Methodinfos methods"


        Public Shared Function CreateDelegate(Of T)(ByVal objDelegate As DelegateInfo(Of T)) As [Delegate]

            If objDelegate.InvocationList.Count = 0 Then
                Return CreateDelegate(Of T)(objDelegate.TypeName, objDelegate.MethodName)
            Else
                Dim tempList As New List(Of [Delegate])(objDelegate.InvocationList.Count)
                For Each subDelegate As DelegateInfo(Of T) In objDelegate.InvocationList
                    tempList.Add(subDelegate.GetDelegate)
                Next
                Return [Delegate].Combine(tempList.ToArray)
            End If

        End Function

        Public Shared Function CreateDelegate(Of T)(ByVal typeName As String, ByVal methodName As String) As [Delegate]

            Dim targetType As Type = ReflectionHelper.CreateType(typeName)
            Dim method As MethodInfo = DirectCast(ReflectionHelper.GetMember(targetType, methodName), MethodInfo)
            If method IsNot Nothing Then
                Dim target As Object = Nothing
                If Not method.IsStatic Then
                    target = ReflectionHelper.CreateObject(targetType)
                End If
                Return [Delegate].CreateDelegate(GetType(T), target, method)
            End If
            Return Nothing

        End Function



#End Region

#Region "Serialization methods"
        Private _BinaryFormatter As BinaryFormatter

        Public ReadOnly Property BinaryFormatter() As BinaryFormatter
            Get
                If _BinaryFormatter Is Nothing Then
                    _BinaryFormatter = New BinaryFormatter()
                End If
                Return _BinaryFormatter
            End Get
        End Property

        Public Shared Function IsFullySerializable(ByVal objToCheck As Object) As Boolean
            Dim objDico As New Dictionary(Of Object, Boolean)
            Return IsFullySerializable(objToCheck, objDico)
        End Function

        Public Shared Function IsFullySerializable(ByVal objToCheck As Object, ByRef objDico As Dictionary(Of Object, Boolean)) As Boolean
            If Not IsMainObjectSerializable(objToCheck) Then
                Return False
            End If
            objDico(objToCheck) = True
            If TypeOf objToCheck Is IDictionary Then
                Dim objDictionary As IDictionary = DirectCast(objToCheck, IDictionary)
                If objDictionary.Keys.Count > 0 Then
                    Dim enumTor As IEnumerator = objDictionary.Keys.GetEnumerator
                    If enumTor.MoveNext Then
                        If Not IsFullySerializable(enumTor.Current, objDico) Then
                            Return False
                        End If
                        If Not IsFullySerializable(objDictionary(enumTor.Current), objDico) Then
                            Return False
                        End If
                    End If
                End If
            Else
                If TypeOf objToCheck Is ICollection Then
                    Dim objCollect As ICollection = DirectCast(objToCheck, ICollection)
                    If objCollect.Count > 0 Then
                        Dim enumTor As IEnumerator = objCollect.GetEnumerator
                        If enumTor.MoveNext Then
                            If Not IsFullySerializable(enumTor.Current, objDico) Then
                                Return False
                            End If
                        End If
                    End If
                End If
            End If
            'Dim props As Dictionary(Of String, PropertyInfo) = Aricie.Services.ReflectionHelper.GetPropertiesDictionary(objToCheck.GetType)
            'For Each prop As PropertyInfo In props.Values
            '    Dim propObj As Object = prop.GetValue(objToCheck, Nothing)
            '    If Not objDico.ContainsKey(propObj) Then
            '        If Not IsFullySerializable(propObj, objDico) Then
            '            Return False
            '        End If
            '    End If
            'Next
            Return True
        End Function

        ''' <summary>
        ''' Permet de vérifier si la chaine d'objets dont descend l'objet passé en paramètre est sérialisable
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns>Booléen indiquant si l'objet est sérialisable</returns>
        ''' <remarks>Mis en place pour contourner le problème sur les objets héritants de ContentItem qui n'est plus sérialisable depuis DNN 5.6</remarks>
        Public Shared Function IsMainObjectSerializable(ByVal obj As Object) As Boolean
            If obj IsNot Nothing Then
                Dim CurrentObjectType = obj.GetType
                While CurrentObjectType IsNot Nothing
                    If Not CurrentObjectType.IsSerializable Then
                        Return False
                    End If
                    CurrentObjectType = CurrentObjectType.BaseType
                End While
            End If
            Return True
        End Function



        Public Shared Function ContainsGenericsSelfReference(ByVal obj As Object) As Boolean
            If obj IsNot Nothing Then
                Dim objType As Type = obj.GetType
                Dim hierarchy As New Stack(Of Type)
                hierarchy.Push(objType)
                Dim parentType As Type = objType.BaseType
                While parentType IsNot Nothing AndAlso parentType IsNot objType
                    objType = parentType
                    hierarchy.Push(objType)
                    parentType = objType.BaseType
                End While
                While hierarchy.Count > 1
                    objType = hierarchy.Pop
                    If objType.IsGenericType AndAlso objType.GetInterface("IEnumerable") IsNot Nothing Then
                        Dim childType As Type = hierarchy.Peek
                        Dim args As Type() = objType.GetGenericArguments
                        For Each argT As Type In args
                            If hierarchy.Contains(argT) Then
                                'If argT Is childType Then
                                'If hierarchy.Contains(argT) Then
                                'Throw New ApplicationException(String.Format("Generic type {0} with self referencing argument will break with a stack overflow with default WCF serialization", ReflectionHelper.GetSafeTypeName(vType)))
                                Return True
                            End If
                        Next
                    End If
                End While
            End If
            Return False
        End Function

#End Region

#Region "XmlSerialization methods"



#Region "GetSerializer"


        ''' <summary>
        ''' Create a type specific XmlSerializer.
        ''' </summary>
        ''' <typeparam name="T"> the type to build an XmlSerializer for</typeparam>
        ''' <returns>the XmlSerializer specific to T</returns>
        ''' <remarks>Si le type est un générics, les types parametres sont ajoutés en extra type</remarks>
        Public Shared Function GetSerializer(Of T)() As XmlSerializer


            Return GetSerializer(GetType(T), Nothing, True, Nothing)

        End Function

        Public Shared Function GetSerializer(Of T)(ByVal rootName As String) As XmlSerializer
            Return GetSerializer(GetType(T), Nothing, True, rootName)
        End Function

        Public Shared Function GetSerializer(Of T)(ByVal extraTypes() As Type, ByVal rootName As String) As XmlSerializer


            Return GetSerializer(GetType(T), extraTypes, True, rootName)

        End Function


        Public Shared Function GetSerializer(Of T)(ByVal extraTypes() As Type) As XmlSerializer


            Return GetSerializer(GetType(T), extraTypes, True, Nothing)

        End Function

        Public Shared Function GetSerializer(Of T)(ByVal useCache As Boolean) As XmlSerializer
            Return GetSerializer(GetType(T), Nothing, useCache, Nothing)

        End Function


        Public Shared Function GetSerializer(ByVal objType As Type) As XmlSerializer

            Return GetSerializer(objType, Nothing, True, Nothing)

        End Function

        Public Shared Function GetSerializer(ByVal objType As Type, ByVal useCache As Boolean) As XmlSerializer



            Return GetSerializer(objType, Nothing, useCache, Nothing)

        End Function

        Public Shared Function GetSerializer(ByVal objType As Type, ByVal extraTypes() As Type, ByVal useCache As Boolean) As XmlSerializer
            Return GetSerializer(objType, extraTypes, useCache, Nothing)
        End Function

        Public Shared Function GetSerializer(ByVal objType As Type, ByVal extraTypes() As Type, ByVal useCache As Boolean, ByVal rootName As String) As XmlSerializer


            Dim toReturn As XmlSerializer = Nothing

            If useCache Then


                Dim tempDico As Dictionary(Of String, XmlSerializer) = Nothing

                If Not Instance._XmlSerializers.TryGetValue(objType, tempDico) Then
                    tempDico = New Dictionary(Of String, XmlSerializer)
                End If
                Dim params As New List(Of String)
                If extraTypes IsNot Nothing Then
                    For Each extraType As Type In extraTypes
                        params.Add(extraType.FullName)
                    Next
                End If
                If Not String.IsNullOrEmpty(rootName) Then
                    params.Add(rootName)
                End If
                Dim key As String = ""
                If params.Count > 0 Then
                    key = Constants.GetKey(params.ToArray)
                End If
                If Not tempDico.TryGetValue(key, toReturn) Then
                    toReturn = BuildSerializer(objType, extraTypes, rootName)
                    tempDico(key) = toReturn
                    SyncLock Instance._XmlSerializers
                        Instance._XmlSerializers(objType) = tempDico
                    End SyncLock
                End If

                'toReturn = GetSingleton(Of XmlSerializer)(objType, New CreateInstanceCallBack(AddressOf BuildSerializer))
            Else
                toReturn = BuildSerializer(objType, extraTypes, rootName)
            End If


            Return toReturn

        End Function

#End Region

#Region "Serialize"

        Public Shared Function Serialize(ByVal objObject As Object) As XmlDocument

            Return Serialize(objObject, False)

        End Function

        Public Shared Function Serialize(ByVal objObject As Object, ByVal omitDeclaration As Boolean) As XmlDocument


            Dim objStringBuilder As New StringBuilder

            Using objTextWriter As TextWriter = New StringWriter(objStringBuilder)
                ReflectionHelper.Serialize(objObject, omitDeclaration, objTextWriter)
            End Using

            Dim xmlSerializedObject As New XmlDocument

            Using objStringReader As New StringReader(objStringBuilder.ToString())
                xmlSerializedObject.Load(objStringReader)
            End Using

            Return xmlSerializedObject

        End Function

        Public Shared Sub Serialize(ByVal objObject As Object, ByVal omitDeclaration As Boolean, ByRef objTextWriter As TextWriter)


            'New XmlTextWriter(objTextWriter)
            Dim objXmlSettings As New XmlWriterSettings
            objXmlSettings.Encoding = Encoding.UTF8
            objXmlSettings.Indent = True
            If omitDeclaration Then
                objXmlSettings.OmitXmlDeclaration = True
            End If

            Using objXmlWriter As XmlWriter = XmlTextWriter.Create(objTextWriter, objXmlSettings)
                ReflectionHelper.Serialize(objObject, objXmlWriter)
                objXmlWriter.Flush()
            End Using

        End Sub


        Public Shared Sub Serialize(ByVal objObject As Object, ByRef objXmlWriter As XmlWriter)

            Dim objType As Type = objObject.GetType

            Dim objXmlSerializer As XmlSerializer = GetSerializer(objType)


            objXmlSerializer.Serialize(objXmlWriter, objObject)


        End Sub

#End Region


#Region "Deserialize"

        Public Shared Function Deserialize(Of T)(ByVal strObject As String) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, strObject), T)

        End Function

        Public Shared Function Deserialize(ByVal mainType As Type, ByVal objString As String) As Object


            Using textReader As New StringReader(objString)
                'Try

                Return Deserialize(mainType, textReader)
                'Finally
                '    textReader.Close()
                'End Try
            End Using


        End Function

        Public Shared Function Deserialize(Of T)(ByVal reader As TextReader) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, reader), T)

        End Function


        Public Shared Function Deserialize(ByVal mainType As Type, ByVal reader As TextReader) As Object


            Dim readerSettings As New XmlReaderSettings()
            readerSettings.ProhibitDtd = False
            readerSettings.IgnoreProcessingInstructions = True
            Using xmlReader As XmlReader = xmlReader.Create(reader, readerSettings)
                'Try
                Return Deserialize(mainType, xmlReader)
                'Finally
                '    xmlReader.Close()
                'End Try
            End Using

        End Function

        Public Shared Function Deserialize(Of T)(ByVal reader As XmlReader) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, reader), T)

        End Function

        Public Shared Function Deserialize(ByVal mainType As Type, ByVal reader As XmlReader) As Object


            Dim objXmlSerializer As XmlSerializer = GetSerializer(mainType)
            Return objXmlSerializer.Deserialize(reader)

        End Function

#End Region




#End Region


#Region "Objects manipulation"

        Public Shared Function CreateObject(ByVal objectType As Type) As Object

            Dim toReturn As Object

            If ReflectionHelper.IsTrueReferenceType(objectType) Then
                toReturn = Activator.CreateInstance(objectType)
            Else
                If objectType Is GetType(String) Then
                    toReturn = ""
                ElseIf objectType Is GetType(DateTime) Then
                    toReturn = DateTime.Now
                Else
                    toReturn = Convert.ChangeType(0, Type.GetTypeCode(objectType))
                    'toReturn = Convert.ChangeType(0, objectType)
                    If objectType.IsEnum Then
                        toReturn = [Enum].Parse(objectType, toReturn.ToString)
                    End If
                End If
            End If

            Return toReturn

        End Function

        Public Shared Function CreateObject(ByVal typeName As String) As Object


            Dim objectType As Type = ReflectionHelper.CreateType(typeName)

            Return CreateObject(objectType)

        End Function

        Public Shared Function CreateObject(Of T)(ByVal typeName As String) As T
            Return DirectCast(CreateObject(typeName), T)

        End Function

        Public Shared Function CreateObject(Of T)() As T
            Return Activator.CreateInstance(Of T)()

        End Function



        Public Shared Function CloneObject(Of T)(ByVal objectToClone As T) As T
            Return CloneObject(Of T)(objectToClone, False)
        End Function


        Public Shared Function CloneObject(Of T)(ByVal objectToClone As T, useSerialization As Boolean) As T
            If useSerialization Then
                Dim striSerialized As String = ReflectionHelper.Serialize(objectToClone).OuterXml
                Return ReflectionHelper.Deserialize(Of T)(striSerialized)
            Else
                Dim cloneDico As New Dictionary(Of Object, Object)
                Return DirectCast(CloneObject(objectToClone, cloneDico), T)
            End If
        End Function


        Public Shared Function CloneObject(ByVal objectToClone As Object) As Object
            Dim cloneDico As New Dictionary(Of Object, Object)
            Return CloneObject(objectToClone, cloneDico)
        End Function



        Private Shared Function CloneObject(ByVal objectToClone As Object, ByRef cloneDico As Dictionary(Of Object, Object)) As Object

            'first check if we already cloned the object
            If cloneDico.ContainsKey(objectToClone) Then
                Return cloneDico(objectToClone)
            End If

            'check for null
            If objectToClone Is Nothing Then
                Return Nothing
            End If

            'SAMY: Contournement du problème de l'arrivée du TimeZoneInfo dans le monde de DNN.
            'TODO: changer le fonctionnement du clonage...
            If (TypeOf objectToClone Is TimeZoneInfo) Then
                Return TimeZoneInfo.FromSerializedString(DirectCast(objectToClone, TimeZoneInfo).ToSerializedString())
            End If


            Dim objType As Type = objectToClone.GetType()


            'if the object is a value type or a string, return it
            If Not IsTrueReferenceType(objType) Then
                Return objectToClone
            End If

            Dim toReturn As Object = Nothing

            'if the object is an tractable enumerable, we can create a new object an feed it with inner clones 
            '(we check that before ICloneable otherwise the inner objects won't be cloned)
            Dim objIEnumerableType As Type = objType.GetInterface("IEnumerable", False)
            If Not (objIEnumerableType Is Nothing) Then

                If objType.IsArray Then
                    Dim arrayToClone As Array = DirectCast(objectToClone, Array)
                    toReturn = Array.CreateInstance(objType.GetElementType, arrayToClone.Length)
                    cloneDico(objectToClone) = toReturn
                    For i As Integer = 0 To arrayToClone.Length - 1
                        Dim obj As Object = arrayToClone.GetValue(i)
                        DirectCast(toReturn, Array).SetValue(CloneObject(obj, cloneDico), i)
                    Next
                Else



                    Dim objIDicType As Type = objType.GetInterface("IDictionary", False)
                    If Not (objIDicType Is Nothing) Then
                        Dim origDic As IDictionary = CType(objectToClone, IDictionary)
                        toReturn = ReflectionHelper.CreateObject(objType)
                        cloneDico(objectToClone) = toReturn
                        For Each key As Object In origDic.Keys
                            DirectCast(toReturn, IDictionary)(ReflectionHelper.CloneObject(key, cloneDico)) = ReflectionHelper.CloneObject(origDic(key), cloneDico)
                        Next

                    Else
                        Dim objIListType As Type = objType.GetInterface("IList", False)

                        If Not (objIListType Is Nothing) Then
                            Dim origList As IList = CType(objectToClone, IList)
                            toReturn = ReflectionHelper.CreateObject(objType)
                            cloneDico(objectToClone) = toReturn
                            Dim obj As Object
                            For j As Integer = 0 To origList.Count - 1
                                obj = origList(j)
                                DirectCast(toReturn, IList).Add(ReflectionHelper.CloneObject(obj, cloneDico))
                            Next
                        End If
                    End If


                End If

            End If

            If toReturn Is Nothing Then
                'check if the object is cloneable
                Dim objICloneType As Type = objType.GetInterface("ICloneable", False)

                If Not (objICloneType Is Nothing) Then
                    Dim objIClone As ICloneable = CType(objectToClone, ICloneable)

                    If Not (objIClone Is Nothing) Then
                        toReturn = objIClone.Clone()
                        cloneDico(objectToClone) = toReturn
                        'we don't need further action, return
                        Return toReturn
                    End If
                End If
            End If





            If toReturn Is Nothing Then
                toReturn = ReflectionHelper.CreateObject(objType)
                cloneDico(objectToClone) = toReturn
            End If

            'Finally, try to clone any writeable property (it may complete or even discard the ienumerable actions)
            Dim props As Dictionary(Of String, PropertyInfo) = GetPropertiesDictionary(objType)

            For Each propName As String In props.Keys
                Dim p As PropertyInfo = props(propName)

                If p.CanWrite AndAlso Not p.GetIndexParameters.Length > 0 Then 'AndAlso p.GetCustomAttributes(GetType(XmlIgnoreAttribute), True).Length = 0
                    Dim pSourceValue As Object = p.GetValue(objectToClone, Nothing)
                    If pSourceValue IsNot Nothing Then

                        Dim pDestValue As Object = CloneObject(pSourceValue, cloneDico)

                        p.SetValue(toReturn, pDestValue, Nothing)
                    End If
                End If

            Next



            Return toReturn

        End Function


        Public Shared Function BuildParameters(ByVal params As ParameterInfo(), ByVal values As List(Of String)) As Object()
            Dim paramValues As New List(Of Object)
            If params.Length > 0 Then
                If params.Length > values.Count Then
                    Throw New ArgumentException("invalid call to a method or indexed property takes more argument than supplied in " & String.Join(":", values.ToArray))
                End If
                'Dim objIndexParameter As ParameterInfo
                Dim strParameterValue As String
                For i As Integer = 0 To params.Length - 1
                    Dim objIndexParameter As ParameterInfo = params(i)
                    strParameterValue = values(i)
                    Dim objParamValue As Object = Convert.ChangeType(strParameterValue, objIndexParameter.ParameterType, CultureInfo.InvariantCulture)
                    paramValues.Add(objParamValue)
                Next
            End If

            If paramValues.Count > 0 Then
                Return paramValues.ToArray
            End If
            Return Nothing
        End Function

        Public Shared Function GetFriendlyName(value As Object) As String

            Dim toReturn As String = String.Empty
            If value IsNot Nothing Then
                Dim valueType As Type = value.GetType
                If valueType.GetInterface("IConvertible") IsNot Nothing Then
                    toReturn = DirectCast(value, IConvertible).ToString(CultureInfo.InvariantCulture)
                Else
                    For Each attr As Attribute In valueType.GetCustomAttributes(GetType(DefaultPropertyAttribute), True)
                        Dim defPropAttr As DefaultPropertyAttribute = DirectCast(attr, DefaultPropertyAttribute)
                        Dim defProp As PropertyInfo = valueType.GetProperty(defPropAttr.Name)
                        If defProp IsNot Nothing Then
                            Dim subValue As Object = defProp.GetValue(value, Nothing)
                            If subValue IsNot Nothing Then
                                toReturn = GetFriendlyName(subValue)
                            End If
                        Else
                            Throw New NotImplementedException(String.Format("The type ""{0}"" doesn't have a property ""{1}"".", valueType.FullName, defPropAttr.Name))
                        End If
                        Exit For
                    Next
                    If toReturn = "" Then
                        Dim keyProp As PropertyInfo = Nothing
                        Dim valueProp As PropertyInfo = Nothing
                        If ReflectionHelper.GetPropertiesDictionary(valueType).TryGetValue("Key", keyProp) _
                            AndAlso ReflectionHelper.GetPropertiesDictionary(valueType).TryGetValue("Value", valueProp) Then
                            Dim keyValue As Object = keyProp.GetValue(value, Nothing)
                            Dim keyName As String = GetFriendlyName(keyValue)
                            Dim valueValue As Object = valueProp.GetValue(value, Nothing)
                            Dim valueName As String = GetFriendlyName(valueValue)
                            toReturn = String.Format("{0}  -  {1}", keyName, valueName)
                        End If
                    End If
                    If toReturn = "" Then
                        toReturn = valueType.Name
                        If valueType.IsGenericType Then
                            toReturn &= String.Format("<{0}>", valueType.GetGenericArguments()(0).Name)
                        End If
                    End If
                End If
            End If
            Return toReturn

        End Function

#End Region



#Region "Private methods"

        Private Shared Function BuildSerializer(ByVal objType As Type) As Object

            Return BuildSerializer(objType, Nothing, Nothing)

        End Function

        Private Shared Function BuildSerializer(ByVal objType As Type, ByVal extraTypes() As Type, ByVal rootName As String) As XmlSerializer


            Dim toReturn As XmlSerializer = Nothing
            'le cache ne sert a rien, il vaut mieux laisser le factory en singleton gerer ca
            'If useCache Then
            '    toReturn = GetGlobal(Of XmlSerializer)(objType.FullName)
            'End If

            If toReturn Is Nothing Then

                Dim extraTypesList As New List(Of Type)

                If extraTypes IsNot Nothing Then
                    extraTypesList.AddRange(extraTypes)
                End If

                If objType.IsGenericType Then
                    extraTypesList.AddRange(objType.GetGenericArguments())
                End If


                If Not String.IsNullOrEmpty(rootName) Then
                    Dim rootAttribute As New XmlRootAttribute(rootName)
                    If extraTypesList.Count = 0 Then
                        toReturn = ReflectionHelper._XmlSerializerFactory.CreateSerializer(objType, rootAttribute)
                    Else
                        toReturn = ReflectionHelper._XmlSerializerFactory.CreateSerializer(objType, Nothing, extraTypesList.ToArray(), rootAttribute, Nothing)
                    End If
                Else
                    If extraTypesList.Count = 0 Then
                        toReturn = ReflectionHelper._XmlSerializerFactory.CreateSerializer(objType)
                    Else
                        toReturn = ReflectionHelper._XmlSerializerFactory.CreateSerializer(objType, extraTypesList.ToArray())
                    End If
                End If


                'If useCache Then
                '    SetGlobal(Of XmlSerializer)(toReturn, objType.FullName)
                'End If

            End If

            Return toReturn

        End Function

#End Region

    End Class
End Namespace

