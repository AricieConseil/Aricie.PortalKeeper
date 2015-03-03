
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Diagnostics

Namespace Entities
    Public Class DomainName
#Region "Private members"

        Private _subDomain As String = String.Empty
        Private _domain As String = String.Empty
        Private _tld As String = String.Empty
        Private _tldRule As TLDRule = Nothing

#End Region

#Region "Public properties"

        ''' <summary>
        ''' The subdomain portion
        ''' </summary>
        Public ReadOnly Property SubDomain() As String
            Get
                Return _subDomain
            End Get
        End Property

        ''' <summary>
        ''' The domain name portion, without the subdomain or the TLD
        ''' </summary>
        Public ReadOnly Property Domain() As String
            Get
                Return _domain
            End Get
        End Property

        ''' <summary>
        ''' The domain name portion, without the subdomain or the TLD
        ''' </summary>
        Public ReadOnly Property SLD() As String
            Get
                Return _domain
            End Get
        End Property

        ''' <summary>
        ''' The TLD portion
        ''' </summary>
        Public ReadOnly Property TLD() As String
            Get
                Return _tld
            End Get
        End Property

        ''' <summary>
        ''' The matching TLD rule
        ''' </summary>
        Public ReadOnly Property TLDRule() As TLDRule
            Get
                Return _tldRule
            End Get
        End Property

#End Region

#Region "Construction"

        ''' <summary>
        ''' Constructs a DomainName object from the string representation of a domain. 
        ''' </summary>
        ''' <param name="domainString"></param>
        Public Sub New(domainString As String)
            '  If an exception occurs it should bubble up past this
            ParseDomainName(domainString, _tld, _domain, _subDomain, _tldRule)
        End Sub

        ''' <summary>
        ''' Constructs a DomainName object from its 3 parts
        ''' </summary>
        ''' <param name="TLD">The top-level domain</param>
        ''' <param name="SLD">The second-level domain</param>
        ''' <param name="SubDomain">The subdomain portion</param>
        ''' <param name="TLDRule">The rule used to parse the domain</param>
        Private Sub New(TLD As String, SLD As String, SubDomain As String, TLDRule As TLDRule)
            Me._tld = TLD
            Me._domain = SLD
            Me._subDomain = SubDomain
            Me._tldRule = TLDRule
        End Sub

#End Region

#Region "Parse domain - private static method"

        ''' <summary>
        ''' Converts the string representation of a domain to it's 3 distinct components: 
        ''' Top Level Domain (TLD), Second Level Domain (SLD), and subdomain information
        ''' </summary>
        ''' <param name="domainString">The domain to parse</param>
        ''' <param name="TLD"></param>
        ''' <param name="SLD"></param>
        ''' <param name="SubDomain"></param>
        ''' <param name="MatchingRule"></param>
        Private Shared Sub ParseDomainName(domainString As String, ByRef TLD As String, ByRef SLD As String, ByRef SubDomain As String, ByRef MatchingRule As TLDRule)
            TLD = String.Empty
            SLD = String.Empty
            SubDomain = String.Empty
            MatchingRule = Nothing

            '  If the fqdn is empty, we have a problem already
            If domainString.Trim() = String.Empty Then
                Throw New ArgumentException("The domain cannot be blank")
            End If

            '  Next, find the matching rule:
            MatchingRule = FindMatchingTLDRule(domainString)

            '  At this point, no rules match, we have a problem
            If MatchingRule Is Nothing Then
                Throw New FormatException("The domain does not have a recognized TLD")
            End If

            '  Based on the tld rule found, get the domain (and possibly the subdomain)
            Dim tempSudomainAndDomain As String = String.Empty
            Dim tldIndex As Integer = 0

            '  First, determine what type of rule we have, and set the TLD accordingly
            Select Case MatchingRule.Type
                Case TLDRule.RuleType.Normal
                    tldIndex = domainString.IndexOf("." + MatchingRule.Name, StringComparison.InvariantCultureIgnoreCase)
                    tempSudomainAndDomain = domainString.Substring(0, tldIndex)
                    TLD = domainString.Substring(tldIndex + 1)
                    Exit Select
                Case TLDRule.RuleType.Wildcard
                    '  This finds the last portion of the TLD...
                    tldIndex = domainString.IndexOf("." + MatchingRule.Name, StringComparison.InvariantCultureIgnoreCase)
                    tempSudomainAndDomain = domainString.Substring(0, tldIndex)

                    '  But we need to find the wildcard portion of it:
                    tldIndex = tempSudomainAndDomain.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase)
                    tempSudomainAndDomain = domainString.Substring(0, tldIndex)
                    TLD = domainString.Substring(tldIndex + 1)
                    Exit Select
                Case TLDRule.RuleType.Exception
                    tldIndex = domainString.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase)
                    tempSudomainAndDomain = domainString.Substring(0, tldIndex)
                    TLD = domainString.Substring(tldIndex + 1)
                    Exit Select
            End Select

            '  See if we have a subdomain:
            Dim lstRemainingParts As New List(Of String)(tempSudomainAndDomain.Split("."c))

            '  If we have 0 parts left, there is just a tld and no domain or subdomain
            '  If we have 1 part, it's the domain, and there is no subdomain
            '  If we have 2+ parts, the last part is the domain, the other parts (combined) are the subdomain
            If lstRemainingParts.Count > 0 Then
                '  Set the domain:
                SLD = lstRemainingParts(lstRemainingParts.Count - 1)

                '  Set the subdomain, if there is one to set:
                If lstRemainingParts.Count > 1 Then
                    '  We strip off the trailing period, too
                    SubDomain = tempSudomainAndDomain.Substring(0, tempSudomainAndDomain.Length - SLD.Length - 1)
                End If
            End If
        End Sub

#End Region

#Region "TryParse method(s)"

        ''' <summary>
        ''' Converts the string representation of a domain to its DomainName equivalent.  A return value
        ''' indicates whether the operation succeeded.
        ''' </summary>
        ''' <param name="domainString"></param>
        ''' <param name="result"></param>
        ''' <returns></returns>
        Public Shared Function TryParse(domainString As String, ByRef result As DomainName) As Boolean
            Dim retval As Boolean = False

            '  Our temporary domain parts:
            Dim _tld As String = String.Empty
            Dim _sld As String = String.Empty
            Dim _subdomain As String = String.Empty
            Dim _tldrule As TLDRule = Nothing
            result = Nothing

            Try
                '  Try parsing the domain name ... this might throw formatting exceptions
                ParseDomainName(domainString, _tld, _sld, _subdomain, _tldrule)

                '  Construct a new DomainName object and return it
                result = New DomainName(_tld, _sld, _subdomain, _tldrule)

                '  Return 'true'
                retval = True
            Catch
                '  Looks like something bad happened -- return 'false'
                retval = False
            End Try

            Return retval
        End Function

