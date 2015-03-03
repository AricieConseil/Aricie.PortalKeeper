
Imports Aricie.Web
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls

Namespace Configuration


    


    ''' <summary>
    ''' Configuration element for an ASP.Net HttpHandler class
    ''' </summary>
    <Serializable()> _
    <DefaultProperty("FriendlyName")> _
    Public Class HttpHandlerInfo
        Inherits WebServerElementInfo


        <Browsable(False)> _
        Public Overridable ReadOnly Property FriendlyName As String
            Get
                If HasType Then
                    Return String.Format("{1}{0}{2}{0}{3}", UIConstants.TITLE_SEPERATOR, Me.Name, Me.FriendlyPathsAndVerbs, IIf(Me.IsInstalled, "registered", "unregistered"))
                End If
                Return String.Format("{1}{0}{2}", UIConstants.TITLE_SEPERATOR, Me.Name, Me.FriendlyPathsAndVerbs)
            End Get
        End Property


        Public Sub New()
            MyBase.New()
            Me.sectionNameIIS6 = "httpHandlers"
            Me.sectionNameIIS7 = "handlers"
            Me.keyNameIIS6 = "path"
        End Sub


        Public Sub New(ByVal name As String, ByVal handlerType As Type, ByVal path As String, ByVal verb As String, ByVal strPrecondition As String)
            MyBase.New(name, handlerType)
            Me._Path = path
            Me._Verb = verb
            Me.sectionNameIIS6 = "httpHandlers"
            Me.sectionNameIIS7 = "handlers"
            Me.keyNameIIS6 = "path"
            Me.precondition = strPrecondition

        End Sub


        Private _Path As String = ""
        Private _Verb As String = ""

        <Required(True)> _
        Public Property Path() As String
            Get
                Return _Path
            End Get
            Set(ByVal value As String)
                _Path = value
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property FriendlyPathsAndVerbs As String
            Get
                Dim toReturn As String = Me.Path & " (" & Me.Verb & ")"
                For Each objAlternate As HttpHandlerInfo In Me.AlternateRegistrations
                    toReturn &= ", " & objAlternate.Path & " (" & objAlternate.Verb & ")"
                Next
                Return toReturn
            End Get
        End Property




        <XmlIgnore()> _
        Public Property HttpVerb As HttpVerb
            Get
                Return GetHttpVerb(_Verb)
            End Get
            Set(value As HttpVerb)
                Dim isWildCard As Boolean = True
                Dim enumMembers As List(Of HttpVerb) = Common.GetEnumMembers(Of HttpVerb)()
                For Each objVerb As HttpVerb In enumMembers
                    If (objVerb And value) <> objVerb Then
                        isWildCard = False
                        Exit For
                    End If
                Next
                If isWildCard Then
                    _Verb = "*"
                Else
                    Dim tempVerb As String = ""
                    For Each objVerb As HttpVerb In enumMembers
                        If CInt(objVerb) <> 0 AndAlso (objVerb And value) = objVerb Then
                            tempVerb = tempVerb & ", " & objVerb.ToString()
                        End If
                    Next
                    If tempVerb.Length > 1 Then
                        tempVerb = tempVerb.Substring(2)
                    End If
                    _Verb = tempVerb.ToUpperInvariant
                End If
            End Set
        End Property

        Public Property AlternateRegistrations As New List(Of HttpHandlerInfo)

        Public Function GetHttpVerb(strVerb As String) As HttpVerb
            Dim toReturn As HttpVerb = HttpVerb.Unknown
            strVerb = strVerb.Trim()
            Dim enumMembers As List(Of HttpVerb) = Common.GetEnumMembers(Of HttpVerb)()
            If strVerb = "*" Then
                For Each objVerb As HttpVerb In enumMembers
                    toReturn = toReturn Or objVerb
                Next
            Else
                Dim splitVerbs As String() = strVerb.Split(","c)
                For Each splitVerb As String In splitVerbs
                    splitVerb = splitVerb.Trim()
                    For Each objenum As HttpVerb In enumMembers
                        If splitVerb.ToLowerInvariant = objenum.ToString().ToLowerInvariant Then
                            toReturn = toReturn Or objenum
                        End If
                    Next
                Next
            End If
            Return toReturn
        End Function


        <Browsable(False)> _
        Public Property Verb() As String
            Get
                Return _Verb
            End Get
            Set(ByVal value As String)
                _Verb = value
            End Set
        End Property

        Private _SuffixRegex As Regex

        Private Shared metaRegex As Regex
        Private Shared questRegex As Regex
        Private Shared starRegex As Regex
        Private Shared commaRegex As Regex
        Private Shared slashRegex As Regex
        Private Shared backslashRegex As Regex

        <Browsable(False)> _
        Public ReadOnly Property SuffixRegex As Regex
            Get
                If _SuffixRegex Is Nothing Then
                    Dim pattern As String = Me.Path
                    Dim regexOption As RegexOptions = RegexOptions.RightToLeft
                    regexOption = regexOption Or RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant
                    If metaRegex Is Nothing Then
                        metaRegex = New Regex("[\+\{\\\[\|\(\)\.\^\$]")
                        questRegex = New Regex("\?")
                        starRegex = New Regex("\*")
                        commaRegex = New Regex(",")
                        slashRegex = New Regex("(?=/)")
                        backslashRegex = New Regex("(?=[\\:])")
                    End If
                    pattern = metaRegex.Replace(pattern, "\$0")
                    pattern = questRegex.Replace(pattern, "[^/]")
                    pattern = starRegex.Replace(pattern, "[^/]*")
                    pattern = commaRegex.Replace(pattern, "\z|(?:\A|(?<=/))")
                    _SuffixRegex = New Regex(String.Concat("(?:\A|(?<=/))", pattern, "\z"), regexOption)
                End If
                Return _SuffixRegex
            End Get
        End Property


        Protected Overrides Function GetKeyIIS6() As String
            Return Me.Path

        End Function

        Public Function Matches(verb As String, path As String) As Boolean
            For Each objAlternate As HttpHandlerInfo In Me.AlternateRegistrations
                If objAlternate.Matches(verb, path) Then
                    Return True
                End If
            Next
            Dim objHttpVerb As HttpVerb = GetHttpVerb(verb)
            If Not (objHttpVerb And Me.HttpVerb) = objHttpVerb Then
                Return False
            End If
            Return SuffixRegex.IsMatch(path)
        End Function


        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, actionType As ConfigActionType)
            MyBase.AddConfigNodes(targetNodes, actionType)
            For Each objAlternate In Me.AlternateRegistrations
                objAlternate.Type = Me.Type
                objAlternate.AddConfigNodes(targetNodes, actionType)
            Next
        End Sub

        Protected Overloads Overrides Function BuildAddNode(ByVal usePrecondition As Boolean) As WebServerAddInfo
            If usePrecondition Then
                Return New HttpHandlerAddInfo(Me.Name, Me.Type, Me.Path, Me.Verb, precondition)
            Else
                Return New HttpHandlerAddInfo(Me.Name, Me.Type, Me.Path, Me.Verb)
            End If

        End Function

    End Class


End Namespace


