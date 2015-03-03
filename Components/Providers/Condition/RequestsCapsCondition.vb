Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Tachometer, IconOptions.Normal)> _
    <DisplayName("Requests caps Condition")> _
    <Description("Matches according to a set of maximum numbers of requests per unit of time")> _
    <Serializable()> _
    Public Class RequestsCapsCondition
        Inherits DosEnabledConditionProvider(Of RequestEvent)


        Private _RequestsCaps As New List(Of RequestsCapInfo)

        <ExtendedCategory("Condition")> _
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
           <InnerEditor(GetType(PropertyEditorEditControl)), LabelMode(LabelMode.Top)> _
           <CollectionEditor(False, False, True, True, 10)> _
        Public Property RequestsCaps() As List(Of RequestsCapInfo)
            Get
                Return _RequestsCaps
            End Get
            Set(ByVal value As List(Of RequestsCapInfo))
                _RequestsCaps = value
            End Set
        End Property

        Public Shared Function GetDefaultRequestCaps() As List(Of RequestsCapInfo)
            Dim toReturn As New List(Of RequestsCapInfo)
            toReturn.Add(New RequestsCapInfo(New RequestSource(RequestSourceType.IPAddress), RequestScope.Any, 30, TimeSpan.FromSeconds(2)))
            toReturn.Add(New RequestsCapInfo(New RequestSource(RequestSourceType.IPAddress), RequestScope.Any, 500, TimeSpan.FromMinutes(5)))
            toReturn.Add(New RequestsCapInfo(New RequestSource(RequestSourceType.IPAddress), RequestScope.Any, 2000, TimeSpan.FromHours(2)))
            toReturn.Add(New RequestsCapInfo(New RequestSource(RequestSourceType.IPAddress), RequestScope.DNNPageOnly, 20, TimeSpan.FromMinutes(1)))
            toReturn.Add(New RequestsCapInfo(New RequestSource(RequestSourceType.IPAddress), RequestScope.DNNPageOnly, 100, TimeSpan.FromMinutes(20)))
            toReturn.Add(New RequestsCapInfo(New RequestSource(RequestSourceType.Country), RequestScope.DNNPageOnly, 1000, TimeSpan.FromMinutes(10)))

            Return toReturn
        End Function

        Public Overrides Function FastGetKey(ByVal context As PortalKeeperContext(Of RequestEvent), ByVal clue As Object) As String
            Return DirectCast(clue, RequestSource).GenerateKey(context)
        End Function

        Public Overloads Overrides Function Match(ByVal context As PortalKeeperContext(Of RequestEvent), ByRef clue As Object, ByRef key As String) As Boolean
            If Me.DebuggerBreak Then
                CallDebuggerBreak()
            End If
            For Each cap As RequestsCapInfo In Me._RequestsCaps
                If Not cap.IsValid(context, clue, key) Then
                    Return True
                End If
            Next
            Return False
        End Function
    End Class
End Namespace