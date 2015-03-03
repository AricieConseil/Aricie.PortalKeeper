
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Diagnostics
Imports Microsoft.VisualBasic

Namespace Entities
    Public NotInheritable Class TLDRulesCache
        Private Shared _uniqueInstance As TLDRulesCache
        Private Shared _syncObj As New Object()
        Private Shared _syncList As New Object()
        Private _lstTLDRules As List(Of TLDRule)

        Private Sub New()
            '  Initialize our internal list:
            _lstTLDRules = GetTLDRules()
            _lstTLDRules.Sort()
        End Sub

        ''' <summary>
        ''' Returns the singleton instance of the class
        ''' </summary>
        Public Shared ReadOnly Property Instance() As TLDRulesCache
            Get
                If _uniqueInstance Is Nothing Then
                    SyncLock _syncObj
                        If _uniqueInstance Is Nothing Then
                            _uniqueInstance = New TLDRulesCache()
                        End If
                    End SyncLock
                End If
                Return (_uniqueInstance)
            End Get
        End Property

        ''' <summary>
        ''' List of TLD rules
        ''' </summary>
        Public Property TLDRuleList() As List(Of TLDRule)
            Get
                Return _lstTLDRules
            End Get
            Set(value As List(Of TLDRule))
                _lstTLDRules = value
            End Set
        End Property

        ''' <summary>
        ''' Resets the singleton class and flushes all the cached 
        ''' values so they will be re-cached the next time they are requested
        ''' </summary>
        Public Shared Sub Reset()
            SyncLock _syncObj
                _uniqueInstance = Nothing
            End SyncLock
        End Sub

        ''' <summary>
        ''' Gets the list of TLD rules from the cache
        ''' </summary>
        ''' <returns></returns>
        Private Function GetTLDRules() As List(Of TLDRule)
            Dim results As New List(Of TLDRule)()

            '  If the cached suffix rules file exists...
            'If File.Exists(Settings.[Default].SuffixRulesFileLocation) Then
            '  Load the rules from the cached text file
            'List<string> lstTLDRuleStrings = File.ReadAllLines(Settings.Default.SuffixRulesFileLocation, Encoding.UTF8).ToList();
            Dim delimiters As Char() = New Char() {ControlChars.Cr, ControlChars.Lf}
            Dim lstTLDRuleStrings As List(Of String) = My.Resources.PublicSuffix.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList()


            '  Strip out any lines that are:
            '  a.) A comment
            '  b.) Blank
            Dim lstTLDRules As IEnumerable(Of TLDRule) = From ruleString In lstTLDRuleStrings _
                                                         Where Not ruleString.StartsWith("//", StringComparison.InvariantCultureIgnoreCase) _
                                                         AndAlso Not (ruleString.Trim().Length = 0) _
                                                         Select New TLDRule(ruleString)

            '  Transfer this list to the results:
            results = lstTLDRules.ToList()
            'End If

            '  Return our results:
            Debug.WriteLine(String.Format("Loaded {0} rules into cache.", results.Count))
            Return results
        End Function
    End Class
End Namespace


