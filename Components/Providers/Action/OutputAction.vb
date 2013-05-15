Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class OutputAction(Of TEngineEvents As IConvertible)
        Inherits CacheableAction(Of TEngineEvents)




        Private _OutputName As String = ""

        Private _Simulation As Boolean
        Private _SimulationData As CData = ""




        <ExtendedCategory("Specifics")> _
        Public Property OutputName() As String
            Get
                Return _OutputName
            End Get
            Set(ByVal value As String)
                _OutputName = value
            End Set
        End Property

        Private _AddItems As Boolean

        <ExtendedCategory("Specifics")> _
        Public Property AddItems() As Boolean
            Get
                Return _AddItems
            End Get
            Set(ByVal value As Boolean)
                _AddItems = value
            End Set
        End Property


        <ExtendedCategory("Specifics")> _
        Public Property Simulation() As Boolean
            Get
                Return _Simulation
            End Get
            Set(ByVal value As Boolean)
                _Simulation = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
            <Width(500)> _
            <LineCount(8)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <ConditionalVisible("Simulation", False, True, True)> _
        Public Overridable Property SimulationData() As CData
            Get
                Return _SimulationData
            End Get
            Set(ByVal value As CData)
                _SimulationData = value
            End Set
        End Property





        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            If Not _Simulation Then
                Return MyBase.Run(actionContext, aSync)
            Else
                Return Me.RunFromObject(actionContext, aSync, Me._SimulationData.ToString)
            End If
        End Function







        Public Overrides Function RunFromObject(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean, ByVal cachedObject As Object) As Boolean
            If Me._OutputName <> "" Then
                If _AddItems Then
                    Dim existing As Object = Nothing
                    If actionContext.Items.TryGetValue(Me._OutputName, existing) Then
                        If cachedObject IsNot Nothing Then
                            If TypeOf existing Is IList Then
                                Dim existingList As IList = DirectCast(existing, IList)
                                Dim newItems As IList = DirectCast(cachedObject, IList)
                                For Each newItem As Object In newItems
                                    existingList.Add(newItem)
                                Next
                            ElseIf TypeOf existing Is IDictionary Then
                                Dim existingDico As IDictionary = DirectCast(existing, IDictionary)
                                Dim newItems As IDictionary = DirectCast(cachedObject, IDictionary)
                                For Each newItem As DictionaryEntry In newItems
                                    existingDico.Add(newItem.Key, newItem.Value)
                                Next
                            ElseIf TypeOf existing Is String Then
                                actionContext.Items(Me._OutputName) = DirectCast(existing, String) & DirectCast(cachedObject, String)
                            Else
                                actionContext.Items(Me._OutputName) = cachedObject
                            End If
                        End If
                    Else
                        actionContext.Items(Me._OutputName) = cachedObject
                    End If
                Else
                    actionContext.Items(Me._OutputName) = cachedObject
                End If


                'Dim enableStopWatch As Boolean = actionContext.EnableStopWatch
                'If enableStopWatch Then
                '    Dim paramsKey As New KeyValuePair(Of String, String)("Output Value", ReflectionHelper.Serialize(returnResult).InnerXml)
                '    Dim objStep As New StepInfo(Debug.RequestTiming, Me._OutputName & " - Output Action Return", WorkingPhase.InProgress, False, False, -1, _
                '                                actionContext.FlowId, paramsKey)
                '    PerformanceLogger.Instance.AddDebugInfo(objStep)
                'End If
            End If
            Return True
        End Function
    End Class
End Namespace