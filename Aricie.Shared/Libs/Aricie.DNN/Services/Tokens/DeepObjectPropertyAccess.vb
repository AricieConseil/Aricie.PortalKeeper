Imports Aricie.Collections
Imports DotNetNuke.Services.Tokens
Imports System.Globalization
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions

Namespace Services

    Public Enum TokenLevelAccess
        All
        PropertiesOnly
    End Enum

    ''' <summary>
    ''' Reflection access for complex types in the ATR context
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class DeepObjectPropertyAccess
        Implements IPropertyAccess

        Private _cacheable As CacheLevel = CacheLevel.notCacheable
        Private _TokenSource As Object
        Friend PropertiesDeepAccess As New HybridDictionary(Of String, DeepObjectPropertyAccess)(0, StringComparer.InvariantCultureIgnoreCase, 5)
        Private _ResourceFile As String

#Region "cTors"

        Public Sub New(ByVal tokenSource As Object)
            Me._TokenSource = tokenSource
        End Sub

        Public Sub New(ByVal tokenSource As Object, ByVal resourceFile As String)
            Me.New(tokenSource)
            Me._ResourceFile = resourceFile
        End Sub

        Public Sub New(ByVal objTokenSource As Object, ByVal cacheablility As CacheLevel)
            Me.New(objTokenSource)
            Me._cacheable = cacheablility
        End Sub

        Public Sub New(ByVal objTokenSource As Object, ByVal resourceFile As String, ByVal cacheablility As CacheLevel)
            Me.New(objTokenSource, resourceFile)
            Me._cacheable = cacheablility
        End Sub


        Public Property LevelAccess As TokenLevelAccess = TokenLevelAccess.All



#End Region
        ''' <summary>
        ''' Data source for the deep property access
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property TokenSource() As Object
            Get
                Return Me._TokenSource
            End Get
            Set(ByVal value As Object)
                Me._TokenSource = value
                Me.PropertiesDeepAccess.Clear()
            End Set
        End Property

        ''' <summary>
        ''' Returns boolean indicating whether the obect can be cached
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Cacheability() As CacheLevel Implements IPropertyAccess.Cacheability
            Get
                Return Me._cacheable
            End Get
        End Property



#Region "Public methods"

        Public Overloads Function GetValue(ByVal strPropertyName As String) As Object

            Return GetValue(strPropertyName, False)

        End Function


        Private Overloads Function GetValue(ByVal strPropertyName As String, ByRef isLocalized As Boolean) As Object

            Dim propExplorer As New PropertyExplorer(strPropertyName, Me._TokenSource, Me.LevelAccess)

            propExplorer.GetDeepPropertyValue(Me)

            'If propExplorer.CurrentValue Is Nothing Then
            '    Throw New ArgumentException(String.Format("Target Property {0} not found", strPropertyName))
            'End If
            isLocalized = propExplorer.IsLocalized
            Return propExplorer.CurrentValue

        End Function


        ''' <summary>
        ''' Returns property value of the object as IEnumerable
        ''' </summary>
        ''' <param name="strPropertyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnumerable(ByVal strPropertyName As String) As IEnumerable

            Dim toReturn As IEnumerable = Nothing
            'Dim propExplorer As New PropertyExplorer(strPropertyName, Me._TokenSource)

            'propExplorer.GetDeepPropertyValue(Me)

            Dim objValue As Object = Me.GetValue(strPropertyName)


            If objValue Is Nothing Then
                Throw New ArgumentException(String.Format("Target Property {0} not found", strPropertyName))
            End If

            If TypeOf objValue Is String Then
                Dim strValue As String = DirectCast(objValue, String)
                toReturn = New List(Of String)
                If Not String.IsNullOrEmpty(strValue) AndAlso strValue.ToUpperInvariant <> "FALSE" Then
                    DirectCast(toReturn, List(Of String)).Add(strValue)
                End If
            ElseIf TypeOf objValue Is IEnumerable Then
                toReturn = DirectCast(objValue, IEnumerable)
            ElseIf TypeOf objValue Is IConvertible Then
                toReturn = New List(Of Boolean)
                If DirectCast(objValue, IConvertible).ToBoolean(CultureInfo.InvariantCulture) Then
                    DirectCast(toReturn, List(Of Boolean)).Add(True)
                End If
            Else
                Throw New ArgumentException(String.Format("Target Property {0} not a collection of items", strPropertyName))
            End If

            Return toReturn

        End Function

        ''' <summary>
        ''' Returns property value as string
        ''' </summary>
        ''' <param name="strPropertyName"></param>
        ''' <param name="strFormat"></param>
        ''' <param name="formatProvider"></param>
        ''' <param name="accessingUser"></param>
        ''' <param name="accessLevel"></param>
        ''' <param name="propertyNotFound"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, _
                                     ByVal formatProvider As CultureInfo, ByVal accessingUser As UserInfo, _
                                     ByVal accessLevel As Scope, ByRef propertyNotFound As Boolean) As String _
            Implements IPropertyAccess.GetProperty
            Dim toReturn As String = ""
            Try

                propertyNotFound = True
                If strPropertyName <> "" Then
                    'Dim propExplorer As New PropertyExplorer(strPropertyName, Me._TokenSource)

                    'propExplorer.GetDeepPropertyValue(Me)

                    Dim isLocalized As Boolean
                    Dim objValue As Object = Me.GetValue(strPropertyName, isLocalized)

                    If Not objValue Is Nothing Then

                        propertyNotFound = False

                        Dim propType As Type = objValue.GetType

                        Select Case propType.Name
                            Case "String"
                                toReturn = PropertyAccess.FormatString(CStr(objValue), strFormat)
                            Case "Boolean"
                                toReturn = _
                                    (PropertyAccess.Boolean2LocalizedYesNo(CBool(objValue), formatProvider))
                            Case Else
                                If TypeOf objValue Is IFormattable Then
                                    If strFormat = String.Empty Then strFormat = "g"
                                    toReturn = CType(objValue, IFormattable).ToString(strFormat, formatProvider)
                                Else
                                    toReturn = objValue.ToString()
                                End If
                        End Select

                        'localization
                        If isLocalized AndAlso toReturn <> "" Then
                            toReturn = Localization.GetString(toReturn, Me._ResourceFile)

                        End If

                    End If

                End If

            Catch ex As Exception
                'LogException(New InvalidOperationException("Property Access failed during token replace: Object: " & _
                '                                             Me.TokenSource.ToString & ", PropertyName:" & _
                '                                             strPropertyName, ex))
                ' SAMY: 16/05/2012
                'Modification pour ne pas logguer les erreurs: ces dernières pouvant arriver en grand nombre, on remplace le résultat par un NA?
                'TODO: venir brancher le logger sur ce point

                propertyNotFound = True
            End Try

            Return toReturn
        End Function

#End Region






#Region "Private methods"
        ''' <summary>
        ''' Returns recursive DOPA
        ''' </summary>
        ''' <param name="param"></param>
        ''' <param name="propExplorer"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function GetDeepPropertyAccess(ByVal param As String, ByVal propExplorer As PropertyExplorer) As DeepObjectPropertyAccess
            Dim toReturn As DeepObjectPropertyAccess = Nothing
            If propExplorer.CurrentValue IsNot Nothing Then
                toReturn = New DeepObjectPropertyAccess(propExplorer.CurrentValue, Me._ResourceFile, Me._cacheable)
                Me.PropertiesDeepAccess(param) = toReturn
            Else
                Dim message As String = String.Format("Member Not found for params {0}", param)
                Throw New ArgumentException(message)
            End If
            Return toReturn
        End Function

#End Region



    End Class
End Namespace
