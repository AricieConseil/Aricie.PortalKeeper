

Namespace Configuration

    ''' <summary>
    ''' Represents the base installable component configuration
    ''' </summary>
    Public Interface IConfigElementInfo

        ''' <summary>
        ''' Checks if the current component is already installed
        ''' </summary>
        Function IsInstalled() As Boolean
        ''' <summary>
        ''' Performs the install/uninstall action with current parameters
        ''' </summary>
        Sub ProcessConfig(ByVal actionType As ConfigActionType)

    End Interface


End Namespace


