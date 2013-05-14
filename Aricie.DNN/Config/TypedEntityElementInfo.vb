Imports System.Xml.Serialization

Namespace Configuration


    ''' <summary>
    ''' Base class for custom types of configurable entities
    ''' </summary>
    <Serializable()> _
    Public MustInherit Class TypedEntityElementInfo
        Implements IConfigElementInfo




        Private _EntityType As Type

        Public Sub New()

        End Sub

        Public Sub New(ByVal objType As Type)
            Me._EntityType = objType
        End Sub

        <XmlIgnore()> _
        Public Property EntityType() As Type
            Get
                Return _EntityType
            End Get
            Set(ByVal value As Type)
                _EntityType = value
            End Set
        End Property

        Public MustOverride Overloads Function IsInstalled(ByVal type As Type) As Boolean





        Public Overloads Function IsInstalled() As Boolean Implements IConfigElementInfo.IsInstalled
            Return Me.IsInstalled(Me.EntityType)
        End Function


        Public MustOverride Sub ProcessConfig(ByVal actionType As ConfigActionType) Implements IConfigElementInfo.ProcessConfig

    End Class


End Namespace


