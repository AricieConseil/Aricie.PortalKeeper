Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports DotNetNuke.UI.WebControls
Imports System.Threading
Imports Aricie.DNN.Services.Files

Namespace Aricie.DNN.Modules.PortalKeeper



    <Serializable()> _
    Public MustInherit Class FileAccessActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        Protected Shared RWLock As New ReaderWriterLock
        Protected Shared LockTimeSpan As TimeSpan = TimeSpan.FromSeconds(5)

        <ExtendedCategory("File")> _
        <SortOrder(1000)>
        Public Property FilePath As New FilePathInfo

        'todo, make obsolete after migration of old parameters
        '<Obsolete("use FilePath subentity instead")> _
        <Browsable(False)>
        Public Property PathMode As FilePathMode
            Get
                Return Me.FilePath.PathMode
            End Get
            Set(value As FilePathMode)
                Me.FilePath.PathMode = value
            End Set
        End Property

        <Browsable(False)>
        Public Property PortalId As Integer
            Get
                Return Me.FilePath.PortalId
            End Get
            Set(value As Integer)
                Me.FilePath.PortalId = value
            End Set
        End Property


        <Browsable(False)>
        Public Property PathExpression() As FleeExpressionInfo(Of String)
            Get
                Return Me.FilePath.Path.Expression
            End Get
            Set(value As FleeExpressionInfo(Of String))
                Me.FilePath.Path.Mode = SimpleOrExpressionMode.Expression
                Me.FilePath.Path.Expression = value
            End Set
        End Property



        Protected Function GetFileMapPath(actionContext As PortalKeeperContext(Of TEngineEvents)) As String
            Return Me.FilePath.GetMapPath(actionContext, actionContext)
        End Function

       


    End Class
End Namespace