Namespace Services.OpenXML

    Public Class xlsxDataField
        Private _DbFieldName As String
        Public Property DbFieldName() As String
            Get
                Return _DbFieldName
            End Get
            Set(ByVal value As String)
                _DbFieldName = value
            End Set
        End Property

        Private _DbFieldId As String
        Public Property DbFieldId() As String
            Get
                Return _DbFieldId
            End Get
            Set(ByVal value As String)
                _DbFieldId = value
            End Set
        End Property

        Private _ExcelRow As ExcelRowEntity
        Public Property ExcelRow() As ExcelRowEntity
            Get
                Return _ExcelRow
            End Get
            Set(ByVal value As ExcelRowEntity)
                _ExcelRow = value
            End Set
        End Property


        Public Class ExcelRowEntity
            Private _t As String
            ''' <summary>
            ''' Title of the Column
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property t() As String
                Get
                    Return _t
                End Get
                Set(ByVal value As String)
                    _t = value
                End Set
            End Property

            Private _v As String
            ''' <summary>
            ''' Index of the column
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property v() As String
                Get
                    Return _v
                End Get
                Set(ByVal value As String)
                    _v = value
                End Set
            End Property
            Private _db As String
            ''' <summary>
            ''' Id of the dbField, cf xslxDataField.DbFieldId
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property db() As String
                Get
                    Return _db
                End Get
                Set(ByVal value As String)
                    _db = value
                End Set
            End Property

        End Class
    End Class

End Namespace