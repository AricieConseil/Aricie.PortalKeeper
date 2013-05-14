

Namespace Configuration

    ''' <summary>
    ''' Configuration element for an ASP.Net HttpModule
    ''' </summary>
    Public Class HttpModuleInfo
        Inherits WebServerElementInfo

        <Obsolete("precondtion should always be explicitly defined")> _
        Public Sub New(ByVal name As String, ByVal moduleType As Type)
            Me.New(name, moduleType, "managedHandler")
        End Sub

        Public Sub New(ByVal name As String, ByVal moduleType As Type, ByVal strPrecondition As String)
            MyBase.New(name, moduleType)
            Me.sectionNameIIS6 = "httpModules"
            Me.sectionNameIIS7 = "modules"
            Me.precondition = strPrecondition
        End Sub




        Protected Overloads Overrides Function BuildAddNode(ByVal usePrecondition As Boolean) As WebServerAddInfo
            If usePrecondition Then
                Return New HttpModuleAddInfo(Me.Name, Me.Type, precondition)
            Else
                Return New HttpModuleAddInfo(Me.Name, Me.Type)
            End If

        End Function

    End Class


End Namespace