#End Region

#Region "Rule matching"
        ''' <summary>
        ''' Finds matching rule for a domain.  If no rule is found, 
        ''' returns a null TLDRule object
        ''' </summary>
        ''' <param name="domainString"></param>
        ''' <returns></returns>
        Private Shared Function FindMatchingTLDRule(domainString As String) As TLDRule
            '  Split our domain into parts (based on the '.')
            '  ...Put these parts in a list
            '  ...Make sure these parts are in reverse order (we'll be checking rules from the right-most pat of the domain)
            Dim lstDomainParts As List(Of String) = domainString.Split("."c).ToList()
            lstDomainParts.Reverse()

            '  Begin building our partial domain to check rules with:
            Dim checkAgainst As String = String.Empty

            '  Our 'matches' collection:
            Dim ruleMatches As New List(Of TLDRule)()

            For Each domainPart As String In lstDomainParts
                '  Add on our next domain part:
                checkAgainst = String.Format("{0}.{1}", domainPart, checkAgainst)

                '  If we end in a period, strip it off:
                If checkAgainst.EndsWith(".") Then
                    checkAgainst = checkAgainst.Substring(0, checkAgainst.Length - 1)
                End If

                '  Try to match an exception rule:
                Dim exceptionresults = From test In TLDRulesCache.Instance.TLDRuleList Where test.Name.Equals(checkAgainst, StringComparison.InvariantCultureIgnoreCase) AndAlso test.Type = TLDRule.RuleType.Exception Select test

                '  Add our matches:
                ruleMatches.AddRange(exceptionresults.ToList())

                '  See if we have a match yet.
                Debug.WriteLine(String.Format("Domain part {0} matched {1} exception rules", checkAgainst, exceptionresults.Count()))

                '  Try to match an exception rule:
                Dim wildcardresults = From test In TLDRulesCache.Instance.TLDRuleList Where test.Name.Equals(checkAgainst, StringComparison.InvariantCultureIgnoreCase) AndAlso test.Type = TLDRule.RuleType.Wildcard Select test

                '  Add our matches:
                ruleMatches.AddRange(wildcardresults.ToList())

                '  See if we have a match yet.
                Debug.WriteLine(String.Format("Domain part {0} matched {1} wildcard rules", checkAgainst, wildcardresults.Count()))

                '  Try to match a normal rule:
                Dim normalresults = From test In TLDRulesCache.Instance.TLDRuleList Where test.Name.Equals(checkAgainst, StringComparison.InvariantCultureIgnoreCase) AndAlso test.Type = TLDRule.RuleType.Normal Select test

                '  See if we have a match yet.
                Debug.WriteLine(String.Format("Domain part {0} matched {1} normal rules", checkAgainst, normalresults.Count()))

                '  Add our matches:
                ruleMatches.AddRange(normalresults.ToList())
            Next

            '  Sort our matches list (longest rule wins, according to :
            Dim results = From match In ruleMatches Order By match.Name.Length Descending Select match

            '  Take the top result (our primary match):
            Dim primaryMatch As TLDRule = results.Take(1).SingleOrDefault()

            If primaryMatch IsNot Nothing Then
                Debug.WriteLine(String.Format("Looks like our match is: {0}, which is a(n) {1} rule.", primaryMatch.Name, primaryMatch.Type))
            Else
                Debug.WriteLine(String.Format("No rules matched domain: {0}", domainString))
            End If

            Return primaryMatch
        End Function
#End Region
    End Class
End Namespace


