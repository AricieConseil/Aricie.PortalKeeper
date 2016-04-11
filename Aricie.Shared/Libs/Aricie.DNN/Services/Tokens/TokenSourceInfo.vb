Imports Aricie.Collections
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Filtering
    ''' <summary>
    ''' Class holder for token data source information
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Class TokenSourceInfo

        Private _ReflectedProviders As Dictionary(Of String, Object)

        ''' <summary>
        ''' List of static tokens in the source
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(DictionaryEditControl), GetType(EditControl))> _
            <KeyEditor(GetType(CustomTextEditControl), GetType(TokenKeyAttributes))> _
            <ValueEditor(GetType(CustomTextEditControl), GetType(TokenValueAttributes))> _
            <LabelMode(LabelMode.Top), SortOrder(0)> _
            <CollectionEditor(False, True, True, True, 10)> _
        Public Property StaticTokens() As New SerializableDictionary(Of String, String)


        Public Property TokenVariables As New Aricie.DNN.Services.Flee.Variables()

        Public Property CacheTokenReplace() As Boolean

        ''' <summary>
        ''' List of tokens providers
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(DictionaryEditControl), GetType(EditControl)), _
            KeyEditor(GetType(CustomTextEditControl), GetType(TokenKeyAttributes)), _
            ValueEditor(GetType(CustomTextEditControl), GetType(TokenValueAttributes)), _
            LabelMode(LabelMode.Top), SortOrder(1)> _
            <CollectionEditor(False, True, True, True, 10)> _
        Public Property TokenProviders() As New SerializableDictionary(Of String, String)

        ''' <summary>
        ''' List of conditional tokens
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(ListEditControl), GetType(EditControl)), _
            LabelMode(LabelMode.Top), CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, False), SortOrder(2), Orientation(Orientation.Vertical)> _
        Public Property ConditionalTokens() As New List(Of ConditionalTokenInfo)

        ''' <summary>
        ''' List of reflected tokens
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore(), Browsable(False)> _
        Public ReadOnly Property ReflectedProviders() As Dictionary(Of String, Object)
            Get
                If Me._ReflectedProviders Is Nothing Then
                    Me._ReflectedProviders = New Dictionary(Of String, Object)
                    For Each key As String In Me.TokenProviders.Keys
                        Try
                            Dim provider As Object = ReflectionHelper.GetSingleton(Me.TokenProviders(key))
                            Me._ReflectedProviders(key) = provider
                        Catch ex As Exception
                            Dim wrongProvider As New ArgumentException(String.Format("token provider type {0} could not be reflected", Me.TokenProviders(key)), ex)
                            Aricie.Services.ExceptionHelper.LogException(wrongProvider)
                        End Try
                    Next
                End If
                Return Me._ReflectedProviders
            End Get
        End Property

        ''' <summary>
        ''' Sets the token from the source in the advanced token replace
        ''' </summary>
        ''' <param name="atr"></param>
        ''' <remarks></remarks>
        Public Sub SetTokens(ByVal atr As AdvancedTokenReplace, contextOwner As Object, contextLookup As IContextLookup)
            If contextLookup IsNot Nothing Then
                If Me.TokenVariables.Instances.Count > 0 Then
                    Dim vars As SerializableDictionary(Of String, Object) = Me.TokenVariables.EvaluateVariables(contextOwner, contextLookup)
                    For Each objVarPair As KeyValuePair(Of String, Object) In vars
                        atr.SetObjectReplace(objVarPair.Value, objVarPair.Key)
                    Next
                End If
            End If
            Dim conditional As TokenSourceInfo = GetConditionalSource(atr)

            If conditional IsNot Nothing Then
                conditional.SetTokens(atr, contextOwner, contextLookup)
            End If

            Me.SetTokens(atr, False)
        End Sub



        ''' <summary>
        ''' Sets the token from the source in the advanced token replace
        ''' </summary>
        ''' <param name="atr"></param>
        ''' <remarks></remarks>
        Public Sub SetTokens(ByVal atr As AdvancedTokenReplace)
            Me.SetTokens(atr, True)
        End Sub

        Public Sub SetTokens(ByVal atr As AdvancedTokenReplace, setConditional As Boolean)
            For Each key As String In Me.StaticTokens.Keys
                atr.SimpleTokens(key) = Me.StaticTokens(key)
            Next

            For Each key As String In Me.ReflectedProviders.Keys
                atr.SetObjectReplace(Me.ReflectedProviders(key), key)
            Next
            If setConditional Then
                Dim conditional As TokenSourceInfo = GetConditionalSource(atr)

                If conditional IsNot Nothing Then
                    conditional.SetTokens(atr)
                End If
            End If

            atr.SetIsSet(Me)

        End Sub

        Private Function GetConditionalSource(ByVal atr As AdvancedTokenReplace) As TokenSourceInfo
            Dim tempValue, formattedToken As String
            Dim subTokenSource As TokenSourceInfo = Nothing
            For Each token As ConditionalTokenInfo In Me.ConditionalTokens


                Dim conditional As ConditionalTokenSourceInfo = token.ConditionalToken
                formattedToken = FormatToken(token.Key)
                tempValue = atr.ReplaceAllTokens(formattedToken)


                If conditional.ConditionalSources.TryGetValue(tempValue, subTokenSource) Then
                    Return subTokenSource
                End If

            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Token format
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatToken(ByVal token As String) As String

            Dim toReturn As String = token
            If Not toReturn.StartsWith("[") Then
                toReturn = "["c & toReturn

            End If
            If Not toReturn.EndsWith("]") Then
                toReturn = toReturn & "]"c
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' Attribute for key edition
        ''' </summary>
        ''' <remarks></remarks>
        Public Class TokenKeyAttributes
            Implements IAttributesProvider


            Public Function GetAttributes() As IEnumerable(Of Attribute) Implements IAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                toReturn.Add(New WidthAttribute(200))
                Return toReturn
            End Function
        End Class

        ''' <summary>
        ''' Attribute for vzalue edition
        ''' </summary>
        ''' <remarks></remarks>
        Public Class TokenValueAttributes
            Implements IAttributesProvider


            Public Function GetAttributes() As IEnumerable(Of Attribute) Implements IAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                toReturn.Add(New LineCountAttribute(3))
                toReturn.Add(New WidthAttribute(400))
                Return toReturn
            End Function
        End Class


        Private _cachedTokenReplace as AdvancedTokenReplace

        Public Function GetTokenReplace(contextVars As IContextLookup) As AdvancedTokenReplace

            If _cachedTokenReplace IsNot Nothing
                Return _cachedTokenReplace
            End If

            Dim toReturn as New AdvancedTokenReplace()
            Me.SetTokens(toReturn, contextVars, contextVars)
            if Me.CacheTokenReplace
                _cachedTokenReplace = toReturn
            End If
            Return toReturn

        End Function


    End Class

End Namespace
