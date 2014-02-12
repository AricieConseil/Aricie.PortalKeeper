Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Threading
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class FileAccessActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        Protected Shared RWLock As New ReaderWriterLock
        Protected Shared LockTimeSpan As TimeSpan = TimeSpan.FromSeconds(5)





        Private _PathExpression As New FleeExpressionInfo(Of String)


        Private _PathMode As FilePathMode
        Private _PortalId As Integer




        <SortOrder(1000)> _
        <ExtendedCategory("File")> _
        Public Property PathMode() As FilePathMode
            Get
                Return _PathMode
            End Get
            Set(ByVal value As FilePathMode)
                _PathMode = value
            End Set
        End Property

        <SortOrder(1001)> _
        <ExtendedCategory("File")> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
            End Set
        End Property


        <SortOrder(1002)> _
        <ExtendedCategory("File")> _
        Public Property PathExpression() As FleeExpressionInfo(Of String)
            Get
                Return _PathExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _PathExpression = value
            End Set
        End Property




        Protected Function GetFileMapPath(actionContext As PortalKeeperContext(Of TEngineEvents)) As String
            Dim expressionPath As String = Me._PathExpression.Evaluate(actionContext, actionContext)
            Return GetFileMapPath(actionContext, expressionPath)
        End Function

        Protected Function GetFileMapPath(actionContext As PortalKeeperContext(Of TEngineEvents), expressionPath As String) As String
            Dim toReturn As String = expressionPath
            Select Case Me._PathMode
                Case FilePathMode.RootPath
                    toReturn = DotNetNuke.Common.Globals.ApplicationMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
                Case FilePathMode.HostPath
                    toReturn = DotNetNuke.Common.Globals.HostMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
                Case FilePathMode.AdminPath
                    toReturn = NukeHelper.PortalInfo(Me._PortalId).HomeDirectoryMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
            End Select
            Return toReturn
        End Function


    End Class
End Namespace