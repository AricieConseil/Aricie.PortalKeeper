

Namespace Configuration

    ''' <summary>
    ''' Configuration element for an ASP.Net HttpHandler class
    ''' </summary>
    Public Class HttpHandlerInfo
        Inherits WebServerElementInfo




        Public Sub New(ByVal name As String, ByVal moduleType As Type, ByVal path As String, ByVal verb As String, ByVal strPrecondition As String)
            MyBase.New(name, moduleType)
            Me._Path = path
            Me._Verb = verb
            Me.sectionNameIIS6 = "httpHandlers"
            Me.sectionNameIIS7 = "handlers"
            Me.keyNameIIS6 = "path"
            Me.precondition = strPrecondition

        End Sub


        Private _Path As String = ""
        Private _Verb As String = ""

        Public Property Path() As String
            Get
                Return _Path
            End Get
            Set(ByVal value As String)
                _Path = value
            End Set
        End Property



        Public Property Verb() As String
            Get
                Return _Verb
            End Get
            Set(ByVal value As String)
                _Verb = value
            End Set
        End Property


        Protected Overrides Function GetKeyIIS6() As String
            Return Me.Path

        End Function



        Protected Overloads Overrides Function BuildAddNode(ByVal usePrecondition As Boolean) As WebServerAddInfo
            If usePrecondition Then
                Return New HttpHandlerAddInfo(Me.Name, Me.Type, Me.Path, Me.Verb, precondition)
            Else
                Return New HttpHandlerAddInfo(Me.Name, Me.Type, Me.Path, Me.Verb)
            End If

        End Function

    End Class


End Namespace


