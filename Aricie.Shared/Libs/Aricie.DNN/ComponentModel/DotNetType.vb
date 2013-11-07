Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls

Namespace ComponentModel

    Public Enum TypeSelector
        CommonTypes
        NewType
    End Enum

    <Serializable()> _
    Public Class DotNetType
        Implements ISelector(Of DotNetType)

        Private _TypeName As String = ""



        Public Sub New()

        End Sub

        Private Shared _CommonTypes As New HashSet(Of String)


        Private Sub AddCommonType(strType As String)
            SyncLock _CommonTypes
                    _CommonTypes.Add(strType)
            End SyncLock
        End Sub


        Public Sub New(ByVal typeName As String)
            Me._TypeName = typeName
        End Sub

        Public Sub New(ByVal objType As Type)
            Me.New(ReflectionHelper.GetSafeTypeName(objType))
        End Sub


        <Browsable(False)> _
        Public Overridable ReadOnly Property Name() As String
            Get
                Dim objType As Type = Me.GetDotNetType
                Return ReflectionHelper.GetSimpleTypeName(objType)
            End Get
        End Property



        Public Property TypeSelector As TypeSelector


        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("TypeSelector", False, True, TypeSelector.CommonTypes)> _
        <Selector("Name", "TypeName", False, False, "", "", False, False)> _
        <AutoPostBack> _
        <XmlIgnore()> _
        Public Property CommonType() As String
            Get
                Return _TypeName
            End Get
            Set(value As String)
                If Me._TypeName <> value Then
                    Me._TypeName = value
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
                _TypeName = value
                If Not String.IsNullOrEmpty(value) Then
                    AddCommonType(value)
                End If
            End Set
        End Property

        <ConditionalVisible("TypeSelector", False, True, TypeSelector.NewType)> _
        <Required(True)> _
        <Width(400)> _
        <LineCount(2)> _
        <AutoPostBack()> _
        <XmlIgnore> _
        Public Property EditableTypeName As String
         

        <ConditionalVisible("TypeSelector", False, True, TypeSelector.NewType)> _
        <ActionButton("~/images/action_import.gif")> _
        Public Sub ValidateNewType(ByVal pe As AriciePropertyEditorControl)
            Try
                Dim objType As Type = Me.GetDotNetType(EditableTypeName, True)
                If objType IsNot Nothing Then
                    Me.TypeName = ReflectionHelper.GetSafeTypeName(objType)
                    Me.TypeSelector = ComponentModel.TypeSelector.CommonTypes
                    Me.EditableTypeName = ""
                End If
            Catch ex As Exception
                Dim newEx As New ApplicationException("There was an error trying to create your type. See the complete Stack for more details", ex)
                Throw newEx
            End Try
        End Sub

        Public Function GetDotNetType() As Type
            Return Me.GetDotNetType(Me._TypeName, False)
        End Function

        Public Function GetDotNetType(typeName As String, throwException As Boolean) As Type
            If Not String.IsNullOrEmpty(typeName) Then
                Return ReflectionHelper.CreateType(typeName, throwException)
            End If
            Return Nothing
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As List(Of DotNetType) = DirectCast(GetSelectorG(propertyName), List(Of DotNetType))
            Return toReturn
        End Function

        Public Function GetSelectorG(propertyName As String) As IList(Of DotNetType) Implements ISelector(Of DotNetType).GetSelectorG
            Dim toReturn As New List(Of DotNetType)
            Dim tmpTypes As New HashSet(Of Type)
            If _CommonTypes.Count = 0 Then
                AddCommonType(ReflectionHelper.GetSafeTypeName(GetType(Object)))
                AddCommonType(ReflectionHelper.GetSafeTypeName(GetType(String)))
                AddCommonType(ReflectionHelper.GetSafeTypeName(GetType(Integer)))
            End If
            For Each strType As String In _CommonTypes
                Dim tmpType As Type = ReflectionHelper.CreateType(strType, False)
                If tmpType IsNot Nothing AndAlso Not tmpTypes.Contains(tmpType) Then
                    tmpTypes.Add(tmpType)
                    toReturn.Add(New DotNetType(tmpType))
                End If
            Next
            Return toReturn
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
    End Class

    'Public Class DotNetType(Of TVariable As {IProviderSettings})
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