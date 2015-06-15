
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Text
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Globalization
Imports System.Web.Compilation
Imports System.Security
Imports System.Security.Permissions
Imports Aricie.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports System.ComponentModel
Imports System.Linq
Imports System.Linq.Expressions
Imports Aricie.Business.Filters
Imports Aricie.Collections
Imports System.Reflection.Emit

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
        Private Shared ReadOnly _SingletonLock As New Object
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
            For Each objSingletonPair As KeyValuePair(Of Type, Object) In New List(Of KeyValuePair(Of Type, Object))(Me._Singletons)
                If objSingletonPair.Value IsNot Nothing AndAlso TypeOf objSingletonPair.Value Is IDisposable Then
                    DirectCast(objSingletonPair.Value, IDisposable).Dispose()
                End If
            Next
            Me._Singletons.Clear()
        End Sub

        Public Sub [Stop](ByVal immediate As Boolean) Implements System.Web.Hosting.IRegisteredObject.Stop
            Me.DisposeAll()
            System.Web.Hosting.HostingEnvironment.UnregisterObject(Me)
        End Sub

#End Region


#Region "Private members"

        Private Const glbDependency As String = Aricie.Constants.Cache.Dependency & "Reflection"

        Private ReadOnly _Singletons As New Dictionary(Of Type, Object)
        Private ReadOnly _XmlSerializers As New Dictionary(Of String, XmlSerializerBag)
        Private Shared _XmlSerializerFactory As New XmlSerializerFactory
        Private ReadOnly _TypesByTypeName As New Dictionary(Of String, Type)
        Private ReadOnly _PropertiesByType As New Dictionary(Of Type, Dictionary(Of String, PropertyInfo))

        Private Shared _Casts As New Dictionary(Of Type, List(Of Type))() From { _
  {GetType(Decimal), New List(Of Type)() From { _
      GetType(SByte), _
      GetType(Byte), _
      GetType(Short), _
      GetType(UShort), _
      GetType(Integer), _
      GetType(UInteger), _
      GetType(Long), _
      GetType(ULong), _
      GetType(Char) _
  }}, _
  {GetType(Double), New List(Of Type)() From { _
      GetType(SByte), _
      GetType(Byte), _
      GetType(Short), _
      GetType(UShort), _
      GetType(Integer), _
      GetType(UInteger), _
      GetType(Long), _
      GetType(ULong), _
      GetType(Char), _
      GetType(Single) _
  }}, _
  {GetType(Single), New List(Of Type)() From { _
      GetType(SByte), _
      GetType(Byte), _
      GetType(Short), _
      GetType(UShort), _
      GetType(Integer), _
      GetType(UInteger), _
      GetType(Long), _
      GetType(ULong), _
      GetType(Char), _
      GetType(Single) _
  }}, _
  {GetType(ULong), New List(Of Type)() From { _
      GetType(Byte), _
      GetType(UShort), _
      GetType(UInteger), _
      GetType(Char) _
  }}, _
  {GetType(Long), New List(Of Type)() From { _
      GetType(SByte), _
      GetType(Byte), _
      GetType(Short), _
      GetType(UShort), _
      GetType(Integer), _
      GetType(UInteger), _
      GetType(Char) _
  }}, _
  {GetType(UInteger), New List(Of Type)() From { _
      GetType(Byte), _
      GetType(UShort), _
      GetType(Char) _
  }}, _
  {GetType(Integer), New List(Of Type)() From { _
      GetType(SByte), _
      GetType(Byte), _
      GetType(Short), _
      GetType(UShort), _
      GetType(Char) _
  }}, _
  {GetType(UShort), New List(Of Type)() From { _
      GetType(Byte), _
      GetType(Char) _
  }}, _
  {GetType(Short), New List(Of Type)() From { _
      GetType(Byte) _
  }} _
}


        Private Shared ReadOnly _RegexSafeTypeName As New Regex("([^\[\]]+)(\,\sVersion\=[^\[\]]+)", RegexOptions.Compiled)
        'TODO: For some reason, System and System.XX core libraries require assembly qualified name unless we remove the assembly, in which case the buildmanager may find the target type, depending on the context.
        Private Shared ReadOnly _RegexSafeTypeNameMinusSystem As New Regex("([^\[\]]+?)(\,\sSystem(?:[^\,]+)?)?(?:\,\sVersion\=[^\[\]]+)", RegexOptions.Compiled)

        Private Shared ReadOnly _SafeTypeNames As New Dictionary(Of String, String)

        Private Shared ReadOnly _CanCreateTypes As New Dictionary(Of Type, Boolean)

        Private Shared ReadOnly _CustomAttributes As New Dictionary(Of MemberInfo, Object())
        Private Shared ReadOnly _DefaultProperties As New Dictionary(Of Type, PropertyInfo)

#End Region



#Region "type methods"

        'Public Shared Function GetCachingKey(ByVal typeName As String, ByVal mode As ReflectionMembers) As String
        '    Return Constants.GetKey(Of Type)(typeName, mode.ToString)

        'End Function

        Public Shared Function GetDisplayName(ByVal objType As Type) As String
            Dim attributes As Object()
            attributes = ReflectionHelper.GetCustomAttributes(objType).Where(Function(objAttribute) TypeOf objAttribute Is DisplayNameAttribute).ToArray()
            If attributes.Length > 0 Then
                Return DirectCast(attributes(0), DisplayNameAttribute).DisplayName
            Else
                Return objType.Name
            End If
        End Function

        Public Shared Function GetDescription(ByVal objType As Type) As String
            Dim attributes As Object()
            attributes = ReflectionHelper.GetCustomAttributes(objType).Where(Function(objAttribute) TypeOf objAttribute Is DescriptionAttribute).ToArray()
            If attributes.Length > 0 Then
                Return DirectCast(attributes(0), DescriptionAttribute).Description
            Else
                Return String.Empty
            End If
        End Function


        Public Shared Function CreateType(ByVal typeName As String) As Type
            Return CreateType(typeName, True)

        End Function

        Private Shared _DebugSurrogates As CreateInstanceCallBack

        Public Shared Sub RegisterDebugSurrogate(callBack As CreateInstanceCallBack)

            _DebugSurrogates = callBack
        End Sub



        Public Shared Function CreateType(ByVal typeName As String, ByVal throwOnError As Boolean) As Type
            Dim toReturn As Type = Nothing








#If DEBUG Then
            toReturn = BuildManager.GetType(typeName, throwOnError, True)
            If toReturn IsNot Nothing Then
                Dim debugType As Type = Nothing
                If _DebugSurrogates IsNot Nothing Then
                    Dim dest As Object = _DebugSurrogates.Invoke(toReturn)
                    If dest IsNot Nothing Then
                        debugType = dest.GetType()
                    End If
                    'Else
                    '    Dim debugTypeName As String = toReturn.Namespace & ".Debug." & toReturn.Name
                    '    debugType = CreateType(debugTypeName, False)
                End If

                If debugType IsNot Nothing Then
                    Return debugType
                End If
            End If
