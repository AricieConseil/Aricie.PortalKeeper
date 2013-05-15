Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Users

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ProbeSettings(Of TEngineEvent As IConvertible)
        Inherits NamedConfig


        Private _ProbeValueExpression As New FleeExpressionInfo(Of IComparable)
        Private _UseFilter As Boolean
        Private _ProbeFilterExpression As New FleeExpressionInfo(Of Boolean)
        Private _ProbeHeaderExpression As New FleeExpressionInfo(Of String)("Value.ToString()")
        Private _DumpVariables As String = String.Empty
        Private _dumpVarLock As New Object
        Private _DumpVarList As List(Of String)
        'Private _UserDisplay As Boolean




        Private _RankingsSize As Integer = 3
        Private _SortDirection As SortDirection

        <ExtendedCategory("Evaluation")> _
        Public Property SortDirection As SortDirection
            Get
                Return _SortDirection
            End Get
            Set(value As SortDirection)
                _SortDirection = value
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        Public Property ProbeValueExpression() As FleeExpressionInfo(Of IComparable)
            Get
                Return _ProbeValueExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of IComparable))
                _ProbeValueExpression = value
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        Public Property UseFilter() As Boolean
            Get
                Return _UseFilter
            End Get
            Set(ByVal value As Boolean)
                _UseFilter = value
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        <ConditionalVisible("UseFilter", False, True)> _
        Public Property ProbeFilterExpression() As FleeExpressionInfo(Of Boolean)
            Get
                Return _ProbeFilterExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of Boolean))
                _ProbeFilterExpression = value
            End Set
        End Property
        

        <ExtendedCategory("Display")> _
        Public Property RankingsSize() As Integer
            Get
                Return _RankingsSize
            End Get
            Set(ByVal value As Integer)
                _RankingsSize = value
            End Set
        End Property

        

        <ExtendedCategory("Display")> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
            LineCount(4), Width(500)> _
        Public Property DumpVariables() As String
            Get
                Return _DumpVariables
            End Get
            Set(ByVal value As String)
                _DumpVariables = value
            End Set
        End Property





        <ExtendedCategory("Display")> _
        Public Property ProbeHeaderExpression() As FleeExpressionInfo(Of String)
            Get
                Return _ProbeHeaderExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _ProbeHeaderExpression = value
            End Set
        End Property




        <XmlIgnore()> _
            <Browsable(False)> _
        Public ReadOnly Property DumpVarList() As List(Of String)
            Get
                If _DumpVarList Is Nothing Then
                    SyncLock _dumpVarLock
                        If _DumpVarList Is Nothing Then
                            '_DumpVarList = New List(Of String)
                            'Dim strVars As String() = Me._DumpVariables.Split(","c)
                            'For Each strVar As String In strVars
                            '    If strVar.Trim <> "" Then
                            '        _DumpVarList.Add(strVar.Trim())
                            '    End If
                            'Next
                            _DumpVarList = Common.ParseStringList(Me._DumpVariables)
                        End If
                    End SyncLock
                End If
                Return _DumpVarList
            End Get
        End Property

        


        'Public Property UserDisplay() As Boolean
        '    Get
        '        Return _UserDisplay
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _UserDisplay = value
        '    End Set
        'End Property


        Public Function GetProbe(objUserBotInfo As UserBotInfo, user As UserInfo) As ProbeInstance
            Dim toReturn As ProbeInstance = Nothing
            Dim objParams As IDictionary(Of String, Object)
            If user IsNot Nothing Then
                objParams = objUserBotInfo.GetParameterValues(user)
            Else
                objParams = objUserBotInfo.GetParameterValues()
            End If

            Dim context As New PortalKeeperContext(Of TEngineEvent)
            context.SetVars(objParams)
            Dim objValue As System.IComparable = Me._ProbeValueExpression.Evaluate(context, context)
            context.SetVar("Value", objValue)
            If Not Me._UseFilter OrElse Me._ProbeFilterExpression.Evaluate(context, context) Then
                Dim objHeader As String = Me._ProbeHeaderExpression.Evaluate(context, context)
                Dim objDump As SerializableDictionary(Of String, Object) = context.GetDump(False, Me.DumpVarList)
                toReturn = New ProbeInstance(objHeader, objValue, objDump)
                If Not objUserBotInfo.AnonymousRanking Then
                    toReturn.User = user.DisplayName
                End If
            End If
            Return toReturn
        End Function


    End Class
End Namespace