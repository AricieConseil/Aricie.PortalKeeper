Namespace ComponentModel
    Public Interface IModuleIdentity

        Function GetModuleName() As String



    End Interface


    Public MustInherit Class ModuleIdentity
        Implements IModuleIdentity


        Public MustOverride Function GetModuleName() As String Implements IModuleIdentity.GetModuleName




    End Class

End Namespace