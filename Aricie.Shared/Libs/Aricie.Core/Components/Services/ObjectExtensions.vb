Imports System.Reflection.Emit
Imports System.ComponentModel
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports System.Globalization
Imports System.Web.Script.Serialization

Namespace Services
    ''' <summary>
    ''' A Utility class used to merge the properties of
    ''' heterogenious objects
    ''' </summary>
    Public Module ObjectExtensions
        'assembly/module builders
        Private asmBuilder As System.Reflection.Emit.AssemblyBuilder = Nothing
        Private modBuilder As ModuleBuilder = Nothing

        'object type cache
        Private anonymousMergeTypes As IDictionary(Of [String], Type) = New Dictionary(Of [String], Type)()

        'used for thread-safe access to Type Dictionary
        Private _syncLock As New [Object]()

        Private _DynamicTypes As New Dictionary(Of String, Type)

        Public ReadOnly Property DynamicTypes As Dictionary(Of String, Type)
            Get
                Return _DynamicTypes
            End Get
        End Property

        ''' <summary>
        ''' Merge two different object instances into a single
        ''' object which is a super-set
        ''' of the properties of both objects
        ''' </summary>
        Public Function MergeTypes(values1 As [Object], values2 As [Object]) As [Object]
            'create a name from the names of both Types
            Dim name As [String] = [String].Format("{0}_{1}", values1.[GetType](), values2.[GetType]())
            Dim name2 As [String] = [String].Format("{0}_{1}", values2.[GetType](), values1.[GetType]())

            Dim newValues As [Object] = CreateInstance(name, values1, values2)
            If newValues IsNot Nothing Then
                Return newValues
            End If

            newValues = CreateInstance(name2, values2, values1)
            If newValues IsNot Nothing Then
                Return newValues
            End If

            'lock for thread safe writing
            SyncLock _syncLock
                'now that we're inside the lock - check one more time
                newValues = CreateInstance(name, values1, values2)
                If newValues IsNot Nothing Then
                    Return newValues
                End If

                'merge list of PropertyDescriptors for both objects
                Dim pdc As PropertyDescriptor() = GetProperties(values1, values2)

                ''make sure static properties are properly initialized
                'InitializeAssembly()

                'create the type definition
                Dim newType As Type = CreateType(name, pdc)

                'add it to the cache
                anonymousMergeTypes.Add(name, newType)

                'return an instance of the new Type
                Return CreateInstance(name, values1, values2)
            End SyncLock
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToObject(Of T)(source As IDictionary(Of String, Object)) As T
            Return DirectCast(GetType(T).ToObject(source), T)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToObject(someObjectType As Type, source As IDictionary(Of String, Object)) As Object
            Dim someObject As Object = ReflectionHelper.CreateObject(someObjectType)
            For Each item As KeyValuePair(Of String, Object) In source
                ReflectionHelper.GetPropertiesDictionary(someObjectType)(item.Key).SetValue(someObject, item.Value, Nothing)
            Next

            Return someObject
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToObject(someObjectType As Type, source As IDictionary(Of String, String)) As Object
            Return someObjectType.ToObject(source, CultureInfo.InvariantCulture)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToObject(someObjectType As Type, source As IDictionary(Of String, String), culture As CultureInfo) As Object
            Return someObjectType.ToObject(source, CultureInfo.InvariantCulture, False)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToObject(someObjectType As Type, source As IDictionary(Of String, String), culture As CultureInfo, createEmptyObjects As Boolean) As Object
            Return someObjectType.ToObject(source, CultureInfo.InvariantCulture, createEmptyObjects, False)

        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToObject(someObjectType As Type, source As IDictionary(Of String, String), culture As CultureInfo, createEmptyObjects As Boolean, initializeLists As Boolean) As Object
            Dim someObject As Object = ReflectionHelper.CreateObject(someObjectType)
            For Each item As KeyValuePair(Of String, String) In source
                Dim objProp As PropertyInfo = Nothing
                If ReflectionHelper.GetPropertiesDictionary(someObjectType).TryGetValue(item.Key, objProp) AndAlso objProp.CanWrite Then
                    objProp.SetValue(someObject, ConvertFromString(objProp.PropertyType, item.Value, culture, createEmptyObjects, initializeLists), Nothing)
                End If
            Next

            Return someObject
        End Function




        <System.Runtime.CompilerServices.Extension> _
        Public Function AsDictionary(source As Object) As IDictionary(Of String, Object)
            Return ReflectionHelper.GetPropertiesDictionary(source.[GetType]).ToDictionary(Function(propInfo) propInfo.Key, Function(propInfo) propInfo.Value.GetValue(source, Nothing))

        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function AsStringDictionary(source As Object) As Dictionary(Of String, String)
            Return AsStringDictionary(source, CultureInfo.InvariantCulture)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function AsStringDictionary(source As Object, culture As CultureInfo) As Dictionary(Of String, String)
            Return ReflectionHelper.GetPropertiesDictionary(source.GetType()) _
                .ToDictionary(Function(propInfo) propInfo.Key, _
                              Function(propInfo) ObjectExtensions.ConvertToString(propInfo.Value.GetValue(source, Nothing), culture))

        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ConvertToString(source As Object) As String
            Return ConvertToString(source, CultureInfo.InvariantCulture)
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function ConvertToString(source As Object, culture As CultureInfo) As String
            If source Is Nothing Then
                Return String.Empty
            End If
            'Dim convertibleSource As IConvertible = TryCast(source, IConvertible)
            'If convertibleSource IsNot Nothing Then
            '    Return convertibleSource.ToString(CultureInfo.InvariantCulture)
            'End If
            Dim typeConverter As TypeConverter = TypeDescriptor.GetConverter(source.GetType())

            If typeConverter IsNot Nothing AndAlso typeConverter.CanConvertTo(GetType(String)) _
                    AndAlso (Not typeConverter.GetType() Is GetType(TypeConverter) _
                    OrElse TypeOf source Is IFormattable) Then
                Return DirectCast(typeConverter.ConvertTo(Nothing, culture, source, GetType(String)), String)
            End If
            Return ReflectionHelper.Serialize(source).Beautify()
            'Return JavascriptSerializer.Serialize(source)
        End Function

        Public Function ConvertFromString(targetType As Type, source As String, culture As CultureInfo) As Object
            Return ConvertFromString(targetType, source, culture, False)
        End Function

        Public Function ConvertFromString(targetType As Type, source As String, culture As CultureInfo, createEmptyObjects As Boolean) As Object
            Return ConvertFromString(targetType, source, culture, createEmptyObjects, False)
        End Function

        'Private _JavascriptSerializer As JavaScriptSerializer

        'Public ReadOnly Property JavascriptSerializer As JavaScriptSerializer
        '    Get
        '        If _JavascriptSerializer Is Nothing Then
        '            _JavascriptSerializer = New JavaScriptSerializer()
        '        End If
        '        Return _JavascriptSerializer
        '    End Get
        'End Property

        Public Function ConvertFromString(targetType As Type, source As String, culture As CultureInfo, createEmptyObjects As Boolean, initializeLists As Boolean) As Object
            Dim typeConverter As TypeConverter = TypeDescriptor.GetConverter(targetType)

            If typeConverter IsNot Nothing AndAlso typeConverter.CanConvertFrom(GetType(String)) Then
                Return typeConverter.ConvertFrom(Nothing, culture, source)
            End If
            'If GetType(IConvertible).IsAssignableFrom(targetType) Then
            '    Return DirectCast(source, IConvertible).ToType(targetType, CultureInfo.InvariantCulture)
            'End If
            If Not source.IsNullOrEmpty() Then
                Try
                    'Return JavascriptSerializer.Deserialize(Of Object)(source)
                    Return ReflectionHelper.Deserialize(targetType, source)
                Catch ex As Exception
                    ExceptionHelper.LogException(ex)
                    Return Nothing
                End Try
            End If
            If createEmptyObjects AndAlso ReflectionHelper.CanCreateObject(targetType) Then
                Dim toReturn As Object = ReflectionHelper.CreateObject(targetType)
                If initializeLists Then
                    Dim enumerable As IList = TryCast(toReturn, IList)
                    If enumerable IsNot Nothing Then
                        Dim innerType As Type = ReflectionHelper.GetCollectionElementType(enumerable)
                        If ReflectionHelper.CanCreateObject(innerType) Then
                            enumerable.Add(ReflectionHelper.CreateObject(innerType))
                        End If
                    End If
                End If
                Return toReturn
            End If
            Return Nothing
        End Function

        Public Function ConvertFromString(targetType As Type, source As String) As Object
            Return ConvertFromString(targetType, source, CultureInfo.InvariantCulture)
        End Function


        'Private _DynamicTypes As New Dictionary(Of String, Type)

        <System.Runtime.CompilerServices.Extension> _
        Public Function ToCustomObject(fromProperties As IDictionary(Of String, Object), customTypeName As String) As Object
            Dim propDescriptors As New List(Of PropertyDescriptor)


            For Each objPair As KeyValuePair(Of String, Object) In fromProperties
                Dim objDumbProp As DumbPropertyDescriptor
                If objPair.Value Is Nothing Then
                    objDumbProp = New DumbPropertyDescriptor(objPair.Key, GetType(Object))
                Else
                    Dim objType As Type = objPair.Value.GetType()
                    objDumbProp = New DumbPropertyDescriptor(objPair.Key, objType)
                End If
                propDescriptors.Add(objDumbProp)
            Next

            Dim dynType As Type = Nothing

            dynType = CreateType(customTypeName, propDescriptors.ToArray)
            Dim toReturn As Object = Activator.CreateInstance(dynType, fromProperties.Values.ToArray())
            Return toReturn
        End Function




        ''' <summary>
        ''' Create a new Type definition from the list
        ''' of PropertyDescriptors
        ''' </summary>
        Public Function CreateType(name As [String], pdc As PropertyDescriptor()) As Type

            InitializeAssembly()
            If Not name.IsNullOrEmpty() Then
                name = "." & name
            End If

            Dim typeName As String = GetCustomTypeId(pdc) & name
            Dim typeFullName As String = String.Format("{0}, {1}", typeName, modBuilder.Assembly.GetName().Name)

            Dim toReturn As Type = Nothing
            If Not _DynamicTypes.TryGetValue(typeFullName, toReturn) Then
                'create TypeBuilder
                Dim typeBuilder As TypeBuilder = CreateTypeBuilder(typeName)

                'get list of types for ctor definition
                Dim propertyTypes As Type() = GetTypes(pdc)

                'create priate fields for use w/in the ctor body and properties
                Dim fields As FieldBuilder() = BuildFields(typeBuilder, pdc)

                If propertyTypes.Length > 0 Then
                    'define/emit the empty Ctor
                    BuildCtor(typeBuilder, New List(Of FieldBuilder)().ToArray(), New List(Of Type)().ToArray())
                End If


                'define/emit the Ctor
                BuildCtor(typeBuilder, fields, propertyTypes)

                'define/emit the properties
                BuildProperties(typeBuilder, fields)

                'return Type definition
                Dim newToReturn As Type = typeBuilder.CreateType()
                SyncLock _DynamicTypes
                    If Not _DynamicTypes.TryGetValue(typeFullName, toReturn) Then
                        toReturn = newToReturn
                        _DynamicTypes(typeFullName) = toReturn
                    End If
                End SyncLock
            End If


            Return toReturn

        End Function

        Private Function GetCustomTypeId(pdc As PropertyDescriptor()) As String
            Dim objNameBuilder As New StringBuilder()
            For Each objPair As PropertyDescriptor In pdc
                objNameBuilder.Append(objPair.Name)
                objNameBuilder.Append(ReflectionHelper.GetSimpleTypeName(objPair.PropertyType))
            Next
            Return "Custom" & Common.GetStringHashCode(objNameBuilder.ToString()).ToString(CultureInfo.InvariantCulture).Trim("-"c)
        End Function


        'Private createTypeLock As New Object



        ''' <summary>
        ''' Instantiates an instance of an existing Type from cache
        ''' </summary>
        Private Function CreateInstance(name As [String], values1 As [Object], values2 As [Object]) As [Object]
            Dim newValues As [Object] = Nothing

            'merge all values together into an array
            Dim allValues As [Object]() = MergeValues(values1, values2)

            'check to see if type exists
            If anonymousMergeTypes.ContainsKey(name) Then
                'get type
                Dim type As Type = anonymousMergeTypes(name)

                'make sure it isn't null for some reason
                If type IsNot Nothing Then
                    'create a new instance
                    newValues = Activator.CreateInstance(type, allValues)
                Else
                    'remove null type entry
                    SyncLock _syncLock
                        anonymousMergeTypes.Remove(name)
                    End SyncLock
                End If
            End If

            'return values (if any)
            Return newValues
        End Function





        ''' <summary>
        ''' Merge PropertyDescriptors for both objects
        ''' </summary>
        Private Function GetProperties(values1 As [Object], values2 As [Object]) As PropertyDescriptor()
            'dynamic list to hold merged list of properties
            Dim properties As New List(Of PropertyDescriptor)()

            'get the properties from both objects
            Dim pdc1 As PropertyDescriptorCollection = TypeDescriptor.GetProperties(values1)
            Dim pdc2 As PropertyDescriptorCollection = TypeDescriptor.GetProperties(values2)

            'add properties from values1
            For i As Integer = 0 To pdc1.Count - 1
                properties.Add(pdc1(i))
            Next

            'add properties from values2
            For i As Integer = 0 To pdc2.Count - 1
                properties.Add(pdc2(i))
            Next

            'return array
            Return properties.ToArray()
        End Function

        ''' <summary>
        ''' Get the type of each property
        ''' </summary>
        Private Function GetTypes(pdc As PropertyDescriptor()) As Type()
            Dim types As New List(Of Type)()

            For i As Integer = 0 To pdc.Length - 1
                types.Add(pdc(i).PropertyType)
            Next

            Return types.ToArray()
        End Function

        ''' <summary>
        ''' Merge the values of the two types into an object array
        ''' </summary>
        Private Function MergeValues(values1 As [Object], values2 As [Object]) As [Object]()
            Dim pdc1 As PropertyDescriptorCollection = TypeDescriptor.GetProperties(values1)
            Dim pdc2 As PropertyDescriptorCollection = TypeDescriptor.GetProperties(values2)

            Dim values As New List(Of [Object])()
            For i As Integer = 0 To pdc1.Count - 1
                values.Add(pdc1(i).GetValue(values1))
            Next

            For i As Integer = 0 To pdc2.Count - 1
                values.Add(pdc2(i).GetValue(values2))
            Next

            Return values.ToArray()
        End Function

        ''' <summary>
        ''' Initialize static objects
        ''' </summary>
        Private Sub InitializeAssembly()
            'check to see if we've already instantiated
            'the static objects
            If asmBuilder Is Nothing Then
                'create a new dynamic assembly
                Dim assembly As New AssemblyName()
                assembly.Name = CustomTypesAssemblyName

                'get the current application domain
                Dim domain As AppDomain = Thread.GetDomain()

                'get a module builder object
                asmBuilder = domain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run)
                modBuilder = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name, False)
            End If
        End Sub

        Public Const CustomTypesAssemblyName = "Aricie.CustomTypes"

        ''' <summary>
        ''' Create a type builder with the specified name
        ''' </summary>
        Private Function CreateTypeBuilder(typeName As String) As TypeBuilder
            'define class attributes
            Dim typeBuilder As TypeBuilder = modBuilder.DefineType(typeName, _
                                                                   TypeAttributes.Public _
                                                                   Or TypeAttributes.Class _
                                                                   Or TypeAttributes.AutoClass _
                                                                   Or TypeAttributes.AnsiClass _
                                                                   Or TypeAttributes.BeforeFieldInit _
                                                                   Or TypeAttributes.AutoLayout, GetType(Object))

            'return new type builder
            Return typeBuilder
        End Function

        ''' <summary>
        ''' Define/emit the ctor and ctor body
        ''' </summary>
        Private Sub BuildCtor(typeBuilder As TypeBuilder, fields As FieldBuilder(), types As Type())
            'define ctor()
            Dim ctor As ConstructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.[Public], CallingConventions.Standard, types)
            'Dim params = ctor.GetParameters()
            For i As Integer = 0 To fields.Length - 1
                'Dim parameter As ParameterInfo = fields(i)
                Dim parameterBuilder = ctor.DefineParameter(i + 1, Nothing, fields(i).Name.Trim("_"c).ToCamelCase())

            Next
            'build ctor()
            Dim ctorGen As ILGenerator = ctor.GetILGenerator()

            'create ctor that will assign to private fields
            For i As Integer = 0 To fields.Length - 1
                'load argument (parameter)
                ctorGen.Emit(OpCodes.Ldarg_0)
                ctorGen.Emit(OpCodes.Ldarg, (i + 1))

                'store argument in field
                ctorGen.Emit(OpCodes.Stfld, fields(i))
            Next

            'return from ctor()
            ctorGen.Emit(OpCodes.Ret)
        End Sub

        ''' <summary>
        ''' Define fields based on the list of PropertyDescriptors
        ''' </summary>
        Private Function BuildFields(typeBuilder As TypeBuilder, pdc As PropertyDescriptor()) As FieldBuilder()
            Dim fields As New List(Of FieldBuilder)()

            'build/define fields
            For i As Integer = 0 To pdc.Length - 1
                Dim pd As PropertyDescriptor = pdc(i)

                'define field as '_[Name]' with the object's Type
                Dim field As FieldBuilder = typeBuilder.DefineField([String].Format("_{0}", pd.Name), pd.PropertyType, FieldAttributes.[Private])

                'add to list of FieldBuilder objects
                fields.Add(field)
            Next

            Return fields.ToArray()
        End Function

        ''' <summary>
        ''' Build a list of Properties to match the list of private fields
        ''' </summary>
        Private Sub BuildProperties(typeBuilder As TypeBuilder, fields As FieldBuilder())
            'build properties
            For i As Integer = 0 To fields.Length - 1
                'remove '_' from name for public property name
                Dim propertyName As [String] = fields(i).Name.Substring(1)

                'define the property
                Dim objProp As PropertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, fields(i).FieldType, Nothing)

                'define 'Get' method 
                Dim getMethod As MethodBuilder = typeBuilder.DefineMethod([String].Format("Get_{0}", propertyName), _
                                                                          MethodAttributes.HideBySig Or MethodAttributes.Public Or MethodAttributes.SpecialName, fields(i).FieldType, Type.EmptyTypes)

                'build 'Get' method
                Dim methGen As ILGenerator = getMethod.GetILGenerator()

                'method body
                methGen.Emit(OpCodes.Ldarg_0)
                'load value of corresponding field
                methGen.Emit(OpCodes.Ldfld, fields(i))
                'return from 'Get' method
                methGen.Emit(OpCodes.Ret)

                'assign method to property 'Get'
                objProp.SetGetMethod(getMethod)




                Dim setMethod As MethodBuilder = typeBuilder.DefineMethod([String].Format("Set_{0}", propertyName), _
                                                                          MethodAttributes.[Public] Or MethodAttributes.SpecialName Or MethodAttributes.HideBySig, _
                                                                          Nothing, New Type() {fields(i).FieldType})

                Dim setIL As ILGenerator = setMethod.GetILGenerator()

                setIL.Emit(OpCodes.Ldarg_0)
                setIL.Emit(OpCodes.Ldarg_1)
                setIL.Emit(OpCodes.Stfld, fields(i))
                setIL.Emit(OpCodes.Ret)

                objProp.SetSetMethod(setMethod)

            Next
        End Sub
    End Module
End Namespace