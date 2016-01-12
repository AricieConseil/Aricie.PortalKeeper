Imports System.Xml.Serialization

Namespace Diagnostics

    ''' <summary>
    ''' Enumeration used to distinguish time consumed by the application from external time.
    ''' </summary>
    Public Enum WorkingPhase
        InProgress
        EndOverhead
    End Enum

    ''' <summary>
    ''' Base class for a performance logger measurements.
    ''' </summary>
    ''' <remarks>Allows to log precisely timed sequences of custom events together with additional data.</remarks>
    
    Public Class StepInfo
        Inherits DebugInfo



        Private _FlowId As String = ""

        Private _FlowStartTime As DateTime

        Private _IsNew As Boolean

        Private _StepNumber As Integer = -1

        Private _IsLastStep As Boolean

        Private _WorkingPhase As WorkingPhase

        Private _Elapsed As TimeSpan = TimeSpan.Zero

        Private _StopElapsed As TimeSpan = TimeSpan.Zero

        Private _TotalElapsed As TimeSpan = TimeSpan.Zero






        Private _Cumulated As Boolean

        Private _NbCumulatedSteps As Integer

        Private _CumulatedElapsed As TimeSpan = TimeSpan.Zero

        Public Sub New()

        End Sub

        Public Sub New(ByVal debugType As String, ByVal label As String, ByVal workingPhase As WorkingPhase, ByVal isLastStep As Boolean, ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            Me.New(debugType, label, workingPhase, isLastStep, False, -1, "", additionalProperties)
        End Sub



        ''' <summary>
        ''' Builds a DNN event Log to record the timing of a particular process.
        ''' Computes each step duration according to inner records</summary>
        ''' <param name="debugType">name for the process flow being monitored</param>
        ''' <param name="label">label for the current step</param>
        ''' <param name="isLastStep">States if this is the last step to reset the inner counters</param>
        ''' <param name="logMemoryUsage">True if a log property must be added with a memory footprint obtained from the garbage collector</param>
        ''' <param name="portalId">The portal Id of the corresponding event if available, -1 is espected otherwise</param>
        ''' <param name="flowId">Specific key associated to each of concurrent processes. empty string otherwise to associate with the current thread id</param>
        ''' <param name="additionalProperties">optional additional properties entered as Pairs of IConvertible (Property Name, Property Value)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal debugType As String, ByVal label As String, ByVal workingPhase As WorkingPhase, ByVal isLastStep As Boolean, _
                       ByVal logMemoryUsage As Boolean, ByVal portalId As Integer, ByVal flowId As String, ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            MyBase.New(debugType, label, "", logMemoryUsage, portalId, additionalProperties)
            Me._IsLastStep = isLastStep
            Me._FlowId = flowId
            Me._WorkingPhase = workingPhase
        End Sub


        Public Property FlowStartTime() As DateTime
            Get
                Return _FlowStartTime
            End Get
            Set(ByVal value As DateTime)
                _FlowStartTime = value
            End Set
        End Property

        Public Property IsNew() As Boolean
            Get
                Return _IsNew
            End Get
            Set(ByVal value As Boolean)
                _IsNew = value
            End Set
        End Property

        Public Property IsLastStep() As Boolean
            Get
                Return _IsLastStep
            End Get
            Set(ByVal value As Boolean)
                _IsLastStep = value
            End Set
        End Property

        Public Property StepNumber() As Integer
            Get
                Return _StepNumber
            End Get
            Set(ByVal value As Integer)
                _StepNumber = value
            End Set
        End Property

        Public Property FlowId() As String
            Get
                Return _FlowId
            End Get
            Set(ByVal value As String)
                _FlowId = value
            End Set
        End Property



        Public Property WorkingPhase() As WorkingPhase
            Get
                Return Me._WorkingPhase
            End Get
            Set(ByVal value As WorkingPhase)
                Me._WorkingPhase = value
            End Set
        End Property




        <XmlIgnore()> _
        Public Property Elapsed() As TimeSpan
            Get
                Return _Elapsed
            End Get
            Set(ByVal value As TimeSpan)
                _Elapsed = value
            End Set
        End Property


        Public Property XmlElapsed() As String
            Get
                Return Me._Elapsed.ToString()
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Me._Elapsed = TimeSpan.Parse(value)
                End If
            End Set
        End Property

        <XmlIgnore()> _
        Public Property StopElapsed() As TimeSpan
            Get
                Return Me._StopElapsed
            End Get
            Set(ByVal value As TimeSpan)
                _StopElapsed = value
            End Set
        End Property

        Public Property XmlStopElapsed() As String
            Get
                Return Me._StopElapsed.ToString()
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Me._StopElapsed = TimeSpan.Parse(value)
                End If
            End Set
        End Property

        <XmlIgnore()> _
        Public Property TotalElapsed() As TimeSpan
            Get
                Return _TotalElapsed
            End Get
            Set(ByVal value As TimeSpan)
                _TotalElapsed = value
            End Set
        End Property


        Public Property XmlTotalElapsed() As String
            Get
                Return Me._TotalElapsed.ToString()
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Me._TotalElapsed = TimeSpan.Parse(value)
                End If
            End Set
        End Property

        Public Property Cumulated() As Boolean
            Get
                Return _Cumulated
            End Get
            Set(ByVal value As Boolean)
                _Cumulated = value
            End Set
        End Property

        <XmlIgnore()> _
        Public Property CumulatedElapsed() As TimeSpan
            Get
                Return _CumulatedElapsed
            End Get
            Set(ByVal value As TimeSpan)
                _CumulatedElapsed = value
            End Set
        End Property

        Public Property XmlCumulatedElapsed() As String
            Get
                Return Me._CumulatedElapsed.ToString()
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Me._CumulatedElapsed = TimeSpan.Parse(value)
                End If
            End Set
        End Property

        Public Property NbCumulatedSteps() As Integer
            Get
                Return _NbCumulatedSteps
            End Get
            Set(ByVal value As Integer)
                _NbCumulatedSteps = value
            End Set
        End Property
    End Class




End Namespace