#Else
             If Not ReflectionHelper.Instance._TypesByTypeName.TryGetValue(typeName, toReturn) Then



                ' use reflection to get the type of the class
                toReturn = BuildManager.GetType(typeName, throwOnError, True)
                SyncLock ReflectionHelper.Instance._TypesByTypeName
                    ReflectionHelper.Instance._TypesByTypeName(typeName) = toReturn
                End SyncLock


                'CacheHelper.SetGlobal(Of Type)(toReturn, typeName)

            End If

#End If


            Return toReturn

        End Function






        Public Shared Function GetSafeTypeName(ByVal objType As Type) As String
            If objType Is Nothing Then
                'Throw New ArgumentException("Type parameter cannot be null", "objType")
                Return String.Empty
            End If
            If objType.IsGenericParameter Then
                'Throw New ArgumentException(String.Format("Type parameter {0} has null assemblyqualifiedname", objType.ToString()), "objType")
                Return objType.Name
            End If
            If String.IsNullOrEmpty(objType.AssemblyQualifiedName) AndAlso objType.IsGenericType Then
                Dim objGenParams As Type() = objType.GetGenericArguments()
                Dim toReturn As String = ReflectionHelper.GetSafeTypeName(objType.GetGenericTypeDefinition())

                Dim paramsString As String = String.Empty
                For Each objGenParam As Type In objGenParams
                    If Not objGenParam.IsGenericParameter Then
                        paramsString &= "["c & ReflectionHelper.GetSafeTypeName(objGenParam) & "],"
                    Else
                        paramsString &= "[],"
                    End If
                Next
                paramsString = paramsString.TrimEnd(","c)
                Dim split As String() = toReturn.Split(","c)
                If split.Length > 1 Then
                    toReturn = String.Format("{0}[{1}],{2}", split(0), paramsString, split(1))
                Else
                    toReturn = String.Format("{0}[{1}]", split(0), paramsString)
                End If
                Return toReturn
            End If
            Return GetSafeTypeName(objType.AssemblyQualifiedName)
        End Function

        Public Shared Function GetSafeTypeName(ByVal objAssemblyQualifiedName As String) As String

            Dim toReturn As String = ""
            If Not String.IsNullOrEmpty(objAssemblyQualifiedName) AndAlso Not _SafeTypeNames.TryGetValue(objAssemblyQualifiedName, toReturn) Then
                toReturn = _RegexSafeTypeName.Replace(objAssemblyQualifiedName, "$1")
                Try
                    ReflectionHelper.CreateType(toReturn)
                Catch
                    toReturn = _RegexSafeTypeNameMinusSystem.Replace(objAssemblyQualifiedName, "$1")
                    Try
                        ReflectionHelper.CreateType(toReturn)
                    Catch
                        toReturn = objAssemblyQualifiedName
                    End Try
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
                    Dim toReturn As String = objType.Name
                    If toReturn.IndexOf("`"c) > 0 Then
                        toReturn = toReturn.Substring(0, toReturn.IndexOf("`"c))
                    End If
                    toReturn &= "["c
                    Dim genTypes As Type() = objType.GetGenericArguments
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

        Public Shared Function GetSimpleTypeFileName(objtype As Type) As String
            Return GetSimpleTypeName(objtype).Replace("[[", "_"c).Replace("["c, "").Replace("]"c, "").Replace(","c, "_")
        End Function


        Public Shared Function GetCollectionTypeElementType(collectionType As Type) As Type
            Return GetCollectionTypeElementType(collectionType, True)
        End Function

        Public Shared Function GetCollectionTypeElementType(collectionType As Type, throwOnError As Boolean) As Type
            Dim objetType As Type
            If collectionType.IsGenericType Then
                Dim args As Type() = collectionType.GetGenericArguments()
                objetType = args(args.Length - 1)
            Else
                objetType = collectionType.GetElementType()
            End If
            If throwOnError AndAlso objetType Is Nothing Then
                Throw New ArgumentException("type is not an array or a generic list ", "collectionType")
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

        Public Shared Function IsSimpleType(ByVal objType As Type) As Boolean
            Return objType.IsPrimitive OrElse objType Is GetType(String)
        End Function


        Public Shared Function IsStatic(ByVal objType As Type) As Boolean
            Return objType.IsAbstract AndAlso objType.IsSealed
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

        Public Shared Function MakeGenerics(genericsDefinitionType As Type, parameterTypes As IEnumerable(Of Type)) As Type
            If parameterTypes IsNot Nothing AndAlso parameterTypes.Count > 0 Then
                Return genericsDefinitionType.MakeGenericType(parameterTypes.ToArray)
            End If
            Return genericsDefinitionType
        End Function



        Public Shared Function CanConvert(fromType As Type, toType As Type) As Boolean

            Dim targetList As List(Of Type) = Nothing
            If _Casts.TryGetValue(toType, targetList) AndAlso targetList.Contains(fromType) Then
                Return True
            Else
                Dim castable As Boolean
                If toType.IsAssignableFrom(fromType) Then
                    castable = True
                Else
                    castable = fromType.GetMethods(BindingFlags.[Public] Or BindingFlags.[Static]).Any(Function(m) m.ReturnType.FullName = toType.FullName AndAlso (m.Name = "op_Implicit" OrElse m.Name = "op_Explicit")) _
                  OrElse toType.GetMethods(BindingFlags.[Public] Or BindingFlags.[Static]).Any(Function(m) m.ReturnType.FullName = fromType.FullName AndAlso (m.Name = "op_Implicit" OrElse m.Name = "op_Explicit"))
                End If
                If castable Then
                    If targetList Is Nothing Then
                        targetList = New List(Of Type)
                    End If
                    targetList.Add(fromType)
                    SyncLock _Casts
                        _Casts(toType) = targetList
                    End SyncLock
                End If
                Return castable
            End If
        End Function



        Public Shared Function CanCreateObject(objectType As Type) As Boolean
            Dim toReturn As Boolean
            If Not _CanCreateTypes.TryGetValue(objectType, toReturn) Then
                toReturn = Not (objectType.IsAbstract() _
                                OrElse (objectType.IsInterface _
                                         AndAlso Not ReflectionHelper.GetCustomAttributes(objectType).Where(Function(objAttr) (TypeOf objAttr Is System.Runtime.InteropServices.CoClassAttribute)).Any() _
                                OrElse (ReflectionHelper.IsTrueReferenceType(objectType)) AndAlso Not HasDefaultConstructor(objectType) AndAlso Not objectType.IsArray))
                SyncLock _CanCreateTypes
                    _CanCreateTypes(objectType) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function

#End Region


