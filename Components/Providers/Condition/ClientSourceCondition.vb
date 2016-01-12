Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.MapMarker, IconOptions.Normal)>
    <DisplayName("Client Source Condition")>
    <Description("Matches according to client identifying source parameters (Session, IP Address, country etc.)")>
    Public Class ClientSourceCondition
        Inherits ClientSourceCondition(Of RequestEvent)

    End Class



    <ActionButton(IconName.MapMarker, IconOptions.Normal)>
    <DisplayName("Client Source Condition")>
    <Description("Matches according to client identifying source parameters (Session, IP Address, country etc.)")>
    Public Class ClientSourceCondition(Of TEngineEvents As IConvertible)
        Inherits DosEnabledConditionProvider(Of TEngineEvents)

        Private _RequestSource As New RequestSource(RequestSourceType.IPAddress)
        Private _ValueList As New List(Of String)
        Private _RegexList As New List(Of String)
        Private _Regexes As List(Of Regex)




        <ExtendedCategory("Condition")>
        Public Property RequestSource() As RequestSource
            Get
                Return _RequestSource
            End Get
            Set(ByVal value As RequestSource)
                _RequestSource = value
            End Set
        End Property


        <ExtendedCategory("Condition")>
        <InnerEditor(GetType(CustomTextEditControl), GetType(ClientSourceAttributes))>
        <CollectionEditor(False, False, True, True, 10)>
        Public Property ValueList() As List(Of String)
            Get
                Return _ValueList
            End Get
            Set(ByVal value As List(Of String))
                _ValueList = value
            End Set
        End Property

        <ExtendedCategory("Condition")>
        <Editor(GetType(ListEditControl), GetType(EditControl)),
            InnerEditor(GetType(CustomTextEditControl), GetType(ClientSourceAttributes))>
        <CollectionEditor(False, False, True, True, 10)>
        Public Property RegexList() As List(Of String)
            Get
                Return _RegexList
            End Get
            Set(ByVal value As List(Of String))
                _RegexList = value
            End Set
        End Property

        <Browsable(False)>
        Private ReadOnly Property Regexes() As IEnumerable(Of Regex)
            Get
                If _Regexes Is Nothing Then
                    _Regexes = New List(Of Regex)
                    For Each strRegex As String In Me._RegexList
                        _Regexes.Add(New Regex(strRegex, RegexOptions.Compiled))
                    Next
                End If
                Return _Regexes
            End Get
        End Property

        Public Function MatchInternal(ByVal context As PortalKeeperContext(Of TEngineEvents), ByRef key As String) As Boolean
            key = Me._RequestSource.GenerateKey(context)
            context.Items("ClientSource") = key
            For Each value As String In Me.ValueList
                If key.StartsWith(value) Then
                    Return True
                End If
            Next
            For Each objRegex As Regex In Me.Regexes
                If objRegex.IsMatch(key) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Overrides Function FastGetKey(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal clue As Object) As String
            Return Me._RequestSource.GenerateKey(context)
        End Function

        Public Overloads Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents), ByRef clue As Object, ByRef key As String) As Boolean
            If Me.DebuggerBreak Then
                CallDebuggerBreak()
            End If
            clue = True
            Return MatchInternal(context, key)
        End Function

    End Class


    Public Class ClientSourceAttributes
        Implements IAttributesProvider

        Public Function GetAttributes() As IEnumerable(Of Attribute) Implements IAttributesProvider.GetAttributes
            Dim toReturn As New List(Of Attribute)
            'toReturn.Add(New LineCountAttribute(2))
            toReturn.Add(New WidthAttribute(300))
            Return toReturn
        End Function
    End Class

End Namespace