Imports System.Web.UI
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditControls

    Public Class DecimalEditControl
        Inherits AricieEditControl

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs an IntegerEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            Me.SystemType = "System.Decimal"
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Property StringValue() As String
            Get
                Return DecimalValue.ToString
            End Get
            Set(ByVal Value As String)
                Dim setValue As Decimal = Decimal.Parse(Value)
                Me.Value = setValue
            End Set
        End Property

        Protected ReadOnly Property DecimalValue() As Decimal
            Get
                Dim decValue As Decimal = Null.NullDecimal
                Try
                    'Try and cast the value to an Integer
                    decValue = CType(Value, Decimal)
                Catch ex As Exception
                End Try
                Return decValue
            End Get
        End Property

        Protected ReadOnly Property OldDecimalValue() As Decimal
            Get
                Dim decValue As Decimal = Null.NullDecimal
                Try
                    'Try and cast the value to an Integer
                    decValue = CType(OldValue, Decimal)
                Catch ex As Exception
                End Try
                Return decValue
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = DecimalValue
            args.OldValue = OldDecimalValue
            args.StringValue = StringValue
            MyBase.OnValueChanged(args)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Size, "8")
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
        End Sub

#End Region

    End Class

End Namespace