#Region "Objects manipulation"

        Public Shared Function CreateObject(ByVal objectType As Type) As Object

            If Not CanCreateObject(objectType) Then
                Throw New ApplicationException(String.Format("Cannot create object of Type {0}, because it has no default constructor", objectType.AssemblyQualifiedName))
            End If

            Dim toReturn As Object

            If ReflectionHelper.IsTrueReferenceType(objectType) Then
                If objectType.IsArray Then
                    toReturn = Activator.CreateInstance(objectType, New Object() {1})
                Else
                    toReturn = Activator.CreateInstance(objectType)
                End If
            Else
                If objectType Is GetType(String) Then
                    toReturn = String.Empty
                ElseIf objectType Is GetType(DateTime) Then
                    toReturn = DateTime.Now
                Else
                    toReturn = Convert.ChangeType(0, Type.GetTypeCode(objectType))
                    'toReturn = Convert.ChangeType(0, objectType)
                    If objectType.IsEnum Then
                        toReturn = [Enum].Parse(objectType, toReturn.ToString())
                    End If
                End If
            End If

            Return toReturn

        End Function

        Public Shared Function CreateObject(ByVal typeName As String) As Object


            Dim objectType As Type = ReflectionHelper.CreateType(typeName)

#If DEBUG Then
            If _DebugSurrogates IsNot Nothing Then
                Dim dest As Object = _DebugSurrogates.Invoke(objectType)
                If dest IsNot Nothing Then
                    Return dest
                End If
            End If
