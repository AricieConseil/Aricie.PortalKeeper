
Imports System

Namespace Entities
    ''' <summary>
    ''' Meta information class for an individual TLD rule
    ''' </summary>
    Public Class TLDRule
        Implements IComparable(Of TLDRule)
        ''' <summary>
        ''' The rule name
        ''' </summary>
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Private Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Name As String

        ''' <summary>
        ''' The rule type
        ''' </summary>
        Public Property Type() As RuleType
            Get
                Return m_Type
            End Get
            Private Set(value As RuleType)
                m_Type = value
            End Set
        End Property
        Private m_Type As RuleType

        ''' <summary>
        ''' Construct a TLDRule based on a single line from
        ''' the www.publicsuffix.org list
        ''' </summary>
        ''' <param name="RuleInfo"></param>
        Public Sub New(RuleInfo As String)
            '  Parse the rule and set properties accordingly:
            If RuleInfo.StartsWith("*", StringComparison.InvariantCultureIgnoreCase) Then
                Me.Type = RuleType.Wildcard
                Me.Name = RuleInfo.Substring(2)
            ElseIf RuleInfo.StartsWith("!", StringComparison.InvariantCultureIgnoreCase) Then
                Me.Type = RuleType.Exception
                Me.Name = RuleInfo.Substring(1)
            Else
                Me.Type = RuleType.Normal
                Me.Name = RuleInfo
            End If
        End Sub

#Region "IComparable<TLDRule> Members"

        Public Function CompareTo(other As TLDRule) As Integer Implements IComparable(Of TLDRule).CompareTo
            If other Is Nothing Then
                Return -1
            End If

            Return System.String.Compare(Name, other.Name, System.StringComparison.Ordinal)
        End Function

#End Region

#Region "RuleType enum"

        ''' <summary>
        ''' TLD Rule type
        ''' </summary>
        Public Enum RuleType
            ''' <summary>
            ''' A normal rule
            ''' </summary>
            Normal

            ''' <summary>
            ''' A wildcard rule, as defined by www.publicsuffix.org
            ''' </summary>
            Wildcard

            ''' <summary>
            ''' An exception rule, as defined by www.publicsuffix.org
            ''' </summary>
            Exception
        End Enum

#End Region
    End Class
End Namespace


