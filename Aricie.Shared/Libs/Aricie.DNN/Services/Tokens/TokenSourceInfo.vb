Imports Aricie.Collections
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.Services
Imports DotNetNuke.Services.Exceptions
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Filtering

    ''' <summary>
    ''' Class holder for token data source information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class TokenSourceInfo

        <NonSerialized()> _
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
        Public Property StaticTokens() As new SerializableDictionary(Of String, String)

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
        Public Property TokenProviders() As  New SerializableDictionary(Of String, String)
            
        ''' <summary>
        ''' List of conditional tokens
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(ListEditControl), GetType(EditControl)), _
            LabelMode(LabelMode.Top), CollectionEditor(False, False, True, True, 10), SortOrder(2), Orientation(Orientation.Vertical)> _
        Public Property ConditionalTokens() As new List(Of ConditionalTokenInfo)

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
        Public Sub SetTokens(ByVal atr As AdvancedTokenReplace)


            For Each key As String In Me.StaticTokens.Keys
                atr.SimpleTokens(key) = Me.StaticTokens(key)
            Next

            For Each key As String In Me.ReflectedProviders.Keys
                atr.SetObjectReplace(Me.ReflectedProviders(key), key)
            Next

            Dim tempValue, formattedToken As String

            For Each token As ConditionalTokenInfo In Me.ConditionalTokens


                Dim conditional As ConditionalTokenSourceInfo = token.ConditionalToken
                formattedToken = FormatToken(token.Key)
                tempValue = atr.ReplaceAllTokens(formattedToken)

                If conditional.ConditionalSources.ContainsKey(tempValue) Then
                    conditional.ConditionalSources(tempValue).SetTokens(atr)
                End If

            Next


        End Sub


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
                toReturn.Add(New WidthAttribute(100))
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

    End Class

End Namespace