#End If

            Return CreateObject(objectType)

        End Function

        Public Shared Function CreateObject(Of T)(ByVal typeName As String) As T
            Return DirectCast(CreateObject(typeName), T)

        End Function

        Public Shared Function CreateObject(Of T)() As T
            Return DirectCast(CreateObject(GetType(T)), T)
        End Function

        Public Shared Function GetFriendlyName(value As Object) As String
            Dim toReturn As String = String.Empty
            If value IsNot Nothing Then
                Dim valueType As Type = value.GetType
                If valueType.GetInterface("IConvertible") IsNot Nothing Then
                    toReturn = DirectCast(value, IConvertible).ToString(CultureInfo.InvariantCulture)
                    If toReturn.Length > 500 Then
                        toReturn = toReturn.Substring(0, 500) & " (...)"
                    End If
                Else
                    Dim defProp As PropertyInfo = GetDefaultProperty(valueType)
                    If defProp IsNot Nothing Then
                        Dim subValue As Object = defProp.GetValue(value, Nothing)
                        If subValue IsNot Nothing Then
                            toReturn = GetFriendlyName(subValue)
                        End If
                    End If
                    If toReturn.IsNullOrEmpty() Then
                        Dim keyProp As PropertyInfo = Nothing
                        Dim valueProp As PropertyInfo = Nothing
                        If ReflectionHelper.GetPropertiesDictionary(valueType).TryGetValue("Key", keyProp) _
                            AndAlso ReflectionHelper.GetPropertiesDictionary(valueType).TryGetValue("Value", valueProp) Then
                            Dim keyValue As Object = keyProp.GetValue(value, Nothing)
                            Dim keyName As String = GetFriendlyName(keyValue)
                            Dim valueValue As Object = valueProp.GetValue(value, Nothing)
                            Dim valueName As String = GetFriendlyName(valueValue)
                            If valueName.Length < 100 Then
                                toReturn = String.Format("{0} {1} {2}", keyName, UIConstants.TITLE_SEPERATOR, valueName)
                            Else
                                toReturn = keyName
                            End If
                        End If
                    End If
                    If toReturn.IsNullOrEmpty() Then
                        toReturn = GetSimpleTypeName(valueType)
                        If valueType.GetInterface("ICollection") IsNot Nothing Then
                            toReturn = String.Format("{0} {1} {2} items", toReturn, UIConstants.TITLE_SEPERATOR, DirectCast(value, ICollection).Count.ToString(CultureInfo.InvariantCulture))
                        End If
                    End If
                End If
            End If
            Return toReturn

        End Function


        Public Shared Function GetFields(value As Object, maxDepth As Integer) As Object
            Dim visited As New HashSet(Of Object)
            Return ReflectionHelper.GetFields(value, maxDepth, visited, 0)
        End Function

        Public Shared Function GetFields(value As Object, maxdepth As Integer, visited As HashSet(Of Object), depth As Integer) As Object
            Dim toReturn As SerializableDictionary(Of String, Object) = Nothing
            If value IsNot Nothing Then
                If depth < maxdepth Then
                    toReturn = New SerializableDictionary(Of String, Object)
                    Dim valueType As Type = value.GetType()
                    If Not ReflectionHelper.IsTrueReferenceType(valueType) OrElse TypeOf value Is SignatureHelper OrElse TypeOf value Is Pointer Then
                        Return value.ToString()
                    Else
                        If Not visited.Contains(value) Then
                            visited.Add(value)
                            Try
                                Dim valueEnumerable As IEnumerable = TryCast(value, IEnumerable)
                                If valueEnumerable IsNot Nothing Then
                                    Dim valueDict As IDictionary = TryCast(value, IDictionary)
                                    If valueDict IsNot Nothing Then
                                        For Each objKey In valueDict.Keys

                                            toReturn(objKey.ToString) = GetFields(valueDict(objKey), maxdepth, visited, depth + 1)
                                        Next
                                    Else
                                        Dim idx As Integer = 0
                                        For Each objItem In valueEnumerable
                                            toReturn(idx.ToString(CultureInfo.InvariantCulture)) = GetFields(objItem, maxdepth, visited, depth + 1)
                                            idx += 1
                                        Next
                                    End If
                                Else
                                    Dim objFields = ReflectionHelper.GetMembersDictionary(valueType, True, True).Values.OfType(Of FieldInfo)()
                                    For Each objField As FieldInfo In objFields
                                        Try
                                            Dim objAddedValue As Object = Nothing
                                            Dim fieldValue As Object = objField.GetValue(value)
                                            If fieldValue IsNot Nothing Then
                                                objAddedValue = GetFields(fieldValue, maxdepth, visited, depth + 1)
                                            End If
                                            toReturn(objField.Name) = objAddedValue
                                        Catch ex As Exception
                                            ExceptionHelper.LogException(ex)
                                        End Try
                                    Next
                                End If
                            Catch ex As Exception
                                ExceptionHelper.LogException(ex)
                            End Try
                        Else
                            Return Nothing
                        End If
                    End If
                Else
                    Return value.ToString
                End If
            End If
            Return toReturn
        End Function




        Public Shared Sub MergeObjects(sourceObject As Object, targetObject As Object)
            If sourceObject IsNot Nothing AndAlso targetObject IsNot Nothing Then
                Dim objType As Type = sourceObject.GetType()
                Dim props As Dictionary(Of String, PropertyInfo) = GetPropertiesDictionary(objType)
                Dim targetProps As Dictionary(Of String, PropertyInfo) = Nothing
                If targetObject.GetType() IsNot objType Then
                    targetProps = GetPropertiesDictionary(targetObject.GetType())
                End If
                Dim targetProp As PropertyInfo = Nothing

                For Each propName As String In props.Keys
                    Dim p As PropertyInfo = props(propName)

                    If p.CanWrite AndAlso Not p.GetIndexParameters.Length > 0 AndAlso (targetProps Is Nothing OrElse targetProps.TryGetValue(p.Name, targetProp)) Then 'AndAlso p.GetCustomAttributes(GetType(XmlIgnoreAttribute), True).Length = 0
                        Dim pSourceValue As Object = p.GetValue(sourceObject, Nothing)
                        If targetProp IsNot Nothing Then
                            targetProp.SetValue(targetObject, pSourceValue, Nothing)
                        Else
                            p.SetValue(targetObject, pSourceValue, Nothing)
                        End If

                    End If
                Next
            End If
        End Sub

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
                If ReflectionHelper.CanCreateObject(objType) Then
                    toReturn = ReflectionHelper.CreateObject(objType)
                    cloneDico(objectToClone) = toReturn
                Else
                    toReturn = objectToClone
                    cloneDico(objectToClone) = toReturn
                    Return toReturn
                End If
            End If

            'Finally, try to clone any writeable property (it may complete or even discard the ienumerable actions)
            Dim props As Dictionary(Of String, PropertyInfo) = GetPropertiesDictionary(objType)

            For Each propName As String In props.Keys
                Dim p As PropertyInfo = props(propName)

                If p.CanWrite AndAlso Not p.GetIndexParameters.Length > 0 Then ' AndAlso p.GetCustomAttributes(GetType(XmlIgnoreAttribute), True).Length = 0 (There are times when it is explicitally needed to overwrite such xmlignored properties )
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




        Public Shared Sub AddEventHandler(Of TEventArgs As EventArgs)(objEventInfo As EventInfo, item As Object, action As EventHandler(Of TEventArgs))
            Dim handler As [Delegate] = WrapDelegate(objEventInfo.EventHandlerType, action)
            'Dim objParameterInfos As ParameterInfo() = GetEventParameters(objEventInfo)
            'Dim parameters As ParameterExpression() = objParameterInfos.[Select](Function(parameter) Expression.Parameter(parameter.ParameterType, parameter.Name)).ToArray()
            ''Dim parameterTypes As Type() = objParameterInfos.Select(Function(parameter) parameter.ParameterType).ToArray()
            'Dim callExpression As MethodCallExpression = Expression.[Call](Expression.Constant(action), "Invoke", Type.EmptyTypes, parameters)
            'Dim handler As [Delegate] = Expression.Lambda(objEventInfo.EventHandlerType, callExpression, parameters).Compile()
            objEventInfo.AddEventHandler(item, handler)
        End Sub



        'Public Shared Sub AddEventHandler(objEventInfo As EventInfo, item As Object, action As Action)
        '    'Dim parameters As ParameterExpression() = GetEventParameters(objEventInfo).[Select](Function(parameter) Expression.Parameter(parameter.ParameterType, parameter.Name)).ToArray()
        '    'Dim handler As [Delegate] = Expression.Lambda(objEventInfo.EventHandlerType, Expression.[Call](Expression.Constant(action), "Invoke", Type.EmptyTypes), parameters).Compile()
        '    Dim handler As [Delegate] = WrapDelegate(objEventInfo.EventHandlerType, action)
        '    objEventInfo.AddEventHandler(item, handler)
        'End Sub

        Public Shared Sub AddEventHandler(objEventInfo As EventInfo, item As Object, sourceHandler As [Delegate])
            Dim handler As [Delegate] = WrapDelegate(objEventInfo.EventHandlerType, sourceHandler)
            objEventInfo.AddEventHandler(item, handler)
        End Sub

        Public Shared Function CombineDelegate(targetDelegate As [Delegate], newInvocation As [Delegate]) As MulticastDelegate

            Dim objInvoc As [Delegate]() = targetDelegate.GetInvocationList()
            Dim handlerType As Type = targetDelegate.GetType()
            Dim toReturn As MulticastDelegate = DirectCast([Delegate].Combine(objInvoc.Union({WrapDelegate(handlerType, newInvocation)}).ToArray()), MulticastDelegate)

            Return toReturn
        End Function

        Private Shared _WrappedDelegates As New Dictionary(Of [Delegate], Dictionary(Of Type, [Delegate]))

        Public Shared Function WrapDelegate(targetDelegateType As Type, compatibleDelegate As [Delegate]) As [Delegate]
            If targetDelegateType.IsInstanceOfType(compatibleDelegate) Then
                Return compatibleDelegate
            End If
            Dim targetDico As Dictionary(Of Type, [Delegate]) = Nothing
            If Not _WrappedDelegates.TryGetValue(compatibleDelegate, targetDico) Then
                targetDico = New Dictionary(Of Type, [Delegate])
                SyncLock _WrappedDelegates
                    _WrappedDelegates(compatibleDelegate) = targetDico
                End SyncLock
            End If
            Dim toReturn As [Delegate] = Nothing
            If Not targetDico.TryGetValue(targetDelegateType, toReturn) Then
                Dim objParameterInfos As ParameterInfo() = GetDelegateTypeParameters(targetDelegateType)
                Dim parameters As ParameterExpression() = objParameterInfos.[Select](Function(parameter) Expression.Parameter(parameter.ParameterType, parameter.Name)).ToArray()
                Dim sourceParams As ParameterInfo() = GetDelegateParameters(compatibleDelegate)
                Dim isParamArray As Boolean
                If sourceParams.Length > 0 Then
                    isParamArray = sourceParams(sourceParams.Length - 1).GetCustomAttributes(False).Any(Function(objAttribute) TypeOf objAttribute Is ParamArrayAttribute)
                End If
                Dim callExpression As MethodCallExpression
                If isParamArray Then
                    callExpression = Expression.[Call](Expression.Constant(compatibleDelegate), "Invoke", Type.EmptyTypes, parameters)
                Else
                    callExpression = Expression.[Call](Expression.Constant(compatibleDelegate), "Invoke", Type.EmptyTypes, parameters.Take(Math.Min(parameters.Length, sourceParams.Length)).ToArray())
                End If
                toReturn = Expression.Lambda(targetDelegateType, callExpression, parameters).Compile()
                SyncLock targetDico
                    targetDico(targetDelegateType) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function

        Public Shared Function GetCollectionFileName(ByVal objCollection As ICollection) As String
            Dim toReturn As String = "Collection"
            If objCollection IsNot Nothing Then
                Dim collecType As Type = objCollection.GetType()
                toReturn = ReflectionHelper.GetSimpleTypeFileName(collecType)
                If Not collecType.IsGenericType Then
                    If objCollection.Count > 0 Then
                        Dim collecEnum As IEnumerator = objCollection.GetEnumerator
                        If collecEnum.MoveNext Then
                            Dim itemType As Type = collecEnum.Current.GetType
                            toReturn &= "_" & ReflectionHelper.GetSimpleTypeFileName(itemType)
                        End If
                    End If
                End If
            End If
            Return toReturn
        End Function

        Public Shared Function GetCollectionElementType(ByVal collection As ICollection) As Type
            Return GetCollectionElementType(collection, True)
        End Function

        Public Shared Function GetCollectionElementType(ByVal collection As ICollection, throwOnerror As Boolean) As Type

            Dim objetType As Type

            If collection IsNot Nothing AndAlso collection.Count > 0 Then
                Dim enumTor As IEnumerator = collection.GetEnumerator
                enumTor.MoveNext()
                objetType = enumTor.Current.GetType()
            Else
                objetType = GetCollectionTypeElementType(collection.GetType, throwOnerror)

            End If


            Return objetType

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
                            toReturn = ReflectionHelper.CreateObject(objType.AssemblyQualifiedName)

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


