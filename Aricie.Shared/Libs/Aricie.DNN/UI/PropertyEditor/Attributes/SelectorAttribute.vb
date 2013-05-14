Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class ProvidersSelectorAttribute
        Inherits SelectorAttribute

        Public Sub New(ByVal nameFieldName As String)
            MyBase.New(nameFieldName, nameFieldName, False, False, "", "", False, False)
        End Sub

        Public Sub New()
            Me.New("Name")
        End Sub

    End Class


    <AttributeUsage(AttributeTargets.Property)> _
    Public Class SelectorAttribute
        Inherits Attribute

        'Private _IsISelector As Boolean
        'Private _selectorTypeName As String
        'Private _dataTextField As String
        'Private _dataValueField As String
        'Private _exclusive As Boolean
        'Private _addNullItem As Boolean
        'Private _nullItemValue As String
        'Private _LocalizeItems As Boolean
        'Private _LocalizeNull As Boolean

        Private _SelectorInfo As New SelectorInfo



        'Public Sub New(ByVal dataTextField As String, ByVal dataValueField As String, ByVal enumerableType As Type, _
        '              ByVal exclusive As Boolean, ByVal addNullItem As Boolean, ByVal nullItemName As String, ByVal nullItemValue As String, ByVal localizeItems As Boolean, ByVal localizeNull As Boolean)
        '    Me.New("", dataTextField, dataValueField, exclusive, addNullItem, nullItemName, nullItemValue, localizeItems, localizeNull)
        '    Me._enumerableType = enumerableType
        'End Sub


        '<Obsolete("Use overload with type instead of string type")> _
        Public Sub New(ByVal selectorTypeName As String, ByVal dataTextField As String, ByVal dataValueField As String, ByVal exclusive As Boolean, ByVal addNullItem As Boolean)
            Me._SelectorInfo.SelectorTypeName = selectorTypeName
            Me._SelectorInfo.DataTextField = dataTextField
            Me._SelectorInfo.DataValueField = dataValueField
            Me._SelectorInfo.IsExclusive = exclusive
            Me._SelectorInfo.InsertNullItem = addNullItem
        End Sub


        Public Sub New(ByVal selectorTypeName As String, ByVal dataTextField As String, ByVal dataValueField As String, _
                       ByVal exclusive As Boolean, ByVal addNullItem As Boolean, ByVal nullItemName As String, ByVal nullItemValue As String, ByVal localizeItems As Boolean, ByVal localizeNull As Boolean)
            Me.New(selectorTypeName, dataTextField, dataValueField, exclusive, addNullItem)
            Me._SelectorInfo.NullItemValue = nullItemValue
            Me._SelectorInfo.LocalizeItems = localizeItems
            Me._SelectorInfo.LocalizeNull = localizeNull
        End Sub

        Public Sub New(ByVal obSelectorType As Type, ByVal dataTextField As String, ByVal dataValueField As String, _
                      ByVal exclusive As Boolean, ByVal addNullItem As Boolean, ByVal nullItemName As String, ByVal nullItemValue As String, ByVal localizeItems As Boolean, ByVal localizeNull As Boolean)
            Me.New(ReflectionHelper.GetSafeTypeName(obSelectorType), dataTextField, dataValueField, exclusive, addNullItem, nullItemName, nullItemValue, localizeItems, localizeNull)
        End Sub

        Public Sub New(ByVal dataTextField As String, ByVal dataValueField As String, _
                      ByVal exclusive As Boolean, ByVal addNullItem As Boolean, ByVal nullItemName As String, ByVal nullItemValue As String, ByVal localizeItems As Boolean, ByVal localizeNull As Boolean)
            Me.New("", dataTextField, dataValueField, exclusive, addNullItem, nullItemName, nullItemValue, localizeItems, localizeNull)
            Me._SelectorInfo.IsIselector = True
        End Sub




        'Public ReadOnly Property SelectorTypeName() As String
        '    Get
        '        Return _selectorTypeName
        '    End Get
        'End Property

        'Public ReadOnly Property DataTextField() As String
        '    Get
        '        Return _dataTextField
        '    End Get
        'End Property

        'Public ReadOnly Property DataValueField() As String
        '    Get
        '        Return _dataValueField
        '    End Get
        'End Property

        'Public ReadOnly Property Exclusive() As Boolean
        '    Get
        '        Return _exclusive
        '    End Get
        'End Property

        'Public ReadOnly Property AddNullItem() As Boolean
        '    Get
        '        Return Me._addNullItem
        '    End Get
        'End Property

        'Public ReadOnly Property NullItemValue() As String
        '    Get
        '        Return Me._nullItemValue
        '    End Get
        'End Property

        'Public ReadOnly Property LocalizeItems() As Boolean
        '    Get
        '        Return Me._LocalizeItems
        '    End Get
        'End Property

        'Public ReadOnly Property LocalizeNull() As Boolean
        '    Get
        '        Return Me._LocalizeNull
        '    End Get
        'End Property

        'Public ReadOnly Property IsISelector() As Boolean
        '    Get
        '        Return _IsISelector
        '    End Get
        'End Property

        Public Property SelectorInfo() As SelectorInfo
            Get
                Return _SelectorInfo
            End Get
            Set(ByVal value As SelectorInfo)
                _SelectorInfo = value
            End Set
        End Property
    End Class

End Namespace
