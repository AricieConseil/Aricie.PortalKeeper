Namespace Security.Trial
    ''' <summary>
    ''' Type of limitation de trial will present to the user
    ''' </summary>
    ''' <remarks></remarks>
    <Flags()> _
        <CLSCompliant(True)> _
    Public Enum TrialLimitation
        Limitation = 1
        Expiration = 2
        LocalhostOnly = 4
        LimitedKey = 8
        FlagModules = 16
        MaxNbInstances = 32
        ExpireView = 64
        ExplainView = 128
    End Enum
End Namespace