#Region "Member methods"

        Public Shared Function GetDefaultProperty(objType As Type) As PropertyInfo
            Dim toReturn As PropertyInfo = Nothing
            If Not _DefaultProperties.TryGetValue(objType, toReturn) Then
                For Each attr As Attribute In ReflectionHelper.GetCustomAttributes(objType)
                    If TypeOf attr Is DefaultPropertyAttribute Then
                        Dim defPropAttr As DefaultPropertyAttribute = DirectCast(attr, DefaultPropertyAttribute)
                        toReturn = objType.GetProperty(defPropAttr.Name)
                        SyncLock _DefaultProperties
                            _DefaultProperties(objType) = toReturn
                        End SyncLock
                        Exit For
                    End If
                Next
            End If
            Return toReturn
        End Function


        Public Shared Function GetCustomAttributes(objMember As MemberInfo) As Object()
            Dim toReturn As Object() = Nothing
            If Not _CustomAttributes.TryGetValue(objMember, toReturn) Then
                toReturn = objMember.GetCustomAttributes(True)
                SyncLock _CustomAttributes
                    _CustomAttributes(objMember) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function



        Public Shared Function GetPropertiesDictionary(Of T)() As Dictionary(Of String, PropertyInfo)
            Return GetPropertiesDictionary(GetType(T))
        End Function

        Public Shared Function GetMembersDictionary(Of T)() As Dictionary(Of String, MemberInfo)
            Return GetMembersDictionary(GetType(T))
        End Function

        Public Shared Function GetMembersDictionary(ByVal objType As Type) As Dictionary(Of String, MemberInfo)
            Return GetMembersDictionary(objType, False, False)
        End Function

        Public Shared Function GetMemberReturnType(objMember As MemberInfo, Optional publicOnly As Boolean = False) As Type
            Select Case objMember.MemberType
                Case MemberTypes.Property
                    Dim objGetMethod As MethodInfo = DirectCast(objMember, PropertyInfo).GetGetMethod(True)
                    If objGetMethod IsNot Nothing AndAlso (Not publicOnly OrElse objGetMethod.IsPublic) Then
                        Return objGetMethod.ReturnType
                    End If
                Case MemberTypes.Field
                    If (Not publicOnly OrElse DirectCast(objMember, FieldInfo).IsPublic) Then
                        Return DirectCast(objMember, FieldInfo).FieldType
                    End If
                Case MemberTypes.Method
                    Dim objMethod As MethodInfo = DirectCast(objMember, MethodInfo)
                    If objMethod.ReturnType IsNot Nothing AndAlso (Not publicOnly OrElse objMethod.IsPublic) Then
                        Return objMethod.ReturnType
                    End If
                Case Else
                    Return Nothing
            End Select
            Return Nothing
        End Function

        Public Shared Function GetMembersDictionary(ByVal objType As Type, includeHierarchy As Boolean, includePrivateFields As Boolean) As Dictionary(Of String, MemberInfo)


            ' Use the cache because the reflection used later is expensive
            Dim objMembers As Dictionary(Of String, MemberInfo) = GetGlobal(Of Dictionary(Of String, MemberInfo))(objType.FullName, includeHierarchy.ToString(CultureInfo.InvariantCulture), includePrivateFields.ToString(CultureInfo.InvariantCulture))

            If objMembers Is Nothing Then
                objMembers = New Dictionary(Of String, MemberInfo)(StringComparer.OrdinalIgnoreCase)
                Dim currentType As Type = objType
                Do
                    FillMembersDictionary(currentType, objMembers, includePrivateFields)
                    currentType = currentType.BaseType
                Loop Until (Not includeHierarchy) OrElse currentType Is Nothing
                SetCacheDependant(Of Dictionary(Of String, MemberInfo))(objMembers, glbDependency, Constants.Cache.NoExpiration, objType.FullName, includeHierarchy.ToString(CultureInfo.InvariantCulture), includePrivateFields.ToString(CultureInfo.InvariantCulture))
            End If

            Return objMembers

        End Function


        Private Shared Sub FillMembersDictionary(ByVal objType As Type, ByRef objMembers As Dictionary(Of String, MemberInfo), includePrivateFields As Boolean)
            Dim objBindingFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.[Static] Or BindingFlags.[Public]
            If includePrivateFields Then
                objBindingFlags = objBindingFlags Or BindingFlags.NonPublic
            End If
            For Each objMember As MemberInfo In objType.GetMembers(objBindingFlags)
                If Not objMembers.ContainsKey(objMember.Name) Then
                    objMembers(objMember.Name) = objMember
                End If

            Next
        End Sub



        Public Shared Function GetFullMembersDictionary(ByVal objType As Type) As Dictionary(Of String, List(Of MemberInfo))
            Return GetFullMembersDictionary(objType, False, False)
        End Function

        Public Shared Function GetFullMembersDictionary(ByVal objType As Type, includeHierarchy As Boolean, includePrivateFields As Boolean) As Dictionary(Of String, List(Of MemberInfo))


            ' Use the cache because the reflection used later is expensive
            Dim objMembers As Dictionary(Of String, List(Of MemberInfo)) = GetGlobal(Of Dictionary(Of String, List(Of MemberInfo)))(objType.AssemblyQualifiedName, includeHierarchy.ToString(CultureInfo.InvariantCulture), includePrivateFields.ToString(CultureInfo.InvariantCulture))

            If objMembers Is Nothing Then
                objMembers = New Dictionary(Of String, List(Of MemberInfo))(StringComparer.OrdinalIgnoreCase)
                Dim currentType As Type = objType
                Do
                    FillFullMembersDictionary(currentType, objMembers, includePrivateFields)
                    currentType = currentType.BaseType
                Loop Until (Not includeHierarchy) OrElse currentType Is Nothing

                SetCacheDependant(Of Dictionary(Of String, List(Of MemberInfo)))(objMembers, glbDependency, Constants.Cache.NoExpiration, objType.AssemblyQualifiedName, includeHierarchy.ToString(CultureInfo.InvariantCulture), includePrivateFields.ToString(CultureInfo.InvariantCulture))
            End If

            Return objMembers

        End Function

        Private Shared Sub FillFullMembersDictionary(ByVal objType As Type, ByRef dicoToFill As Dictionary(Of String, List(Of MemberInfo)), includePrivateFields As Boolean)
            Dim objBindingFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.[Static] Or BindingFlags.[Public]
            If includePrivateFields Then
                objBindingFlags = objBindingFlags Or BindingFlags.NonPublic
            End If
            Dim objMembers As MemberInfo() = objType.GetMembers(objBindingFlags)

            For Each objMember As MemberInfo In objType.GetMembers(objBindingFlags)
                Dim memberList As List(Of MemberInfo) = Nothing
                If Not dicoToFill.TryGetValue(objMember.Name, memberList) Then
                    memberList = New List(Of MemberInfo)
                End If
                If Not memberList.Contains(objMember) Then
                    memberList.Add(objMember)
                End If
                dicoToFill(objMember.Name) = memberList
            Next
        End Sub


        Public Shared Function GetMember(ByVal objtype As Type, ByVal memberName As String) As MemberInfo
            Return GetMember(objtype, memberName, False, False)
        End Function

        Public Shared Function GetMember(ByVal objtype As Type, ByVal memberName As String, includeHierarchy As Boolean, includePrivateFields As Boolean) As MemberInfo
            Dim toReturn As MemberInfo = Nothing
            If objtype IsNot Nothing Then
                If memberName.IndexOf("-"c) > 0 Then
                    Dim memberSplit As String() = memberName.Split("-"c)
                    If memberSplit.Length = 2 AndAlso memberSplit(1).IsNumber() Then
                        Dim objMembers As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(objtype, True, True)
                        Dim objList As List(Of MemberInfo) = Nothing
                        If objMembers.TryGetValue(memberSplit(0), objList) Then
                            Dim idx As Integer = Integer.Parse(memberSplit(1))
                            If idx < objList.Count Then
                                toReturn = objList(idx)
                            End If
                        End If
                    End If
                Else
                    Dim objMembers As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(objtype, True, True)
                    objMembers.TryGetValue(memberName, toReturn)
                End If
            End If
            Return toReturn
        End Function



        Public Shared Function GetPropertiesDictionary(ByVal objType As Type) As Dictionary(Of String, PropertyInfo)


            ' Use the cache because the reflection used later is expensive
            Dim objProperties As Dictionary(Of String, PropertyInfo) = Nothing

            If Not ReflectionHelper.Instance._PropertiesByType.TryGetValue(objType, objProperties) Then
                objProperties = New Dictionary(Of String, PropertyInfo)(StringComparer.OrdinalIgnoreCase)
                Dim objProperty As PropertyInfo
                For Each objProperty In objType.GetProperties()
                    If Not objProperties.ContainsKey(objProperty.Name) Then
                        objProperties(objProperty.Name) = objProperty
                    End If

                Next
                SyncLock ReflectionHelper.Instance._PropertiesByType
                    ReflectionHelper.Instance._PropertiesByType(objType) = objProperties
                End SyncLock

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
                For Each tempProperty As KeyValuePair(Of String, PropertyInfo) In tempProperties
                    If tempProperty.Value.PropertyType.GetInterface("IEntity2") IsNot Nothing Then
                        objProperties(tempProperty.Key) = tempProperty.Value
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
                Dim tempProperties As Dictionary(Of String, PropertyInfo) = GetPropertiesDictionary(objType)
                objProperties = (From pkey In tempProperties.Keys Where tempProperties(pkey).PropertyType.GetInterface("IEntityCollection2") IsNot Nothing).ToDictionary(Function(pkey) pkey, Function(pkey) tempProperties(pkey))
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

        Public Shared Function GetEventParameters(objEventInfo As EventInfo) As ParameterInfo()
            Return GetDelegateTypeParameters(objEventInfo.EventHandlerType)
        End Function

        Public Shared Function GetDelegateParameters(objDelegate As [Delegate]) As ParameterInfo()
            Return GetDelegateTypeParameters(objDelegate.GetType())
        End Function


        Public Shared Function GetDelegateTypeParameters(objDelegateType As Type) As ParameterInfo()
            Return objDelegateType.GetMethod("Invoke").GetParameters()
        End Function




        Public Shared Function GetMemberNamefromSignature(sig As String) As String
            Dim toReturn As String = sig
            Dim trimIndex As Integer = toReturn.IndexOf("<"c)
            If trimIndex > 0 Then
                toReturn = Left(toReturn, trimIndex)
            End If
            trimIndex = toReturn.IndexOf("("c)
            If trimIndex > 0 Then
                toReturn = Left(toReturn, trimIndex).Trim()
            End If
            trimIndex = toReturn.IndexOf(" "c)
            If trimIndex > 0 Then
                toReturn = toReturn.Substring(trimIndex).Trim()
            End If
            Return toReturn
        End Function


        ''' <summary>
        ''' Return the member declaration as a string.
        ''' </summary>
        ''' <param name="member">The Member</param>
        ''' <returns>Member signature</returns>
        Public Shared Function GetMemberSignature(member As MemberInfo, Optional callable As Boolean = False) As String
            If TypeOf member Is MethodBase Then
                Return GetSignature(DirectCast(member, MethodBase), callable)
            ElseIf TypeOf member Is PropertyInfo Then
                Return GetPropertySignature(DirectCast(member, PropertyInfo), callable)
            ElseIf TypeOf member Is EventInfo Then
                Return GetEventSignature(DirectCast(member, EventInfo), callable)
            End If
            Return member.Name
        End Function


        Public Shared Function GetReturnSubMembers(objType As Type, includePrivate As Boolean) As IDictionary(Of String, MemberInfo)
            Dim objMembers = ReflectionHelper.GetFullMembersDictionary(objType, True, includePrivate) _
                                     .Where(Function(objMemberPair) _
                                                objMemberPair.Value.Any(Function(objMemberInfo) As Boolean
                                                                            Dim objReturnType = ReflectionHelper.GetMemberReturnType(objMemberInfo, Not includePrivate)
                                                                            Return objReturnType IsNot Nothing _
                                                                                    AndAlso objReturnType IsNot GetType(System.Void) _
                                                                                    AndAlso Not objMemberPair.Key.StartsWith("get_") _
                                                                                    AndAlso Not objMemberPair.Key.StartsWith("set_")
                                                                        End Function))
            Return (From objMemberListPair In objMembers _
                        From objMemberInfo In objMemberListPair.Value _
                            Select New KeyValuePair(Of String, MemberInfo)(ReflectionHelper.GetMemberSignature(objMemberInfo, True), objMemberInfo)) _
            .Distinct(New SimpleComparer(Of KeyValuePair(Of String, MemberInfo))(Function(objMemberPair1, objMemberPair2) _
                                                                                     String.Compare(objMemberPair1.Key, objMemberPair2.Key, System.StringComparison.Ordinal), _
                                                                                     Function(objMemberPair) objMemberPair.Key.GetHashCode())) _
            .OrderBy(Function(objMemberInfoPair) objMemberInfoPair.Value.Name) _
                                .ToDictionary(Function(objMemberInfoPair) objMemberInfoPair.Key, Function(objMemberInfoPair) objMemberInfoPair.Value)
        End Function





        ''' <summary>
        ''' Return the Event declaration as a string.
        ''' </summary>
        ''' <param name="objEvent">The Event</param>
        ''' <returns>Event signature</returns>
        Public Shared Function GetEventSignature(objEvent As EventInfo, Optional callable As Boolean = False) As String

            Dim sigBuilder = New StringBuilder()
            If Not callable Then
                sigBuilder.Append("public event")
                sigBuilder.Append(ReflectionHelper.GetSimpleTypeName(objEvent.EventHandlerType))
                sigBuilder.Append(" "c)
            End If

            sigBuilder.Append(objEvent.Name)

            Return sigBuilder.ToString()
        End Function


        ''' <summary>
        ''' Return the Property signature as a string.
        ''' </summary>
        ''' <param name="prop">The Property</param>
        ''' <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
        ''' <returns>Property signature</returns>
        Public Shared Function GetPropertySignature(prop As PropertyInfo, Optional callable As Boolean = False) As String

            Dim sigBuilder = New StringBuilder()
            If Not callable Then
                Dim accessMethod As MethodInfo = prop.GetGetMethod(True)
                Dim readonlyWriteOnly As String = ""
                If Not prop.CanRead Then
                    readonlyWriteOnly = "writeonly "
                    accessMethod = prop.GetSetMethod()
                ElseIf Not prop.CanWrite Then
                    readonlyWriteOnly = "readonly "
                End If

                If accessMethod.IsPublic Then
                    sigBuilder.Append("public ")
                ElseIf accessMethod.IsPrivate Then
                    sigBuilder.Append("private ")
                ElseIf accessMethod.IsAssembly Then
                    sigBuilder.Append("internal ")
                ElseIf accessMethod.IsFamily Then
                    sigBuilder.Append("protected ")
                End If

                If accessMethod.IsStatic Then
                    sigBuilder.Append("static ")
                End If

                If Not readonlyWriteOnly.IsNullOrEmpty() Then
                    sigBuilder.Append(readonlyWriteOnly)
                End If

                sigBuilder.Append(ReflectionHelper.GetSimpleTypeName(prop.PropertyType))
                sigBuilder.Append(" "c)
            End If
            sigBuilder.Append(prop.Name)
            Dim objParams As ParameterInfo() = prop.GetIndexParameters()
            If objParams.Length > 0 Then
                sigBuilder.Append("(")
                Dim firstParam = True
                For Each param As ParameterInfo In objParams
                    If firstParam Then
                        firstParam = False
                    Else
                        sigBuilder.Append(", ")
                    End If
                    If param.ParameterType.IsByRef Then
                        sigBuilder.Append("ref ")
                    ElseIf param.IsOut Then
                        sigBuilder.Append("out ")
                    End If
                    If Not callable Then
                        sigBuilder.Append(GetSimpleTypeName(param.ParameterType))
                        sigBuilder.Append(" "c)
                    End If
                    sigBuilder.Append(param.Name)
                Next
                sigBuilder.Append(")")
            End If

            Return sigBuilder.ToString()
        End Function





        ''' <summary>
        ''' Return the method signature as a string.
        ''' </summary>
        ''' <param name="method">The Method</param>
        ''' <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
        ''' <returns>Method signature</returns>
        Public Shared Function GetSignature(method As MethodBase, Optional callable As Boolean = False) As String
            Dim firstParam = True
            Dim sigBuilder = New StringBuilder()
            If callable = False Then
                If method.IsPublic Then
                    sigBuilder.Append("public ")
                ElseIf method.IsPrivate Then
                    sigBuilder.Append("private ")
                ElseIf method.IsAssembly Then
                    sigBuilder.Append("internal ")
                ElseIf method.IsFamily Then
                    sigBuilder.Append("protected ")
                End If

                If method.IsStatic Then
                    sigBuilder.Append("static ")
                End If
                If TypeOf method Is MethodInfo Then
                    sigBuilder.Append(ReflectionHelper.GetSimpleTypeName(DirectCast(method, MethodInfo).ReturnType))
                End If
                sigBuilder.Append(" "c)
            End If
            sigBuilder.Append(method.Name)

            ' Add method generics
            If method.IsGenericMethod Then
                sigBuilder.Append("<")
                For Each g As Type In method.GetGenericArguments()
                    If firstParam Then
                        firstParam = False
                    Else
                        sigBuilder.Append(", ")
                    End If
                    sigBuilder.Append(ReflectionHelper.GetSimpleTypeName(g))
                Next
                sigBuilder.Append(">")
            End If
            sigBuilder.Append("(")
            firstParam = True
            Dim secondParam = False
            For Each param As ParameterInfo In method.GetParameters()
                If firstParam Then
                    firstParam = False
                    If method.IsDefined(GetType(System.Runtime.CompilerServices.ExtensionAttribute), False) Then
                        If callable Then
                            secondParam = True
                            Continue For
                        End If
                        sigBuilder.Append("this ")
                    End If
                ElseIf secondParam = True Then
                    secondParam = False
                Else
                    sigBuilder.Append(", ")
                End If
                If param.ParameterType.IsByRef Then
                    sigBuilder.Append("ref ")
                ElseIf param.IsOut Then
                    sigBuilder.Append("out ")
                End If
                If Not callable Then
                    sigBuilder.Append(ReflectionHelper.GetSimpleTypeName(param.ParameterType))
                    sigBuilder.Append(" "c)
                End If
                sigBuilder.Append(param.Name)
            Next
            sigBuilder.Append(")")
            Return sigBuilder.ToString()
        End Function

        Public Shared Function GetMemberEnumerableSignature(objMember As MemberInfo) As String

            Dim sigBuilder = New StringBuilder()
            Dim objMemberType As Type = ReflectionHelper.GetMemberReturnType(objMember, False)
            If objMemberType IsNot Nothing AndAlso (objMemberType.IsArray OrElse objMemberType.GetInterface("ICollection") IsNot Nothing _
                                                    OrElse objMemberType.GetInterface("ICollection`1") IsNot Nothing) Then
                sigBuilder.Append("["c)
                If objMemberType.GetInterface("IDictionary") IsNot Nothing OrElse objMemberType.GetInterface("IDictionary`2") IsNot Nothing Then
                    sigBuilder.Append("key")
                Else
                    sigBuilder.Append("index")
                End If

                sigBuilder.Append("]"c)


            End If
            Return sigBuilder.ToString()
        End Function

        <Obsolete("Use Inner Method")> _
        Public Shared Function CreateDelegate(Of T)(ByVal objDelegate As DelegateInfo(Of T)) As [Delegate]
            Return objDelegate.GetDelegate()
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

                Dim targetXmlSerializerBag As XmlSerializerBag = GetXmlSerializerBag(objType, extraTypes, rootName)

                If extraTypes IsNot Nothing Then
                    For Each extraType As Type In extraTypes
                        targetXmlSerializerBag.AddSubType(extraType)
                    Next
                End If

                toReturn = targetXmlSerializerBag.Serializer

            Else
                toReturn = BuildSerializer(objType, extraTypes, rootName)
            End If


            Return toReturn

        End Function

        Public Shared Function GetSerializerTypes(objType As Type, ByVal rootName As String) As IList(Of Type)
            Dim targetXmlSerializerBag As XmlSerializerBag = GetXmlSerializerBag(objType, Nothing, rootName)
            Return targetXmlSerializerBag.SubTypes.ToList()
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

            Dim xmlSerializedObject As New XmlDocument()

            Using objStringReader As New StringReader(objStringBuilder.ToString())
                xmlSerializedObject.Load(objStringReader)
            End Using

            Return xmlSerializedObject

        End Function


        Public Shared Function GetStandardXmlWriterSettings() As XmlWriterSettings
            Dim objXmlSettings As New XmlWriterSettings
            objXmlSettings.Encoding = Encoding.UTF8
            objXmlSettings.Indent = True
            'objXmlSettings.NewLineHandling = NewLineHandling.Replace
            objXmlSettings.NewLineChars = vbLf
            Return objXmlSettings
        End Function


        Private Shared writersDictionary As New Dictionary(Of XmlWriter, HashSet(Of Type))

        ''' <summary>
        ''' that method is a (dirty?) hack to minimize the number of xmlserializers that have to be generated, specifically for the serializablelist class.
        ''' Since a global list of subtypes is maintained, it is included once for each generic type argument to create a complete serializer once at deserialization time.
        ''' The method works only if the xmlwriter was created by the method below. 
        ''' </summary>
        ''' <returns>true if the subtypes have not been included yet, false otherwise</returns>
        Public Shared Function AddSubtypes(objwriter As XmlWriter, objtype As Type) As Boolean
            Dim currentTypes As HashSet(Of Type) = Nothing
            If writersDictionary.TryGetValue(objwriter, currentTypes) Then
                If Not currentTypes.Contains(objtype) Then
                    currentTypes.Add(objtype)
                    Return True
                End If
            End If
            Return False
        End Function

        Public Shared Sub Serialize(ByVal objObject As Object, ByVal omitDeclaration As Boolean, ByRef objTextWriter As TextWriter)

            'New XmlTextWriter(objTextWriter)
            Dim objXmlSettings As XmlWriterSettings = GetStandardXmlWriterSettings()
            If omitDeclaration Then
                objXmlSettings.OmitXmlDeclaration = True
            End If
            Using objXmlWriter As XmlWriter = XmlTextWriter.Create(objTextWriter, objXmlSettings)
                Try
                    SyncLock writersDictionary
                        writersDictionary.Add(objXmlWriter, New HashSet(Of Type))
                    End SyncLock
                    ReflectionHelper.Serialize(objObject, objXmlWriter)
                    objXmlWriter.Flush()
                Catch
                    Throw
                Finally
                    SyncLock writersDictionary
                        writersDictionary.Remove(objXmlWriter)
                    End SyncLock
                End Try
            End Using
        End Sub


        Public Shared Sub Serialize(ByVal objObject As Object, ByRef objXmlWriter As XmlWriter)

            Dim objType As Type = objObject.GetType

            Dim objXmlSerializer As XmlSerializer = GetSerializer(objType)


            objXmlSerializer.Serialize(objXmlWriter, objObject)


        End Sub

        Public Shared Function SerializeToBytes(objObject As Object, ByVal omitDeclaration As Boolean) As Byte()

            Using objStream As New MemoryStream()
                Using objWriter As New StreamWriter(objStream)
                    Serialize(objObject, omitDeclaration, DirectCast(objWriter, TextWriter))
                End Using
                Return objStream.ToArray()
            End Using

        End Function


