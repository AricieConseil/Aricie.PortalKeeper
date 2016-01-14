Imports System.ComponentModel
Imports System.Reflection
Imports System.Xml.Serialization
Imports Fasterflect

Namespace ComponentModel
    Public Class SubDotNetType
        Inherits DotNetType

        Public sub  New()

        End sub

        Public sub New(objBaseType As type)
            Me.BaseType = New DotNetType( objBaseType)
        End sub


        <Browsable(False)> _
        Public Overridable Property BaseType() As New DotNetType()

        Public Overrides Function GetSelectorG(propertyName As String) As IList(Of DotNetType)
            Return MyBase.GetSelectorG(propertyName).Where(Function(objDotNetType) objDotNetType.GetDotNetType() IsNot Nothing _
                                                                                   AndAlso  IsCandidateSubType(objDotNetType.GetDotNetType())).ToList()
        End Function

        Private Function IsCandidateSubType(objType As Type) As Boolean
            Return objType.IsGenericTypeDefinition OrElse objType.InheritsOrImplements(BaseType.GetDotNetType())
        End Function

        Public Overrides Function GetSelectorG1(propertyName As String) As IList(Of AssemblyName)
            Return MyBase.GetSelectorG1(propertyName).Where(Function(objAssembly)
                Try
                    Return Assembly.Load(objAssembly).GetTypes() _
                                                               .Any(Function(objType) IsCandidateSubType(objType))
                Catch 
                    Return false
                                                               End Try
                                                               End Function
                                                            ).ToList()
        End Function

        Public Overrides Function GetSelectorG2(propertyName As String) As IList(Of String)
            Dim toReturn As IList(Of String) = MyBase.GetSelectorG2(propertyName)
            Dim objAssemblyTypes As Type() = Assembly.Load(New AssemblyName(AssemblyNameSelect)).GetTypes()
            Return toReturn.Where(Function(strNamespace) objAssemblyTypes _
                                     .Where(Function(objType) objType.Namespace = strNamespace _
                                                              AndAlso IsCandidateSubType(objType)) _
                                     .Any()) _
                .ToList()
        End Function

        <Browsable(False)> _
        Public Overrides Property PickerMode As TypePickerMode
            Get
                Return TypePickerMode.SelectType
            End Get
            Set(value As TypePickerMode)
            End Set
        End Property


    End Class


     Public Class SubDotNetType(Of TBaseType)
        Inherits SubDotNetType

      Public Sub New()
            MyBase.New(GetType(TBaseType))
      End Sub

        Private  _BaseType As new DotNetType(GetType(TBaseType))

        <Browsable(False)> _
        <XmlIgnore()> _
      Public Overrides Property BaseType As DotNetType
        get
                Return _BaseType
        End Get
          Set(value As DotNetType)

          End Set
      End Property

     End Class
End NameSpace