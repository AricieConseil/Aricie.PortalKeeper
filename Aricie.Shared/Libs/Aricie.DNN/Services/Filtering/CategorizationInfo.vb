Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports System.ComponentModel
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Aricie.Business.Filters
Imports Aricie.DNN.Services.Flee
Imports Telerik.Web.Data.Extensions

Namespace Services.Filtering

    Public Enum CategorizationMode
        DictionaryContainsInput
        InputContainsKey
        CompoundRegex
        RadixTree
    End Enum

       


    Public Class CategorizationInfo

        <ExtendedCategory("Categories")> _
        Public Property DefaultToUnchanged As Boolean

        <ExtendedCategory("Categories")> _
        <DefaultValue("N.A.")> _
        <ConditionalVisible("DefaultToUnchanged", True)> _
        Public Property DefaultValue As String = "N.A."

          <ExtendedCategory("Categories")> _
         <DefaultValue(False)> _
        Public  Property DynamicCategories() As Boolean

        <ExtendedCategory("Categories")> _
        <ConditionalVisible("DynamicCategories")> _
        Public  Property NoCache() As Boolean

        <ExtendedCategory("Categories")> _
         <ConditionalVisible("DynamicCategories",True)> _
        <CollectionEditor(False, True, False, True, 20, CollectionDisplayStyle.Accordion, True, 0, "Key")> _
        <Orientation(System.Web.UI.WebControls.Orientation.Vertical)> _
        Public Property SubTree As New SerializableSortedDictionary(Of String, List(Of String))

        <ExtendedCategory("Categories")> _
          <ConditionalVisible("DynamicCategories")> _
        <CollectionEditor(False, True, False, True, 20, CollectionDisplayStyle.Accordion, True, 0, "Key")> _
        <Orientation(System.Web.UI.WebControls.Orientation.Vertical)> _
        Public Property DynamicSubTree As New SerializableSortedDictionary(Of String, AnonymousGeneralVariableInfo(Of String()))

        <ExtendedCategory("Comparison")> _
        Public Property Mode As CategorizationMode = CategorizationMode.InputContainsKey

        <ConditionalVisible("Mode", False, True, CategorizationMode.DictionaryContainsInput)> _
         <ExtendedCategory("Comparison")> _
        Public Property Comparer As New ComparerInfo()

        <DefaultValue(StringComparison.OrdinalIgnoreCase)> _
        <ConditionalVisible("Mode", False, True, CategorizationMode.InputContainsKey)> _
         <ExtendedCategory("Comparison")> _
        Public Property Comparizon As StringComparison = StringComparison.OrdinalIgnoreCase

         <ExtendedCategory("Comparison")> _
        <DefaultValue(System.Text.RegularExpressions.RegexOptions.Compiled Or System.Text.RegularExpressions.RegexOptions.CultureInvariant)> _
        <ConditionalVisible("Mode", False, True, CategorizationMode.CompoundRegex)> _
        Public Property RegexOptions As RegexOptions = System.Text.RegularExpressions.RegexOptions.Compiled Or System.Text.RegularExpressions.RegexOptions.CultureInvariant

         <ExtendedCategory("Comparison")> _
        <ConditionalVisible("Mode", False, True, CategorizationMode.CompoundRegex)> _
        Public Property RegexFormat As String = "{0}"

         <ExtendedCategory("Comparison")> _
        <DefaultValue(False)> _
         <ConditionalVisible("Mode", False, True, CategorizationMode.RadixTree)> _
        Public Property Reverse As Boolean 

         <ExtendedCategory("Comparison")> _
        <DefaultValue(False)> _
         <ConditionalVisible("Mode", False, True, CategorizationMode.RadixTree)> _
        Public Property AllowPartialMatches As Boolean 

       


        Public Function GetSubTree(contextVars As IContextLookup) As SerializableSortedDictionary(Of String, List(Of String))
            if Not DynamicCategories Then
                Return SubTree
            Else
                Dim toReturn As SerializableSortedDictionary(Of String, List(Of String)) = Nothing
                If toReturn Is Nothing Then
                    toReturn = New SerializableSortedDictionary(Of String, List(Of String))
                    For Each variablePair As KeyValuePair(Of String, AnonymousGeneralVariableInfo(Of String())) In DynamicSubTree
                        Dim objList As String() = variablePair.Value.EvaluateTyped(contextVars, contextVars)
                        Dim strList As New List(Of  String)(objList)
                        'For Each objVal As Object In objList
                        '    strList.Add(objVal.ToString())
                        'Next
                        toReturn(variablePair.Key) = strList
                    Next
                End If
                Return toReturn
            End If
        End Function

        Private _ReverseDictionary As SerializableDictionary(Of String, String)

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property ReverseDictionary(contextVars As IContextLookup)  As SerializableDictionary(Of String, String)
            Get
                If _ReverseDictionary Is Nothing Orelse (DynamicCategories andalso NoCache) Then
                    SyncLock Me
                        Dim tempDico As New SerializableDictionary(Of String, String)(Me.Comparer.GetComparer())
                        For Each objPair As KeyValuePair(Of String, List(Of String)) In Me.GetSubTree(contextVars)
                            objPair.Value.ForEach(Sub(objValue) tempDico(objValue) = objPair.Key)
                        Next
                        If Not (DynamicCategories AndAlso NoCache) Then
                            _ReverseDictionary = tempDico
                        Else
                            Return tempDico
                        End If
                    End SyncLock
                End If
                Return _ReverseDictionary
            End Get
        End Property

         Private _CompoundRegexes As Dictionary(Of String, Regex)

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property CompoundRegexes(contextVars As IContextLookup)  as Dictionary(Of String, Regex)
            Get
                If _CompoundRegexes Is Nothing Orelse (DynamicCategories andalso NoCache) Then

                    SyncLock Me
                        Dim tempDico As New Dictionary(Of String, Regex)
                        For Each objPair As KeyValuePair(Of String, List(Of String)) In Me.GetSubTree(contextVars)
                            Dim strRegexBuilder As New StringBuilder()
                            objPair.Value.ForEach(Sub(strValue) strRegexBuilder.Append(String.Format(Me.RegexFormat, Regex.Escape( strValue)) & "|"c))
                            Dim strRegex as String = strRegexBuilder.ToString().TrimEnd("|"c)
                            Dim objRegex as New Regex(strRegex, RegexOptions)
                            tempDico(objPair.Key) = objRegex
                        Next
                        if Not (DynamicCategories andalso NoCache) Then
                            _CompoundRegexes = tempDico
                        Else
                            Return tempDico
                        End If
                       
                    End SyncLock
                End If
                Return _CompoundRegexes
            End Get
        End Property


        Private _RadixTree As RadixTree(Of Char, String, String)

        <Browsable(False)> _
        <XmlIgnore()> _
         Public ReadOnly Property RadixTree(contextVars As IContextLookup)  as RadixTree(Of Char, String, String)
            Get
                If _RadixTree Is Nothing Orelse (DynamicCategories andalso NoCache) Then
                    SyncLock Me
                        Dim tempRadix As New RadixTree(Of Char, String, String)(New InvariantCharComparer(), 7)
                        For Each objPair As KeyValuePair(Of String, List(Of String)) In Me.GetSubTree(contextVars)
                            For Each strValue As String In objPair.Value
                                if Reverse
                                    strValue = strValue.Reverse()
                                End If
                                tempRadix.Set(strValue, objPair.Key)
                            Next
                        Next
                        if Not (DynamicCategories andalso NoCache) Then
                            _RadixTree = tempRadix
                        Else
                            Return tempRadix
                        End If
                       
                    End SyncLock
                End If
                Return _RadixTree
            End Get
        End Property


       Public Function Match(value As String) As String
            Return Match(value, Nothing)
       End Function

        Public Function Match(value As String, contextVars As IContextLookup) As String
            Select Case Mode
                Case CategorizationMode.InputContainsKey
                    For Each objPair As KeyValuePair(Of String, String) In ReverseDictionary(contextVars)
                        If value.IndexOf(objPair.Key, Me.Comparizon) > -1 Then
                            Return objPair.Value
                        End If
                    Next
                Case CategorizationMode.DictionaryContainsInput
                    Dim toReturn As String = Nothing
                    If ReverseDictionary(contextVars).TryGetValue(value, toReturn) Then
                        Return toReturn
                    End If
                Case CategorizationMode.CompoundRegex
                    For Each regexPair As KeyValuePair(Of String,Regex) In CompoundRegexes(contextVars)
                        If regexPair.Value.IsMatch(value)
                            Return regexPair.Key
                        End If
                    Next
                Case CategorizationMode.RadixTree
                    Dim strToSearch As String = value
                    if Reverse
                        strToSearch = strToSearch.Reverse()
                    End If
                    Dim partialMatch As Boolean
                    Dim candidateCategory As string = RadixTree(contextVars).Search(strToSearch, partialMatch)
                    If Not candidateCategory.IsNullOrEmpty() AndAlso (Not partialMatch OrElse AllowPartialMatches)
                        Return candidateCategory
                    End If
            End Select
            If Me.DefaultToUnchanged Then
                Return value
            Else
                Return DefaultValue
            End If
        End Function


    End Class
End NameSpace