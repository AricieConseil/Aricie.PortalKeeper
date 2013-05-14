Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace ComponentModel

    <Serializable()> _
    Public Class DotNetType

        Private _TypeName As String = ""



        Public Sub New()

        End Sub



        Public Sub New(ByVal typeName As String)
            Me._TypeName = typeName
        End Sub

        Public Sub New(ByVal objType As Type)
            Me.New(ReflectionHelper.GetSafeTypeName(objType))
        End Sub

        <Browsable(False)> _
        <ExtendedCategory("")> _
        Public Overridable ReadOnly Property Name() As String
            Get
                Dim objType As Type = Me.GetDotNetType
                Return ReflectionHelper.GetSimpleTypeName(objType)
            End Get
        End Property


        <ExtendedCategory("")> _
            <Required(True)> _
            <Width(350)> _
            <LineCount(2)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <AutoPostBack()> _
        Public Property TypeName() As String
            Get
                Return _TypeName
            End Get
            Set(ByVal value As String)
                _TypeName = value
            End Set
        End Property

        Public Function GetDotNetType() As Type
            If Not String.IsNullOrEmpty(Me._TypeName) Then
                Return ReflectionHelper.CreateType(Me._TypeName, False)
            End If
            Return Nothing
        End Function

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
                    Return GenericVariableType.Name & "[" & MyBase.Name & "]"
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