#End Region


#Region "Deserialize"

        Public Shared Function Deserialize(Of T)(ByVal strObject As String) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, strObject), T)

        End Function


        Public Shared Function Deserialize(Of T)(ByVal objBytes As Byte()) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, objBytes), T)

        End Function

        Public Shared Function Deserialize(Of T)(ByVal reader As TextReader) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, reader), T)

        End Function

        Public Shared Function Deserialize(Of T)(ByVal reader As XmlReader) As T

            Dim objType As Type = GetType(T)


            Return DirectCast(Deserialize(objType, reader), T)

        End Function

        Public Shared Function Deserialize(ByVal mainType As Type, ByVal objString As String) As Object


            Using textReader As New StringReader(objString)

                Return Deserialize(mainType, textReader)

            End Using


        End Function


        Public Shared Function Deserialize(ByVal mainType As Type, ByVal objBytes As Byte()) As Object

            Using objInputStram As New MemoryStream(objBytes)
                Using textReader As New StreamReader(objInputStram)
                    Return Deserialize(mainType, textReader)
                End Using
            End Using

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



        Public Shared Function Deserialize(ByVal mainType As Type, ByVal reader As XmlReader) As Object


            Dim objXmlSerializer As XmlSerializer = GetSerializer(mainType)
            Return objXmlSerializer.Deserialize(reader)

        End Function

