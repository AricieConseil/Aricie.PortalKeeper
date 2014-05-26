Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls
Imports System.Reflection

Namespace ComponentModel

    Public Enum TypeSelector
        CommonTypes
        BrowseHierarchy
        NewType
    End Enum

    <ActionButton(IconName.PuzzlePiece, IconOptions.Normal)> _
    <DefaultProperty("Name")> _
    <Serializable()> _
    Public Class DotNetType
        Implements ISelector
        Implements ISelector(Of DotNetType)
        Implements ISelector(Of AssemblyName)
        Implements ISelector(Of String)




        Private _TypeName As String = ""



        Public Sub New()

        End Sub

        Private Shared _CommonTypes As New Dictionary(Of String, Type)


        Private Function AddCommonType(strType As String) As String
            Dim tmpType As Type = Nothing
            If Not _CommonTypes.TryGetValue(strType, tmpType) Then
                If Not String.IsNullOrEmpty(strType) Then
                    tmpType = ReflectionHelper.CreateType(strType, False)
                    If tmpType IsNot Nothing Then
                        strType = ReflectionHelper.GetSafeTypeName(tmpType)
                    End If
                End If
                SyncLock _CommonTypes
                    _CommonTypes(strType) = tmpType
                End SyncLock
            End If
            Return strType
        End Function


        Public Sub New(ByVal typeName As String)
            Me.TypeName = typeName
        End Sub

        Public Sub New(ByVal objType As Type)
            If Not objType.IsGenericParameter Then
                Me.SetTypeName(ReflectionHelper.GetSafeTypeName(objType))
            End If
        End Sub




        <Browsable(False)> _
        Public Overridable ReadOnly Property Name() As String
            Get
                Dim objType As Type = Me.GetDotNetType
                Return ReflectionHelper.GetSimpleTypeName(objType)
            End Get
        End Property

        Private _TypeSelector As Nullable(Of TypeSelector)


        <XmlIgnore()> _
        Public Property TypeSelector As TypeSelector
            Get
                If Not _TypeSelector.HasValue Then
                    Dim targetType As Type = Nothing
                    If Not String.IsNullOrEmpty(_TypeName) AndAlso (Not _CommonTypes.TryGetValue(_TypeName, targetType) OrElse targetType Is Nothing) Then
                        Return TypeSelector.BrowseHierarchy
                    Else
                        Return TypeSelector.CommonTypes
                    End If
                End If
                Return _TypeSelector.Value
            End Get
            Set(value As TypeSelector)
                _TypeSelector = value
            End Set
        End Property


        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("TypeSelector", False, True, TypeSelector.CommonTypes)> _
        <Selector("Name", "TypeName", False, True, "<Select Type By Name>", "", False, True)> _
        <AutoPostBack> _
        <XmlIgnore()> _
        Public Property CommonType() As String
            Get
                Return _TypeName
            End Get
            Set(value As String)
                If Me._TypeName <> value Then
                    Me.SetTypeName(value)
                End If
            End Set
        End Property


        <ConditionalVisible("TypeSelector", False, True, TypeSelector.CommonTypes)> _
       <IsReadOnly(True)> _
        Public Property TypeName() As String
            Get
                Return _TypeName
            End Get
            Set(ByVal value As String)
                If _TypeName <> value Then
                    Me.SetTypeName(AddCommonType(value))
                End If
            End Set
        End Property


        <ConditionalVisible("TypeSelector", False, True, TypeSelector.BrowseHierarchy)> _
        <Selector("Name", "FullName", False, True, "<Select an Assembly>", "", False, True)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <XmlIgnore()> _
        Public Property AssemblyNameSelect As String = ""

        <ConditionalVisible("IsGenericParameter", True, True)> _
         <ConditionalVisible("AssemblyNameSelect", True, True, "")> _
        <ConditionalVisible("TypeSelector", False, True, TypeSelector.BrowseHierarchy)> _
        <SelectorAttribute("", "", False, True, "<Select Namespace>", "", False, True)> _
        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <XmlIgnore()> _
        Public Property NamespaceSelect As String = ""


        <AutoPostBack()> _
        <ConditionalVisible("AssemblyNameSelect", True, True, "")> _
        <ConditionalVisible("TypeSelector", False, True, TypeSelector.BrowseHierarchy)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Name", "TypeName", False, True, "<Select a Type by Name>", "", False, True)> _
        <XmlIgnore()> _
        Public Property TypeNameSelect As String
            Get
                Return _TypeNameSelect
            End Get
            Set(value As String)
                If _TypeNameSelect <> value Then
                    _TypeNameSelect = value
                    Dim objType As Type = Me.GetDotNetType(_TypeNameSelect, False)
                    If objType IsNot Nothing Then
                        If objType.IsGenericType Then
                            PrepareGenericType(objType)
                        End If
                    End If
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property IsSelectedGeneric As Boolean
            Get
                'Return Me.TypeNameSelect.Contains("[")
                Return GenericTypes.Count > 0
            End Get
        End Property

        ' <ConditionalVisible("IsGenericParameter", True, True)> _
        ' <XmlIgnore()> _
        '<ConditionalVisible("TypeSelector", False, True, TypeSelector.BrowseHierarchy)> _
        '<ConditionalVisible("IsSelectedGeneric", False, True, TypeSelector.BrowseHierarchy)> _
        ' Public Property IncludeGenericTypes As Boolean

        <ConditionalVisible("IsSelectedGeneric", False, True)> _
        <ConditionalVisible("AssemblyNameSelect", True, True, "")> _
        <XmlIgnore()> _
      <ConditionalVisible("TypeSelector", False, True, TypeSelector.BrowseHierarchy)> _
      <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List, EnableExport:=False, _
          ItemsReadOnly:=False, MaxItemNb:=-1, NoAdd:=True, NoDeletion:=True, Ordered:=False, Paged:=False, ShowAddItem:=False)> _
        Public Property GenericTypes As New List(Of DotNetType)

        <Browsable(False)> _
        Public ReadOnly Property CurrentlySelectedType As Type
            Get
                Dim toReturn As Type = Nothing
                If Not String.IsNullOrEmpty(TypeNameSelect) Then
                    toReturn = Me.GetDotNetType(TypeNameSelect, False)
                    If toReturn IsNot Nothing AndAlso toReturn.IsGenericTypeDefinition Then
                        Dim genParams As Type() = toReturn.GetGenericArguments()
                        Dim passedParams As New List(Of Type)
                        For i As Integer = 0 To genParams.Count - 1
                            Dim genericParam As Type = Nothing
                            If Me.GenericTypes.Count > i Then
                                genericParam = Me.GenericTypes(i).GetDotNetType()
                            End If
                            If genericParam Is Nothing Then
                                genericParam = genParams(i)
                            End If
                            passedParams.Add(genericParam)
                        Next
                        toReturn = toReturn.MakeGenericType(passedParams.ToArray())
                    End If
                End If
                Return toReturn
            End Get
        End Property

        <ConditionalVisible("TypeSelector", False, True, TypeSelector.BrowseHierarchy)> _
        Public ReadOnly Property CurrentlySelectedTypeName As String
            Get
                Return ReflectionHelper.GetSafeTypeName(CurrentlySelectedType)
            End Get
        End Property

        <ConditionalVisible("TypeSelector", False, True, TypeSelector.NewType)> _
        <Required(True)> _
        <Width(500)> _
        <LineCount(3)> _
        <AutoPostBack()> _
        <XmlIgnore> _
        Public Property EditableTypeName As String


        <ConditionalVisible("TypeSelector", False, True, TypeSelector.NewType, TypeSelector.BrowseHierarchy)> _
        <ActionButton(IconName.Refresh, IconOptions.Normal)> _
        Public Sub Refresh(ByVal pe As AriciePropertyEditorControl)
        End Sub



        <ConditionalVisible("TypeSelector", False, True, TypeSelector.NewType, TypeSelector.BrowseHierarchy)> _
        <ActionButton("~/images/action_import.gif")> _
        Public Sub ValidateNewType(ByVal pe As AriciePropertyEditorControl)
            Try
                Dim objType As Type = Nothing
                Select Case Me.TypeSelector
                    Case ComponentModel.TypeSelector.NewType
                        objType = Me.GetDotNetType(EditableTypeName, True)
                    Case ComponentModel.TypeSelector.BrowseHierarchy
                        objType = CurrentlySelectedType
                End Select
                If objType IsNot Nothing Then
                    Me.SetTypeName(AddCommonType(ReflectionHelper.GetSafeTypeName(objType)))
                    Me.TypeSelector = ComponentModel.TypeSelector.CommonTypes
                    pe.DisplayMessage("Type correctly validated", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    pe.DisplayMessage("Type did not validate", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                End If
            Catch ex As Exception
                Dim newEx As New ApplicationException("There was an error trying to create your type. See the complete Stack for more details", ex)
                Throw newEx
            End Try
        End Sub



        Private _Type As Type
        Private _TypeNameSelect As String

        Public Function GetDotNetType() As Type
            If _Type Is Nothing AndAlso Not String.IsNullOrEmpty(Me._TypeName) Then
                _Type = Me.GetDotNetType(Me._TypeName, False)
            End If
            Return _Type
        End Function

        Public Function GetDotNetType(strTypeName As String, throwException As Boolean) As Type
            If Not String.IsNullOrEmpty(strTypeName) Then
                Return ReflectionHelper.CreateType(strTypeName, throwException)
            End If
            Return Nothing
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As IList = Nothing
            Select Case propertyName
                Case "CommonType"
                    toReturn = DirectCast(GetSelectorG(propertyName), List(Of DotNetType))
                Case "AssemblyNameSelect"
                    toReturn = DirectCast(GetSelectorG1(propertyName), List(Of AssemblyName))
                Case "NamespaceSelect"
                    toReturn = DirectCast(GetSelectorG2(propertyName), List(Of String))
                Case "TypeNameSelect"
                    toReturn = DirectCast(GetSelectorG(propertyName), List(Of DotNetType))
            End Select
            Return toReturn
        End Function




        Public Function GetSelectorG(propertyName As String) As IList(Of DotNetType) Implements ISelector(Of DotNetType).GetSelectorG
            Dim toReturn As New List(Of DotNetType)
            Select Case propertyName
                Case "CommonType"
                    If _CommonTypes.Count = 0 Then
                        AddCommonType(ReflectionHelper.GetSafeTypeName(GetType(Object)))
                        AddCommonType(ReflectionHelper.GetSafeTypeName(GetType(String)))
                        AddCommonType(ReflectionHelper.GetSafeTypeName(GetType(Integer)))
                    End If
                    toReturn.AddRange((From tmpType In _CommonTypes.Values.Distinct() Where tmpType IsNot Nothing Order By tmpType.Assembly.GetName().Name Select New DotNetType(tmpType)))
                Case "TypeNameSelect"
                    If Not String.IsNullOrEmpty(AssemblyNameSelect) Then
                        Dim objAssembly As Assembly = Assembly.Load(New AssemblyName(AssemblyNameSelect))
                        If objAssembly IsNot Nothing Then
                            toReturn.AddRange(From objType In objAssembly.GetTypes() Where objType.Namespace = Me.NamespaceSelect AndAlso Not String.IsNullOrEmpty(objType.AssemblyQualifiedName) Select New DotNetType(objType))
                        End If
                    End If
            End Select
            Return toReturn.OrderBy(Function(objDotNetType) objDotNetType.GetDotNetType().Name).ToList()
        End Function

        'Public Overrides Function Equals(ByVal obj As Object) As Boolean
        '    If TypeOf (obj) Is DotNetType Then
        '        Return Me.TypeName.Equals(DirectCast(obj, DotNetType).TypeName)
        '    End If
        '    Return False
        'End Function

        'Public Overrides Function GetHashCode() As Integer
        '    Return Me.TypeName.GetHashCode()
        'End Function

        Public Function GetSelectorG1(propertyName As String) As IList(Of AssemblyName) Implements ISelector(Of AssemblyName).GetSelectorG
            Dim toReturn As New List(Of AssemblyName)
            For Each objAssembly As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                Try
                    Dim objTypes As Type() = objAssembly.GetTypes()
                    Dim toAdd As AssemblyName = objAssembly.GetName()
                    If Not (toAdd.Name.StartsWith("App_") OrElse toAdd.Name.StartsWith("Microsoft.GeneratedCode")) Then
                        toReturn.Add(toAdd)
                    End If
                Catch
                End Try
            Next
            toReturn.Sort(Function(objAssemblyName1, objAssemblyName2) String.Compare(objAssemblyName1.FullName, objAssemblyName2.FullName, StringComparison.InvariantCultureIgnoreCase))
            Return toReturn
        End Function

        Public Function GetSelectorG2(propertyName As String) As IList(Of String) Implements ISelector(Of String).GetSelectorG
            Dim toReturnSet As New HashSet(Of String)
            If Not String.IsNullOrEmpty(AssemblyNameSelect) Then
                Dim objAssembly As Assembly = Assembly.Load(New AssemblyName(AssemblyNameSelect))
                If objAssembly IsNot Nothing Then
                    For Each objType As Type In objAssembly.GetTypes()
                        If Not String.IsNullOrEmpty(objType.Namespace) Then
                            If Not toReturnSet.Contains(objType.Namespace) Then
                                toReturnSet.Add(objType.Namespace)
                            End If
                        End If
                    Next
                End If
            End If
            Dim toReturn As List(Of String) = toReturnSet.ToList()
            toReturn.Sort()
            Return toReturn
        End Function


        Private Sub SetTypeName(value As String)
            Me._TypeName = value
            Me._EditableTypeName = Me._TypeName
            Me._Type = Nothing
            Dim objType As Type = Me.GetDotNetType(_TypeName, False)
            If objType IsNot Nothing Then
                Me.AssemblyNameSelect = objType.Assembly.GetName().FullName
                Me.NamespaceSelect = objType.Namespace
                If objType.IsGenericType Then
                    Me.TypeNameSelect = ReflectionHelper.GetSafeTypeName(objType.GetGenericTypeDefinition())
                    PrepareGenericType(objType)
                Else
                    Me.TypeNameSelect = ReflectionHelper.GetSafeTypeName(objType)
                End If
            End If
        End Sub

        Private Sub PrepareGenericType(objType As Type)
            Dim argTypes As Type() = objType.GetGenericArguments()
            If argTypes.Count > 0 Then
                Me.GenericTypes.Clear()
                For Each argType As Type In argTypes
                    Me.GenericTypes.Add(New DotNetType(argType))
                Next
            End If
        End Sub


    End Class

    Public Class DotNetType(Of TVariable)
        Inherits DotNetType
        Implements IProviderConfig(Of IGenericizer(Of TVariable))

        Private _GenericVariableType As Type
        Private _TargetTypes As List(Of DotNetType)

        Public Sub New(ByVal targetType As DotNetType)
            Me.TypeName = targetType.TypeName
        End Sub

        Public Sub New(ByVal genericVariableType As Type, ByVal ParamArray targetTypes As DotNetType())
            Me.TypeName = targetTypes(targetTypes.Length - 1).TypeName
            Me._TargetTypes = New List(Of DotNetType)(targetTypes)
            Me._GenericVariableType = genericVariableType
        End Sub

        <Browsable(False)> _
        <ExtendedCategory("")> _
        Public Overrides ReadOnly Property Name() As String
            Get
                If Me._GenericVariableType IsNot Nothing Then
                    Return ProviderConfig.GetDisplayName(GenericVariableType) & " of [" & MyBase.Name & "]"
                    'Return ReflectionHelper.GetSafeTypeName(Me.GetTypedProvider.GetNewProviderSettings.GetType)
                End If
                Return MyBase.Name
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property GenericVariableType() As Type
            Get
                Return _GenericVariableType
            End Get
        End Property

        Public ReadOnly Property TargetTypes() As List(Of DotNetType)
            Get
                Return _TargetTypes
            End Get
        End Property

        Public Function GetProvider() As Object Implements IProviderConfig.GetProvider
            Return GetTypedProvider()
        End Function

        Public Function GetTypedProvider() As IGenericizer(Of TVariable) Implements IProviderConfig(Of IGenericizer(Of TVariable)).GetTypedProvider
            Dim toReturn As New GenericsProvider()
            toReturn.Config = Me
            Return toReturn
        End Function


        Public Class GenericsProvider
            Implements IGenericizer(Of TVariable)

            Private _Config As DotNetType(Of TVariable)
            'Private _Settings As TVariable


            Public Sub SetConfig(ByVal config As IProviderConfig) Implements IProvider.SetConfig
                Me._Config = DirectCast(config, DotNetType(Of TVariable))
            End Sub

            'Public Sub SetSettings(ByVal settings As IProviderSettings) Implements IProvider.SetSettings
            '    Me._Settings = DirectCast(settings, TVariable)
            'End Sub

            Public Property Config() As DotNetType(Of TVariable) Implements IProvider(Of DotNetType(Of TVariable)).Config
                Get
                    Return Me._Config
                End Get
                Set(ByVal value As DotNetType(Of TVariable))
                    Me._Config = value
                End Set
            End Property

            Public Function GetNewProviderSettings() As TVariable Implements IProvider(Of DotNetType(Of TVariable), TVariable).GetNewProviderSettings
                If Me.Config.GenericVariableType IsNot Nothing Then
                    Dim targetTypes As New List(Of Type)
                    For Each targetDotNetType As DotNetType In Me.Config.TargetTypes
                        targetTypes.Add(targetDotNetType.GetDotNetType)
                    Next
                    'Dim genType As Type = Me.Config.GenericVariableType.MakeGenericType(Me.Config.GetDotNetType)
                    Dim genType As Type = Me.Config.GenericVariableType.MakeGenericType(targetTypes.ToArray)
                    Return ReflectionHelper.CreateObject(Of TVariable)(genType.FullName)
                End If
                Return ReflectionHelper.CreateObject(Of TVariable)(Me.Config.TypeName)

            End Function

            'Public Property Settings() As TVariable Implements IProvider(Of DotNetType(Of TVariable), TVariable).Settings
            '    Get
            '        Return Me._Settings
            '    End Get
            '    Set(ByVal value As TVariable)
            '        Me._Settings = value
            '    End Set
            'End Property
        End Class

    End Class


End Namespace