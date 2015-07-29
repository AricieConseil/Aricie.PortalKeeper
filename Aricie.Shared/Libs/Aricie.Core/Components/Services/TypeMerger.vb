Imports System.Reflection.Emit
Imports System.ComponentModel
Imports System.Reflection
Imports System.Threading

Namespace Services
    ''' <summary>
    ''' A Utility class used to merge the properties of
    ''' heterogenious objects
    ''' </summary>
    Public Class TypeMerger
        'assembly/module builders
        Private Shared asmBuilder As System.Reflection.Emit.AssemblyBuilder = Nothing
        Private Shared modBuilder As ModuleBuilder = Nothing

        'object type cache
        Private Shared anonymousTypes As IDictionary(Of [String], Type) = New Dictionary(Of [String], Type)()

        'used for thread-safe access to Type Dictionary
        Private Shared _syncLock As New [Object]()

        ''' <summary>
        ''' Merge two different object instances into a single
        ''' object which is a super-set
        ''' of the properties of both objects
        ''' </summary>
        Public Shared Function MergeTypes(values1 As [Object], values2 As [Object]) As [Object]
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
                anonymousTypes.Add(name, newType)

                'return an instance of the new Type
                Return CreateInstance(name, values1, values2)
            End SyncLock
        End Function

        ''' <summary>
        ''' Instantiates an instance of an existing Type from cache
        ''' </summary>
        Private Shared Function CreateInstance(name As [String], values1 As [Object], values2 As [Object]) As [Object]
            Dim newValues As [Object] = Nothing

            'merge all values together into an array
            Dim allValues As [Object]() = MergeValues(values1, values2)

            'check to see if type exists
            If anonymousTypes.ContainsKey(name) Then
                'get type
                Dim type As Type = anonymousTypes(name)

                'make sure it isn't null for some reason
                If type IsNot Nothing Then
                    'create a new instance
                    newValues = Activator.CreateInstance(type, allValues)
                Else
                    'remove null type entry
                    SyncLock _syncLock
                        anonymousTypes.Remove(name)
                    End SyncLock
                End If
            End If

            'return values (if any)
            Return newValues
        End Function

        ''' <summary>
        ''' Merge PropertyDescriptors for both objects
        ''' </summary>
        Private Shared Function GetProperties(values1 As [Object], values2 As [Object]) As PropertyDescriptor()
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
        Private Shared Function GetTypes(pdc As PropertyDescriptor()) As Type()
            Dim types As New List(Of Type)()

            For i As Integer = 0 To pdc.Length - 1
                types.Add(pdc(i).PropertyType)
            Next

            Return types.ToArray()
        End Function

        ''' <summary>
        ''' Merge the values of the two types into an object array
        ''' </summary>
        Private Shared Function MergeValues(values1 As [Object], values2 As [Object]) As [Object]()
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
        Private Shared Sub InitializeAssembly()
            'check to see if we've already instantiated
            'the static objects
            If asmBuilder Is Nothing Then
                'create a new dynamic assembly
                Dim assembly As New AssemblyName()
                assembly.Name = "Aricie.AnonymousTypeExentions"

                'get the current application domain
                Dim domain As AppDomain = Thread.GetDomain()

                'get a module builder object
                asmBuilder = domain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run)
                modBuilder = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name, False)
            End If
        End Sub

        ''' <summary>
        ''' Create a new Type definition from the list
        ''' of PropertyDescriptors
        ''' </summary>
        Public Shared Function CreateType(name As [String], pdc As PropertyDescriptor()) As Type

            InitializeAssembly()
            'create TypeBuilder
            Dim typeBuilder As TypeBuilder = CreateTypeBuilder(name)

            'get list of types for ctor definition
            Dim types As Type() = GetTypes(pdc)

            'create priate fields for use w/in the ctor body and properties
            Dim fields As FieldBuilder() = BuildFields(typeBuilder, pdc)

            'define/emit the empty Ctor
            BuildCtor(typeBuilder, New List(Of FieldBuilder)().ToArray(), New List(Of Type)().ToArray())

            'define/emit the Ctor
            BuildCtor(typeBuilder, fields, types)

            'define/emit the properties
            BuildProperties(typeBuilder, fields)

            'return Type definition
            Return typeBuilder.CreateType()
        End Function

        ''' <summary>
        ''' Create a type builder with the specified name
        ''' </summary>
        Private Shared Function CreateTypeBuilder(typeName As String) As TypeBuilder
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
        Private Shared Sub BuildCtor(typeBuilder As TypeBuilder, fields As FieldBuilder(), types As Type())
            'define ctor()
            Dim ctor As ConstructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.[Public], CallingConventions.Standard, types)

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
        Private Shared Function BuildFields(typeBuilder As TypeBuilder, pdc As PropertyDescriptor()) As FieldBuilder()
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
        Private Shared Sub BuildProperties(typeBuilder As TypeBuilder, fields As FieldBuilder())
            'build properties
            For i As Integer = 0 To fields.Length - 1
                'remove '_' from name for public property name
                Dim propertyName As [String] = fields(i).Name.Substring(1)

                'define the property
                Dim objProp As PropertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, fields(i).FieldType, Nothing)

                'define 'Get' method only (anonymous types are read-only)
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
    End Class
End NameSpace