#End Region




#End Region




#Region "Private methods"

        Private Shared Function GetXmlSerializerBag(objType As Type, ByVal extraTypes() As Type, ByVal rootName As String) As XmlSerializerBag
            Dim targetXmlSerializerBag As XmlSerializerBag = Nothing
            Dim key As String = objType.FullName & "||" & rootName
            If Not Instance._XmlSerializers.TryGetValue(key, targetXmlSerializerBag) Then
                targetXmlSerializerBag = New XmlSerializerBag(objType, rootName, extraTypes)
                SyncLock Instance._XmlSerializers
                    Instance._XmlSerializers(key) = targetXmlSerializerBag
                End SyncLock
            End If
            Return targetXmlSerializerBag
        End Function

        Public Class XmlSerializerBag

            Public Sub New()

            End Sub

            Public Sub New(targetType As Type, rootName As String)
                Me._TargetType = targetType
                Me._RootName = rootName
            End Sub

            Public Sub New(targetType As Type, rootName As String, subTypes As IEnumerable(Of Type))
                Me.New(targetType, rootName)
                If subTypes IsNot Nothing Then
                    For Each objtype As Type In subTypes
                        Me.SubTypes.Add(objtype)
                    Next
                End If
            End Sub

            Private _TargetType As Type
            Private _RootName As String

            Private _SubTypes As New HashSet(Of Type)

            Public Sub AddSubType(objType As Type)
                If Not SubTypes.Contains(objType) Then
                    SyncLock Me
                        SubTypes.Add(objType)
                        _Serializer = Nothing
                    End SyncLock
                End If
            End Sub

            Private _Serializer As XmlSerializer

            Public ReadOnly Property Serializer As XmlSerializer
                Get
                    If _Serializer Is Nothing Then
                        SyncLock Me
                            _Serializer = BuildSerializer(_TargetType, SubTypes.ToArray(), _RootName)
                        End SyncLock
                    End If
                    Return _Serializer
                End Get
            End Property

            Public Property SubTypes As HashSet(Of Type)
                Get
                    Return _SubTypes
                End Get
                Set(value As HashSet(Of Type))
                    _SubTypes = value
                End Set
            End Property
        End Class

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

                Try
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
                Catch ex As Exception
                    Dim messageBuilder As New StringBuilder()
                    While ex.InnerException IsNot Nothing
                        messageBuilder.AppendLine(ex.Message)
                        ex = ex.InnerException
                    End While
                    messageBuilder.AppendLine(ex.Message)
                    Dim newEx As New ApplicationException(String.Format("Cannot create serializer for type {0}. Stack of messages: {1}.", objType.AssemblyQualifiedName, messageBuilder.ToString()), ex)
                    Throw (newEx)
                End Try


                'If useCache Then
                '    SetGlobal(Of XmlSerializer)(toReturn, objType.FullName)
                'End If

            End If

            Return toReturn

        End Function

#End Region

    End Class
End Namespace

